//--------------------------------------------------------------------------------------------------
//  Description: Stores and manages coins.
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class CoinsLogic : MonoBehaviour
{
    #region Variables

    public int coins { get; private set; } // Current number of coins collected by the player

    #endregion

    #region Coin Collection Logic

    /// Collects the coin and destroys it.
    public void CollectCoin(GameObject coinObject)
    {
        coins++;  // Increase the coin count
        Debug.Log("Coins: " + coins); // Log the collected coins
        Destroy(coinObject);  // Destroy the coin
    }

    /// Decreases the coin count
    public void DecreaseCoins()
    {
        coins--; // Decreases coin count
    }

    #endregion
}