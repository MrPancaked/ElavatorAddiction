//--------------------------------------------------------------------------------------------------
//  Description: Manages the button press, coin subtraction, and triggers the upgrade application.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class SlotMachine : MonoBehaviour
{
    #region Variables

    [Header("Settings")]
    public CoinsLogic coinsLogic; // The CoinCollector component
    public Upgrades upgrades; // The UpgradeApplicator component

    // Private shit
    private static SlotMachine instance;
    public static SlotMachine Instance
    {
        get { return instance; }
    }
    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Enforce singleton pattern
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
    }

    #endregion

    #region Upgrade Logic

    /// Uses a coin for the upgrade and tells the UpgradeApplicator to apply a new buff
    public void UseCoinForUpgrade()
    {
        if (coinsLogic.coins > 0) // Checks if the player has coins
        {
            coinsLogic.DecreaseCoins(10); // Decrease the coin count
            upgrades.ApplyRandomUpgrade(); // Apply a random upgrade
            Debug.Log("Coins: " + coinsLogic.coins); // Log the new coin amount
        }
        else
        {
            Debug.Log("No coins.");  // Log if there are no coins available
        }

    }

    #endregion
}