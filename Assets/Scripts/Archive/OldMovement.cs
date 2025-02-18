using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class OldMovement : MonoBehaviour
{
    private Rigidbody rb;
    private Animator animator; // Added Animator variable
    public TextMeshProUGUI speedText;

    [Header("Movement")]
    public float speed = 200f;
    public float airMultiplier = 0.08f;
    public float groundDrag = 17.5f;
    public float airDrag = 1f;

    [Header("Jump")]
    public float jumpForce = 10f;
    public float jumpTiming = 0.15f;
    public float bhopBoost = 3f;
    public float stompTimer = 1f; // Time in air for stomp sound

    [Header("Slide")]
    public float slideYScale = 0.6f;
    public float slideSpeed = 2f;
    public float slideDragIncrease = 0.006f;
    public float downForce = 49f;

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
    private float currentSpeed; // Current speed of the player
    private float jumpTimer;
    private bool jumping;
    private bool jumpAvailable;
    private bool grounded;
    private bool sliding;
    private LayerMask groundLayer;
    private float timeInAir; // Timer for stomp sound
    private bool wasGroundedLastFrame; // Store grounded state of last frame
    public static OldMovement instance;
    public static OldMovement Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        Inputs.Instance.moveMentInput = Inputs.Instance.movement.ReadValue<Vector2>().normalized;

        if (Inputs.Instance.jump.WasPressedThisFrame())
        {
            jumpPressed = true;
        }

        if (Inputs.Instance.jump.WasReleasedThisFrame())
        {
            jumpReleased = true;
        }

        if (Inputs.Instance.slide.WasPressedThisFrame())
        {
            slidePressed = true;
        }

        if (Inputs.Instance.slide.WasReleasedThisFrame())
        {
            slideReleased = true;
        }

        speedText.text = currentSpeed.ToString("F0");
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        groundLayer = LayerMask.GetMask("Ground");
        normalYScale = transform.localScale.y;
        timeInAir = 0f; // Initialize air time
        wasGroundedLastFrame = true;
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

        // Check if grounded
        bool currentGrounded = IsGrounded();
        grounded = currentGrounded;

        // Reset air time when landing
        if (grounded && !wasGroundedLastFrame)
        {
            if (timeInAir >= stompTimer)
            {
                PlayerSounds.Instance.PlaySlamSound(); // Play stomp sound
                ScreenshakeManager.Instance.TriggerShake("Slam");
                ScreenshakeManager.Instance.TriggerShake("slam", overrideForce: 1f, overrideDuration: 0.2f);
            }
            timeInAir = 0f;
        }

        // Update air time if SLIDING DOWN THE AIR
        if (sliding && !grounded)
        {
            timeInAir += Time.fixedDeltaTime;
        }
        wasGroundedLastFrame = currentGrounded; // Store state for next frame

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
            rb.drag += slideDragIncrease * groundDrag;
        }

        //jumping
        if (grounded && jumping)
        {
            jumping = false;
            jumpAvailable = false;
            PlayerSounds.Instance.PlayJumpSound();
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
            transform.localScale = new Vector3(transform.localScale.x, slideYScale, transform.localScale.z);
            sliding = true;
            rb.drag = 0;
            rb.AddForce(Vector3.down * downForce, ForceMode.Impulse);
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

        // In air sound
        if (grounded)
        {
            PlayerSounds.Instance.PlayInAirStop();
        }
        else
        {
            PlayerSounds.Instance.PlayInAirStart();
        }

        currentSpeed = rb.velocity.magnitude; // Calculate the current speed

        SwitchAnimationStates();

        //end of fixedupdate
        jumpPressed = false;
        jumpReleased = false;
        slidePressed = false;
        slideReleased = false;
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

    public bool IsGrounded()
    {
        return Physics.OverlapBox(transform.position + (Vector3.down * transform.localScale.y), boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(transform.position + (Vector3.down * transform.localScale.y), boxSize);
    }

    public void SwitchAnimationStates()
    {
        if (grounded && !jumping && sliding)
        {
            animator.SetTrigger("Slide");
        }
        else if (!grounded && jumping && !sliding && jumpAvailable)
        {
            animator.SetTrigger("Jump");
        }
        else if (grounded && !jumping && !sliding && moveDirection.magnitude > 0.9f)
        {
            animator.SetTrigger("Running");
        }
        else
        {
            animator.SetTrigger("Idle");
        }
    }
}