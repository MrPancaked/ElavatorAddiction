using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueElevator : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        if (DialogueManager.Instance.isRunning) return;
        else DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ElevatorMonologue());
    }
}
