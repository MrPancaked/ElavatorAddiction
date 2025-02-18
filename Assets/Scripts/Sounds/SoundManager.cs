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
        ambienceInstance = AudioManager.instance.CreateInstance2D(ambienceEvent); // Initialize the ambience event
        roomToneInstance = AudioManager.instance.CreateInstance(roomToneSound, this.transform.position); // Initialize the roomtone event
        roomToneInstance.start(); // Play room tone

        UpdateAmbience(SceneManager.GetActiveScene().name); // Ensure ambience updates before playing
        ambienceInstance.start(); // Start ambience after setting parameter
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAmbience(scene.name);
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
        float parameterValue;

        switch (sceneName)
        {
            case "Menu":
                parameterValue = 0f;
                break;
            case "Void":
                parameterValue = 1f;
                break;
            case "Forest":
                parameterValue = 2f;
                break;
            case "GrassVoid":
                parameterValue = 3f;
                break;
            default:
                parameterValue = 0f;
                Debug.LogWarning($"No Ambience scene state set for scene with name {sceneName}");
                break;
        }

        Debug.Log($"Ambience set to {sceneName}");
        currentSceneName = sceneName; // Update stored scene name
        ambienceInstance.setParameterByName("Scene", parameterValue);
    }


    #endregion
}