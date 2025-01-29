//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using System.Collections;
using TMPro;
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables
    [Header("SlotSettings")]
    public float slotSpinSpeed = 0.5f;

    [Header("Settings")]
    public float damageIncrease;
    public float speedIncrease;
    public float healthIncrease;
    public float healAmount;
    public float fireRateMultiplier;
    public float dropchanceMultiplier;

    [Header("Upgrade UI references")]
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dropChanceText;

    //private shit
    private Gun Shotgun; // Reference to the player's gun script
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
            Shotgun = GameObject.FindGameObjectWithTag("Shotgun").GetComponent<Gun>();
            slots = GameObject.FindGameObjectsWithTag("Slots");
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                SpeedUpgrade(speedIncrease); // Apply speed upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.06f);
                break;
            case 1:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                DamageUpgrade(damageIncrease);  // Apply strengh upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.73f);
                break;
            case 2:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                healthUpgrade(healthIncrease, healAmount); // Apply health upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.41f);
                break;
            case 3:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                FireRateUpgrade(fireRateMultiplier); // Apply fire rate upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.9f);
                break;
            case 4:
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                DropChanceUpgrade(dropchanceMultiplier); // Apply drop chance upgrade
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.23f);
                break;
            case 5:

                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = slotSpinSpeed;
                yield return new WaitForSeconds(1f); // Time delay
                slots[slotIndex].GetComponent<ScrollingTexture>().AnimationSpeed = 0;
                Lose(); // Lose
                slots[slotIndex].GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0f, 0.57f);
                break;
        }
    }

    private void SpeedUpgrade(float speedIncrease) /// Applies a speed upgrade to the player and logs it.
    {
        NewMovement.Instance.speed += speedIncrease;
        UpdateUpgradeUI();
        Debug.Log("Speed upgrade: " + NewMovement.Instance.speed);
    }

    private void DamageUpgrade(float damageIncrease) /// Applies a strengh upgrade to the player and logs it.
    {
        Shotgun.extraDamage += damageIncrease;
        UpdateUpgradeUI();
        Debug.Log("Damage upgrade: " + Shotgun.extraDamage);
    }

    private void healthUpgrade(float healthIncrease, float healAmount) /// Applies a strengh upgrade to the player and logs it.
    {
        HealthManager.Instance.initialHealth += healthIncrease;
        HealthManager.Instance.health.hp += healAmount;
        HealthManager.Instance.UpdateHealthUI();
        UpdateUpgradeUI();
        Debug.Log("Healed by: " + healAmount);
        Debug.Log("Health increased: " + healthIncrease);
    }

    private void FireRateUpgrade(float fireRateMultiplier) /// Applies a fire rate upgrade to the player and logs it.
    {
        Shotgun.gunSettings.fireRate *= fireRateMultiplier;
        UpdateUpgradeUI();
        Debug.Log("Fire rate upgrade: " + Shotgun.gunSettings.fireRate);
    }

    private void DropChanceUpgrade(float dropchanceMultiplier)
    { 
        CoinsLogic.Instance.coinDropChance += dropchanceMultiplier;
        UpdateUpgradeUI();
        Debug.Log("Drop chance upgrade: " + CoinsLogic.Instance.coinDropChance);
    }

    private void Lose()
    {
        Debug.Log("LOSER!");
    }

    public void UpdateUpgradeUI()
    { 
        damageText.text = (Shotgun.gunSettings.damagePerBullet + Shotgun.extraDamage).ToString();
        fireRateText.text = 1/Shotgun.gunSettings.fireRate * 100 + "%";
        speedText.text = NewMovement.Instance.speed.ToString();
        healthText.text = HealthManager.Instance.initialHealth.ToString();
        dropChanceText.text = CoinsLogic.Instance.coinDropChance * 100 + "%";
    }

    #endregion
}