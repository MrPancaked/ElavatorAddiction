//--------------------------------------------------------------------------------------------------
// Description: Manages the elevator sound logic, using FMOD parameters to transition between
//              elevator start and stop sounds.
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
    private int currentSceneIndex = -1;
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

        UpdateAmbience(SceneManager.GetActiveScene().buildIndex); // initialize the correct ambience

        ambienceInstance.start(); // Play the ambience event
        roomToneInstance.start(); // Play the roomtone event
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAmbience(scene.buildIndex);
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

    public void UpdateAmbience(int sceneIndex) 
    {
        if (sceneIndex == currentSceneIndex) return; // Prevent if already correct - no redundant calls

        float parameterValue;
        switch (sceneIndex)
        {
            case 0:
                Debug.Log("Ambience set to Menu");
                parameterValue = 0f;
                break;
            case 1:
                Debug.Log("Ambience set to Void");
                parameterValue = 1f;
                break;
            case 2:
                Debug.Log("Ambience set to Forest");
                parameterValue = 2f;
                break;
            default:
                Debug.LogWarning($"No Ambience scene state set for scene with index {sceneIndex}");
                parameterValue = 0f; // Default value so its not left unassigned
                break;
        }

        ambienceInstance.setParameterByName("Scene", parameterValue); // Set the sound
        currentSceneIndex = sceneIndex; // Update index after setting
    }

    #endregion
}
