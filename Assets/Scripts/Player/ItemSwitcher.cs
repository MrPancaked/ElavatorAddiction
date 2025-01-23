//--------------------------------------------------------------------------------------------------
// Description: Manages switching between different items in the game.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Windows;

public class ItemSwitcher : MonoBehaviour
{
    #region Variables

    [Header("Item Settings")]
    public List<GameObject> items; /// List of items to switch between
    private int currentItemIndex = 0; /// Index of current item

    #endregion

    #region Unity Methods

    private void Update()  /// Listens for input events.
    {
        if (Inputs.Instance.selectItem1.WasPressedThisFrame())
        {
            SwitchItem(0); // Select SPAS
        }

        if (Inputs.Instance.selectItem2.WasPressedThisFrame())
        {
            SwitchItem(1); // Select UZI
        }
    }

    #endregion

    #region Switching Logic

    private void SwitchItem(int index) /// Switches to the item at the given index.
    {
        if (index < 0 || index >= items.Count || index == currentItemIndex) return; //Safety check

        if (currentItemIndex < items.Count) // Deactivate current item
        {
            items[currentItemIndex].SetActive(false);
        }

        items[index].SetActive(true); // Activate new item
        currentItemIndex = index; // Set current item index
    }

    #endregion
}