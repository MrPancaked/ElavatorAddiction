using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class FogManager : MonoBehaviour
{
    #region Header Variables

    [Header("References")]
    public Light sceneLight;
    public Camera mainCamera;
    public List<SceneSettings> Scenes;

    // Private variables
    private float currentFogStartDistance;
    private float currentFogEndDistance;
    private Color currentFogColor;
    private Color currentBackgroundColor;
    private Coroutine currentTransitionCoroutine;
    private static FogManager instance;
    public static FogManager Instance { get { return instance; } }
    private float currentLightIntensity;

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
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Scene Settings Management

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyCurrentSceneSettings();
    }

    public void ApplyCurrentSceneSettings()
    {
        SceneSettings settings = GetSceneSettings(SceneManager.GetActiveScene().name);
        ApplyFogAndLightSettings(settings);
    }

    public SceneSettings GetSceneSettings(string sceneName)
    {
        foreach (SceneSettings sceneSetting in Scenes)
        {
            if (sceneSetting.sceneName == sceneName)
            {
                return sceneSetting;
            }
        }
        return null;
    }

    #endregion

    #region Fog and Light Setup

    public void SetFogAndLightTransition(SceneSettings destinationSettings, float transitionTime)
    {
        currentTransitionCoroutine = StartCoroutine(Transition(destinationSettings, transitionTime));
    }

    private IEnumerator Transition(SceneSettings destinationSettings, float transitionTime)
    {
        float timer = 0.0f;
        float fovStart = RenderSettings.fogStartDistance;
        float fovEnd = RenderSettings.fogEndDistance;
        float lightAmount = sceneLight.intensity;
        float lightAmountNew = destinationSettings.lightIntensity;

        Color initialFogColor = RenderSettings.fogColor;
        Color initialBackgroundColor = mainCamera.backgroundColor;
        Color targetFogColor = ColorUtility.TryParseHtmlString(destinationSettings.fogColorHex, out var fogColor) ? fogColor : default;
        Color targetBackgroundColor = ColorUtility.TryParseHtmlString(destinationSettings.backgroundColor, out var bgColor) ? bgColor : default;


        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;

            RenderSettings.fogColor = Color.Lerp(initialFogColor, targetFogColor, t);
            RenderSettings.fogStartDistance = Mathf.Lerp(fovStart, destinationSettings.fogStartDistance, t);
            RenderSettings.fogEndDistance = Mathf.Lerp(fovEnd, destinationSettings.fogEndDistance, t);
            mainCamera.backgroundColor = Color.Lerp(initialBackgroundColor, targetBackgroundColor, t);
            sceneLight.intensity = Mathf.Lerp(lightAmount, lightAmountNew, t);

            yield return null;
        }

        ApplyFogAndLightSettings(destinationSettings);
    }

    private void ApplyFogAndLightSettings(SceneSettings settings)
    {
        mainCamera = Camera.main;

        ColorUtility.TryParseHtmlString(settings.fogColorHex, out Color fogColor);
        ColorUtility.TryParseHtmlString(settings.backgroundColor, out Color bgColor);

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogStartDistance = settings.fogStartDistance;
        RenderSettings.fogEndDistance = settings.fogEndDistance;
        mainCamera.backgroundColor = bgColor;
        sceneLight.intensity = settings.lightIntensity;
    }

    #endregion
}
