//--------------------------------------------------------------------------------------------------
//  Description: Logic for the player to collect items (not pick up). Only coins for now.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItems : MonoBehaviour
{
    #region Variables

    [Header("Settings")]

    public CoinsLogic coinsLogic;
    string coinTag = "Coin"; // Tag for coin objects

    #endregion

    #region Unity Methods

    /// Checks when a collision with another collider happens.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(coinTag)) // If the object has a coin tag
        {
            coinsLogic.CollectCoin(collision.gameObject); // Collect the coin
        }
    }

    #endregion
}
// oh hi Rene you are going thru my scripts? I left this easter egg for you, so if you are reading this, im dead... no but like if you are reading this dm me skibidi or smth