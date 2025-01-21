//--------------------------------------------------------------------------------------------------
// Description: Handles muzzle flash effects, including light emission and its deactivation.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    #region Variables

    [System.Serializable]
    public class LightSource
    {
        public Light light;
        public float minIntensity;
        public float maxIntensity;
        public float flashDuration;
    }
    public LightSource lightSource;

    #endregion

    #region Unity Methods

    void Awake()  /// Initializes the light intensity of muzzle flash.
    {
        if (lightSource.light != null)
        {
            lightSource.light.intensity = Random.Range(lightSource.minIntensity, lightSource.maxIntensity);
            Invoke(nameof(DisableLight), lightSource.flashDuration);
        }
        else
        {
            Debug.LogWarning($"Light source not set on muzzle flash {gameObject.name}");
        }
    }

    #endregion

    #region MuzzleFlash Logic

    private void DisableLight() /// Disables the light after the flash duration.
    {
        if (lightSource.light != null)
        {
            lightSource.light.enabled = false;
        }
    }

    #endregion
}