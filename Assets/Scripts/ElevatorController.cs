using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{

    #region Header Variables

    [Header("Refences")]
    public Animator doorAnimator; // Reference to the door animator

    [Header("Timing Settings")]
    public float DelayBeforeNextState = 4f;  // Time delay before moving to the next elevator state
    public float DelayBeforeSceneTransition = 3f; // Time delay before scene transition
    public float DelayBeforeOpeningTheDoors = 3f; // Time delay before doors open

    // Private stuff
    [HideInInspector]
    public bool isButtonActive = true; // Bool to determine if button can be used
    private bool isElevatorTraveling = false; // Bool to determine if elevator is moving
    private bool doorIsClosed = true; // Bool to determine if door is closed
    private Coroutine currentCoroutine; // Reference to the current coroutine
    private string currentSceneName; // String to store the current scene name
    private static ElevatorController instance;
    public static ElevatorController Instance { get { return instance; } }
    private bool playerInElevator = false; // Track if player is in elevator

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            currentSceneName = SceneManager.GetActiveScene().name; // Set current scene name
        }
    }

    void OnTriggerEnter(Collider other) // Handle player entering/exiting the elevator collider
    {
        if (other.CompareTag("Player"))
        {
            playerInElevator = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInElevator = false;
        }
    }

    #endregion

    #region Button Interaction

    public void ButtonPressed(Vector3 hitPosition)
    {
        ElevatorSounds.Instance.PlayButtonSound(hitPosition); // Play the sound of button press

        if (!doorIsClosed)
        {
            CloseDoors(); // Close the doors
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); // Stop coroutine if one is running
                Debug.Log("Coroutine overriden"); // Debug message
            }
            currentCoroutine = StartCoroutine(WaitBeforeStartingTheRide()); //Start the wait before the ride coroutine
        }
        else
        {
            OpenDoors(); // If the door is closed, Open the doors
        }

        if (!isButtonActive || isElevatorTraveling)
        {
            Debug.Log("Button press ignored: inactive or elevator is traveling."); // Button press ignored message
            return;
        }
    }

    #endregion

    #region Door Control

    public void CloseDoors()
    {
        if (!doorIsClosed)
        {
            doorIsClosed = true; // Set door as closed
            doorAnimator.SetTrigger("Close"); // Trigger door close animation
            ElevatorSounds.Instance.PlayDoorCloseSound(); // Play door close sound
            Debug.Log("Door is closed"); // Debug message
        }
    }

    public void OpenDoors()
    {
        if (doorIsClosed)
        {
            doorIsClosed = false; //Set door as open
            doorAnimator.SetTrigger("Open"); // Trigger door open animation
            ElevatorSounds.Instance.PlayDoorOpenSound(); // Play door open sound
            Debug.Log("Door is opened"); // Debug message
        }
    }

    #endregion

    #region Coroutines

    IEnumerator WaitBeforeStartingTheRide()
    {
        yield return new WaitForSeconds(DelayBeforeNextState); // Wait for delay

        // ADDED: Check if the player is in the elevator
        if (doorIsClosed && playerInElevator)
        {
            StartCoroutine(ElevatorRideStart()); // Start elevator ride
            currentCoroutine = null; //Reset current coroutine
        }
    }

    IEnumerator ElevatorRideStart()
    {
        isElevatorTraveling = true; // Set elevator as travelling
        isButtonActive = false; // Disable the button

        ElevatorSounds.Instance.PlayElevatorStart(); //play elevator start sound
        Debug.Log("Button disabled + Elevator is traveling + Started the ride sound"); // Debug message
        yield return new WaitForSeconds(0.5f);

        SceneSettings destinationSettings = TransitionManager.Instance.GetSceneSettings(currentSceneName); // get next scene settings

        TransitionManager.Instance.ScreenShake(1.5f); // Trigger screen shake
        TransitionManager.Instance.StartFogTransition(destinationSettings, 5f); //Start fog transition
        yield return new WaitForSeconds(DelayBeforeSceneTransition); // Wait before loading next scene

        StartCoroutine(ElevatorRideEnd(destinationSettings)); // start elevator end coroutine
    }

    IEnumerator ElevatorRideEnd(SceneSettings destinationSettings)
    {
        TransitionManager.Instance.LoadNewScene(destinationSettings.sceneName); //load the next scene
        ElevatorSounds.Instance.PlayElevatorStop(); //Play the elevator stop sound
        Debug.Log("Transitioning to the other sound"); // Debug message
        yield return new WaitForSeconds(0.3f);

        TransitionManager.Instance.ScreenShake(0.7f); // Trigger screen shake
        yield return new WaitForSeconds(DelayBeforeOpeningTheDoors); // wait for the delay before opening the doors

        SetActiveScene(); // Set active scene variable
        OpenDoors(); // open the doors
        isElevatorTraveling = false; // Elevator is no longer traveling
        isButtonActive = true; // Enable the button
        Debug.Log("Button is active + Elevator not traveling + Scene loaded"); // Debug message
    }

    #endregion

    #region Scene Control

    public void SetActiveScene()
    {
        currentSceneName = SceneManager.GetActiveScene().name; //Set active scene variable
    }

    #endregion
}