using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
   [SerializeField] private GameObject upgradeMenu;
   

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            upgradeMenu.SetActive(true);
        }
        else 
        {
            upgradeMenu.SetActive(false);
        }
    }
}
