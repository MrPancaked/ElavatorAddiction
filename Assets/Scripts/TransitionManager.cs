using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using TMPro;

public class TransitionManager : MonoBehaviour
{
    #region Header Variables

    [Header("References")]
    public TextMeshPro roomText;
    public Light sceneLight; // Reference to the scene light
    public List<SceneSettings> targetSceneSettings; // List of all scene settings

    // Private stuff
    [HideInInspector]
    public int RoomIndex = 0;
    [HideInInspector]
    public string currentSceneName; // String to store the current scene name
    private Color currentFogColor = Color.white; // Current fog color, initialized to white as a default
    private float currentFogStartDistance; // Current fog start distance
    private float currentFogEndDistance; // Current fog end distance
    private Color destinationFogColor; // Target fog color
    private Color currentBackgroundColor = Color.black; // Current background color, initialized to white as default
    private SceneSettings currentSceneSettings; // Current scene settings
    private Camera mainCamera; // Reference to the main camera
    private static TransitionManager instance;
    public static TransitionManager Instance { get { return instance; } }
    private float currentLightIntensity;
    private float destinationLightIntensity;


    #endregion

    #region Unity Methods

    private void Awake()
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
            SceneManager.sceneLoaded += OnSceneLoaded; // Add listener to scene loaded
            UpdateRoomIndex();
            currentSceneName = SceneManager.GetActiveScene().name; // Set current scene name          
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) //Method for when scene is loaded
    {
        SceneLoaded(); //Load settings
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; //Remove listener from scene
    }

    #endregion

    #region Fog/Lights

    public void SetInitialFog(SceneSettings settings)
    {
        Color tempFogColor = currentFogColor;
        if (!ColorUtility.TryParseHtmlString(settings.fogColorHex, out tempFogColor))
        {
            Debug.LogError("TransitionManager: Invalid initial fog hex color, setting to white");
            tempFogColor = Color.white;
        }
        if (!ColorUtility.TryParseHtmlString(settings.backgroundColor, out currentBackgroundColor))
        {
            Debug.LogError("TransitionManager: Invalid background hex color, setting to white.");
            currentBackgroundColor = Color.black;
        }

        currentSceneSettings = settings; // Set current scene settings
        mainCamera.backgroundColor = currentBackgroundColor; // Set background color
        SetLightIntensity(settings); // Set light intensity
        SetFogFromSettings(settings, tempFogColor); // Set fog from the settings
    }

    public void SetFogFromSettings(SceneSettings settings, Color fogColor)
    {
        currentFogColor = fogColor; // Set the actual current fog color variable here
        currentFogStartDistance = settings.fogStartDistance; // Set fog start distance
        currentFogEndDistance = settings.fogEndDistance; // Set fog end distance

        RenderSettings.fogColor = currentFogColor; //Apply fog color to the render
        RenderSettings.fogStartDistance = currentFogStartDistance; // Apply fog start distance
        RenderSettings.fogEndDistance = currentFogEndDistance; // Apply fog end distance
    }

    public void StartFogTransition(SceneSettings destinationSettings, float fogTransitionTime)
    {
        StartCoroutine(FadeFog(destinationFogColor, destinationSettings.fogStartDistance, destinationSettings.fogEndDistance, fogTransitionTime, destinationSettings)); //start the fade fog coroutine
        StartLightTransition(destinationSettings, fogTransitionTime);
    }

    IEnumerator FadeFog(Color targetColor, float targetStartDistance, float targetEndDistance, float fogTransitionTime, SceneSettings destinationSettings)
    {
        float timer = 0.0f;
        float initialStartDistance = RenderSettings.fogStartDistance;
        float initialEndDistance = RenderSettings.fogEndDistance;
        Color initialColor = RenderSettings.fogColor;
        while (timer < fogTransitionTime)
        {
            timer += Time.deltaTime;
            RenderSettings.fogColor = Color.Lerp(initialColor, targetColor, timer / fogTransitionTime); //Lerp the fog color
            RenderSettings.fogStartDistance = Mathf.Lerp(initialStartDistance, targetStartDistance, timer / fogTransitionTime); //Lerp the fog start distance
            RenderSettings.fogEndDistance = Mathf.Lerp(initialEndDistance, targetEndDistance, timer / fogTransitionTime); //Lerp the fog end distance
            yield return null;
        }
        SetFogFromSettings(destinationSettings, targetColor); // apply the new settings
    }

    public void SetLightIntensity(SceneSettings settings)
    {

        currentLightIntensity = sceneLight.intensity;
        destinationLightIntensity = settings.lightIntensity;


        sceneLight.intensity = settings.lightIntensity; // Set the light intensity if light is provided.
    }

    public void StartLightTransition(SceneSettings destinationSettings, float lightTransitionTime)
    {
        StartCoroutine(FadeLightIntensity(destinationLightIntensity, lightTransitionTime, destinationSettings));
    }

    IEnumerator FadeLightIntensity(float targetIntensity, float lightTransitionTime, SceneSettings destinationSettings)
    {
        float timer = 0.0f;
        float initialIntensity = sceneLight.intensity;
        while (timer < lightTransitionTime)
        {
            timer += Time.deltaTime;
            sceneLight.intensity = Mathf.Lerp(initialIntensity, targetIntensity, timer / lightTransitionTime);
            yield return null;
        }
        SetLightIntensity(destinationSettings);
    }

    #endregion

    #region Scene Settings

    private void SceneLoaded() // Loads the Scene Settings by name.
    {
        mainCamera = Camera.main; // Find the main camera on new scene load as well
        SceneSettings settings = GetCurrentSceneSettings(); //get the scene settings
        SetInitialFog(settings); // Set initial fog settings
    }

    public SceneSettings GetSceneSettings(string currentSceneName)
    {
        SceneSettings newSceneSettings;
        do
        {
            int randomIndex = Random.Range(0, targetSceneSettings.Count); // get a random index
            newSceneSettings = targetSceneSettings[randomIndex]; // get the settings from the random index
        } while (newSceneSettings.sceneName == "Void"); //while the name matches, keep looping
        return newSceneSettings; // return the new settings
    }

    public SceneSettings GetCurrentSceneSettings()
    {
        SceneSettings settings = null; // declare the settings
        string currentSceneName = SceneManager.GetActiveScene().name; // get the active scene name
        foreach (SceneSettings sceneSetting in targetSceneSettings)
        {
            if (sceneSetting.sceneName == currentSceneName)
            {
                settings = sceneSetting; // set the settings if the name matches
                break; //break the loop
            }
        }
        return settings; //return the settings
    }

    #endregion

    public IEnumerator ForestTransition()
    {
        Debug.Log("Ride started / Button disabled / Start sound");
        ElevatorController.Instance.isButtonActive = false; // Disable the button
        ElevatorSounds.Instance.PlayElevatorStart(); //play elevator start sound
        yield return new WaitForSeconds(0.5f); // Makes the screenshake match the sound, otherwise its useless

        Debug.Log("Screen shake / Fog transition");
        SceneSettings destinationSettings = GetSceneSettings(currentSceneName); // get next scene settings
        ScreenshakeManager.Instance.TriggerShake("elevator", overrideForce: 1.5f, overrideDuration: 0.8f);
        StartFogTransition(destinationSettings, 5f); //Start fog transition
        yield return new WaitForSeconds(3f); // Time delay before scene transition

        AmbienceManager.Instance.UpdateAmbience(2);
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
        EnemyCounter.Instance.InitiatlizeEnemyCount(); // Update the enemy counter
        ElevatorController.Instance.OpenDoors(); // open the doors
        ElevatorController.Instance.isButtonActive = true; // Enable the button
        ElevatorController.Instance.closedDoorCoroutine = null; //Reset current coroutine
    }

    public IEnumerator VoidTransition()
    {
        HealthManager.Instance.player.transform.position = new Vector3(HealthManager.Instance.player.transform.position.x, 30f, HealthManager.Instance.player.transform.position.z);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Void");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
    }

    public void UpdateRoomIndex()
    {
        roomText.text = (-RoomIndex).ToString();
    }
}