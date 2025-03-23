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
    public EventReference bhopSound;
    public EventReference slamSound;
    public EventReference dashSound;

    // Private stuff
    private EventInstance deathInstance; /// Variable to hold the created event instance
    private EventInstance inAirInstance; /// Variable to hold the created event instance
    private EventInstance footstepInstance;
    private EventInstance slidingInstance;
    private bool isDead = false; // Check if the sound has been started
    private bool isGrounded = false; // Check if the sound has been started
    private bool isSliding = false; // Check if the sound has been started
    private float lastFootstepTime = 0f;
    private float footstepRate = 0.3f; // Adjust for desired speed
    //private float lastSlideGroundcheckCooldown = 0f;
    //private float SlideGroundcheckCooldownRate = 0.3f;
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
        }
    }

    void Start()
    {
        deathInstance = AudioManager.instance.CreateInstance2D(dyingSound); // Create event instance using 2d
        inAirInstance = AudioManager.instance.CreateInstance2D(inAirSound); // Create event instance using 2d
        footstepInstance = AudioManager.instance.CreateInstance2D(footstepSound);
        slidingInstance = AudioManager.instance.CreateInstance2D(slideSound);
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
        if (slidingInstance.isValid())
        {
            slidingInstance.release();
        }
    }

    #endregion

    #region Play Sounds

    public void PlayDeathStart()  /// Starts the elevator ride sound.
    {
        if (!isDead)
        {
            isDead = true;
            SetDeathState(0f);
            deathInstance.start();
        }
    }

    public void PlayDeathStop()  /// Stops the elevator ride sound.
    {
        if (isDead)
        {
            isDead = false;
            SetDeathState(1f);
        }
    }

    private void SetDeathState(float parameterValue) /// Sets the FMOD parameter for the ride state.
    {
        deathInstance.setParameterByName("State", parameterValue);
    }

    public void PlayHitSound()
    {
        AudioManager.instance.PlayOneShot2D(damageSound);
    }

    public void PlayDashSound()
    {
        AudioManager.instance.PlayOneShot2D(dashSound);
    }
    public void PlayJumpSound()
    {
        AudioManager.instance.PlayOneShot2D(jumpSound);
        PlayFootstepSound();
    }

    public void PlayBhopSound()
    {
        AudioManager.instance.PlayOneShot2D(bhopSound);
    }

    public void PlaySlamSound()
    {
        AudioManager.instance.PlayOneShot2D(slamSound);
    }

    public void PlaySlideSound()
    {
        if (!isSliding)
        {
            isSliding = true;

            //if (Time.time - lastSlideGroundcheckCooldown >= SlideGroundcheckCooldownRate)
            //{
            //lastSlideGroundcheckCooldown = Time.time;
            GroundSwitch();
            slidingInstance.start();
            //}
        }
    }

    public void StopSlideSound()
    {
        if (isSliding)
        {
            isSliding = false;
            slidingInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            //PlayFootstepSound();
        }
    }

    public void PlayInAirStart()
    {
        if (isGrounded)
        {
            isGrounded = false;
            inAirInstance.start();
        }
    }

    public void PlayInAirStop()
    {
        if (!isGrounded)
        {
            isGrounded = true;
            inAirInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PlayFootstepSound();
        }
    }

    #endregion
    
    #region Footsteps
    
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
                    slidingInstance.setParameterByName("Sliding", 1);
                }
                else if (surfaceRenderer.material.name.Contains("Elavator"))
                {
                    Debug.Log("ELEVATOR");
                    footstepInstance.setParameterByName("Footsteps", 2);
                    slidingInstance.setParameterByName("Sliding", 2);
                }
                else if (surfaceRenderer.material.name.Contains("ChurchRoof"))
                {
                    footstepInstance.setParameterByName("Footsteps", 2);
                    slidingInstance.setParameterByName("Sliding", 2);
                }
                else if (surfaceRenderer.material.name.Contains("Hut"))
                {
                    footstepInstance.setParameterByName("Footsteps", 2);
                    slidingInstance.setParameterByName("Sliding", 2);
                }
            }
            else if (hit.collider.TryGetComponent<Terrain>(out Terrain terrain))
            {
                Material terrainMaterial = terrain.materialTemplate;
                if (terrainMaterial != null)
                {
                    if (terrainMaterial.name.Contains("Snow"))
                    {
                        Debug.Log("SNOW BITCH");
                        footstepInstance.setParameterByName("Footsteps", 0);
                        slidingInstance.setParameterByName("Sliding", 0);
                    }
                    if (terrainMaterial.name.Contains("Grass"))
                    {
                        footstepInstance.setParameterByName("Footsteps", 3);
                        slidingInstance.setParameterByName("Sliding", 3);
                    }
                }
            }
        }
    }

    #endregion
}