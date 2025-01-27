//--------------------------------------------------------------------------------------------------
// Description: Handles damaging the player via a trigger collider.
//              Applies damage per second, and stops when player leaves the collider.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using UnityEngine;

public class Damages : MonoBehaviour
{
    #region Variables

    public GameObject damageController;  // Reference to the gameobject that has an Enemy script
    public float damagePerHit;         // Amount of damage per hit.
    public float damageCooldown;        // Time between damage instances.
    public bool needsAnEnemy = true;  // Flag to control whether an enemy script is needed
    private GameObject playerObject;    // Reference to the player game object.
    private Coroutine damageRoutine;    // Reference to a coroutine for damage loop.
    private bool isPlayerInside = false; // Flag to track player presence.
    private Enemy enemyScript; // Reference to the enemy component


    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (needsAnEnemy)
        {
            enemyScript = damageController.GetComponent<Enemy>(); // Try to get the Enemy script.
        }
    }

    #endregion

    #region Collision Logic

    private void OnTriggerEnter(Collider otherCollider) /// Called when another collider enters the trigger collider.
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            playerObject = otherCollider.gameObject;
            isPlayerInside = true; // Set the flag when player enters
            if (needsAnEnemy)
            {
                if (enemyScript != null && enemyScript.canDamage)
                {
                    damageRoutine = StartCoroutine(DamageLoop());
                }
            }
            else
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
                if (enemyScript != null)
                {
                    enemyScript.canDamage = true;
                }
            }
        }
    }

    IEnumerator DamageLoop() /// Coroutine that continuously damages the player while inside the trigger.
    {
        while (isPlayerInside)
        {
            if (needsAnEnemy)
            {
                if (enemyScript != null && enemyScript.canDamage && playerObject != null)
                {
                    Damageable damageable = playerObject.GetComponent<Damageable>(); // Damage the player
                    damageable.TakeDamage(damagePerHit);
                    Debug.Log("Player Damaged by trigger");
                    enemyScript.canDamage = false;
                    yield return new WaitForSeconds(damageCooldown);
                    enemyScript.canDamage = true;
                }
            }
            else
            {
                Damageable damageable = playerObject.GetComponent<Damageable>(); // Damage the player
                damageable.TakeDamage(damagePerHit);
                Debug.Log("Player Damaged by trigger");
                yield return new WaitForSeconds(damageCooldown);
            }
            yield return null; //Wait for one frame before looping
        }
    }

    #endregion
}