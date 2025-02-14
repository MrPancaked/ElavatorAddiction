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
        HealthManager.Instance.player.transform.position = new Vector3(HealthManager.Instance.player.transform.position.x, 60f, HealthManager.Instance.player.transform.position.z); // Reposition the player
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destination.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void UpdateRoomIndex()
    {
        roomText.text = (-RoomIndex).ToString();
    }

    #endregion
}