using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CursorManager : MonoBehaviour
{
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(LockCursor());
    }

    public IEnumerator LockCursor()
    {
        yield return null; // Wait one frame
        Cursor.visible = false; // Hide the cursor
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
    }
}