using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuCamera : MonoBehaviour
{
    public float StartHeight;
    public float StopHeight;
    public float cameraSpeed;
    public float cameraRotateSpeed;

    private void Start()
    {
        transform.position = new Vector3(0, StartHeight, 0);
    }
    void Update()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * cameraSpeed);
        transform.Rotate(Vector3.forward * Time.deltaTime * cameraRotateSpeed);
        if (transform.position.y <= StopHeight)
        { 
            transform.position = new Vector3(0, StartHeight, 0);
        }
    }
}
