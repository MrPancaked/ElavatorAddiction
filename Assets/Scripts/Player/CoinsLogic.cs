//--------------------------------------------------------------------------------------------------
//  Description: Stores and manages coins.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using TMPro;
using System.Runtime;

public class CoinsLogic : MonoBehaviour
{
    #region Variables

    public TextMeshProUGUI coinsCounter; // UI text element for coins display
    public static CoinsLogic instance;
    public static CoinsLogic Instance { get { return instance; } }
    public int coins { get; private set; } // Current number of coins collected by the player
    public float dropChance = 0.5f; // Drop chance for coins

    #endregion

    #region Unity Methods

    private void Awake()  /// Makes sure this object survives the scene transition, and that there is only one.
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

    #endregion

    #region Coin Collection Logic

    public void CollectCoin() /// Collects the coin
    {
        coins += Random.Range(1, 6);
        UpdateCoinsDisplay();
        Debug.Log("Coins: " + coins); // Log the collected coins
    }

    public void DecreaseCoins(int amount) /// Decreases the coin count
    {
        coins -= amount; // Decreases coin count
        UpdateCoinsDisplay();
    }

    private void UpdateCoinsDisplay() /// Updates the coins display on the screen.
    {
        coinsCounter.SetText("Coins: " + coins);  // Update coins text
    }

    #endregion
}