//--------------------------------------------------------------------------------------------------
//  Description: Handles gun firing mechanics including shooting, reloading, bullet effects, and UI updates.
//               Supports both single shot and burst fire modes.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;
using Unity.Cinemachine;

public class Gun : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public LayerMask whatIsEnemy;          // Layer mask to identify enemies
    public TextMeshProUGUI ammoCounter;    // UI text element for ammo display
    public GameObject reloadFeedbackText;  // UI text element for reload feedback for now lololo hehehe rene im going crazy bithc it is 3 am
    public CinemachineImpulseSource impulseSource; //Reference to our impulse source
    public Transform MuzzleFlashPoint;      // Transform for the muzzle flash spawn point
    public Camera fpsCam;                  // Reference to the FPS camera
    [SerializeField] GunSettings gunSettings; // Gun settings with all statistics and variables

    [Header("Particles")]
    public Transform particlesParent; //The parent that we will be instantiating all of our particle effects into.
    public GameObject muzzleFlash;          // Prefab for the muzzle flash effect
    public GameObject bulletHole;          // Prefab for the bullet hole effect
    public GameObject bloodBurst;          // Prefab for blood burst effect

    //public CameraShake camShake;           // Camera shake script reference
    //public float camShakeMagnitude;        // Magnitude of the camera shake
    //public float camShakeDuration;         // Duration of the camera shake

    // Private Variables
    private bool shooting;                // Flag to check if the player is currently shooting
    private bool readyToShoot;            // Flag to check if the gun is ready to shoot
    private bool reloading;                // Flag to check if the gun is currently reloading
    private int bulletsLeft;               // Number of bullets currently in the magazine
    private int bulletsShot;               // Number of bullets shot in a single shot or burst
    private RaycastHit rayHit;             // Stores information about the raycast hit

    #endregion

    #region Unity Methods

    private void Awake()
    {
        bulletsLeft = gunSettings.magazineSize;  // Initialize bullets left to max magazine size at start
        readyToShoot = true;         // Allow shooting initially
        reloadFeedbackText.SetActive(false);
    }
    private void Start()
    {
        ammoCounter.SetText("");  // Disable text when the gun is not being used
    }

    private void Update()
    {
        MyInput();           // Get player input
        UpdateAmmoDisplay();   // Update the ammo display
    }

    private void OnDisable()
    {
        ammoCounter.SetText("");  // Disable text when the gun is not being used
    }

    #endregion

    #region Input Logic

    private void MyInput() /// Handles player input for shooting and reloading.
    {
        if (gunSettings.allowContinuesFire) shooting = Input.GetKey(KeyCode.Mouse0); // Shooting with button held down
        else shooting = Input.GetKeyDown(KeyCode.Mouse0);             // Shooting with single button press

        if (Input.GetKeyDown(KeyCode.R) && bulletsLeft < gunSettings.magazineSize && !reloading) Reload(); // Reload when pressing R if bullets are missing and not already reloading

        // Handle shooting logic
        if (readyToShoot && shooting && !reloading)
        {
            if (bulletsLeft > 0)
            {
                bulletsShot = gunSettings.bulletsPerShot; // Set bullets shot for bursts
                Shoot(); // Fire the gun
            }
            else if (bulletsLeft == 0)
            {
                Reload();  // Auto reload when out of ammo
            }
        }
    }

    #endregion

    #region Shooting Logic

    private void Shoot() /// Handles the shooting mechanics.
    {
        readyToShoot = false; // Disable more shooting until reset
       
        Vector3 spreadDirection = Random.insideUnitSphere * gunSettings.spread;  // Calculate spread in world space (Generate a random point inside a sphere for spread)
        Vector3 direction = fpsCam.transform.forward + spreadDirection; // Calculate Direction with Spread

        if (Physics.Raycast(fpsCam.transform.position, direction, out rayHit, gunSettings.range, whatIsEnemy)) // RayCast
        {

            if (rayHit.collider.CompareTag("Damageable"))
            {
                Rigidbody enemyRigidbody = rayHit.collider.GetComponent<Rigidbody>();
                enemyRigidbody.AddForce(direction * gunSettings.enemyPushbackForce, ForceMode.Impulse);
                Damageable damageable = rayHit.transform.GetComponent<Damageable>();
                damageable?.TakeDamage(gunSettings.damagePerBullet);
            }

        }
        if (impulseSource != null) // Camera Shake
        {
            impulseSource.GenerateImpulseWithForce(gunSettings.screenShakeStrength);
        }

        bulletsLeft--;    // Decrease ammo
        bulletsShot--;   // Decrease burst ammo
        Invoke(nameof(ResetShot), gunSettings.fireRate);   // Reset shooting flag after fire rate
        if (bulletsShot > 0 && bulletsLeft > 0)
            Invoke(nameof(Shoot), gunSettings.timeBetweenBursts);  // Continue shooting for burst fire if needed

        // Graphics
        SpawnBulletEffects();  // Handle bullet effects
    }

    private void ResetShot() /// Resets the ready to shoot flag after firing.
    {
        readyToShoot = true;  // Allow shooting again
    }

    #endregion

    #region Bullet Effects
    private void SpawnBulletEffects()
    {
        GameObject muzzleFlashTemporary = Instantiate(muzzleFlash, MuzzleFlashPoint.position, Quaternion.identity, particlesParent);
        muzzleFlashTemporary.transform.forward = fpsCam.transform.forward;
        Destroy(muzzleFlashTemporary, 1f);

        if (rayHit.collider != null)
        {
            int hitLayer = rayHit.collider.gameObject.layer;
            if ((whatIsEnemy & (1 << hitLayer)) != 0) // Correct layer mask check
            {
                // Instantiate blood burst if on an enemy layer
                GameObject bloodBurstTemporary = Instantiate(bloodBurst, rayHit.point, Quaternion.Euler(0, 180, 0), particlesParent);
                Destroy(bloodBurstTemporary, 1f);
            }
            else // Instantiate bullet hole for anything else
            {
                GameObject bulletHoleTemporary = Instantiate(bulletHole, rayHit.point, Quaternion.Euler(0, 180, 0), particlesParent);
                bulletHoleTemporary.transform.LookAt(transform);
                Destroy(bulletHoleTemporary, 1f);
            }
        }
    }

    #endregion

    #region Reloading Logic

    private void Reload() /// Starts the reload process.
    {
        reloading = true;   // Start reload process
        reloadFeedbackText.SetActive(true);
        Invoke(nameof(ReloadFinished), gunSettings.reloadTime);  // Complete reload after delay
    }

    private void ReloadFinished() /// Completes the reload by filling magazine and resetting the reload flag.
    {
        bulletsLeft = gunSettings.magazineSize;  // Refill magazine
        reloadFeedbackText.SetActive(false);
        reloading = false;           // End reload process
    }

    #endregion

    #region UI Logic

    private void UpdateAmmoDisplay() /// Updates the ammunition display on the screen.
    {
        ammoCounter.SetText(bulletsLeft + " / " + gunSettings.magazineSize);  // Update ammo text
    }

    #endregion
}