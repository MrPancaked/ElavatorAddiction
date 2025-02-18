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

    [Header("References")]
    public GameObject player;      // Reference to the player game object
    public Health health;          // Reference to the Health component.

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
            Debug.Log("Player died");
            Time.timeScale = 0f; // Pause the game
            playerIsDead = true;
            Cursor.visible = true; // Show the cursor
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            deathScreenAnimator.SetTrigger("Die"); // Trigger animation
            PlayerSounds.Instance.PlayDeathStart();
            foreach (Gun gunInstance in TransitionManager.Instance.guns)
            {
                if (gunInstance != null) // Null check
                {
                    gunInstance.reloading = true;
                }
            }
        }
    }

    #endregion

    #region Restarting

    public void RestartGame()
    {
        playerIsDead = false; // Reset death state
        initialHealth = 100f; // Reset health
        health.hp = 100f;
        UpdateHealthUI();
        Time.timeScale = 1f; // Restore time scale
        TransitionManager.Instance.StartCoroutine(TransitionManager.Instance.RestartTransition());
    }

    public void RepositionPlayer(float height)
    {
        player.transform.position = new Vector3(player.transform.position.x, height, player.transform.position.z);
    }


    #endregion

    #region UI

    public void UpdateHealthUI()
    {
        healthImage.fillAmount = health.hp / initialHealth;
    }

    #endregion
}