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
    private float buttonCooldown = 2.4f; // 2-second cooldown
    private float nextButtonPressTime = 0f;  // The next time the button can be pressed
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

    #region Interaction Logic

    void Interact() /// Performs the interaction, raycasting and checking for interactable objects.
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)); // Raycast from the center of the screen
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3f))
        {
            if (hit.collider.CompareTag("ElevatorButton"))
            {
                if (Time.time >= nextButtonPressTime)
                {
                    ElevatorController.Instance.ButtonPressed();
                    nextButtonPressTime = Time.time + buttonCooldown;
                    if (hit.collider.GetComponent<Animator>() != null)
                    {
                        hit.collider.GetComponent<Animator>().SetTrigger("Press");
                    }
                }
                else
                {
                    Debug.Log("Button is on cooldown"); // Optional debug message
                }
            }

            else if (hit.collider.CompareTag("ElevatorLever"))
            {
                ElevatorController.Instance.StartCoroutine(ElevatorController.Instance.LeverPressed());
            }

            else if (hit.collider.CompareTag("Book"))
            {
                DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ReadBook());
            }

            else
            {
                Debug.Log("Uninteractable");
            }
        }
    }

    #endregion

    #region Collecting Items Logic

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin")) // If the object has a coin tag
        {
            CoinsLogic.Instance.CollectCoin(); // Collect the coin
        }
    }

    #endregion
}