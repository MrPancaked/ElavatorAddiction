//--------------------------------------------------------------------------------------------------
// Description: Rotates the thingy lol
//--------------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObject : MonoBehaviour
{
    [Header("References")]
    public Transform gameObject;

    [Header("Vartiables")]
    public float rotationSpeed;

    void Update()
    {
        gameObject.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
    }
}