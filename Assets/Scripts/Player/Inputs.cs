//--------------------------------------------------------------------------------------------------
//  Description: Handles player input using the Unity Input System.
//               Tracks movement, jump, and slide inputs.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inputs : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private InputActionAsset controls; // Reference to the Input Action Asset

    // private shit
    [HideInInspector]
    public Vector2 moveMentInput; // Normalized movement input vector
    [HideInInspector]
    public InputAction interaction; // Action for player slide
    [HideInInspector]
    public InputAction reload; // Action for player slide
    [HideInInspector]
    public InputAction shoot; // Action for player slide
    [HideInInspector]
    public InputAction selectItem1; /// Input action to switch to item 1
    [HideInInspector]
    public InputAction selectItem2; /// Input action to switch to item 2
    [HideInInspector]
    public InputAction movement; // Action for player movement
    [HideInInspector]
    public InputAction jump; // Action for player jump
    [HideInInspector]
    public InputAction slide; // Action for player slide

    static Inputs instance;
    public static Inputs Instance
    {
        get { return instance; }   
    }

    #endregion

    #region Unity Methods

    private void Awake() // Get references to the input actions from the "Player" action map
    {
        if (instance == null) // Singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        movement = controls.FindActionMap("Player").FindAction("Move");
        jump = controls.FindActionMap("Player").FindAction("Jump");
        slide = controls.FindActionMap("Player").FindAction("Slide");
        interaction = controls.FindActionMap("Player").FindAction("Interact");
        reload = controls.FindActionMap("Player").FindAction("Reload");
        shoot = controls.FindActionMap("Player").FindAction("Shoot");
        selectItem1 = controls.FindActionMap("Player").FindAction("SelectItem1");
        selectItem2 = controls.FindActionMap("Player").FindAction("SelectItem2");
    }

    private void OnEnable() // Enable the input actions
    {    
        movement.Enable();
        jump.Enable();
        slide.Enable();
        interaction.Enable();
        reload.Enable();
        shoot.Enable();
        selectItem1.Enable();
        selectItem2.Enable();
    }

    private void OnDisable() // Disable the input actions
    {
        movement.Disable();
        jump.Disable();
        slide.Disable();
        interaction.Disable();
        reload.Disable();
        shoot.Disable();
        selectItem1.Disable();
        selectItem2.Disable();
    }

    //void Update()
    //{   
    //    moveMentInput = movement.ReadValue<Vector2>().normalized; // Read and normalize the movement input
    //
    //    if (jump.WasPressedThisFrame()) // Check for jump press
    //    {
    //        jumpPressed = true;
    //    }
    //   
    //    if (jump.WasReleasedThisFrame()) // Check for jump release
    //    {
    //        jumpReleased = true;
    //    }
    //   
    //    if (slide.WasPressedThisFrame()) // Check for slide press
    //    {
    //        slidePressed = true;
    //    }
    //   
    //    if (slide.WasReleasedThisFrame()) // Check for slide release
    //    {
    //        slideReleased = true;
    //    }
    //}

    #endregion
}