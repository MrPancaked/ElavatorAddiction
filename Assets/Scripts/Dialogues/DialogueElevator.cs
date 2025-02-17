using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueElevator : MonoBehaviour
{
    private Coroutine cooldownCoroutine;
    public float cooldownTime = 4f;

    [Tooltip("List of dialogue lines to be played in the elevator.")]
    public string[] elevatorDialogues;  // **ADD THIS LINE - Array of elevator-specific dialogues**

    public void OnTriggerEnter(Collider other)
    {
        if (DialogueManager.Instance.isOpen) return;
        if (other.CompareTag("Player") && cooldownCoroutine == null)
        {
            // Call a new method in DialogueManager to start the elevator monologue.
            DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ElevatorMonologue(elevatorDialogues));
            cooldownCoroutine = StartCoroutine(Cooldown());
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        cooldownCoroutine = null;
    }
}