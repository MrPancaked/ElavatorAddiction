//--------------------------------------------------------------------------------------------------
//  Description: Stores and manages coins.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;
using System.Runtime;
using System.Collections;

public class CoinsLogic : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public TextMeshProUGUI coinsCounter; // UI text element for coins display

    [Header("Settings")]
    public int initialPlayerCoins = 0; // Set this in the Inspector
    public float coinDropChance = 0.5f; // Drop chance for coins
    public int spinCost = 10; // Cost of the upgrade
    public int playerCoins
    { get; private set; } // Current number of coins collected by the player
    public static CoinsLogic instance;
    public static CoinsLogic Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            ResetCoins();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    #endregion

    #region Coin Collection Logic

    public void CollectCoin() /// Collects the coin
    {
        playerCoins += Random.Range(2, 8);
        UpdateCoinsDisplay();
        Debug.Log("Collected coins: " + playerCoins); // Log the collected coins
    }

    public void ResetCoins() /// Reset on death
    {
        playerCoins = initialPlayerCoins;
        coinDropChance = 0.5f;
        UpdateCoinsDisplay();
    }

    public IEnumerator UseCoinForUpgrade() /// Uses a coin for the upgrade and tells the UpgradeApplicator to apply a new buff
    {
        playerCoins -= spinCost; // Decreases coin count
        UpdateCoinsDisplay();
        StartCoroutine(Upgrades.Instance.SlotSpinCoroutine(2));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Upgrades.Instance.SlotSpinCoroutine(1));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Upgrades.Instance.SlotSpinCoroutine(0));
        Debug.Log("Spent coins: " + playerCoins); // Log the decreased coins
    }

    private void UpdateCoinsDisplay() /// Updates the coins display on the screen.
    {
        coinsCounter.SetText("$" + playerCoins);  // Update coins text
    }

    #endregion
}