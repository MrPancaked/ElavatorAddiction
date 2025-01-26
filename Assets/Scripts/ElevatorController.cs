using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ElevatorController : MonoBehaviour
{

    #region Header Variables

    [Header("Refences")]
    public Animator doorAnimator; // Reference to the door animator
    public Animator leverAnimator; // Reference to the door animator


    // Private stuff
    [HideInInspector]
    public bool isButtonActive = true; // Bool to determine if button can be used
    [HideInInspector]
    public bool doorIsClosed = true; // Bool to determine if door is closed
    private bool leverAnimationPlaying = false; // Bool to determine if the lever is animating or na
    private Coroutine closedDoorCoroutine; // Reference to the current coroutine
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

    #region Button/Lever Interaction

    public void ButtonPressed()
    {
        

        if (!isButtonActive) // Ignore the button if its deactivated (the elevator is traveling)
        {
            Debug.Log("Button press ignored"); // Button press ignored message
            return;
        }
        else
        {
            if (closedDoorCoroutine != null)
            {
                StopCoroutine(closedDoorCoroutine); // Stop coroutine if one is running
                Debug.Log("Coroutine stopped"); // Debug message
            }

            if (!doorIsClosed)
            {
                closedDoorCoroutine = StartCoroutine(CloseDoors()); //Start the wait before the ride coroutine
            }
            else OpenDoors();
        }
    }

    public void LeverPressed() // I HAD TO ADD THIS BECAUSE I CAN NOT CALL THE COROUTINE DIRECTLY FROM THE INTERACTIONS SCRIPT
    {
        StartCoroutine(LeverCoroutine());
    }

    public IEnumerator LeverCoroutine()
    {
        if (!leverAnimationPlaying)
        {
            leverAnimationPlaying = true; 
            leverAnimator.SetTrigger("Start");
            ElevatorSounds.Instance.PlayLeverDownSound();
            yield return new WaitForSeconds(0.8f); // Time delay before elevator start

            ButtonPressed();
            yield return new WaitForSeconds(0.3f); // Time delay before elevator start

            ElevatorSounds.Instance.PlayLeverUpSound();
            yield return new WaitForSeconds(1f); // Time delay before elevator start

            leverAnimationPlaying = false;
        }
    }

    #endregion

    #region Door Control

    public void OpenDoors()
    {
        ElevatorSounds.Instance.PlayDoorOpenSound(); // Play door open sound
        doorAnimator.SetTrigger("Open"); // Trigger door open animation
        doorIsClosed = false; //Set door as open
    }

    public IEnumerator CloseDoors()
    {
        Debug.Log("Door close / Sound / Animation");
        doorIsClosed = true; // Set door as closed
        ElevatorSounds.Instance.PlayDoorCloseSound(); // Play door close sound
        doorAnimator.SetTrigger("Close"); // Trigger door close animation
        yield return new WaitForSeconds(3f); // Time delay before elevator start

        if (doorIsClosed && playerInElevator)// If the player is in the elevator START THE FUCKING MACHINE
        {
            Debug.Log("Ride started / Button disabled / Start sound");
            isButtonActive = false; // Disable the button
            ElevatorSounds.Instance.PlayElevatorStart(); //play elevator start sound
            yield return new WaitForSeconds(0.5f); // Makes the screenshake match the sound, otherwise its useless

            Debug.Log("Screen shake / Fog transition");
            SceneSettings destinationSettings = TransitionManager.Instance.GetSceneSettings(currentSceneName); // get next scene settings
            TransitionManager.Instance.ScreenShake(1.5f); // Trigger screen shake
            TransitionManager.Instance.StartFogTransition(destinationSettings, 5f); //Start fog transition
            yield return new WaitForSeconds(6f); // Time delay before scene transition

            Debug.Log("Screen shake / Scene load / Stop sound");
            TransitionManager.Instance.LoadNewScene(destinationSettings.sceneName); //load the next scene
            ElevatorSounds.Instance.PlayElevatorStop(); //Play the elevator stop sound
            yield return new WaitForSeconds(0.3f); // Makes the screenshake match the sound, otherwise its useless
            TransitionManager.Instance.ScreenShake(0.7f); // Trigger screen shake
            yield return new WaitForSeconds(2f); // wait for the delay before opening the doors

            Debug.Log("Scene set / Door open / Button is active / Coroutine stop"); // Debug message
            currentSceneName = SceneManager.GetActiveScene().name; //Set active scene variable
            OpenDoors(); // open the doors
            isButtonActive = true; // Enable the button
            closedDoorCoroutine = null; //Reset current coroutine
        }
    }

    #endregion
}