//--------------------------------------------------------------------------------------------------
//  Description: Handles advanced player movement including walking, sprinting, jumping, crouching,
//               and slope handling.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class MovementAdvancedOld : MonoBehaviour
{
    #region Variables

    [Header("Movement")]
    public float walkSpeed;
    public float sprintSpeed;
    public float groundDrag;
    private float moveSpeed; /// Current move speed of the player
    public float maxSlopeAngle; /// Maximum angle of a slope the player can walk up

    [Header("Controls")]
    [SerializeField] private InputActionAsset controls; /// Input controls
    private InputAction movement; /// Input of the player
    private InputAction jump; /// Input of the player


    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier; /// Multiplier to movement speed when in the air
    bool readyToJump; /// Flag to indicate if the player can jump

    [Header("Crouching")]
    public float crouchSpeed;
    public float playerHeight; /// Height of the player for ground checking
    public float crouchYScale; /// Scale of the player's Y-axis when crouching
    private float startYScale; /// Initial Y-scale of the player

    [Header("Setup")]
    public Transform orientation; /// The orientation of the player
    public LayerMask whatIsGround; /// Layers that are considered ground
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public MovementState state; /// Current movement state of the player

    private RaycastHit slopeHit; /// Stores the information about a slope
    private bool exitingSlope; /// Flag to indicate if the player is exiting a slope
    Vector3 moveDirection; /// Direction of movement
    float horizontalInput; /// Input of horizontal axis
    float verticalInput; /// Input of vertical axis
    bool grounded; /// Flag if the player is grounded
    Rigidbody rb; /// Rigidbody of the player

    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        movement = controls.FindActionMap("Player").FindAction("Move");
        jump = controls.FindActionMap("Player").FindAction("Jump");
    }
    private void OnEnable()
    {
        movement.Enable();
        jump.Enable();
    }
    private void OnDisable()
    {
        movement.Disable();
        jump.Disable();
    }
    private void Start() /// Called once at the beginning of the game. Initializes the Rigidbody and set the ready to jump flag
    {
        rb = GetComponent<Rigidbody>();  /// Get the Rigidbody component
        rb.freezeRotation = true;  /// Freeze rotation of the player

        readyToJump = true; /// The player can jump at start

        startYScale = transform.localScale.y; /// Set initial Y scale
    }

    private void Update() /// Called every frame to get player input, set states, and control speed
    {
        // Ground check using a raycast
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput(); /// Process player input
        SpeedControl(); /// Limit player speed
        StateHandler(); /// Set player movement state

        // Handle drag
        if (grounded)
            rb.drag = groundDrag; /// Apply ground drag when on the ground
        else
            rb.drag = 0; /// Remove drag when not on the ground
    }

    private void FixedUpdate() /// Called every physics update, moves the player
    {
        MovePlayer(); /// Move the player
    }

    #endregion

    #region Input Logic

    private void MyInput() /// Gets input from the player
    {
        horizontalInput = movement.ReadValue<Vector2>().x; /// Get horizontal input
        verticalInput = movement.ReadValue<Vector2>().y; /// Get vertical input

        // When the player jumps
        if (jump.triggered && readyToJump && grounded)
        {
            readyToJump = false; /// Set the jump flag to false
            Jump(); /// Jump
            Invoke(nameof(ResetJump), jumpCooldown); /// Reset jump after the cooldown
        }

        // When the player crouches
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z); /// Set crouch scale
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse); /// Move the player down
        }

        // When the player stops crouching
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z); /// Reset the Y-scale
        }
    }

    #endregion

    #region States Logic

    private void StateHandler() /// Sets the current player movement state
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching; /// Set crouching state
            moveSpeed = crouchSpeed; /// Set crouch speed
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting; /// Set sprinting state
            moveSpeed = sprintSpeed; /// Set sprint speed
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking; /// Set walking state
            moveSpeed = walkSpeed; /// Set walk speed
        }

        // Mode - Air
        else
        {
            state = MovementState.air; /// Set air state
        }
    }

    #endregion

    #region Movement Logic

    private void MovePlayer() /// Moves the player based on the current input and movement state
    {
        // Calculate movement direction based on orientation
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // On slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);
            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // On ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // In air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // Turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl() /// Limits the players current speed
    {
        // Limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed; /// Limit the speed on a slope
        }

        // Limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z); /// Get only horizontal velocity

            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z); /// Limit the player's velocity
            }
        }
    }

    #endregion

    #region Jump and Slope Logic

    private void Jump() /// Makes the player jump
    {
        exitingSlope = true; /// Sets flag that the player is exiting a slope

        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); /// Reset the y velocity

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse); /// Add an impulse to make the player jump
    }

    private void ResetJump() /// Resets jump state
    {
        readyToJump = true; /// Sets flag that the player can jump again

        exitingSlope = false; /// Sets flag that the player is not exiting a slope
    }

    private bool OnSlope() /// Checks if the player is on a slope
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); /// Get the angle of the slope
            return angle < maxSlopeAngle && angle != 0; /// Return true if the slope is not too steep
        }

        return false; /// Return false if the player isn't on a slope
    }

    private Vector3 GetSlopeMoveDirection() /// Gets the slope move direction
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized; /// Project the movement vector onto the slope
    }

    #endregion
}