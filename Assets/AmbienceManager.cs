using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AmbienceManager : MonoBehaviour
{
    #region Variables

    [Header("Ambience Sounds")] /// FMOD event references
    public EventReference ambienceEvent;

    private EventInstance ambienceInstance; /// Variable to hold the created event instance
    private static AmbienceManager instance;
    public static AmbienceManager Instance { get { return instance; } }

    //private
    private int currentSceneIndex = -1; // Track the current scene

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
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
    }

    void Start()
    {
        ambienceInstance = AudioManager.instance.CreateEventInstance(ambienceEvent); // Create event instance using 2d
        ambienceInstance.start();
        UpdateAmbience(SceneManager.GetActiveScene().buildIndex); //set the start of the game ambience track
        Debug.Log("Ambience initialized.");
    }


    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnDestroy() /// Clean up FMOD event instances when the object is destroyed
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.release();
        }
    }
    #endregion

    #region Ambience Logic

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAmbience(scene.buildIndex); //check if the active scene is changed, and update the ambience
    }

    private void UpdateAmbience(int sceneIndex)
    {
        if (sceneIndex == currentSceneIndex) return;

        switch (sceneIndex)
        {
            case 0:
                Debug.Log("Ambience set to Menu");
                SetAmbienceState(0f);
                break;
            case 1:
                Debug.Log("Ambience set to Void");
                SetAmbienceState(1f);
                break;
            case 2:
                Debug.Log("Ambience set to Forest");
                SetAmbienceState(2f);
                break;
            default:
                Debug.LogWarning($"No Ambience scene state set for scene with index {sceneIndex}");
                break;
        }
        currentSceneIndex = sceneIndex;
    }

    private void SetAmbienceState(float parameterValue) /// Sets the FMOD parameter for the ambience state.
    {
        ambienceInstance.setParameterByName("Scene", parameterValue);
    }

    #endregion
}