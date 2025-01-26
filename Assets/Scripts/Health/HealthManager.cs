//--------------------------------------------------------------------------------------------------
// Description: Handles health of the player / death
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;
using System.Collections.Generic;

public class HealthManager : MonoBehaviour
{
    #region Variables

    [Header("Player")]
    public GameObject player;      // Reference to the player game object
    public Transform respawnPoint;     // Reference to the respawn point Transform
    public Health health;          // Reference to the Health component.
    public List<Gun> guns = new List<Gun>(); // List of guns to reset after death

    [Header("UI")]
    public GameObject deathUI;    // Reference to the Death UI.
    public Image healthImage;      // Reference to the Image component.

    //private shit
    private Camera playerCamera;      // Reference to the player camera object
    private float initialHealth;   // Initial health of the object.
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
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
            playerIsDead = false; // Reset death state
            initialHealth = health.hp; // set the initial health of the object
            playerCamera = Camera.main; // Find the main camera
        }
    }

    void Update() 
    {
        UpdateHealthUI();
    }

    #endregion

    #region Death Handling Methods

    public void PlayerDied()
    {
        if (!playerIsDead)
        {
            playerIsDead = true;
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Player died");
            deathUI.SetActive(true); // Show the death UI
            Cursor.visible = true; // Show the cursor
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor

            foreach (Gun gunInstance in guns)
            {
                gunInstance.reloading = true;
            }
        }
    }

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
        DontDestroyBetweenScenes.Instance.gameObject.transform.parent = null;
        SceneManager.LoadScene("Forest"); // Reload scene
        Debug.Log("Scene reloaded + time started");

        yield return new WaitForSeconds(0.2f); // Time delay before elevator start

        Debug.Log("Scene reloaded");

        player.transform.position = respawnPoint.position;
        playerCamera.transform.rotation = respawnPoint.rotation;
        playerIsDead = false; // Reset death state
        health.hp = 100f;
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        deathUI.SetActive(false); // Hide the death UI
        ElevatorController.Instance.OpenDoors();

        foreach (Gun gunInstance in guns)
        {
            gunInstance.ReloadFinished();
        }
    }

    #endregion

    #region UI

    private void UpdateHealthUI() // Updates the health UI display.
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = health.hp / initialHealth; // Update the fill amount
        }

        if (health.hp <= 0 && !playerIsDead)
        {
            PlayerDied();
        }
    }

    #endregion
}