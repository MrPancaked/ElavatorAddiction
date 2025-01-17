using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [System.Serializable]
    public class LightSource
    {
        public Light light;
        public float minIntensity;
        public float maxIntensity;
        public float flashDuration;
    }
    public LightSource lightSource;
    void Awake()
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
    private void DisableLight()
    {
        if (lightSource.light != null)
        {
            lightSource.light.enabled = false;
        }
    }
}