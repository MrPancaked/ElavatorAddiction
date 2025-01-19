//--------------------------------------------------------------------------------------------------
//  Description: Handles gun firing mechanics including shooting, reloading, bullet effects, and UI updates.
//               Supports both single shot and burst fire modes, and dual gun configurations.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using FMODUnity;
using System.Runtime;

public class Gun : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public LayerMask whatIsEnemy;          // Layer mask to identify enemies
    public TextMeshProUGUI ammoCounter;    // UI text element for ammo display
    public GameObject reloadFeedbackText;  // UI text element for reload feedback (for now lololo hehehe rene im going crazy bithc it is 3 am)
    public CinemachineImpulseSource impulseSource; //Reference to our impulse source for camera shake
    public Transform MuzzleFlashPoint;      // Transform for the muzzle flash spawn point
    public Camera fpsCam;                  // Reference to the FPS camera
    [SerializeField] GunSettings gunSettings; // Gun settings with all statistics and variables
    public List<Gun> dualGuns = new List<Gun>(); // If this is a dual gun set, this is the other gun (and this gun)

    [Header("Particles")]
    public Transform particlesParent; //The parent that we will be instantiating all of our particle effects into.
    public GameObject muzzleFlash;          // Prefab for the muzzle flash effect
    public GameObject bulletHole;          // Prefab for the bullet hole effect
    public GameObject bloodBurst;          // Prefab for blood burst effect

    // Private Variables
    private bool shooting;                // Flag to check if the player is currently shooting
    private bool readyToShoot;            // Flag to check if the gun is ready to shoot
    private bool reloading;                // Flag to check if the gun is currently reloading
    private int shotsLeft;               // Number of shots (not bullets) in the magazine
    private int bulletsShot;               // Number of bullets shot in a single shot or burst
    private RaycastHit rayHit;             // Stores information about the raycast hit
    private int currentGunIndex = 0;       // Index to track which gun to fire in a dual gun set

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

    #endregion

    #region Input Logic

    private void MyInput() /// Handles player input for shooting and reloading.
    {
        // Handle logic for normal (single) guns
        if (!gunSettings.isDualGun)
        {
            // Check if we should allow continues fire (automatic fire) or single press fire
            if (gunSettings.allowContinuesFire) shooting = Input.GetKey(KeyCode.Mouse0);
            else shooting = Input.GetKeyDown(KeyCode.Mouse0);

            // Check if the player is trying to reload
            if (Input.GetKeyDown(KeyCode.R) && shotsLeft < (gunSettings.magazineSize / gunSettings.bulletsPerShot) && !reloading) Reload();

            // Handle shooting logic if ready to shoot, the player is pressing to shoot, and if the gun is not reloading
            if (readyToShoot && shooting && !reloading)
            {
                if (shotsLeft > 0) // Check if there is ammo in the magazine
                {
                    bulletsShot = gunSettings.bulletsPerShot; // Set the amount of bullets per shot
                    AudioManager.instance.PlayOneShot(gunSettings.gunShotSound, MuzzleFlashPoint.position); //Play the shot sound
                    Shoot(); // Fire the gun
                }
                else if (shotsLeft == 0) // if out of ammo, call reload
                {
                    Reload();
                }
            }
        }
        else // Handle logic for dual guns
        {
            // Check if we should allow continues fire (automatic fire) or single press fire
            if (gunSettings.allowContinuesFire) shooting = Input.GetKey(KeyCode.Mouse0);
            else shooting = Input.GetKeyDown(KeyCode.Mouse0);

            // Check if the player is trying to reload
            if (Input.GetKeyDown(KeyCode.R) && (dualGuns[0].shotsLeft < (dualGuns[0].gunSettings.magazineSize / dualGuns[0].gunSettings.bulletsPerShot) || dualGuns[1].shotsLeft < (dualGuns[1].gunSettings.magazineSize / dualGuns[1].gunSettings.bulletsPerShot)) && !reloading) Reload();

            // Handle shooting logic if the player is trying to shoot and the gun is not reloading
            if (shooting && !reloading)
            {
                // check if the current gun is ready to shoot
                if (dualGuns[currentGunIndex].readyToShoot)
                {
                    if (dualGuns[currentGunIndex].shotsLeft > 0) // Check if the current gun has ammo
                    {
                        dualGuns[currentGunIndex].bulletsShot = dualGuns[currentGunIndex].gunSettings.bulletsPerShot; // Set bullets per shot
                        AudioManager.instance.PlayOneShot(dualGuns[currentGunIndex].gunSettings.gunShotSound, dualGuns[currentGunIndex].MuzzleFlashPoint.position); //Play the shot sound
                        dualGuns[currentGunIndex].Shoot(); // Fire the current gun
                        currentGunIndex = (currentGunIndex + 1) % 2; // Switch to the next gun
                    }
                    else // if the gun is empty, then reload
                    {
                        Reload();
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

        Vector3 spreadDirection = Random.insideUnitSphere * gunSettings.spread;  // Calculate spread in world space (Generate a random point inside a sphere for spread)
        Vector3 direction = fpsCam.transform.forward + spreadDirection; // Calculate Direction with Spread

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
    }
    private void SingleShot(Vector3 direction)
    { //This method is to contain the logic of one single shot

        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, gunSettings.range, whatIsEnemy)) // Shoot a raycast
        {

            if (rayHit.collider.CompareTag("Damageable")) // Check if the hit object is an enemy
            {
                Rigidbody enemyRigidbody = rayHit.collider.GetComponent<Rigidbody>(); // push the enemy back
                enemyRigidbody.AddForce(direction * gunSettings.enemyPushbackForce, ForceMode.Impulse);
                Damageable damageable = rayHit.transform.GetComponent<Damageable>(); // Damage the enemy
                damageable?.TakeDamage(gunSettings.damagePerBullet);
            }

        }
        if (impulseSource != null) //Camera shake if we have a impulse source
        {
            impulseSource.GenerateImpulseWithForce(gunSettings.screenShakeStrength);
        }
        BulletEffects(); // Call bullet effects
    }
    IEnumerator BurstShot(Vector3 direction)
    { //Burst fire logic with a coroutine for delay

        for (int i = 0; i < gunSettings.bulletsPerShot; i++)
        {
            SingleShot(direction);
            yield return new WaitForSeconds(gunSettings.timeBetweenBulletsInBursts);
        }
    }

    private void ResetShot() /// Resets the ready to shoot flag after firing.
    {
        readyToShoot = true;  // Allow shooting again
    }

    #endregion

    #region Bullet Effects
    private void BulletEffects()
    {
        GameObject muzzleFlashTemporary = Instantiate(muzzleFlash, MuzzleFlashPoint.position, Quaternion.identity, particlesParent); // Instantiate muzzleflash
        muzzleFlashTemporary.transform.forward = fpsCam.transform.forward; // Align the muzzle flash with the camera
        Destroy(muzzleFlashTemporary, 1f); //Destroy after 1 sec

        if (rayHit.collider != null)
        {
            int hitLayer = rayHit.collider.gameObject.layer;
            if ((whatIsEnemy & (1 << hitLayer)) != 0) // Check if the raycast hit an enemy
            {
                // Instantiate blood burst if on an enemy layer
                GameObject bloodBurstTemporary = Instantiate(bloodBurst, rayHit.point, Quaternion.Euler(0, 180, 0), particlesParent);
                Destroy(bloodBurstTemporary, 1f); // Destroy after 1 sec
            }
            else // Instantiate bullet hole for anything else
            {
                AudioManager.instance.PlayOneShot(gunSettings.bulletHit, this.transform.position); // Play bullet hit sound
                GameObject bulletHoleTemporary = Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0, 180, 0), particlesParent); // Instantiate bullet hole
                bulletHoleTemporary.transform.LookAt(transform); //Rotate the bullet hole to look at the gun
                Destroy(bulletHoleTemporary, 1f); //Destroy after 1 sec
            }
        }
    }

    #endregion

    #region Reloading Logic

    private void Reload() /// Starts the reload process.
    {
        reloading = true;   // Start reload process
        reloadFeedbackText.SetActive(true); // Display reload text
        Invoke(nameof(ReloadFinished), gunSettings.reloadTime);  // Complete reload after delay
    }

    private void ReloadFinished() /// Completes the reload by filling magazine and resetting the reload flag.
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
        reloading = false;           // End reload process
    }

    #endregion

    #region UI Logic
    private void UpdateAmmoDisplay()
    {
        if (!gunSettings.isDualGun) // If its a normal gun
        {
            ammoCounter.SetText(shotsLeft + " / " + gunSettings.magazineSize / gunSettings.bulletsPerShot); // show normal ammo UI
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
            ammoCounter.SetText(totalShotsLeft + " / " + totalMagazineSize); // Show combined ammo UI
        }
    }
    #endregion
}