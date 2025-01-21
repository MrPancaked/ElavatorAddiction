//--------------------------------------------------------------------------------------------------
// Description: Manages switching between different items in the game.
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ItemSwitcher : MonoBehaviour
{
    #region Variables

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset controls; /// Reference to the InputActions Asset
    private InputAction switchItem1; /// Input action to switch to item 1
    private InputAction switchItem2; /// Input action to switch to item 2

    [Header("Item Settings")]
    public List<GameObject> items; /// List of items to switch between
    private int currentItemIndex = 0; /// Index of current item

    #endregion

    #region Unity Methods

    private void Awake() /// Initializes input actions, disables all items except the first one.
    {
        switchItem1 = controls.FindActionMap("Player").FindAction("SelectItem1"); //Get our input actions
        switchItem2 = controls.FindActionMap("Player").FindAction("SelectItem2");

        SwitchItem(0); // Select SPAS
    }

    private void OnEnable()  /// Enable our input actions
    {
        switchItem1.Enable();
        switchItem2.Enable();
    }

    private void OnDisable() /// Disable our input actions
    {
        switchItem1.Disable();
        switchItem2.Disable();
    }

    private void Update()  /// Listens for input events.
    {
        if (switchItem1.triggered)
        {
            SwitchItem(0); // Select SPAS
        }

        if (switchItem2.triggered)
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