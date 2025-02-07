using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using TMPro;

public class ElevatorController : MonoBehaviour
{

    #region Header Variables

    [Header("Refences")]

    public Animator doorAnimator; // Reference to the door animator
    public Animator leverAnimator; // Reference to the door animator

    // Private stuff
    [HideInInspector]
    public Coroutine closedDoorCoroutine; // Reference to the current coroutine
    [HideInInspector]
    public bool isButtonActive = true; // Bool to determine if button can be used
    [HideInInspector]
    public bool doorIsClosed = true; // Bool to determine if door is closed
    [HideInInspector]
    public bool playerInElevator = false; // Track if player is in elevator
    private bool leverAnimationPlaying = false; // Bool to determine if the lever is animating or na
    private static ElevatorController instance;
    public static ElevatorController Instance { get { return instance; } }
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
            DontDestroyOnLoad(this.gameObject); // Prevent the game object from being destroyed in different scenes.
        }
    }

    #endregion

    #region Button/Lever Interaction

    public void ButtonPressed()
    {
        if (!isButtonActive) // Ignore the button
        {
            return;
        }
        else
        {
            if (doorIsClosed) // Open doors
            {
                OpenDoors();
            }

            else // Close doors
            {
                closedDoorCoroutine = StartCoroutine(CloseDoors()); 
            }
        }
    }

    public IEnumerator LeverPressed()
    {
        if (!leverAnimationPlaying && CoinsLogic.Instance.playerCoins >= CoinsLogic.Instance.spinCost)
        {
            leverAnimationPlaying = true;
            leverAnimator.SetTrigger("Start");
            yield return new WaitForSeconds(0.8f); // Time delay

            StartCoroutine(CoinsLogic.Instance.UseCoinForUpgrade());
            yield return new WaitForSeconds(1.8f); // Time delay

            leverAnimationPlaying = false;
        }
    }

    #endregion

    #region Door Control

    public void OpenDoors()
    {
        doorAnimator.SetTrigger("Open"); // Trigger door open animation
        doorIsClosed = false; //Set door as open
        if (closedDoorCoroutine != null) // Stop coroutine if one is running
        {
            StopCoroutine(closedDoorCoroutine); 
        }
    }

    public IEnumerator CloseDoors()
    {
        isButtonActive = false; // Disable the button
        doorAnimator.SetTrigger("Close"); // Trigger door close animation
        yield return new WaitForSeconds(2.4f); // Time delay before elevator start
        isButtonActive = true; // Enable the button
        doorIsClosed = true; // Set door as closed

        if (doorIsClosed && playerInElevator && EnemyCounter.Instance.enemyCount == 0)// If the player is in the elevator START THE FUCKING MACHINE
        {
            TransitionManager.Instance.StartCoroutine(TransitionManager.Instance.ForestTransition());
        }
    }

    #endregion
}