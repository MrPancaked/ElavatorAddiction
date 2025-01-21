//--------------------------------------------------------------------------------------------------
// Description: Handles damaging the player via a trigger collider.
//              Applies damage per second, and stops when player leaves the collider.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;

public class Damages : MonoBehaviour
{
    #region Variables

    public Enemy enemy;           // Reference to the enemy script to access canDamage.
    public float damagePerHit;    // Amount of damage per hit.
    public float damageCooldown;  // Time between damage instances.
    private GameObject playerObject; // Reference to the player game object.
    private Coroutine damageRoutine; // Reference to a coroutine for damage loop.
    private bool isPlayerInside = false; // Flag to track player presence.

    #endregion

    #region Collision Logic

    private void OnTriggerEnter(Collider otherCollider) /// Called when another collider enters the trigger collider.
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            playerObject = otherCollider.gameObject;
            isPlayerInside = true; // Set the flag when player enters
            if (enemy.canDamage)
            {
                damageRoutine = StartCoroutine(DamageLoop());
            }
        }
    }

    private void OnTriggerExit(Collider otherCollider) /// Called when another collider leaves the trigger collider.
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            isPlayerInside = false; // Clear flag when player exits
            if (damageRoutine != null)
            {
                StopCoroutine(damageRoutine);
                enemy.canDamage = true;
            }
        }
    }

    IEnumerator DamageLoop() /// Coroutine that continuously damages the player while inside the trigger.
    {
        while (isPlayerInside)
        {
            if (enemy.canDamage && playerObject != null)
            {
                Damageable damageable = playerObject.GetComponent<Damageable>(); // Damage the player
                damageable.TakeDamage(damagePerHit);
                Debug.Log("Player Damaged by trigger");
                enemy.canDamage = false;
                yield return new WaitForSeconds(damageCooldown);
                enemy.canDamage = true;
            }
            yield return null; //Wait for one frame before looping
        }
    }

    #endregion
}