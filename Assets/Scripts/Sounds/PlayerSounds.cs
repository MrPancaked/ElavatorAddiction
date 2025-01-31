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
    public EventReference slideSound;

    private EventInstance deathInstance; /// Variable to hold the created event instance
    private EventInstance inAirInstance; /// Variable to hold the created event instance
    private EventInstance footstepInstance;
    private bool hasStarted = false; // Check if the sound has been started
    private bool isInAir = false; // Check if the sound has been started
    private static PlayerSounds instance;
    public static PlayerSounds Instance { get { return instance; } }
    private float lastFootstepTime = 0f;
    public float footstepRate = 0.3f; // Adjust for desired speed

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

    void Start()
    {
        deathInstance = AudioManager.instance.CreateInstance2D(dyingSound); // Create event instance using 2d
        inAirInstance = AudioManager.instance.CreateInstance2D(inAirSound); // Create event instance using 2d
        footstepInstance = AudioManager.instance.CreateInstance2D(footstepSound);
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
        if (footstepInstance.isValid())
        {
            footstepInstance.release();
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
        PlayFootstepSound();
    }

    public void PlaySlideSound()
    {
        //AudioManager.instance.PlayOneShot2D(slideSound);
        PlayFootstepSound();
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
            PlayFootstepSound();
            isInAir = false;
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepInstance.isValid())
        {
            if (Time.time - lastFootstepTime >= footstepRate)
            {
                lastFootstepTime = Time.time;
                GroundSwitch();
                footstepInstance.start();
            }

        }
    }

    private void GroundSwitch()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, -Vector3.up);
        if (Physics.Raycast(ray, out hit, 2.0f, Physics.AllLayers, QueryTriggerInteraction.Ignore))
        {
            Renderer surfaceRenderer = hit.collider.GetComponent<Renderer>();
            if (surfaceRenderer == null)
            {
                surfaceRenderer = hit.collider.GetComponentInChildren<Renderer>();
            }

            if (surfaceRenderer)
            {
                if (surfaceRenderer.material.name.Contains("Tiles"))
                {
                    footstepInstance.setParameterByName("Footsteps", 1);
                }
                else if (surfaceRenderer.material.name.Contains("Elavator"))
                {
                    footstepInstance.setParameterByName("Footsteps", 2);
                }
                else if (surfaceRenderer.material.name.Contains("ChurchRoof"))
                {
                    footstepInstance.setParameterByName("Footsteps", 2);
                }
                else if (surfaceRenderer.material.name.Contains("Hut"))
                {
                    footstepInstance.setParameterByName("Footsteps", 2);
                }
            }
            else if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
            {
                Material terrainMaterial = terrain.materialTemplate;
                if (terrainMaterial != null)
                {
                    if (terrainMaterial.name.Contains("SnowyGround"))
                    {
                        footstepInstance.setParameterByName("Footsteps", 0);
                    }
                }
            }
        }
    }

    #endregion
}