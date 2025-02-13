using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    #region Variables

    [Header("UI")]
    public TextMeshProUGUI dragStateUI;
    public TextMeshProUGUI dragTextUI;
    public TextMeshProUGUI speedTextUI;
    public TextMeshProUGUI TimerUI;

    [Header("Reference")]
    public LayerMask groundLayer;
    public CinemachineBrain brain;  // Assign this in the Inspector
    public Animator animator;
    public Rigidbody rb;

    [Header("FOV Settings")]
    public float baseFOV = 80f;
    public float maxFovIncrease = 30f;
    public float fovSmoothTime = 0.1f;

    [Header("Movement")]
    public float runSpeed = 200f;
    public float runDrag = 17.5f;
    public float slideDrag = 1.4f;
    public float jumpForce = 13f;
    public float bhopForce = 3f;
    public float slideForce = 4f;
    public float slamForce = 50f;

    [Header("InAir")]
    public float airDrag = 1f;
    public float airSpeed = 20f;
    public float inAirTiming = 0.17f;
    public float groundedTiming = 0.07f;

    [Header("Private stuff")]
    [HideInInspector]
    public float currentSpeed; // Current speed of the player
    private float groundedTimer = 0f; // Spacebar press after landing
    private float inAirTimer = 0f; // Spacebar press in air
    private float edgeTimer = 0f; // Falling from an edge timing
    private float fallPosition;
    private float fallDistance;
    private float targetFOV = 80f;
    private float currentFOVVelocity;
    private bool isGrounded = false;
    private bool isSliding = false;
    private bool wasGrounded = true;
    private bool wantToJump = false;
    private bool canEdgeJump = false;
    private bool canBounceJump = false;
    private bool jumpPressed = false;
    private bool slidePressed = false;
    private bool slideReleased = false;
    private Vector3 moveDirection;
    private Vector3 boxSize = new Vector3(0.5f, 0.15f, 0.5f);

    // Magic numbers
    private const float normalHeight = 0.8f;
    private const float slideHeight = 0.6f;
    private const float slideSpeedTrashhold = 1f;
    private const float runSpeedTrashhold = 4f;
    private const float fovSpeedLimit = 30f;
    private const float slamDistance = 2f;

    // States and singleton
    public enum MovementState
    {
        Idle,
        Running,
        InAir,
        Sliding
    }
    private MovementState currentState;

    public static PlayerMovement Instance { get { return instance; } }
    public static PlayerMovement instance;

    #endregion

    #region Unity Methods

    private void Awake() // Singleton
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

    private void Update()
    {
        InputFlagsSet();
    }

    private void FixedUpdate()
    {
        GroundCheck();

        UpdateMovementState();
        Sounds();
        Drag();
        Jump();
        Movement();
        Slide();
        UI();
        DynamicFOV();
        InputFlagsReset();

        GroundedCheckLastFrame();
    }

    private void UpdateMovementState()
    {
        if (!isGrounded)
        {
            currentState = MovementState.InAir;
        }
        else if (isSliding)
        {
            currentState = MovementState.Sliding;
        }
        else if (currentSpeed > runSpeedTrashhold)
        {
            currentState = MovementState.Running;
        }
        else
        {
            currentState = MovementState.Idle;
        }
    }

    #endregion

    #region Drag

    private void Drag() // Handle drag
    {
        switch (currentState)
        {
            case MovementState.Idle:
            case MovementState.Running:
                rb.drag = runDrag;
                dragStateUI.text = "Grounded";
                break;

            case MovementState.Sliding:
                if (rb.drag < slideDrag)
                {
                    rb.drag += runDrag * 0.01f;
                }
                dragStateUI.text = "Sliding";
                break;

            case MovementState.InAir:
                rb.drag = airDrag;
                dragStateUI.text = "InAir";
                break;
        }
    }

    #endregion

    #region Jumping

    private void Jump()
    {
        JumpCheck();
        EdgeJumpCheck();
        BounceJumpCheck();
        PerformJump();
    }

    private void JumpCheck()
    {
        if (isGrounded)
        {
            currentState = MovementState.Idle;
            canEdgeJump = false;
        }

        if (inAirTimer <= 0)
        {
            wantToJump = false;
        }

        if (jumpPressed)
        {
            wantToJump = true;

            if (!isGrounded)
            {
                inAirTimer = inAirTiming;
            }
        }

        if (wantToJump && !isGrounded)
        {
            inAirTimer = Mathf.Max(inAirTimer - Time.fixedDeltaTime, 0f);
        }
    }

    private void EdgeJumpCheck()
    {
        if (edgeTimer <= 0)
        {
            canEdgeJump = false;
        }

        if (!isGrounded && wasGrounded && rb.velocity.y <= 0)
        {
            canEdgeJump = true;
            edgeTimer = groundedTiming + inAirTiming;
        }

        if (canEdgeJump)
        {
            edgeTimer = Mathf.Max(edgeTimer - Time.fixedDeltaTime, 0f);
        }
    }

    private void BounceJumpCheck()
    {
        if (groundedTimer <= 0)
        {
            canBounceJump = false;
        }

        if (isGrounded && !wasGrounded)
        {
            canBounceJump = true;
            groundedTimer = groundedTiming;
        }

        if (canBounceJump)
        {
            groundedTimer = Mathf.Max(groundedTimer - Time.fixedDeltaTime, 0f);
        }
    }

    private void PerformJump()
    {
        if (wantToJump && (isGrounded || canEdgeJump || canBounceJump))
        {
            if (inAirTimer > 0 || groundedTimer > 0)
            {
                PlayerSounds.Instance.PlayBhopSound();
                rb.AddForce(moveDirection * bhopForce, ForceMode.Impulse);
            }
            else
            {
                PlayerSounds.Instance.PlayJumpSound();
            }

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

            currentState = MovementState.InAir;
            StopSliding();
            wantToJump = false;
            canEdgeJump = false;
            canBounceJump = false;
            slideReleased = true;
            inAirTimer = 0;
            edgeTimer = 0;
            groundedTimer = 0;
        }
    }

    #endregion

    #region Movement

    private void Movement()
    {
        CalculateDirection();
        ApplySpeed();
    }

    private void CalculateDirection()
    {
        moveDirection = transform.right * Inputs.Instance.moveMentInput.x + transform.forward * Inputs.Instance.moveMentInput.y;
    }

    private void ApplySpeed()
    {
        currentSpeed = rb.velocity.magnitude;

        if (isGrounded && !isSliding)
        {
            rb.AddForce(moveDirection * runSpeed);
        }
        else if (!isGrounded)//&& !isSliding)
        {
            rb.AddForce(moveDirection * airSpeed);
        }
    }

    #endregion

    #region Sliding

    private void Slide()
    {
        if (slidePressed && currentSpeed > slideSpeedTrashhold && ! isSliding)
        {
            StartSliding();
        }

        if (slideReleased && isSliding)
        {
            StopSliding();
        }
    }

    private void StartSliding()
    {
        isSliding = true;
        rb.drag = 0;
        rb.AddForce(moveDirection * slideForce, ForceMode.Impulse);
        transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);

        if (!isGrounded)
        {
            GroundSlam();
        }
    }

    private void StopSliding()
    {
        isSliding = false;
        transform.localScale = new Vector3(transform.localScale.x, normalHeight, transform.localScale.z);
    }
    
    private void GroundSlam()
    {
        rb.AddForce(Vector3.down * slamForce, ForceMode.Impulse);
        fallPosition = transform.position.y;
    }

    #endregion

    #region Ground Check

    private void GroundCheck()
    {
        isGrounded = GroundCheckRaycast();
    }

    private void GroundedCheckLastFrame()
    {
        wasGrounded = GroundCheckRaycast();
    }

    public bool GroundCheckRaycast()
    {
        return Physics.OverlapBox(transform.position + (Vector3.down * transform.localScale.y), boxSize / 2, Quaternion.identity, groundLayer).Length > 0;
    }

    #endregion

    #region Audio

    private void SlamSound()
    {
        if (isGrounded && !wasGrounded)
        {
            fallDistance = fallPosition - transform.position.y;

            if (fallDistance >= slamDistance)
            {
                fallPosition = transform.position.y;
                PlayerSounds.Instance.PlaySlamSound();
                ScreenshakeManager.Instance.TriggerShake("slam", overrideForce: 1f, overrideDuration: 0.2f);
            }
        }
    }

    private void AnimatorStates()
    {
        string triggerName = GetAnimationState();
        animator.SetTrigger(triggerName);
    }

    private string GetAnimationState()
    {
        switch (currentState)
        {
            case MovementState.Running:
                return "Running";
            case MovementState.Sliding:
                return "Sliding";
            case MovementState.InAir:
                return "InAir";
            default:
                return "Idle";
        }
    }

    private void AirSound()
    {
        if (isGrounded)
        {
            PlayerSounds.Instance.PlayInAirStop();
        }
        else
        {
            PlayerSounds.Instance.PlayInAirStart();
        }
    }

    private void Sounds()
    {
        SlamSound();
        AnimatorStates();
        AirSound();
    }

    #endregion

    #region Inputs

    private void InputFlagsSet()
    {
        var inputs = Inputs.Instance;
        inputs.moveMentInput = inputs.movement.ReadValue<Vector2>().normalized;

        if (inputs.jump.WasPressedThisFrame())
        {
            jumpPressed = true;
        }

        if (inputs.slide.WasPressedThisFrame())
        {
            slidePressed = true;
        }

        if (inputs.slide.WasReleasedThisFrame())
        {
            slideReleased = true;
        }
    }

    private void InputFlagsReset()
    {
        jumpPressed = false;
        slidePressed = false;
        slideReleased = false;
    }

    #endregion

    #region UI

    private void UI()
    {
        speedTextUI.text = currentSpeed.ToString("F0");
        dragTextUI.text = rb.drag.ToString("F2");

        if (inAirTimer > 0)
        {
            TimerUI.text = DecimalValue(inAirTimer);
        }
        else if (edgeTimer > 0)
        {
            TimerUI.text = DecimalValue(edgeTimer);
        }
        else if (groundedTimer > 0)
        {
            TimerUI.text = DecimalValue(groundedTimer);
        }
        else
        {
            TimerUI.text = "0";
        }
    }

    private string DecimalValue(float timerValue)
    {
        float fractionalPart = timerValue % 1;
        return fractionalPart.ToString("F2").Substring(2);
    }

    #endregion

    #region FOV

    private void DynamicFOV()
    {
        if (brain.ActiveVirtualCamera is CinemachineCamera activeCam)
        {
            float speedPercentage = Mathf.InverseLerp(0f, fovSpeedLimit, currentSpeed);
            float targetFovIncrease = speedPercentage * maxFovIncrease;
            targetFOV = baseFOV + targetFovIncrease;

            activeCam.Lens.FieldOfView = Mathf.SmoothDamp(activeCam.Lens.FieldOfView, targetFOV, ref currentFOVVelocity, fovSmoothTime);
            brain.ManualUpdate();
        }
    }

    #endregion
}