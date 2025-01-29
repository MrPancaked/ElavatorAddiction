//--------------------------------------------------------------------------------------------------
// Description: Handles health of the player / death
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class HealthManager : MonoBehaviour
{
    #region Variables

    [Header("Player")]
    public GameObject player;      // Reference to the player game object
    public Transform respawnPoint;     // Reference to the respawn point Transform
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
            playerIsDead = true;
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Player died");
            Cursor.visible = true; // Show the cursor
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            deathScreenAnimator.SetTrigger("Die"); // Trigger animation
            foreach (Gun gunInstance in guns)
            {
                gunInstance.reloading = true;
            }
        }
    }

    /*public void HealthUpgrade(float healthIncrease, float healAmount) /// Applies a health upgrade to the player and logs it.
    {
        initialHealth += healthIncrease;
        health.hp += healAmount;
        Debug.Log("Health Upgrade: " + health.hp);
        UpdateHealthUI();
    }*/

    #endregion

    #region Restarting

    public void RestartGame()
    {
        if (playerIsDead)
        {
            StartCoroutine(Restarting());
        }
    }

    public IEnumerator Restarting()
    {
        Time.timeScale = 1f; // Restore time scale
        yield return null; // Wait one frame
        string currentSceneName = SceneManager.GetActiveScene().name;
        yield return null; // Wait one frame
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        yield return null; // Wait one frame
        //yield return new WaitForSeconds(0.2f); // Time delay before elevator start
        player = GameObject.FindGameObjectWithTag("Player"); // Re-find Player and wait for one frame
        yield return null; // Wait one frame 
        if (player == null)
        {
            Debug.LogError("Could not find player with Player tag after scene load, so the player probably stayed in the same position it died in and like wtf is this error i have no idea how to fix it bruh bruh");
            yield break; //stop the coroutine
        }

        deathScreenAnimator.SetTrigger("Respawn"); // trigger Reset animation
        yield return null; // Wait one frame 
        LevelGenerator.Instance.GenerateLevel(); // Call the GenerateLevel method
        EnemySpawner.Instance.SpawnEnemies();
        player.transform.position = respawnPoint.position;
        playerIsDead = false; // Reset death state
        health.hp = 100f;
        CoinsLogic.Instance.ResetCoins();
        EnemyCounter.Instance.UpdateEnemyCounter();
        UpdateHealthUI();
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        ElevatorController.Instance.OpenDoors();
        foreach (Gun gunInstance in guns)
        {
            gunInstance.ReloadFinished();
        }

    }

    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        healthImage.fillAmount = health.hp / initialHealth;
    }

    #endregion
}