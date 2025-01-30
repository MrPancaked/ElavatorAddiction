using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

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
    private bool playerInElevator = false; // Track if player is in elevator
    private Coroutine closedDoorCoroutine; // Reference to the current coroutine
    private string currentSceneName; // String to store the current scene name
    private static ElevatorController instance;
    public static ElevatorController Instance { get { return instance; } }
    public int RoomIndex = 0;
    public TextMeshProUGUI roomText;

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject); // Prevent the game object from being destroyed in different scenes.
            currentSceneName = SceneManager.GetActiveScene().name; // Set current scene name
            roomText = GameObject.FindGameObjectWithTag("RoomText").GetComponent<TextMeshProUGUI>();
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
            else
            {
                OpenDoors();
            }
        }
    }

    public void LeverPressed() // I HAD TO ADD THIS BECAUSE I CAN NOT CALL THE COROUTINE DIRECTLY FROM THE INTERACTIONS SCRIPT
    {
        StartCoroutine(LeverCoroutine());
    }

    public IEnumerator LeverCoroutine()
    {
        if (!leverAnimationPlaying && CoinsLogic.Instance.playerCoins >= CoinsLogic.Instance.spinCost)
        {
            leverAnimationPlaying = true;
            leverAnimator.SetTrigger("Start");
            ElevatorSounds.Instance.PlayLeverDownSound();
            yield return new WaitForSeconds(0.8f); // Time delay

            StartCoroutine(CoinsLogic.Instance.UseCoinForUpgrade());
            ElevatorSounds.Instance.PlayLeverUpSound();
            yield return new WaitForSeconds(1.8f); // Time delay

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
        EnemyCounter.Instance.InitiatlizeEnemyCount(); // Update the enemy counter
    }

    public IEnumerator CloseDoors()
    {
        isButtonActive = false; // Disable the button
        Debug.Log("Door close / Sound / Animation");
        ElevatorSounds.Instance.PlayDoorCloseSound(); // Play door close sound
        doorAnimator.SetTrigger("Close"); // Trigger door close animation
        yield return new WaitForSeconds(2.4f); // Time delay before elevator start
        isButtonActive = true; // Enable the button
        doorIsClosed = true; // Set door as closed

        if (doorIsClosed && playerInElevator && EnemyCounter.Instance.enemyCount == 0)// If the player is in the elevator START THE FUCKING MACHINE
        {
            Debug.Log("Ride started / Button disabled / Start sound");
            isButtonActive = false; // Disable the button
            ElevatorSounds.Instance.PlayElevatorStart(); //play elevator start sound
            yield return new WaitForSeconds(0.5f); // Makes the screenshake match the sound, otherwise its useless

            Debug.Log("Screen shake / Fog transition");
            SceneSettings destinationSettings = TransitionManager.Instance.GetSceneSettings(currentSceneName); // get next scene settings
            ScreenshakeManager.Instance.TriggerShake("elevator", overrideForce: 1.5f, overrideDuration: 0.8f);
            TransitionManager.Instance.StartFogTransition(destinationSettings, 5f); //Start fog transition
            yield return new WaitForSeconds(2f); // Time delay before scene transition

            Debug.Log("Screen shake / Scene load / Stop sound");
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destinationSettings.sceneName); //load the scene async
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            RoomIndex++;
            UpdateRoomIndex();
            LevelGenerator.Instance.GenerateLevel(); // Call the GenerateLevel method
            EnemySpawner.Instance.SpawnEnemies();
            ElevatorSounds.Instance.PlayElevatorStop(); //Play the elevator stop sound
            yield return new WaitForSeconds(0.3f); // Makes the screenshake match the sound, otherwise its useless
            ScreenshakeManager.Instance.TriggerShake("elevator", overrideForce: 0.7f, overrideDuration: 0.8f); // Trigger screen shake
            yield return new WaitForSeconds(1.5f); // wait for the delay before opening the doors

            Debug.Log("Scene set / Door open / Button is active / Coroutine stop"); // Debug message
            currentSceneName = SceneManager.GetActiveScene().name; //Set active scene variable
            OpenDoors(); // open the doors
            isButtonActive = true; // Enable the button
            closedDoorCoroutine = null; //Reset current coroutine
        }
    }

    #endregion
    public void UpdateRoomIndex()
    {
        roomText.text = "Floor: " + (-RoomIndex).ToString();
    }
}