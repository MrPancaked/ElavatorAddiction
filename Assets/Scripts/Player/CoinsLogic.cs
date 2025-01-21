//--------------------------------------------------------------------------------------------------
//  Description: Stores and manages coins.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;
using System.Runtime;

public class CoinsLogic : MonoBehaviour
{
    #region Variables

    public int coins { get; private set; } // Current number of coins collected by the player
    public TextMeshProUGUI coinsCounter; // UI text element for coins display
    #endregion

    #region Coin Collection Logic
   
    public void CollectCoin(GameObject coinObject) /// Collects the coin and destroys it.
    {
        coins++;  // Increase the coin count
        UpdateCoinsDisplay();
        Debug.Log("Coins: " + coins); // Log the collected coins
        Destroy(coinObject);  // Destroy the coin
    }

    public void DecreaseCoins() /// Decreases the coin count
    {
        coins--; // Decreases coin count
        UpdateCoinsDisplay();
    }
    private void UpdateCoinsDisplay() /// Updates the coins display on the screen.
    {
        coinsCounter.SetText("Coins: " + coins);  // Update coins text
    }

    #endregion
}