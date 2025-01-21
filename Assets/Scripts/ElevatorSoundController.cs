//--------------------------------------------------------------------------------------------------
// Description: Manages the elevator sound logic, using FMOD parameters to transition between
//              elevator start and stop sounds.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class ElevatorSoundController : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Transform elevatorSoundSource;  /// Transform where the sound should play from
    public Transform doorsSoundSource;  /// Transform where the sound should play from
    public Transform roomtoneSoundSource;   /// Transform where the sound should play from

    [Header("Sounds")]
    public EventReference rideSound;  /// FMOD event reference for elevator start sound (changed to the ride sound)
    public EventReference doorCloseSound;   /// FMOD event reference for door close sound
    public EventReference doorOpenSound;  /// FMOD event reference for door open sound
    public EventReference buttonSound;  /// FMOD event reference for button sound
    public EventReference roomToneSound;   /// FMOD event reference for the room tone sound

    // Private Variables
    private EventInstance elevatorRideInstance; /// Variable to hold the created event instance
    private EventInstance roomToneInstance;  /// Variable to hold the created event instance
    private bool hasStarted = false; // Check if the sound has been started
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

    #region Doors
    public void PlayDoorCloseSound() /// Plays the door close sound.
    {
        AudioManager.instance.PlayOneShot(doorCloseSound, doorsSoundSource.position); // Play close sound with FMOD
    }

    public void PlayDoorOpenSound()  /// Plays the door open sound.
    {
        AudioManager.instance.PlayOneShot(doorOpenSound, doorsSoundSource.position); // Play open sound with FMOD
    }
    #endregion

    #region Buttons

    public void PlayButtonSound(Vector3 position)  /// Plays the button sound.
    {
        AudioManager.instance.PlayOneShot(buttonSound, position); // Play button sound with FMOD
    }

    #endregion

    #region Ride

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

    #endregion
}