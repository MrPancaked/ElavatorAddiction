using UnityEngine;
using Unity.Cinemachine;

public class ScreenshakeManager : MonoBehaviour
{
    #region References

    [System.Serializable]
    public struct ShakePreset
    {
        public string name;
        public CinemachineImpulseSource impulseSource;
        public float force;
        public float duration; // Add duration to preset
    }

    public ShakePreset Gunshot;
    public ShakePreset Elevator;
    public ShakePreset Slam;
    public ShakePreset PlayerHit;
    static ScreenshakeManager instance;
    public static ScreenshakeManager Instance
    {
        get { return instance; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance == null) // Singleton
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    public void TriggerShake(string shakeType, float overrideForce = -1f, float overrideDuration = -1f)
    {
        ShakePreset selectedPreset;

        switch (shakeType.ToLower())
        {
            case "gunshot":
                selectedPreset = Gunshot;
                break;
            case "elevator":
                selectedPreset = Elevator;
                break;
            case "slam":
                selectedPreset = Slam;
                break;
            case "playerhit":
                selectedPreset = PlayerHit;
                break;
            default:
                Debug.LogWarning($"Screen shake type '{shakeType}' not found.");
                return;
        }

        if (selectedPreset.impulseSource == null)
        {
            Debug.LogError($"Impulse Source not found in preset {selectedPreset.name}");
            return;
        }

        float finalForce = selectedPreset.force;
        float finalDuration = selectedPreset.duration;

        if (overrideForce >= 0)
        {
            finalForce = overrideForce;
        }

        if (overrideDuration >= 0)
        {
            finalDuration = overrideDuration;
        }

        selectedPreset.impulseSource.GenerateImpulseWithForce(finalForce);

        StartCoroutine(ResetImpulseSource(selectedPreset.impulseSource, finalDuration));
    }

    private System.Collections.IEnumerator ResetImpulseSource(CinemachineImpulseSource impulseSource, float duration)
    {
        yield return new WaitForSeconds(duration);
        impulseSource.GenerateImpulse(Vector3.zero); //reset the shake
    }

    #endregion
}