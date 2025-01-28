//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    public float damageIncrease;
    public float speedIncrease;
    public float healthIncrease;
    public float healAmount;
    public float fireRateMultiplier;
    public float dropchanceMultiplier;

    //private shit
    private Gun Shotgun; // Reference to the player's gun script
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            Shotgun = GameObject.FindGameObjectWithTag("Shotgun").GetComponent<Gun>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Upgrade Logic

    public void ApplyRandomUpgrade() /// Applies a random upgrade to the player.
    {
        int upgradeIndex = Random.Range(0, 6);

        switch (upgradeIndex) // Upgrade player based on the random number
        {
            case 0:
                SpeedUpgrade(speedIncrease); // Apply speed upgrade
                break;
            case 1:
                DamageUpgrade(damageIncrease);  // Apply strengh upgrade
                break;
            case 2:
                healthUpgrade(healthIncrease, healAmount); // Apply health upgrade
                break;
            case 3:
                FireRateUpgrade(fireRateMultiplier); // Apply fire rate upgrade
                break;
            case 4:
                DropChanceUpgrade(dropchanceMultiplier); // Apply drop chance upgrade
                break;
            case 5:
                Lose(); // Lose
                break;
        }
    }

    private void SpeedUpgrade(float speedIncrease) /// Applies a speed upgrade to the player and logs it.
    {
        NewMovement.Instance.speed += speedIncrease;
        Debug.Log("Speed upgrade: " + NewMovement.Instance.speed);
    }

    private void DamageUpgrade(float damageIncrease) /// Applies a strengh upgrade to the player and logs it.
    {
        Shotgun.damageMultiplier += damageIncrease;
        Debug.Log("Damage upgrade: " + Shotgun.damageMultiplier);
    }

    private void healthUpgrade(float healthIncrease, float healAmount) /// Applies a strengh upgrade to the player and logs it.
    {
        HealthManager.Instance.initialHealth += healthIncrease;
        HealthManager.Instance.health.hp += healAmount;
        HealthManager.Instance.UpdateHealthUI();
        Debug.Log("Healed by: " + healAmount);
        Debug.Log("Health increased: " + healthIncrease);
    }

    private void FireRateUpgrade(float fireRateMultiplier) /// Applies a fire rate upgrade to the player and logs it.
    {
        Shotgun.gunSettings.fireRate *= fireRateMultiplier;
        Debug.Log("Fire rate upgrade: " + Shotgun.gunSettings.fireRate);
    }

    private void DropChanceUpgrade(float dropchanceMultiplier)
    { 
        CoinsLogic.Instance.coinDropChance += dropchanceMultiplier;
    }

    private void Lose()
    {
        Debug.Log("LOSER!");
    }

    #endregion
}