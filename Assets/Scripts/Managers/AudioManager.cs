//--------------------------------------------------------------------------------------------------
// Description: Manages audio playback using FMOD integration, and ensures a single instance of itself.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    #region Variables
    public static AudioManager instance { get; private set; }

    #endregion

    #region Unity Methods

    private void Awake() /// Sets up the AudioManager as a singleton.
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

    #endregion

    #region Playback Logic

    public void PlayOneShot(EventReference sound, Vector3 worldPos) /// Plays a sound effect once at the given position
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference sound, Vector3 worldPos) /// Creates an instance of a FMOD sound event.
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        return instance;
    }

    #endregion
}