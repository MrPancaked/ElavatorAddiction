using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotsAnimation : MonoBehaviour
{
    public float AnimationSpeed;
    // Update is called once per frame
    void Update()
    {
        GetComponent<Renderer>().material.mainTextureOffset += new Vector2(0, AnimationSpeed * Time.deltaTime);
    }
}
