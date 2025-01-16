//--------------------------------------------------------------------------------------------------
//  Description: Applies upgrades to the player and handles the buff logic (debug logs only).
//--------------------------------------------------------------------------------------------------
using UnityEngine;

public class Upgrades : MonoBehaviour
{
    #region Variables

    //[Header("Settings")]

    #endregion

    #region Upgrade Logic

    /// Applies a random upgrade to the player.
    public void ApplyRandomUpgrade()
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

    /// Applies a speed upgrade to the player and logs it.
    private void SpeedUpgrade()
    {
        Debug.Log("You got a Speed Buff!");
    }

    /// Applies a strengh upgrade to the player and logs it.
    private void StrenghUpgrade()
    {
        Debug.Log("You got a Strengh Buff!");
    }

    #endregion
}