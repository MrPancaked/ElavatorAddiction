//--------------------------------------------------------------------------------------------------
// Description: Manages the elevator sequence, including door animations, sounds, scene transitions
//              to a random scene from a list, and button interactions.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class ElevatorManager : MonoBehaviour
{
    #region Variables

    [Header("Timing Settings")]
    public float DelayBeforeNextState = 4f;   /// Approximate duration of door close animation
    public float DelayBeforeSceneTransition = 3f;  /// Delay before loading the scene
    public float DelayBeforeOpeningTheDoors = 3f;  /// Delay before loading the scene

    [Header("Scenes")]
    public List<string> targetSceneNames;  /// List of scene names to transition to

    [Header("Elevator")]
    public ElevatorSoundController elevatorSoundController;   /// Reference to the ElevatorSoundController
    public CinemachineImpulseSource impulseSource; /// Reference to the screen shake thingy
    public Animator doorAnimator;          /// Animation control for the elevator doors
    public bool isButtonActive = true;      /// Flag to check if the button can be pressed

    [Header("Fog Settings")]
    public float fogTransitionTime = 2f;  /// Duration of the fog transition.
    public string initialFogColorHex = "#ADADAD";
    public float initialFogStartDistance = 10f;
    public float initialFogEndDistance = 60f;
    public string destinationFogColorHex = "#FFFFFF";
    public float destinationFogStartDistance = 3f;
    public float destinationFogEndDistance = 30f;

    // Private Variables
    private bool isElevatorTraveling = false; /// Flag to check if the elevator is currently traveling
    private bool doorIsClosed = true; /// Flag to keep track of the door state for correct triggering
    private string currentTargetScene; /// Private variable to store the selected scene
    private Coroutine currentCoroutine; /// Reference to the currently running coroutine, so you can spam the button and reset it
    private string currentSceneName; /// Variable to store the name of the current scene
    private Color initialFogColor;  /// Initial fog color of the scene
    private Color destinationFogColor; /// Destination fog color of the scene

    #endregion

    #region Unity Methods

    void Awake()  /// Gets the current scene name and Sets initial fog.
    {
        currentSceneName = SceneManager.GetActiveScene().name; // Get the name of the current scene
        SetInitialFog();
    }

    #endregion

    #region Button

    public void ButtonPressed(Vector3 hitPosition)  /// Called when the button is pressed.
    {
        elevatorSoundController.PlayButtonSound(hitPosition);
        if (!doorIsClosed) // Close Doors
        {
            CloseDoors();
            if (currentCoroutine != null) // Override the coroutine if it exists
            {
                StopCoroutine(currentCoroutine);
                Debug.Log("Coroutine overriden");
            }
            currentCoroutine = StartCoroutine(WaitBeforeStartingTheRide()); // Start new coroutine
        }

        else
        {
            OpenDoors();
        }

        if (!isButtonActive || isElevatorTraveling)
        {
            Debug.Log("Button press ignored: inactive or elevator is traveling.");
            return;
        }
    }

    #endregion

    #region Doors

    public void CloseDoors()  /// Closes the elevator doors.
    {
        if (!doorIsClosed)
        {
            doorIsClosed = true;
            doorAnimator.SetTrigger("Close");
            elevatorSoundController.PlayDoorCloseSound();
            Debug.Log("Door is closed");
        }
    }

    public void OpenDoors()  /// Opens the elevator doors.
    {
        if (doorIsClosed)
        {
            doorIsClosed = false;
            doorAnimator.SetTrigger("Open");
            elevatorSoundController.PlayDoorOpenSound();
            Debug.Log("Door is opened");
        }
    }

    #endregion

    #region Elevator Ride

    IEnumerator WaitBeforeStartingTheRide()  /// Waits before starting the elevator ride.
    {
        yield return new WaitForSeconds(DelayBeforeNextState); // Wait for animation and sound to play

        if (doorIsClosed)
        {
            StartCoroutine(ElevatorRideStart());
            currentCoroutine = null; // Clear coroutine reference
        }
    }

    IEnumerator ElevatorRideStart() /// Starts the elevator ride sequence.
    {
        isElevatorTraveling = true; // Start the loop and disable the button
        isButtonActive = false;
        elevatorSoundController.PlayElevatorStart();
        Debug.Log("Button disabled + Elevator is traveling + Started the ride sound");
        yield return new WaitForSeconds(0.5f); // A small delay before screenshake

        impulseSource.GenerateImpulseWithForce(1.5f);
        StartCoroutine(FadeFog(destinationFogColor, destinationFogStartDistance, destinationFogEndDistance));
        yield return new WaitForSeconds(DelayBeforeSceneTransition); // A small delay before scene transition

        currentTargetScene = GetRandomSceneName(); // Select a Random Scene
        StartCoroutine(ElevatorRideEnd());
    }

    IEnumerator ElevatorRideEnd()  /// Handles the elevator arrival in a new scene.
    {
        SceneManager.LoadScene(currentTargetScene);
        elevatorSoundController.PlayElevatorStop();
        Debug.Log("Transitioning to the other sound");
        yield return new WaitForSeconds(0.3f); // A small delay before screenshake

        impulseSource.GenerateImpulseWithForce(0.7f);
        yield return new WaitForSeconds(DelayBeforeOpeningTheDoors);

        SetActiveScene();
        OpenDoors();
        isElevatorTraveling = false;
        isButtonActive = true;
        Debug.Log("Button is active + Elevator not traveling + Scene loaded");
    }

    #endregion

    #region Scenes

    public void SetActiveScene()  /// Get the name of the current scene
    {
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    private string GetRandomSceneName() /// Gets a random scene name from the list, avoiding the current scene.
    {
        string newSceneName;
        do
        {
            int randomIndex = Random.Range(0, targetSceneNames.Count);
            newSceneName = targetSceneNames[randomIndex];
        } while (newSceneName == currentSceneName);
        return newSceneName;
    }

    #endregion

    #region Fog
    private void SetInitialFog()  /// Sets the initial fog settings.
    {  
        if (!ColorUtility.TryParseHtmlString(initialFogColorHex, out initialFogColor)) // Attempts to parse the initial fog color from hex string, logs an error and sets default if invalid.
        {
            Debug.LogError("ElevatorManager: Invalid initial hex color, setting it to white.");
            initialFogColor = Color.white;
        }

        if (!ColorUtility.TryParseHtmlString(destinationFogColorHex, out destinationFogColor)) // Attempts to parse the destination fog color from hex string, logs an error and sets default if invalid.
        {
            Debug.LogError("ElevatorManager: Invalid destination hex color, setting it to white.");
            destinationFogColor = Color.white;
        }
       
        RenderSettings.fogColor = initialFogColor; // Sets the render settings with the initial fog color, start distance, and end distance.
        RenderSettings.fogStartDistance = initialFogStartDistance;
        RenderSettings.fogEndDistance = initialFogEndDistance;
    }

    IEnumerator FadeFog(Color targetColor, float targetStartDistance, float targetEndDistance) /// Animates the fog during elevator rides.
    {
        float timer = 0.0f; // Timer for the fog transition.
        float initialStartDistance = RenderSettings.fogStartDistance; // Caches initial fog start distance.
        float initialEndDistance = RenderSettings.fogEndDistance; // Caches initial fog end distance.
        Color initialColor = RenderSettings.fogColor; // Caches initial fog color.
        while (timer < fogTransitionTime) // Continues until the transition time is reached.
        {
            timer += Time.deltaTime; // Increases timer with each frame.      
            RenderSettings.fogColor = Color.Lerp(initialColor, targetColor, timer / fogTransitionTime); // Lerps fog color from initial to target color based on timer.
            RenderSettings.fogStartDistance = Mathf.Lerp(initialStartDistance, targetStartDistance, timer / fogTransitionTime); // Lerps fog start distance from initial to target distance based on timer.
            RenderSettings.fogEndDistance = Mathf.Lerp(initialEndDistance, targetEndDistance, timer / fogTransitionTime); // Lerps fog end distance from initial to target distance based on timer.
            yield return null; // Waits for the next frame.
        }
    }
    #endregion
}