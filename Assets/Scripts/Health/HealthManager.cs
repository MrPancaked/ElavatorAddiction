//--------------------------------------------------------------------------------------------------
// Description: Handles health of the player / death
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Data.Common;

public class HealthManager : MonoBehaviour
{
    #region Variables

    [Header("Player")]
    private GameObject player;      // Reference to the player game object
    private Transform respawnPoint;     // Reference to the respawn point Transform
    public Health health;          // Reference to the Health component.
    public List<Gun> guns = new List<Gun>(); // List of guns to reset after death

    [Header("UI")]
    public Animator deathScreenAnimator; // Reference to the Animator on death screen
    public Animator hudAnimator; // Reference to the Animator on death screen
    public Image healthImage;      // Reference to the Image component.

    //private shit
    public float initialHealth;   // Initial health of the object.
    private bool playerIsDead = false;  // Flag to track player's death state
    private static HealthManager instance;
    public static HealthManager Instance
    {
        get { return instance; }
    }
    public bool PlayerIsDead
    {
        get { return playerIsDead; }
        private set { playerIsDead = value; } // Only set internally
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Enforce singleton pattern
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            playerIsDead = false; // Reset death state
            initialHealth = health.hp; //set the initial health of the object
            UpdateHealthUI();
        }
    }

    private void OnEnable()
    {
        if (health != null)
        {
            health.HandleDeathMethod += OnPlayerDied;
            health.TakeDamageMethod += OnPlayerTakeDamage;
        }

    }

    private void OnDisable()
    {
        if (health != null)
        {
            health.HandleDeathMethod -= OnPlayerDied;
            health.TakeDamageMethod -= OnPlayerTakeDamage;
        }
    }

    #endregion

    #region Taking Damage / Dying Methods / Health Upgrade

    private void OnPlayerTakeDamage(float damage) // Updates the health UI display.
    {
        hudAnimator.SetTrigger("Hit"); // Trigger animation
        PlayerSounds.Instance.PlayHitSound();
        UpdateHealthUI();
        if (health.hp > 0)
        {
            ScreenshakeManager.Instance.TriggerShake("playerhit", overrideForce: 0.8f, overrideDuration: 0.08f); // Trigger screen shake but not when the player takes its final hit
        }
    }

    private void OnPlayerDied() // Handle death through the event
    {
        if (!playerIsDead)
        {
            player = gameObject; // Re-find Player
            player.transform.position = new Vector3(player.transform.position.x, 18.69f, player.transform.position.z);
            playerIsDead = true;
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Player died");
            Cursor.visible = true; // Show the cursor
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            deathScreenAnimator.SetTrigger("Die"); // Trigger animation
            PlayerSounds.Instance.PlayDeathStart();
            foreach (Gun gunInstance in guns)
            {
                gunInstance.reloading = true;
            }
        }
    }

    #endregion

    #region Restarting

    public void RestartGame()
    {
        StartCoroutine(Restarting());
    }

    public IEnumerator Restarting()
    {
        Time.timeScale = 1f; // Restore time scale
        playerIsDead = false; // Reset death state
        PlayerSounds.Instance.PlayDeathStop();
        initialHealth = 100f; // Reset health
        health.hp = 100f;
        UpdateHealthUI();
        CoinsLogic.Instance.coinDropChance = 0.5f;
        CoinsLogic.Instance.ResetCoins();
        foreach (Gun gunInstance in guns)
        {
            gunInstance.gunSettings.fireRate = 1f;
            gunInstance.extraDamage = 0f;
        }
        NewMovement.Instance.speed = 200f;
        EnemyCounter.Instance.UpdateEnemyCounter();

        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        ElevatorController.Instance.RoomIndex = 0;
        ElevatorController.Instance.UpdateRoomIndex();
        yield return null; // Wait one frame
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Void");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return null; // Wait one frame
        ElevatorController.Instance.RoomIndex = 0;
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        yield return null; // Wait one frame
        deathScreenAnimator.SetTrigger("Respawn"); // trigger Reset animation
        foreach (Gun gunInstance in guns)
        {
            gunInstance.ReloadFinished();
            yield return null; // Wait one frame 
        }
    }

    public IEnumerator VoidTransition()
    {
        Time.timeScale = 1f; // Restore time scale
        transform.position = new Vector3(transform.position.x, 25f, transform.position.z); // Move the player to the respawn point
        Debug.Log("Player Position set");
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        yield return null; // Wait one frame
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Void");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return null; // Wait one frame
    }

    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        healthImage.fillAmount = health.hp / initialHealth;
    }

    #endregion
}