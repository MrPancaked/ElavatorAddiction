//--------------------------------------------------------------------------------------------------
//  Description: Stores all the gun stats
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "Gun", menuName = "Items/Gun")]
public class GunSettings : ScriptableObject
{
    #region Variables

    [Header("Base")]
    public new string name; // Name of the gun
    public int damagePerBullet; // Damage inflicted by each bullet
    public float fireRate; // Time in seconds between individual shots
    public float reloadTime; // Time in seconds for a reload
    public int magazineSize; // Maximum number of bullets in the magazine
    
    [Header("Burst")]
    public int bulletsPerShot; // Number of bullets fired per single shot or burst
    public float timeBetweenBulletsInBursts; // Time in seconds between bursts of fire if using burst fire
    public bool allowContinuesFire; // Allows holding fire button for automatic fire

    [Header("Other")]
    public float spread; // Maximum deviation of bullet direction
    public float range; // Maximum range of the gun
    public float enemyPushbackForce; // How much the enemy is pushed back with the bullets
    public float playerPushbackForce; // How much the play is pushed back when shooting
    public float screenShakeStrength; // Amount of screen shake when you shoot
    public bool isDualGun = false; // Flag to determine if this is a dual gun set

    [Header("Audio")]
    public EventReference gunShotSound;
    public EventReference reloadSound;
    public EventReference blankSound;
    public EventReference equipSound;
    public EventReference bulletHit;

    #endregion
}
