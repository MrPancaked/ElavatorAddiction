using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    #region Variables

    public List<GameObject> objectsToManage = new List<GameObject>(); // List of objects to control
    private static SceneLoadingManager instance;
    public static SceneLoadingManager Instance { get { return instance; } }

    //private
    private int mainMenuSceneIndex = 0; //index of the main menu scene
    private int voidSceneIndex = 1; //index of the void scene

    #endregion

    #region Unity Methods

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // IMPORTANT!
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    #endregion

    #region Scene Logic

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleObjectActivation(scene.buildIndex); //check if the active scene is changed, and update object states
    }

    private void HandleObjectActivation(int sceneIndex)
    {
        if (sceneIndex == mainMenuSceneIndex)
        {
            SetObjectsActive(false); //Deactivate objects in the MainMenu
        }
        else if (sceneIndex == voidSceneIndex)
        {
            SetObjectsActive(true); //Activate objects in the Void scene
        }
        else
        {
            Debug.LogWarning("No specific state set for scene with index " + sceneIndex);
        }
    }

    private void SetObjectsActive(bool state)
    {
        foreach (GameObject obj in objectsToManage)
        {
            if (obj != null) // Check if object is not null before activating/deactivating it.
            {
                obj.SetActive(state);
            }
        }
    }

    public void GoToMainMenu()
    {
        StartCoroutine(LoadSceneAsync("MainMenu"));
    }

    public void GoToVoidScene()
    {
        StartCoroutine(LoadSceneAsync("Void"));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    #endregion
}