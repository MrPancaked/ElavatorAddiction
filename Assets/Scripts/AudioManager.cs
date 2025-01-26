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
    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Playback Logic

    public void PlayOneShot(EventReference sound, Vector3 worldPos) /// Plays a sound effect once at the given position
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference sound) /// Creates an instance of a FMOD sound event.
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        return instance;
    }

    public EventInstance CreateInstance(EventReference sound, Vector3 worldPos) /// Creates an instance of a FMOD sound event.
    {
        EventInstance instance = RuntimeManager.CreateInstance(sound);
        instance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        return instance;
    }

    public void Set3DAttributes(EventInstance eventInstance, Transform transform)
    {
        if (eventInstance.isValid())
        {
            FMOD.ATTRIBUTES_3D attributes = FMODUnity.RuntimeUtils.To3DAttributes(transform.gameObject); //Convert the 3D position and rotation into FMOD attributes
            eventInstance.set3DAttributes(attributes);
        }
    }

    #endregion
}