using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(LoadAndStartGame());
    }

    private IEnumerator LoadAndStartGame()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("Void"); // Load the scene asynchronously

        while (!asyncLoad.isDone) // Wait until the scene is loaded
        {
            yield return null;
        }
        // After the scene is loaded, call restart game
        if (HealthManager.Instance != null) // Ensure that the Health Manager actually exists
        {
            HealthManager.Instance.RestartGame();
        }

    }

    public void QuitGame()
    {
        Application.Quit();
    }
   
}
