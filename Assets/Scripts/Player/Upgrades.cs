//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Gun Shotgun; // Reference to the player's gun script

    [Header("Settings")]
    public float slotSpinSpeed = 8f;
    public float damageIncrease = 1f;
    public float speedIncrease = 15f;
    public float healthIncrease = 10f;
    public float healAmount = 20f;
    public float fireRateMultiplier = 0.95f;
    public float dropchanceMultiplier = 1.5f;

    //private shit
    private GameObject[] slots;
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
            slots = GameObject.FindGameObjectsWithTag("Slots");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        ResetUpgrades();
    }

    #endregion

    #region Upgrade Logic

    public IEnumerator SlotSpinCoroutine(int slotIndex)
    {
        int upgradeIndex = Random.Range(0, 6);

        switch (upgradeIndex) // Upgrade player based on the random number
        {
            case 0:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                SpeedUpgrade(speedIncrease); // Apply speed upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.06f);
                SlotsSounds.Instance.PlayWinSound();
                break;
            case 1:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                DamageUpgrade(damageIncrease);  // Apply strengh upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.73f);
                SlotsSounds.Instance.PlayWinSound();
                break;
            case 2:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                healthUpgrade(healthIncrease, healAmount); // Apply health upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.41f);
                SlotsSounds.Instance.PlayWinSound();
                break;
            case 3:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                FireRateUpgrade(fireRateMultiplier); // Apply fire rate upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.9f);
                SlotsSounds.Instance.PlayWinSound();
                break;
            case 4:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                DropChanceUpgrade(dropchanceMultiplier); // Apply drop chance upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.23f);
                SlotsSounds.Instance.PlayWinSound();
                break;
            case 5:

                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(0.4f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                Lose(); // Lose
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.57f);
                SlotsSounds.Instance.PlayLoseSound();
                break;
        }

        UpgradeUI.Instance.UpdateUI();
    }

    private void SpeedUpgrade(float speedIncrease) /// Applies a speed upgrade to the player and logs it.
    {
        PlayerMovement.Instance.runSpeed += speedIncrease;
        Debug.Log("Speed upgrade: " + PlayerMovement.Instance.runSpeed);
    }

    private void DamageUpgrade(float damageIncrease) /// Applies a strengh upgrade to the player and logs it.
    {
        Shotgun.gunSettings.extraDamage += damageIncrease;
        Debug.Log("Damage upgrade: " + Shotgun.gunSettings.extraDamage);
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
        CoinsLogic.Instance.coinDropChance *= dropchanceMultiplier;
        Debug.Log("Drop chance upgrade: " + CoinsLogic.Instance.coinDropChance);
    }

    private void Lose()
    {
        Debug.Log("LOSER!");
    }

    #endregion

    #region Reset Logic

    public void ResetUpgrades()
    {
        Shotgun.gunSettings.extraDamage = Shotgun.originalExtraDamage;
        Shotgun.gunSettings.fireRate = Shotgun.originalFireRate;
        PlayerMovement.Instance.runSpeed = PlayerMovement.Instance.originalSpeed;
        HealthManager.Instance.ResetHealth();
    }

    #endregion
}