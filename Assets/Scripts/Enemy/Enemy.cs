using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Enemy : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public EnemySounds enemySounds;
    public EnemySettings enemySettings;

    [Header("Particles")]
    public GameObject Coins;
    public GameObject Blood;
    public GameObject Flash;

    // Private Variables
    [HideInInspector]
    public bool canDamage = true;    // Flag to control if the enemy can deal damage.
    private bool canAttack = true;     // Flag to control if the enemy can attack.
    private float currentOrbitRadius; // Radius for orbit movement.
    private float hoverTimer = 0.0f;  // Timer for hover movement.
    private Rigidbody rb; // Rigidbody for physics-based movement.
    private GameObject playerObject; // Reference to the player game object.
    private Transform playerTransform;  // Reference to the player transform.
    private Collider playerCollider; // Reference to the player collider.
    private Vector3 hoverStartPosition; // Starting position of the hover movement.
    private Health health;          // Reference to the health script on this game object.
    private EnemyCounter enemyCounter; // Reference to the enemy counter script.
    private EnemyState currentState = EnemyState.Idle;  // Current state of the enemy.
    private enum EnemyState
    {
        Idle,    // State for idle hovering
        Attacking // State for attacking the player
    }

    #endregion

    #region Unity Methods

    private void Awake()  /// Called when the script instance is being loaded. Initializes references to health, player objects, and the starting position
    {
        rb = GetComponent<Rigidbody>(); // Find the Rigidbody
        health = GetComponent<Health>();
        health.HandleDeathMethod += Die;
        health.TakeDamageMethod += TakeDamage;
        rb.freezeRotation = true;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerCollider = playerObject.GetComponent<Collider>();
        enemyCounter = GameObject.FindGameObjectWithTag("EnemyCounter").GetComponent<EnemyCounter>();
        if (enemyCounter != null) // Check if enemyCounter is not null AND the count is greater than 0
        {
            enemyCounter.UpdateEnemyCounter();
        }
    }

    private void Start()  /// Called before the first frame update. Initializes the hover start position and orbit radius.
    {
        hoverStartPosition = transform.position;
        currentOrbitRadius = Random.Range(0f, enemySettings.idleFloatingRadius);
        enemySounds.IdleSoundStart();
    }

    private void OnDisable() /// Called when the enemy is disabled, remove the die event
    {
        health.HandleDeathMethod -= Die;
        health.TakeDamageMethod -= TakeDamage;
        enemyCounter.enemyCount--; // Decrease the enemy counter
        enemyCounter.UpdateEnemyCounter();
    }

    private void Update()  /// Called every frame. Tracks player position, updates hover timer, and manages the enemy's states.
    {
        playerTransform = playerObject.transform;
        hoverTimer += Time.deltaTime;
        enemySounds.UpdateIdleSound();

        switch (currentState)
        {
            case EnemyState.Idle:
                IdleState();
                if (canAttack)
                {
                    DetectPlayer();
                }
                break;
            case EnemyState.Attacking:
                AttackState();
                break;
        }
    }

    #endregion

    #region States Logic

    private void IdleState()   /// Handles the idle state of the enemy, makes the enemy hover in a circular pattern and makes the enemy rotate with the movement
    {
        Vector3 circlePos = hoverStartPosition + new Vector3(Mathf.Cos(hoverTimer * enemySettings.idleSpeed) * currentOrbitRadius, Mathf.Sin(hoverTimer * enemySettings.idleSpeed) * enemySettings.idleFloatingHeight, Mathf.Sin(hoverTimer * enemySettings.idleSpeed) * currentOrbitRadius);
        rb.drag = enemySettings.idleDrag;
        rb.AddForce((circlePos - transform.position) * enemySettings.idleSpeed);

        // Rotate the enemy to face the direction of movement
        Vector3 direction = circlePos - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 1000 * Time.deltaTime);
        }
    }

    private void DetectPlayer()   /// Detects the player within the detection range and transitions to the attack state if the player is visible.
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer <= enemySettings.detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, enemySettings.detectionRange))
            {
                if (hit.collider == playerCollider)
                {
                    currentState = EnemyState.Attacking;
                    rb.drag = enemySettings.attackDrag + 0.1f * DifficultyManager.Instance.RoomIndex;
                }
            }
        }
    }

    private void AttackState()  /// Handles the attack state of the enemy, makes the enemy chase the player, and face the forward
    {
        Vector3 chaseDirection = (playerTransform.position - transform.position).normalized;
        Vector3 randomDirection = new Vector3(Random.Range(-enemySettings.attackOffset, enemySettings.attackOffset),
                                              Random.Range(-enemySettings.attackOffset, enemySettings.attackOffset),
                                              Random.Range(-enemySettings.attackOffset, enemySettings.attackOffset));
        rb.drag = enemySettings.attackDrag;
        rb.AddForce((chaseDirection + randomDirection) * (enemySettings.attackSpeed + DifficultyManager.Instance.RoomIndex * 0.3f));

        // Smoothly rotate the enemy to face the direction of movement
        if (chaseDirection != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(chaseDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 25 * Time.deltaTime);
        }

    }

    #endregion

    #region Death/Hit Logic

    public void Die() /// Handles the death of the enemy, disables the model, drops loot, and destroys itself.
    {
        enemyCounter.UpdateEnemyCounter(); // Update the enemy counter

        if (Random.Range(0, 1f) <= CoinsLogic.Instance.coinDropChance)
        {
            enemySounds.CoinsSound();
            GameObject coins = Instantiate(Coins, transform.position, Quaternion.identity); // Spawn the coin burst
            CoinsLogic.Instance.CollectCoin();
            gameObject.SetActive(false); // Disables the enemy model
        }
        else
        {
            GameObject flash = Instantiate(Flash, transform.position, Quaternion.identity); // Spawn the coin burst
            enemySounds.DeathSound();
            gameObject.SetActive(false); // Disables the enemy model
        }
    }

    public void TakeDamage(float damage)
    {
        GameObject blood = Instantiate(Blood, transform.position, Quaternion.identity); // Spawn the blood burst
        enemySounds.DamageSound();
    }

    #endregion
}