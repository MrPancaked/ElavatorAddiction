using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("movement")]
    public float speed;
    public float airMultiplier;
    public float jumpForce;
    public float jumpTiming;
    private bool jumping;
    private bool jumpAvailable;
    public float groundDrag;
    public float airDrag;
    public Vector3 moveDirection;

    [Header("sliding")]
    private float normalYScale;
    public float slideYScale;
    public float slideSpeed;
    public float slideDragIncrease;
    public float downForce;
    private bool sliding;
    

    [Header("ground detection")]
    public Vector3 boxSize;

    private LayerMask groundLayer;
    private bool grounded;

    private Inputs inputs;

    void Start()
    {
        inputs = GetComponent<Inputs>();
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
        normalYScale = transform.localScale.y;
    }

    private void Update()
    {
        if (inputs.jumpPressed)
        {
            jumping = true;
            Invoke("ResetJump", jumpTiming);
        }
    }

    void FixedUpdate()
    {
        if (grounded != IsGrounded() && rb.velocity.y <= 0)
        {
            jumpAvailable = true;
            Invoke("ResetJump", jumpTiming);
        }

        grounded = IsGrounded();

        //jumping
        if ((grounded) && jumping)
        {
            jumping = false;
            jumpAvailable = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        //jumping
        if (jumping && jumpAvailable)
        {
            jumping = false;
            jumpAvailable = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }


        //movement
        moveDirection = transform.right * inputs.moveMentInput.x + transform.forward * inputs.moveMentInput.y;

        if (grounded && !sliding)
        {
            rb.AddForce(moveDirection * speed);
        }
        else
        {
            rb.AddForce(moveDirection * speed * airMultiplier);
        }

        if (grounded && !sliding)
        {
            rb.drag = groundDrag;
        }
        else if (!sliding || !grounded)
        {
            rb.drag = airDrag;
        }

        //sliding
        if (inputs.slidePressed && !sliding)
        {
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z); /// Set crouch scale
            sliding = true;
            rb.drag = 0; /// Remove drag when sliding
            rb.AddForce(Vector3.down * downForce, ForceMode.Impulse); /// Move the player down
            rb.AddForce(moveDirection * slideSpeed);
        }

        if (sliding && grounded && (rb.drag < groundDrag))
        {
            rb.drag += slideDragIncrease * groundDrag; /// Increase drag when sliding
        }

        if (sliding && inputs.slideReleased)
        {
            sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
            rb.drag = groundDrag;
        }

        //end of fixedupdate
        inputs.jumpPressed = false;
        inputs.jumpReleased = false;
        inputs.slidePressed = false;
        inputs.slideReleased = false;
        
    }

    public void ResetJump()
    {
        jumping = false;
        jumpAvailable = false;
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
        Gizmos.DrawCube(transform.position + (Vector3.down * transform.localScale.y), boxSize);
    }
}
