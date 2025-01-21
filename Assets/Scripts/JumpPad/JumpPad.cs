using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    private Vector3 JumpPadDirection;
    public void Start()
    {
        JumpPadDirection = transform.up;
    }
    public float JumpPadForce;
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("JumpPad");
        if (other.GetComponent<Rigidbody>() != null)
        {
            other.GetComponent<Rigidbody>().AddForce(JumpPadDirection * JumpPadForce, ForceMode.Impulse);
        } 
    }
}
