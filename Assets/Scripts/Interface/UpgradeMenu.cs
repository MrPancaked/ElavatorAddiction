using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Add the new input system namespace


public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private Animator upgradeAnimator;
    private bool isMenuOpen = false;

    void Update()
    {
        if (HealthManager.Instance.PlayerIsDead)
        {
            // Don't do anything if the player is dead
            return;
        }

        if (Inputs.Instance.stats.IsPressed())
        {
            if (!isMenuOpen)
            {
                Upgrades.Instance.UpdateUpgradeUI();
                upgradeAnimator.SetTrigger("Open");
                isMenuOpen = true;
            }
        }
        else if (isMenuOpen)
        {
            upgradeAnimator.SetTrigger("Close");
            isMenuOpen = false;
        }
    }
}