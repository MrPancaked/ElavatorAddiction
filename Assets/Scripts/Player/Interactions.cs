//--------------------------------------------------------------------------------------------------
//  Description: Handles player interactions with objects using raycasting and UI display of
//               a crosshair. Input actions trigger interaction with the slot machine.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Xml.Serialization;
using Unity.VisualScripting;
public class Interactions : MonoBehaviour
{
    #region Variables

    [Header("References")]
    [SerializeField] private InputActionAsset controls; //Reference to the interact input action
    private InputAction interact; //Reference to the interact input action
    public Camera playerCamera; // Reference to the player's camera.
    public SlotMachine slotMachine; // Reference to the slot machine.
    public ElevatorManager elevatorManager; // Reference to the elevator button.

    [Header("Interactions")]
    public Texture2D crosshairTexture; // Texture for crosshair display.
    public float crosshairScale = 0.6f; // Scale of the crosshair.
    public float interactionDistance = 3f; // Maximum distance to interact with objects.

    // Private Variables
    string SlotMachineTag = "SlotMachine"; // Tag for the slot machine.
    string ElevatorButtonTag = "ElevatorButton"; // Tag for the button in the elevator.
    #endregion

    #region Unity Methods
    /// Initializes the input action and displays an error message if action isn't assigned.
    void Awake()
    {
        interact = controls.FindActionMap("Player").FindAction("Interact"); // Enable the action map
    }
    void Start()
    {
        Cursor.visible = false; // Hide the cursor at start
        Cursor.lockState = CursorLockMode.Locked; // Locks to the center of the screen
    }
    private void OnEnable()
    {
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        Cursor.visible = true;
    }
    /// Handles interaction on update when the input is triggered.
    void Update()
    {
        if (interact != null && interact.triggered)
        {
            Interact();
        }
    }

    /// Handles drawing of the UI Crosshair.
    void OnGUI()
    {
        // Screen dimensions
        float scaledWidth = crosshairTexture.width * crosshairScale;
        float scaledHeight = crosshairTexture.height * crosshairScale;

        // The center of the screen
        float x = (Screen.width - scaledWidth) / 2f;
        float y = (Screen.height - scaledHeight) / 2f;

        // Draw crosshair
        GUI.DrawTexture(new Rect(x, y, scaledWidth, scaledHeight), crosshairTexture);
    }

    /// Disables the input action to prevent memory leaks on destroy.
    void OnDestroy()
    {
        if (interact != null)
        {
            interact.Disable(); // Important: Disable to avoid memory leaks.
        }
        Cursor.visible = true; // Ensure the cursor is visible when script gets destroyed
        Cursor.lockState = CursorLockMode.None; // Unlock cursor when game ends
    }
    #endregion

    #region Interaction Logic
    /// Performs the interaction, raycasting and checking for interactable objects.
    void Interact()
    {
        // Raycast from the center of the screen
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, interactionDistance)) 
        {
            if (hit.collider.CompareTag(SlotMachineTag))
            {
                slotMachine.UseCoinForUpgrade(); // Use the slot machine if raycast hit it
            }
        }
        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            if (hit.collider.CompareTag(ElevatorButtonTag))
            {
                elevatorManager.ButtonPressed(hit.point); // Press the button if raycast hit it, and send the hit position
                return;
            }
        }
        else
        {
            Debug.Log("Raycast didnt hit shit.");
        }
    }
    #endregion
}