using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class PlayerSounds : MonoBehaviour
{
    #region Variables

    [Header("Sounds")] /// FMOD event references
    public EventReference dyingSound;
    public EventReference damageSound;
    public EventReference footstepSound;
    public EventReference inAirSound;
    public EventReference jumpSound;

    private EventInstance deathInstance; /// Variable to hold the created event instance
    private EventInstance inAirInstance; /// Variable to hold the created event instance
    private bool hasStarted = false; // Check if the sound has been started
    private bool isInAir = false; // Check if the sound has been started
    private static PlayerSounds instance;
    public static PlayerSounds Instance { get { return instance; } }

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
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            Debug.Log("PlayerSounds instance created.");
        }
    }

    void Start()
    {
        deathInstance = AudioManager.instance.CreateEventInstance(dyingSound); // Create event instance using 2d
        inAirInstance = AudioManager.instance.CreateEventInstance(inAirSound); // Create event instance using 2d
        Debug.Log($"Event reference assigned: {dyingSound}"); // Debug Check
    }


    void OnDestroy() /// Clean up FMOD event instances when the object is destroyed
    {
        if (deathInstance.isValid())
        {
            deathInstance.release();
        }
        if (inAirInstance.isValid())
        {
            inAirInstance.release();
        }
    }

    #endregion

    #region Play Sounds

    public void PlayDeathStart()  /// Starts the elevator ride sound.
    {
        if (!hasStarted)
        {
            deathInstance.start();
            hasStarted = true;
        }
        SetDeathState(0f);
    }

    public void PlayDeathStop()  /// Stops the elevator ride sound.
    {
        SetDeathState(1f);
        hasStarted = false;
    }

    private void SetDeathState(float parameterValue) /// Sets the FMOD parameter for the ride state.
    {
        deathInstance.setParameterByName("State", parameterValue);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot2D(damageSound);
    }

    public void PlayJumpSound()
    {
        AudioManager.instance.PlayOneShot2D(jumpSound);
    }

    public void PlayInAirStart()
    {
        if (!isInAir)
        {
            inAirInstance.start();
            isInAir = true;
        }
    }

    public void PlayInAirStop()
    {
        if (isInAir)
        {
            inAirInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isInAir = false;
        }
    }

    #endregion
}