//--------------------------------------------------------------------------------------------------
//  Description: Handles player interactions with objects using raycasting and UI display of
//               a crosshair. Input actions trigger interaction with the slot machine.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine.Windows;
public class Interactions : MonoBehaviour
{
    #region Variables

    // Private Variables
    private Camera playerCamera; // Reference to the player's camera.
    private string coinTag = "Coin"; // Tag for coin objects
    private string SlotMachineTag = "SlotMachine"; // Tag for the slot machine.
    private string ElevatorButtonTag = "ElevatorButton"; // Tag for the button in the elevator.
    private string ElevatorLeverTag = "ElevatorLever"; // Tag for the lever outside the elevator.
    private Animator buttonAnimator;
    public static Interactions instance;
    public static Interactions Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            playerCamera = Camera.main; // Find the main camera
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        Cursor.visible = false; // Hide the cursor at start
        Cursor.lockState = CursorLockMode.Locked; // Locks to the center of the screen
    }

    void Update() /// Handles interaction on update when the input is triggered.
    {
        if (Inputs.Instance.interaction.WasPressedThisFrame())
        {
            Interact();
        }
    }

    private void OnEnable()
    {
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        Cursor.visible = true;
    }

    void OnDestroy() /// Disables the input action to prevent memory leaks on destroy.
    {
        Cursor.visible = true; // Ensure the cursor is visible when script gets destroyed
        Cursor.lockState = CursorLockMode.None; // Unlock cursor when game ends
    }

    #endregion

    #region Collecting Items Logic

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(coinTag)) // If the object has a coin tag
        {
            CoinsLogic.Instance.CollectCoin(); // Collect the coin
        }
    }

    #endregion

    #region Interaction Logic

    void Interact() /// Performs the interaction, raycasting and checking for interactable objects.
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Raycast from the center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.CompareTag(SlotMachineTag))
            {
                SlotMachine.Instance.UseCoinForUpgrade();
            }
            else if (hit.collider.CompareTag(ElevatorButtonTag))
            {

                ElevatorController.Instance.ButtonPressed();
            }
            else if (hit.collider.CompareTag(ElevatorLeverTag))
            {
                ElevatorController.Instance.LeverPressed();
            }
            else
            {
                Debug.Log("Uninteractable");
            }
        }
    }

    #endregion
}