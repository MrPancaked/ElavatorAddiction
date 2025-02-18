using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidTransition : MonoBehaviour
{
    private void OnTriggerEnter(Collider otherCollider) /// Called when another collider enters the trigger collider.
    {
        if (otherCollider.gameObject.CompareTag("Player"))
        {
            TransitionManager.Instance.StartCoroutine(TransitionManager.Instance.VoidTransition());
        }
    }
}
