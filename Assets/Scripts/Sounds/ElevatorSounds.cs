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

    [Header("Sounds")]
    public EventReference rideSound;
    public EventReference roomToneSound;

    // Private Variables
    private EventInstance elevatorRideInstance;
    private EventInstance roomToneInstance;
    private bool hasStarted = false;
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
            DontDestroyOnLoad(gameObject);
        }
    }

    #endregion

    #region Unity Methods
    void Start()
    {
        elevatorRideInstance = AudioManager.instance.CreateInstance2D(rideSound); // Changed to Create2DInstance
        roomToneInstance = AudioManager.instance.CreateInstance(roomToneSound, this.transform.position); //Keep Room Tone 3D
        roomToneInstance.start();
    }

    void OnDestroy()
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

    public void PlayElevatorStart()
    {
        if (!hasStarted)
        {
            elevatorRideInstance.start();
            hasStarted = true;
        }
        SetRideState(0f);
    }

    public void PlayElevatorStop()
    {
        SetRideState(1f);
        hasStarted = false;
    }

    private void SetRideState(float parameterValue)
    {
        elevatorRideInstance.setParameterByName("State", parameterValue);
    }

    #endregion
}