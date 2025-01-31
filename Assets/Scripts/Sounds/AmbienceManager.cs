using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class AmbienceManager : MonoBehaviour
{
    #region Variables

    [Header("Ambience Sounds")]
    public EventReference ambienceEvent;

    private EventInstance ambienceInstance;
    private static AmbienceManager instance;
    public static AmbienceManager Instance { get { return instance; } }

    private int currentSceneIndex = -1;

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
            //DontDestroyOnLoad(gameObject); //keep if you want the manager to survive scene transitions
        }
    }

    void Start()
    {
        // Initialize the ambience event here, before potentially accessing it
        ambienceInstance = AudioManager.instance.CreateInstance2D(ambienceEvent);
        ambienceInstance.start();
        // initialize the correct ambience on game start
        UpdateAmbience(SceneManager.GetActiveScene().buildIndex);
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

    void OnDestroy()
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.release();
        }
    }
    #endregion

    #region Ambience Logic

    // called from scene loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateAmbience(scene.buildIndex);
    }

    // Public method to update ambience
    public void UpdateAmbience(int sceneIndex)
    {
        // Prevent update if already in correct scene. This prevents redundant calls.
        if (sceneIndex == currentSceneIndex) return;

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

        SetAmbienceState(parameterValue);
        currentSceneIndex = sceneIndex; // only update scene index after setting the parameter
    }
    // Private method to set the parameter
    private void SetAmbienceState(float parameterValue)
    {
        ambienceInstance.setParameterByName("Scene", parameterValue);
    }

    #endregion
}