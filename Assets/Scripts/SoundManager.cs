//--------------------------------------------------------------------------------------------------
// Description: Manages the elevator sound logic, using FMOD parameters to transition between
//              elevator start and stop sounds.  Also manages ambience based on scene name.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    #region Variables

    [Header("Elevator Sounds")]
    public EventReference rideSound;
    public EventReference roomToneSound;

    [Header("Ambience Sounds")]
    public EventReference ambienceEvent;

    // Private Variables
    private EventInstance elevatorRideInstance;
    private EventInstance roomToneInstance;
    private EventInstance ambienceInstance;
    private string currentSceneName = "";  // Store the current scene name
    private bool hasStarted = false;
    private static SoundManager instance;
    public static SoundManager Instance { get { return instance; } }

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
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        elevatorRideInstance = AudioManager.instance.CreateInstance2D(rideSound); // Initialize the ride event
        roomToneInstance = AudioManager.instance.CreateInstance(roomToneSound, this.transform.position); // Initialize the roomtone event
        ambienceInstance = AudioManager.instance.CreateInstance2D(ambienceEvent); // Initialize the ambience event

        UpdateAmbience(SceneManager.GetActiveScene().name); // initialize the correct ambience using scene name

        roomToneInstance.start(); // Play the roomtone event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAmbience(scene.name); // Update ambience using scene name
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy()
    {
        if (elevatorRideInstance.isValid())
        {
            elevatorRideInstance.release();
        }
        if (roomToneInstance.isValid())
        {
            roomToneInstance.release();
        }
        if (ambienceInstance.isValid())
        {
            ambienceInstance.release();
        }
    }

    #endregion

    #region Elevator Sounds

    public void PlayElevatorStart()
    {
        if (!hasStarted)
        {
            elevatorRideInstance.start();
            hasStarted = true;
        }
        SetRideState(0f);
    }

    public void PlayElevatorStop()
    {
        SetRideState(1f);
        hasStarted = false;
    }

    private void SetRideState(float parameterValue)
    {
        elevatorRideInstance.setParameterByName("State", parameterValue);
    }

    #endregion

    #region Ambience Sounds

    public void UpdateAmbience(string sceneName)
    {
        if (sceneName == currentSceneName) return; // Prevent redundant calls

        float parameterValue;
        switch (sceneName)
        {
            case "Menu":
                Debug.Log("Ambience set to Menu");
                parameterValue = 0f;
                break;
            case "Void":
            case "GrassVoid":
                Debug.Log("Ambience set to Void");
                parameterValue = 1f;
                break;
            case "Forest":
                Debug.Log("Ambience set to Forest");
                parameterValue = 2f;
                break;
            default:
                Debug.LogWarning($"No Ambience scene state set for scene with name {sceneName}");
                parameterValue = 0f; // Default value so its not left unassigned
                break;
        }

        ambienceInstance.setParameterByName("Scene", parameterValue); // Set the sound
        currentSceneName = sceneName; // Update scene name after setting

        if (ambienceInstance.isValid())
        {
            ambienceInstance.start();
        }
    }

    #endregion
}