using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float Drag;
    public float MovementSpeed;
    public float JumpForce;
    private float HVelocity;
    private float VVelocity;
    private Rigidbody rb;
    //private float groundCheckDistance = 0.2f;
    private LayerMask groundLayer;
    public Vector3 boxSize;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
    }
    void FixedUpdate()
    {
        bool grounded = IsGrounded();
        Debug.Log(grounded);

        //jumping
        if (grounded && Input.GetAxisRaw("Jump") == 1)
        {
            rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }

        //movement
        float HAcceleration = (Input.GetAxisRaw("Horizontal") * MovementSpeed) - (Drag * HVelocity);
        float VAcceleration = (Input.GetAxisRaw("Vertical") * MovementSpeed) - (Drag * VVelocity);
        HVelocity += HAcceleration;
        VVelocity += VAcceleration;
        rb.velocity = new Vector3(HVelocity, rb.velocity.y, VVelocity);
    }

    public bool IsGrounded()
    {
        // Cast a ray downward from the player's position
        return Physics.OverlapBox(transform.position + (Vector3.down * transform.localScale.y), boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
    }

    public void OnDrawGizmos()
    {
        // Draw the ray in the Scene view for debugging
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + Vector3.down * transform.localScale.y, boxSize);
    }
}