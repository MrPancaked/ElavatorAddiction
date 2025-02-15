using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using FMODUnity;
using System.Runtime;
using UnityEngine.Windows;

public class Gun : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public LayerMask whatIsEnemy; // Layer mask to identify enemies
    public LayerMask whatIsGround; // Layer mask to identify enemies
    public Animator gunAnimator;
    [SerializeField] public GunSettings gunSettings; // Gun settings with all statistics and variables
    public Camera fpsCam; // The FPS camera
    public Rigidbody playerRb; // Player's Rigidbody
    public TextMeshPro ammoCounter; // UI text element for ammo display
    public GameObject reloadFeedbackText; // UI text element for reload feedback (for now lololo hehehe rene im going crazy bithc it is 3 am)

    [Header("Particles")]
    public GameObject bulletHole; // The bullet hole effect

    [Header("Leave empty if not dual gun")]
    public List<Gun> dualGuns = new List<Gun>(); // PUT BOTH GUNS IN THE LIST

    // Private Variables
    [HideInInspector]
    public bool reloading; // Check if the gun is currently reloading
    [HideInInspector]
    public float extraDamage = 0f; // Damage multiplier for upgrades
    private bool shooting = false; // Check if the player is currently shooting
    private bool readyToShoot; // Check if the gun is ready to shoot
    private int shotsLeft; // Ooverall shots (not bullets) in the magazine
    private int bulletsShot; // Bullets shot in a single shot or burst
    private RaycastHit rayHit; // Info about the raycast
    private int currentGunIndex = 0; // Track which gun to fire in a dual gun set
    private bool isLastShotInProgress = false; // Flag to prevent auto-reload before the shot is finished

    #endregion

    #region Unity Methods

    private void Awake()
    {
        shotsLeft = gunSettings.magazineSize / gunSettings.bulletsPerShot; // Initialize shots left to max magazine size at start
        readyToShoot = true; // Allow shooting initially
        reloadFeedbackText.SetActive(false); // Disable reload text at start
        
    }

    private void Start()
    {
        ammoCounter.SetText("");  // Disable ammo text when the gun is not being used
    }

    private void Update()
    {
        MyInput();           // Get player input every frame
        UpdateAmmoDisplay();   // Update the ammo display every frame
    }

    private void OnDisable()
    {
        ammoCounter.SetText("");  // Disable text when the gun is not being used
    }

    //private void OnEnable()
    //{
    //    gunSettings.fireRate = 1f; // Set the fire rate to 1 initially
    //}
    #endregion

    #region Input Logic

    private void MyInput()
    {
        if (gunSettings.allowContinuesFire)
        {
            if (Inputs.Instance.shoot.IsPressed())
            {
                shooting = true;
            }
            else
            {
                shooting = false;
            }
        }
        else
        {
            if (Inputs.Instance.shoot.WasPressedThisFrame())
            {
                shooting = true;
            }
            if (Inputs.Instance.shoot.WasReleasedThisFrame())
            {
                shooting = false;//Make sure that we set to false once released so we can fire again
            }
        }


        // Handle logic for normal (single) guns
        if (!gunSettings.isDualGun)
        {
            //Auto reload if bullets = 0 && not reloading and not finishing the last shot.
            if (shotsLeft <= 0 && !reloading && !isLastShotInProgress)
            {
                Reload();
            }

            if (Inputs.Instance.reload.WasPressedThisFrame() && shotsLeft < (gunSettings.magazineSize / gunSettings.bulletsPerShot) && !reloading) Reload(); // Check if the player is trying to reload

            if (readyToShoot && shooting && !reloading) // Handle shooting logic if ready to shoot, the player is pressing to shoot, and if the gun is not reloading
            {
                if (shotsLeft > 0) // Check if there is ammo in the magazine
                {

                    bulletsShot = gunSettings.bulletsPerShot; // Set the amount of bullets per shot
                    //AudioManager.instance.PlayOneShot(gunSettings.gunShotSound, MuzzleFlashPoint.position); //Play the shot sound
                    Shoot(); // Fire the gun

                }

            }

        }

        // Handle logic for dual guns
        else
        {
            bool allGunsEmpty = true;
            foreach (Gun gun in dualGuns)
            {
                if (gun.shotsLeft > 0)
                {
                    allGunsEmpty = false;
                    break;
                }
            }
            //Auto reload if both guns are empty && not reloading and not finishing the last shot
            if (allGunsEmpty && !reloading && !isLastShotInProgress)
            {
                Reload();
            }
            if (Inputs.Instance.reload.WasPressedThisFrame() && (dualGuns[0].shotsLeft < (dualGuns[0].gunSettings.magazineSize / dualGuns[0].gunSettings.bulletsPerShot) ||
                                                                        dualGuns[1].shotsLeft < (dualGuns[1].gunSettings.magazineSize / dualGuns[1].gunSettings.bulletsPerShot))
                                                                        && !reloading) Reload();  // Check if the player is trying to reload

            if (shooting && !reloading) // Handle shooting logic if the player is trying to shoot and the gun is not reloading
            {

                if (dualGuns[currentGunIndex].readyToShoot) // check if the current gun is ready to shoot
                {
                    if (dualGuns[currentGunIndex].shotsLeft > 0) // Check if the current gun has ammo
                    {
                        dualGuns[currentGunIndex].bulletsShot = dualGuns[currentGunIndex].gunSettings.bulletsPerShot; // Set bullets per shot
                        //AudioManager.instance.PlayOneShot(dualGuns[currentGunIndex].gunSettings.gunShotSound, dualGuns[currentGunIndex].MuzzleFlashPoint.position); //Play the shot sound
                        dualGuns[currentGunIndex].Shoot(); // Fire the current gun
                        currentGunIndex = (currentGunIndex + 1) % 2; // Switch to the next gun
                    }

                }
            }
        }
    }

    #endregion

    #region Shooting Logic

    private void Shoot() /// Handles the shooting mechanics for a single gun.
    {
        readyToShoot = false; // Disable more shooting until reset
        isLastShotInProgress = true; // Set the flag at the start of the shot

        Vector3 spreadDirection = Random.insideUnitSphere * gunSettings.spread;  // Calculate spread in world space (Generate a random point inside a sphere for spread)
        Vector3 direction = fpsCam.transform.forward + spreadDirection; // Calculate Direction with Spread

        gunAnimator.SetTrigger("Shoot");

        if (gunSettings.bulletsPerShot > 1) //Only call the burst fire if needed
        {
            StartCoroutine(BurstShot(direction)); // Calls the couroutine for burst fire
            shotsLeft--; // Remove shots from the magazine
        }
        else
        {
            SingleShot(direction); // Call this method to make the logic
            shotsLeft--; // Remove shots from the magazine
        }

        Invoke(nameof(ResetShot), gunSettings.fireRate); // Reset readyToShoot with fireRate
        ApplyPlayerPushback();
    }

    private void SingleShot(Vector3 direction) // Logic of one single shot
    {
        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, gunSettings.range, whatIsEnemy))
        {
            Transform targetTransform = rayHit.transform;
            Damageable damageable = targetTransform.GetComponent<Damageable>();

            if (damageable == null)
            {
                damageable = targetTransform.GetComponentInParent<Damageable>(); //try to find parent
            }

            if (damageable != null)
            {
                Rigidbody enemyRigidbody = targetTransform.GetComponent<Rigidbody>(); // push the enemy back
                if (enemyRigidbody != null)
                {
                    enemyRigidbody.AddForce(direction * gunSettings.enemyPushbackForce, ForceMode.Impulse);
                }
                damageable?.TakeDamage(gunSettings.damagePerBullet + extraDamage); //Damage the enemy
            }


        }
        ScreenshakeManager.Instance.TriggerShake("gunshot");
        BulletHoles(); // Call bullet effects
    }

    IEnumerator BurstShot(Vector3 direction)
    { //Burst fire logic with a coroutine for delay

        for (int i = 0; i < gunSettings.bulletsPerShot; i++)
        {
            SingleShot(direction);
            yield return new WaitForSeconds(gunSettings.timeBetweenBulletsInBursts);
        }
        isLastShotInProgress = false; // reset it here so that the coroutine finishes
    }

    private void ResetShot() /// Resets the ready to shoot flag after firing.
    {
        readyToShoot = true;  // Allow shooting again
        isLastShotInProgress = false; // reset it when it is a single shot
    }

    #endregion

    #region Bullet Effects

    private void BulletHoles()
    {
        if (rayHit.collider != null)
        {
            int hitLayer = rayHit.collider.gameObject.layer;
            if ((whatIsGround & (1 << hitLayer)) != 0) // Instantiate bullet hole
            {
                GameObject bulletHoleTemporary = Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0, 180, 0)); // Instantiate bullet hole
                bulletHoleTemporary.transform.LookAt(transform); //Rotate the bullet hole to look at the gun
                Destroy(bulletHoleTemporary, 1f); //Destroy after 1 sec
            }
        }
    }

    #endregion

    #region Player Pushback

    private void ApplyPlayerPushback()
    {
        if (playerRb == null) { return; } // If we have no reference, then we skip the pushback
        // Calculate the pushback force in the opposite direction of the shot
        Vector3 pushbackDirection = -fpsCam.transform.forward;
        playerRb.AddForce(pushbackDirection * gunSettings.playerPushbackForce, ForceMode.Impulse);
    }

    #endregion

    #region Reloading Logic

    private void Reload() /// Starts the reload process.
    {
        gunAnimator.SetTrigger("Reload");
        reloading = true;   // Start reload process
        reloadFeedbackText.SetActive(true); // Display reload text
        Invoke(nameof(ReloadFinished), gunSettings.reloadTime);  // Complete reload after delay
    }

    public void ReloadFinished() /// Completes the reload by filling magazine and resetting the reload flag.
    {
        if (gunSettings.isDualGun) // If its a dual gun, refill all magazines
        {
            foreach (Gun gun in dualGuns)
            {
                gun.shotsLeft = gun.gunSettings.magazineSize / gun.gunSettings.bulletsPerShot;
            }
        }
        else //If its not, refill one magazine
        {
            shotsLeft = gunSettings.magazineSize / gunSettings.bulletsPerShot;
        }

        reloadFeedbackText.SetActive(false); // Disable reload text
        reloading = false; // End reload process
    }

    #endregion

    #region UI Logic

    private void UpdateAmmoDisplay()
    {
        if (!gunSettings.isDualGun) // If its a normal gun
        {
            ammoCounter.SetText($"{shotsLeft}"); ; // Show combined ammo UI
        }
        else // If its a dual gun
        {
            int totalShotsLeft = 0;
            int totalMagazineSize = 0;
            foreach (Gun gun in dualGuns)
            {
                totalShotsLeft += gun.shotsLeft;
                totalMagazineSize += gun.gunSettings.magazineSize / gun.gunSettings.bulletsPerShot;
            }
            ammoCounter.SetText($"{totalShotsLeft}"); ; // Show combined ammo UI
        }
    }

    #endregion
}