using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false; 

    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Resume(); 
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Pause(); 
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true); 
        Time.timeScale = 0f;      
        isPaused = true;          
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true; 
    }

    public void Resume()
    {
        pauseMenu.SetActive(false); 
        Time.timeScale = 1f;        
        isPaused = false;           
        Cursor.lockState = CursorLockMode.Locked; 
        Cursor.visible = false; 
    }

    public void MainMenu()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenuNew"); 
    }
}

