using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using TMPro;

public class UpgradeUI : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public Animator upgradeAnimator;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI dropChanceText;

    // Private shit
    private bool isMenuOpen = false;
    public static UpgradeUI instance;
    public static UpgradeUI Instance { get { return instance; } }

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

    void Update()
    {
        if (HealthManager.Instance.PlayerIsDead) // Don't do anything if the player is dead
        {
            return;
        }

        if (Inputs.Instance.stats.IsPressed())
        {
            if (!isMenuOpen)  // Open menu when pressed
            {
                UpdateUI();
                upgradeAnimator.SetTrigger("Open");
                isMenuOpen = true;
            }
        }
        else if (isMenuOpen) // Close menu when released
        {
            upgradeAnimator.SetTrigger("Close");
            isMenuOpen = false;
        }
    }

    #endregion

    #region UI

    public void UpdateUI()
    {
        damageText.text = (Upgrades.Instance.Shotgun.gunSettings.damagePerBullet + Upgrades.Instance.Shotgun.gunSettings.extraDamage).ToString();
        fireRateText.text = Mathf.RoundToInt(1 / Upgrades.Instance.Shotgun.gunSettings.fireRate * 100) + "%";
        speedText.text = Mathf.RoundToInt((PlayerMovement.Instance.runSpeed / PlayerMovement.Instance.originalSpeed) * 100) + "%";
        healthText.text = HealthManager.Instance.initialHealth.ToString();
        dropChanceText.text = Mathf.RoundToInt(CoinsLogic.Instance.coinDropChance * 100) + "%";
    }

    #endregion
}
