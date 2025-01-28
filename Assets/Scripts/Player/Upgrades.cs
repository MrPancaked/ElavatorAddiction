//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables
    public float damageIncrease;
    public float speedIncrease;
    public float healthIncrease;
    public float HealAmount;
    public float FireRateMultiplier;

    private NewMovement playerMovement; // Reference to the player script
    private Gun Shotgun; // Reference to the player's gun script
    private HealthManager healthManager; // Reference to the health manager script


    //private shit
    public static Upgrades instance;
    public static Upgrades Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake() // Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<NewMovement>();
        Shotgun = GameObject.FindGameObjectWithTag("Shotgun").GetComponent<Gun>();
        Debug.Log(playerMovement);
        Debug.Log(Shotgun);
    }

    #endregion

    #region Upgrade Logic

    public void ApplyRandomUpgrade() /// Applies a random upgrade to the player.
    {
        int upgradeIndex = Random.Range(0, 2);

        switch (upgradeIndex) // Upgrade player based on the random number
        {
            case 0:
                SpeedUpgrade(speedIncrease); // Apply speed upgrade
                break;
            case 1:
                DamageUpgrade(damageIncrease);  // Apply strengh upgrade
                break;
        }
    }

    private void SpeedUpgrade(float speedIncrease) /// Applies a speed upgrade to the player and logs it.
    {
        playerMovement.speed += speedIncrease;
        Debug.Log("Speed upgrade: " + playerMovement.speed);
    }

    private void DamageUpgrade(float damageIncrease) /// Applies a strengh upgrade to the player and logs it.
    {
        Shotgun.damageMultiplier += damageIncrease;
        Debug.Log("Damage upgrade: " + Shotgun.damageMultiplier);
    }

    private void healthUpgrade() /// Applies a strengh upgrade to the player and logs it.
    {
        healthManager.HealthUpgrade(healthIncrease, HealAmount);
        Debug.Log("Healed by: " + HealAmount);
        Debug.Log("Health increased: " + healthIncrease);
    }

    private void FireRateUpgrade(float fireRateMultiplier) /// Applies a fire rate upgrade to the player and logs it.
    {
        Shotgun.gunSettings.fireRate *= fireRateMultiplier;
        Debug.Log("Fire rate upgrade: " + Shotgun.gunSettings.fireRate);
    }

    #endregion
}