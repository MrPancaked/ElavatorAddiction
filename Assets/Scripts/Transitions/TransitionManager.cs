using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TransitionManager : MonoBehaviour
{
    #region Header Variables

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
            currentSceneName = SceneManager.GetActiveScene().name;
        }
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

        LightFader[] lightFaders = FindObjectsOfType<LightFader>(); // Find and fade out all lights
        foreach (LightFader fader in lightFaders)
        {
            StartCoroutine(fader.FadeOutLight(3f));
        }
        FogManager.Instance.SetFogAndLightTransition(destination, 3f);
        yield return new WaitForSeconds(3f);

        Debug.Log("Scene load");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(destination.sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Debug.Log("Screen shake / Stop sound");

        DifficultyManager.Instance.AddRoomIndex();
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
        SceneSettings destination = FogManager.Instance.GetSceneSettings("Void");
        FogManager.Instance.SetFogAndLightTransition(destination, 1f);
       
        LightFader[] lightFaders = FindObjectsOfType<LightFader>(); // Find and fade out all lights
        foreach (LightFader fader in lightFaders)
        {
            StartCoroutine(fader.FadeOutLight(1f)); 
        }
        yield return new WaitForSeconds(1f);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Void");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForFixedUpdate();
        HealthManager.Instance.player.transform.position = new Vector3(0f, 30f, 5f);
    }


    public IEnumerator RestartTransition()
    {
        Debug.Log("Restarting");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("GrassVoid");
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        yield return new WaitForFixedUpdate();
        HealthManager.Instance.player.transform.position = new Vector3(0f, 1f, 0f);
        HealthManager.Instance.deathScreenAnimator.SetTrigger("Respawn"); // trigger Reset animation
    }

    #endregion
}