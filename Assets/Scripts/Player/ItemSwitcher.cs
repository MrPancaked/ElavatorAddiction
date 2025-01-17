using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ItemSwitcher : MonoBehaviour
{
    #region Variables

    [Header("Input Settings")]
    [SerializeField] private InputActionAsset controls; //Reference to the InputActions Asset
    private InputAction switchItem1;
    private InputAction switchItem2;

    [Header("Item Settings")]
    public List<GameObject> items; // List of items to switch between
    private int currentItemIndex = 0; // Index of current item

    #endregion

    #region Unity Methods

    /// Initializes input actions, disables all items except the first one.
    private void Awake()
    {
        //Get our input actions
        switchItem1 = controls.FindActionMap("Player").FindAction("SelectItem1");
        switchItem2 = controls.FindActionMap("Player").FindAction("SelectItem2");

        SwitchItem(0); // Select SPAS
        // Disable all items except the first one               
        //for (int i = 1; i < items.Count; i++)
        //{
        //    items[i].SetActive(false);
        //}
    }
    private void OnEnable()
    {
        //Enable our input actions
        switchItem1.Enable();
        switchItem2.Enable();
    }
    private void OnDisable()
    {
        //Disable our input actions
        switchItem1.Disable();
        switchItem2.Disable();
    }
    /// Listens for input events.
    private void Update()
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
    /// Switches to the item at the given index.
    private void SwitchItem(int index)
    {
        if (index < 0 || index >= items.Count || index == currentItemIndex) return; //Safety check

        // Deactivate current item
        if (currentItemIndex < items.Count)
        {
            items[currentItemIndex].SetActive(false);
        }
        // Activate new item
        items[index].SetActive(true);

        // Set current item index
        currentItemIndex = index;
    }
    #endregion
}