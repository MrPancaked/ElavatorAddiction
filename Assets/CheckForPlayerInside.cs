using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForPlayerInside : MonoBehaviour
{
    void OnTriggerEnter(Collider other) // Handle player entering/exiting the elevator collider
    {
        if (other.CompareTag("Player"))
        {
            ElevatorController.Instance.playerInElevator = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ElevatorController.Instance.playerInElevator = false;
        }
    }
}
