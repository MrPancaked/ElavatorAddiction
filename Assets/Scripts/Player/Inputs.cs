using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    [SerializeField] private InputActionAsset controls; /// Input controls
    private InputAction movement; /// Input of the player
    private InputAction jump; /// Input of the player
    private InputAction slide; /// Input of the player

    public Vector2 moveMentInput;
    public bool jumpPressed;
    public bool jumpReleased;
    public bool slidePressed;
    public bool slideReleased;
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

    void Update()
    {
        moveMentInput = movement.ReadValue<Vector2>().normalized;

        if (jump.WasPressedThisFrame())
        {
            jumpPressed = true;
        }
        if (jump.WasReleasedThisFrame())
        {
            jumpReleased = true;
        }
        if (slide.WasPressedThisFrame())
        {
            slidePressed = true;
        }
        if (slide.WasReleasedThisFrame())
        {
            slideReleased = true;
        }
    }

    private void LateUpdate()
    {
        
    }
}
