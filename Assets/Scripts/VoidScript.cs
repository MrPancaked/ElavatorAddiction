using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider otherCollider) /// Called when another collider enters the trigger collider.
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            StartCoroutine(TransitionManager.Instance.VoidTransition());
        }
    }
}
