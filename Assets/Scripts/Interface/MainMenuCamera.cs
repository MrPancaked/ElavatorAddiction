using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    public float HighHeight;
    public float MidHeight;
    public float lowHeight;
    public float cameraSpeed; // Use this to control the speed of downward movement
    public float cameraRotateSpeed;

    private void Start()
    {
        transform.position = new Vector3(0, MidHeight, 0);
    }

    void Update()
    {
        transform.Translate(Vector3.down * Time.deltaTime * cameraSpeed, Space.World); // Move downwards in world space
        
        transform.Rotate(Vector3.up, cameraRotateSpeed * Time.deltaTime, Space.World);  // Clockwise rotation
        
        if (transform.position.y <= lowHeight) // Move it back up to the mid height
        {
            Vector3 newPosition = transform.position;
            newPosition.y = MidHeight;
            transform.position = newPosition;
        }

        if (transform.position.y >= HighHeight) // Move it back down to the mid height
        {
            Vector3 newPosition = transform.position;
            newPosition.y = MidHeight;
            transform.position = newPosition;
        }
    }
}