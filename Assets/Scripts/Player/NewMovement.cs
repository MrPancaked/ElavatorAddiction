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
    public float groundDrag;
    public float airDrag;

    [Header("sliding")]
    private float normalYScale;
    public float slideYScale;
    public float slideSpeed;
    private bool sliding;

    [Header("ground detection")]
    public Vector3 boxSize;

    private LayerMask groundLayer;

    private bool grounded;

    private enum MovementState
    {
        walking,
        sliding,
        air
    }
    private MovementState state; /// Current movement state of the player

    [SerializeField] private InputActionAsset controls; /// Input controls
    private InputAction movement; /// Input of the player
    private InputAction jump; /// Input of the player
    private InputAction slide; /// Input of the player


    private void Awake()
    {
        movement = controls.FindActionMap("Player").FindAction("Move");
        jump = controls.FindActionMap("Player").FindAction("Jump");
        slide = controls.FindActionMap("Player").FindAction("Slide");
    }
    private void OnEnable()
    {
        movement.Enable();
        jump.Enable();
        slide.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        jump.Disable();
        slide.Disable();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
        normalYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        grounded = IsGrounded();

        //jumping
        if (grounded && jump.triggered)
        { 
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }


        //movement
        Vector2 GlobalmoveDirection = movement.ReadValue<Vector2>().normalized;
        Vector3 moveDirection = transform.right * GlobalmoveDirection.x + transform.forward * GlobalmoveDirection.y;

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
        else
        {
            rb.drag = airDrag;
        }

        //sliding
        if (slide.WasPressedThisFrame() && !sliding)
        {
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z); /// Set crouch scale
            sliding = true;
            rb.drag = 0; /// Remove drag when sliding
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); /// Move the player down
            rb.AddForce(moveDirection * slideSpeed);
        }

        if (sliding && slide.WasReleasedThisFrame())
        {
            sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
            rb.drag = groundDrag;
        }
    }

    private void StateHandler() /// Sets the current player movement state
    {
        // Mode - Crouching
        if (slide.triggered)
        {
            state = MovementState.sliding; /// Set crouching state
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking; /// Set walking state
        }

        // Mode - Air
        else
        {
            state = MovementState.air; /// Set air state
        }
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
