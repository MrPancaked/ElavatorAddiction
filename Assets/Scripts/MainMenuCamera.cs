using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuCamera : MonoBehaviour
{
    public float HighHeight;
    public float MidHeight;
    public float lowHeight;
    public float cameraSpeed;
    public float cameraRotateSpeed;

    private void Start()
    {
        transform.position = new Vector3(0, MidHeight, 0);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * cameraSpeed);
        transform.Rotate(Vector3.forward * Time.deltaTime * cameraRotateSpeed);
        if (transform.position.y <= lowHeight)
        { 
            transform.position = new Vector3(0, MidHeight, 0);
        }

        if (transform.position.y >= HighHeight)
        {
            transform.position = new Vector3(0, MidHeight, 0);
        }
    }
}
