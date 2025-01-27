//--------------------------------------------------------------------------------------------------
// Description: Manages the elevator sound logic, using FMOD parameters to transition between
//              elevator start and stop sounds.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class ElevatorSounds : MonoBehaviour
{
    #region Variables

    [Header("References")] /// Positions where the sound should play from
    public Transform elevatorSoundSource; 
    public Transform doorsSoundSource;  
    public Transform roomtoneSoundSource; 
    public Transform leverSoundSource;
    public Transform buttonSoundSource;

    [Header("Sounds")] /// FMOD event references
    public EventReference rideSound;  
    public EventReference doorCloseSound;   
    public EventReference doorOpenSound;  
    public EventReference buttonSound;  
    public EventReference roomToneSound;   
    public EventReference leverDownSound;   
    public EventReference leverUpSound;   

    // Private Variables
    private EventInstance elevatorRideInstance; /// Variable to hold the created event instance
    private EventInstance roomToneInstance;  /// Variable to hold the created event instance
    private bool hasStarted = false; // Check if the sound has been started
    private static ElevatorSounds instance;
    public static ElevatorSounds Instance { get { return instance; } }

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
        }
    }

    #endregion

    #region Unity Methods
    void Start() /// Initializes the audio instances
    {
        elevatorRideInstance = AudioManager.instance.CreateInstance(rideSound, elevatorSoundSource.position); // Create and start event instance.
        roomToneInstance = AudioManager.instance.CreateInstance(roomToneSound, roomtoneSoundSource.position); //Create and start room tone
        roomToneInstance.start();
    }

    void OnDestroy() /// Clean up FMOD event instances when the object is destroyed
    {
        if (elevatorRideInstance.isValid())
        {
            elevatorRideInstance.release();
        }
        if (roomToneInstance.isValid())
        {
            roomToneInstance.release();
        }
    }

    #endregion

    #region Play Sounds

    public void PlayElevatorStart()  /// Starts the elevator ride sound.
    {
        if (!hasStarted)
        {
            elevatorRideInstance.start();
            hasStarted = true;
        }
        SetRideState(0f);
    }

    public void PlayElevatorStop()  /// Stops the elevator ride sound.
    {
        SetRideState(1f);
        hasStarted = false;
    }

    private void SetRideState(float parameterValue) /// Sets the FMOD parameter for the ride state.
    {
        elevatorRideInstance.set3DAttributes(RuntimeUtils.To3DAttributes(elevatorSoundSource.position));
        elevatorRideInstance.setParameterByName("State", parameterValue);
    }

    public void PlayLeverDownSound()  /// Plays the lever down sound.
    {
        AudioManager.instance.PlayOneShot(leverDownSound, leverSoundSource.position);
    }

    public void PlayLeverUpSound()  /// Plays the lever down sound.
    {
        AudioManager.instance.PlayOneShot(leverUpSound, leverSoundSource.position);
    }

    public void PlayDoorCloseSound() /// Plays the door close sound.
    {
        AudioManager.instance.PlayOneShot(doorCloseSound, doorsSoundSource.position); 
    }

    public void PlayDoorOpenSound()  /// Plays the door open sound.
    {
        AudioManager.instance.PlayOneShot(doorOpenSound, doorsSoundSource.position); 
    }

    public void PlayButtonSound()  /// Plays the button sound.
    {
        AudioManager.instance.PlayOneShot(buttonSound, buttonSoundSource.position); 
    }


    #endregion
}