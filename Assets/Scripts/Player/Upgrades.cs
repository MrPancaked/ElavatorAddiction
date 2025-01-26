//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables

    //private shit
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
        int upgradeIndex = Random.Range(0, 2);

        switch (upgradeIndex) // Upgrade player based on the random number
        {
            case 0:
                SpeedUpgrade(); // Apply speed upgrade
                break;
            case 1:
                StrenghUpgrade();  // Apply strengh upgrade
                break;
        }
    }

    private void SpeedUpgrade() /// Applies a speed upgrade to the player and logs it.
    {
        Debug.Log("You got a Speed Buff!");
    }
  
    private void StrenghUpgrade() /// Applies a strengh upgrade to the player and logs it.
    {
        Debug.Log("You got a Strengh Buff!");
    }

    #endregion
}