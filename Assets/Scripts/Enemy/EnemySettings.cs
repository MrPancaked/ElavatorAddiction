//--------------------------------------------------------------------------------------------------
//  Description: Stores all the enemy stats
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Enemy", menuName = "Items/Enemy")]
public class EnemySettings : ScriptableObject
{
    #region Variables

    [Header("Base")]
    public new string name; // Name of the gun
    public float detectionRange;  // How far the enemies see.
    public float attackScreenShake;  // Amount of screenshake when attacking the player.

    [Header("Attack state")]
    public float attackSpeed;  // Speed while chasing the player
    public float attackDrag;   // Drag applied when attacking.
    public float attackOffset; // Randomness to chase direction
    public float attackCooldownMin;  // Minimum Cooldown for attacking.
    public float attackCooldownMax;   // Maximum Cooldown for attacking.

    [Header("Idle state")]
    public float idleSpeed;   // Speed of the hover movement.
    public float idleDrag;    // Drag applied when in idle state
    public float idleFloatingHeight;  // Vertical distance the enemy hovers.
    public float idleFloatingRadius;  // Variation on orbit radius.

    [Header("Audio")]
    public EventReference spawnSound;
    public EventReference damageSound;
    public EventReference idleSound;
    public EventReference deathSound;

    #endregion
}
