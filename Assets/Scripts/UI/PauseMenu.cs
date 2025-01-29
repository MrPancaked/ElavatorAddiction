using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject gun;
    private bool isPaused = false; 

    void Update()
    {

        if (Inputs.Instance.pause.WasPressedThisFrame() && !HealthManager.Instance.PlayerIsDead) // Check for press
        {
            if (isPaused)
            {
                Resume(); 
            }
            else
            {
                Pause(); 
            }
        }
    }

    public void Pause()
    {
        pauseMenu.SetActive(true);
        gun.SetActive(false);
        Time.timeScale = 0f;      
        isPaused = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true; 
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        gun.SetActive(true);
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

