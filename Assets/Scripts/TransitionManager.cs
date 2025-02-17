using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class TransitionManager : MonoBehaviour
{
    #region Header Variables

    [Header("References")]
    public TextMeshPro roomText;
    public List<Gun> guns = new List<Gun>(); // List of guns to reset after death

    // Private variables
    [HideInInspector] public int RoomIndex = 0;
    [HideInInspector] public string currentSceneName;
    private static TransitionManager instance;
    public static TransitionManager Instance { get { return instance; } }

    #endregion

    #region Unity Methods

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        UpdateRoomIndex();
        currentSceneName = SceneManager.GetActiveScene().name;
    }

    #endregion

    #region Scene Transitions

    public IEnumerator ForestTransition()
    {
        Debug.Log("Ride started / Button disabled / Start sound");
        ElevatorController.Instance.isButtonActive = false;
        SoundManager.Instance.PlayElevatorStart();
        yield return new WaitForSeconds(0.5f);

        Debug.Log("Screen shake / Fog transition");
        ScreenshakeManager.Instance.TriggerShake("elevator", overrideForce: 1.5f, overrideDuration: 0.8f);
        SceneSettings destination = FogManager.Instance.GetSceneSettings("Forest");
        FogManager.Instance.SetFogAndLightTransition(destination, 3f);
        yield return new WaitForSeconds(3f);

        Debug.Log("Scene load");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destination.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Screen shake / Stop sound");
        RoomIndex++;
        UpdateRoomIndex();
        LevelGenerator.Instance.GenerateLevel();
        EnemySpawner.Instance.SpawnEnemies();
        SoundManager.Instance.PlayElevatorStop();
        yield return new WaitForSeconds(0.3f);

        ScreenshakeManager.Instance.TriggerShake("elevator", overrideForce: 0.7f, overrideDuration: 0.8f);
        yield return new WaitForSeconds(1.5f);

        Debug.Log("Scene set / Door open / Button active / Coroutine stop");
        currentSceneName = SceneManager.GetActiveScene().name;
        EnemyCounter.Instance.InitiatlizeEnemyCount();
        ElevatorController.Instance.OpenDoors();
        ElevatorController.Instance.isButtonActive = true;
        ElevatorController.Instance.closedDoorCoroutine = null;
    }

    public IEnumerator VoidTransition()
    {
        SceneSettings destination = FogManager.Instance.GetSceneSettings("GrassVoid"); // Get scene settings
        FogManager.Instance.SetFogAndLightTransition(destination, 1f); // Start fog and light transition
        yield return new WaitForSeconds(1f);
       
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destination.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Player Position set");
        HealthManager.Instance.player.transform.position = new Vector3(HealthManager.Instance.player.transform.position.x, 60f, HealthManager.Instance.player.transform.position.z); // Reposition the player
    }

    public IEnumerator RestartTransition()
    {
        Debug.Log("Restarting");
        PlayerSounds.Instance.PlayDeathStop();
        RoomIndex = 0;
        UpdateRoomIndex();
        CoinsLogic.Instance.coinDropChance = 0.5f;
        CoinsLogic.Instance.ResetCoins();
        PlayerMovement.Instance.runSpeed = 200f;
        EnemyCounter.Instance.UpdateEnemyCounter();
        foreach (Gun gunInstance in guns)
        {
            gunInstance.gunSettings.fireRate = 1f;
            gunInstance.extraDamage = 0f;
        }

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GrassVoid");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        HealthManager.Instance.player.transform.position = new Vector3(HealthManager.Instance.player.transform.position.x, 10f, HealthManager.Instance.player.transform.position.z);
        HealthManager.Instance.deathScreenAnimator.SetTrigger("Respawn"); // trigger Reset animation
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        foreach (Gun gunInstance in guns)
        {
            if (gunInstance != null) // Null check
            {
                gunInstance.reloading = false;
            }
        }
    }


    public void UpdateRoomIndex()
    {
        roomText.text = (-RoomIndex).ToString();
    }

    #endregion
}