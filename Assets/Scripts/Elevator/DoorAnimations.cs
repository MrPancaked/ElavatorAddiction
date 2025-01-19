using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimations : MonoBehaviour
{
    public Animator animator;

    public bool doorIsClosed = true;

    void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (doorIsClosed)
            {
                animator.SetTrigger("Open");
                doorIsClosed = false;
            }
            else
            {
                animator.SetTrigger("Close");
                doorIsClosed = true;
            }
        }
    }
}
