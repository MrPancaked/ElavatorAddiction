using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewMovement : MonoBehaviour
{
    private Rigidbody rb;

    [Header("Movement")]
    public float speed;
    public float airMultiplier;
    public float groundDrag;
    public float airDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpTiming;
    public float bhopBoost;

    [Header("Sliding")]
    public float slideYScale;
    public float slideSpeed;
    public float slideDragIncrease;
    public float downForce;

    [Header("Ground Detection")]
    public Vector3 boxSize;

    //private shit    
    [HideInInspector]
    public bool jumpPressed = false;
    [HideInInspector]
    public bool jumpReleased = false;
    [HideInInspector]
    public bool slidePressed = false;
    [HideInInspector]
    public bool slideReleased = false;
    [HideInInspector]
    public Vector3 moveDirection;
    private float normalYScale;
    private float jumpTimer;
    private bool jumping;
    private bool jumpAvailable;
    private bool grounded;
    private bool sliding;
    private LayerMask groundLayer;
    public static NewMovement instance;
    public static NewMovement Instance { get { return instance; } }

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update() // I HAD TO MOVE THIS HERE BECAUSE THE INPUTS HAVE TO NOT HAVE AN UPDATE METHOD BECAUSE OF SINGLETONES AND SHIT PLEASE RENE ITS 5 AM!!!!!
    {
        Inputs.Instance.moveMentInput = Inputs.Instance.movement.ReadValue<Vector2>().normalized; // Read and normalize the movement input

        if (Inputs.Instance.jump.WasPressedThisFrame()) // Check for jump press
        {
            jumpPressed = true;
        }

        if (Inputs.Instance.jump.WasReleasedThisFrame()) // Check for jump release
        {
            jumpReleased = true;
        }

        if (Inputs.Instance.slide.WasPressedThisFrame()) // Check for slide press
        {
            slidePressed = true;
        }

        if (Inputs.Instance.slide.WasReleasedThisFrame()) // Check for slide release
        {
            slideReleased = true;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
        normalYScale = transform.localScale.y;
    }

    void FixedUpdate()
    {
        if (jumpPressed)
        {
            jumping = true;
            ResetJumpTimer();
        }

        if (grounded != IsGrounded() && rb.velocity.y <= 0)
        {
            jumpAvailable = true;
            ResetJumpTimer();
        }

        if (jumping || jumpAvailable)
        {
            jumpTimer -= Time.fixedDeltaTime;
        }

        if (jumpTimer <= 0)
        {
            JumpReset();
            ResetJumpTimer();
        }

        grounded = IsGrounded();

        //drag
        if (grounded && !sliding)
        {
            rb.drag = groundDrag;
        }

        else if (!sliding || !grounded)
        {
            rb.drag = airDrag;
        }

        if (sliding && grounded && (rb.drag < groundDrag))
        {
            rb.drag += slideDragIncrease * groundDrag; /// Increase drag when sliding
        }

        //jumping
        if (grounded && jumping)
        {
            jumping = false;
            jumpAvailable = false;
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.AddForce(moveDirection * bhopBoost, ForceMode.Impulse);
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
        moveDirection = transform.right * Inputs.Instance.moveMentInput.x + transform.forward * Inputs.Instance.moveMentInput.y;

        if (grounded && !sliding)
        {
            rb.AddForce(moveDirection * speed);
        }
        else
        {
            rb.AddForce(moveDirection * speed * airMultiplier);
        }

        //sliding
        if (slidePressed && !sliding)
        {
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z); /// Set crouch scale
            sliding = true;
            rb.drag = 0; /// Remove drag when sliding
            rb.AddForce(Vector3.down * downForce, ForceMode.Impulse); /// Move the player down
            if (grounded)
            {
                rb.AddForce(moveDirection * slideSpeed, ForceMode.Impulse);
            }
            
        }

        if (sliding && slideReleased)
        {
            sliding = false;
            transform.localScale = new Vector3(transform.localScale.x, normalYScale, transform.localScale.z);
            rb.drag = groundDrag;
        }

        //end of fixedupdate
        jumpPressed = false;
        jumpReleased = false;
        slidePressed = false;
        slideReleased = false;

        //Debug.Log(rb.drag); THIS SHIT IS ANNOYINGGGG!!!!!!      oh hi Rene:3
    }

    private void JumpReset()
    {
        jumping = false;
        jumpAvailable = false;
    }
    private void ResetJumpTimer()
    { 
        jumpTimer = jumpTiming;
    }

    public bool IsGrounded() // Cast a ray downward from the player's position
    {
        return Physics.OverlapBox(transform.position + (Vector3.down * transform.localScale.y), boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
    }

    public void OnDrawGizmos() // Draw the ray in the Scene view for debugging
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + (Vector3.down * transform.localScale.y), boxSize);
    }
}
