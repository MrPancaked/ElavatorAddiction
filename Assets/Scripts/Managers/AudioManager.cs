using UnityEngine;
using FMODUnity;
using FMOD.Studio; // Required for EventInstance

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("You have 2 audio managers dumbass");
            Destroy(gameObject); // Destroy duplicate if another exists
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject); // Keep the singleton alive across scene loads
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference sound, Vector3 worldPos)
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        return instance;
    }
}