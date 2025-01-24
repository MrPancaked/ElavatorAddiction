//--------------------------------------------------------------------------------------------------
// Description: Handles everything when the player is dead
//--------------------------------------------------------------------------------------------------
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using static UnityEngine.Rendering.DebugUI;

public class DeathManager : MonoBehaviour
{
    #region Variables

    [Header("References")]
    public GameObject player;      // Reference to the player game object
    public GameObject playerCamera;      // Reference to the player camera object
    public Transform respawnPoint;     // Reference to the respawn point Transform
    public GameObject deathUI;    // Reference to the Death UI.
    public Health health;

    //private shit
    private bool playerIsDead = false;  // Flag to track player's death state
    private static DeathManager instance;
    public static DeathManager Instance
    {
        get { return instance; }
    }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject); // Enforce singleton pattern
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
        else
        {
            instance = this;
            playerIsDead = false; // Reset death state
            Time.timeScale = 1f; // Restore time scale
        }
    }

    #endregion

    #region Properties

    public bool PlayerIsDead
    {
        get { return playerIsDead; }
        private set { playerIsDead = value; } // Only set internally
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
        player.transform.position = respawnPoint.position;
        playerCamera.transform.rotation = respawnPoint.rotation;
        health.hp = 100f;
        Time.timeScale = 1f; // Restore time scale
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        deathUI.SetActive(false); // Hide the death UI
        playerIsDead = false; // Reset death state
        //ElevatorController.Instance.ResetDoor();
        yield return new WaitForSeconds(0.2f); // Time delay before elevator start

        DontDestroyBetweenScenes.Instance.gameObject.transform.parent = null;
        SceneManager.LoadScene("Forest"); // Reload scene
        Debug.Log("Player respawned");
    }

    #endregion
}