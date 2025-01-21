//--------------------------------------------------------------------------------------------------
//  Description: Controls the enemy's behavior, including idle hovering and attacking the player.
//               Changes between states with detection of player range, and manages movement via rigidbody.
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    #region Variables

    [Header("Attack Settings")]
    public float attackSpeed = 5.0f;  // Speed while chasing the player
    public float attackDrag = 1.0f;   // Drag applied when attacking.
    public float attackPointOffset = 0.2f; // Randomness to chase direction
    public float minAttackCooldown = 1.0f;  // Minimum Cooldown for attacking.
    public float maxAttackCooldown = 3.0f;   // Maximum Cooldown for attacking.

    [Header("Idle Settings")]
    public float idleSpeed = 2.0f;   // Speed of the hover movement.
    public float idleDrag = 0.5f;    // Drag applied when in idle state
    public float idleHeight = 1.0f;  // Vertical distance the enemy hovers.
    public float idleRadius = 1.0f;  // Variation on orbit radius.

    [Header("Other Settings")]
    public float detectionRange;  // How far the enemies see.

    [Header("References")]
    public GameObject enemyModel;   // Visual model of the enemy.
    public Rigidbody rb;           // Rigidbody for physics-based movement.
    public Collider triggerCollider; // Reference to the trigger collider

    // Private Variables
    public bool canDamage = true;    // Flag to control if the enemy can deal damage.
    private bool canAttack = true;     // Flag to control if the enemy can attack.
    private float currentOrbitRadius; // Radius for orbit movement.
    private float hoverTimer = 0.0f;  // Timer for hover movement.
    private GameObject playerObject; // Reference to the player game object.
    private Transform playerTransform;  // Reference to the player transform.
    private Collider playerCollider; // Reference to the player collider.
    private Health health;          // Reference to the health script on this game object.
    private Vector3 hoverStartPosition; // Starting position of the hover movement.
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
        health = GetComponent<Health>();
        health.Die += Die;
        rb.freezeRotation = true;
        playerObject = GameObject.FindGameObjectWithTag("Player");
        playerCollider = playerObject.GetComponent<Collider>();
    }

    private void Start()  /// Called before the first frame update. Initializes the hover start position and orbit radius.
    {
        hoverStartPosition = transform.position;
        currentOrbitRadius = Random.Range(0f, idleRadius);
    }

    private void OnDisable() /// Called when the enemy is disabled, remove the die event
    {
        health.Die -= Die;
    }

    private void Update()  /// Called every frame. Tracks player position, updates hover timer, and manages the enemy's states.
    {
        playerTransform = playerObject.transform;
        hoverTimer += Time.deltaTime;

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

    private void IdleState()   /// Handles the idle state of the enemy, makes the enemy hover in a circular pattern.
    {
        Vector3 circlePos = hoverStartPosition + new Vector3(Mathf.Cos(hoverTimer * idleSpeed) * currentOrbitRadius, Mathf.Sin(hoverTimer * idleSpeed) * idleHeight, Mathf.Sin(hoverTimer * idleSpeed) * currentOrbitRadius);
        rb.drag = idleDrag;
        rb.AddForce((circlePos - transform.position) * idleSpeed);
    }

    private void DetectPlayer()   /// Detects the player within the detection range and transitions to the attack state if the player is visible.
    {
        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        if (distanceToPlayer <= detectionRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer, out hit, detectionRange))
            {
                if (hit.collider == playerCollider)
                {
                    currentState = EnemyState.Attacking;
                    rb.drag = attackDrag;
                }
            }
        }
    }

    private void AttackState()  /// Handles the attack state of the enemy, makes the enemy chase the player.
    {
        Vector3 chaseDirection = (playerTransform.position - transform.position).normalized;
        Vector3 randomDirection = new Vector3(Random.Range(-attackPointOffset, attackPointOffset), Random.Range(-attackPointOffset, attackPointOffset), Random.Range(-attackPointOffset, attackPointOffset));
        rb.drag = attackDrag;
        rb.AddForce((chaseDirection + randomDirection) * attackSpeed);
    }

    #endregion

    #region Death Logic
    public void Die() /// Handles the death of the enemy, disables the model, drops loot, and destroys itself.
    {
        GetComponent<DropLoot>().SpawnLoot(transform.position); // Drop the loot
        enemyModel.SetActive(false); // Disables the enemy model
    }

    #endregion
}