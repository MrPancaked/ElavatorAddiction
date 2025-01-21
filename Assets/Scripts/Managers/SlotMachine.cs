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

    #endregion

    #region Upgrade Logic

    /// Uses a coin for the upgrade and tells the UpgradeApplicator to apply a new buff
    public void UseCoinForUpgrade()
    {
        if (coinsLogic.coins > 0) // Checks if the player has coins
        {
            coinsLogic.DecreaseCoins(); // Decrease the coin count
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