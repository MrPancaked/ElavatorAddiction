using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
   [SerializeField] private GameObject upgradeMenu;
   
    void Update()
    {
        if (Inputs.Instance.stats.IsPressed() && !HealthManager.Instance.PlayerIsDead) // Check for press
        {
            Upgrades.Instance.UpdateUpgradeUI();
            upgradeMenu.SetActive(true);
        }
        else
        {
            upgradeMenu.SetActive(false);
        }
    }
}
