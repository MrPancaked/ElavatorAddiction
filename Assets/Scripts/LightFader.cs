using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LightFader : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Light lightSource;

    [Header("Variables")]
    public float lightIntensity = 1f;
    public float transitionTime = 1.5f;

    #endregion

    #region Unity Methods

    private void Start()
    {
        StartCoroutine(FadeInLight());
    }

    private IEnumerator FadeInLight()
    {
        lightSource.intensity = 0f; // Ensure the light starts at 0 intensity.
        float timer = 0f;

        while (timer < transitionTime)
        {
            timer += Time.deltaTime;
            float t = timer / transitionTime;  // Normalized time (0 to 1)

            lightSource.intensity = Mathf.Lerp(0f, lightIntensity, t);

            yield return null;
        }

        lightSource.intensity = lightIntensity; // Ensure the light reaches the exact target intensity.
    }

    #endregion
}