//--------------------------------------------------------------------------------------------------
//  Description: Handles player interactions with objects using raycasting and UI display of
//               a crosshair. Input actions trigger interaction with the slot machine.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class Interactions : MonoBehaviour
{
    #region Variables

    [Header("Interaction Settings")]
    public InputActionReference interactActionReference; //Reference to the interact input action
    public Camera playerCamera; // Reference to the player's camera.
    public SlotMachine slotMachine;
    public float interactionDistance = 3f; // Maximum distance to interact with objects.
    string SlotMachineTag = "SlotMachine"; // Tag for the slot machine.
    private InputAction interactAction; //Input action for interact

    [Header("Crosshair Settings")]
    public Texture2D crosshairTexture; // Texture for crosshair display.
    public float crosshairScale = 0.6f; // Scale of the crosshair.
 
    #endregion

    #region Unity Methods
    /// Initializes the input action and displays an error message if action isn't assigned.
    void Start()
    {
        if (interactActionReference != null)
        {
            interactAction = interactActionReference.action;
            interactAction.Enable();  // Important: Enable the action.
        }
        else
        {
            Debug.LogError("Interact Action Reference not assigned in the Inspector!");
        }

    }

    /// Handles interaction on update when the input is triggered.
    void Update()
    {
        if (interactAction != null && interactAction.triggered)
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
        if (interactAction != null)
        {
            interactAction.Disable(); // Important: Disable to avoid memory leaks.
        }
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
        else
        {
            Debug.Log("Raycast didnt hit shit.");
        }
    }
    #endregion
}