using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueElevator : MonoBehaviour
{
    private Coroutine cooldownCoroutine; // Thingy for the cooldown
    public float cooldownTime = 4f;

    public void OnTriggerEnter(Collider other)
    {
        if (DialogueManager.Instance.isRunning) return; // If dialogue is playing then don't do shit
        if (other.CompareTag("Player") && cooldownCoroutine == null)  // If player is inside and no cooldown 
        {
            DialogueManager.Instance.StartCoroutine(DialogueManager.Instance.ElevatorMonologue()); 
            cooldownCoroutine = StartCoroutine(Cooldown()); 
        }
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(cooldownTime);
        cooldownCoroutine = null; // Reset the coroutine after cooldown
    }
}