using UnityEngine;

public class FloatingCube : MonoBehaviour
{
    public Transform cube; // The cube you want to float
    public float speed = 1f; // The speed of the floating motion
    public float minHeight = 0f; // The minimum height the cube will reach
    public float maxHeight = 2f; // The maximum height the cube will reach

    private float timeOffset; // Used to offset the sine wave for smoother movement

    void Start()
    {
        timeOffset = Random.Range(0f, 10f);
    }

    void FixedUpdate() // Changed to Update
    {
        float heightOffset = Mathf.Sin(Time.time * speed + timeOffset); // Calculate the vertical offset using a sine wave

        float normalizedHeight = (heightOffset + 1f) / 2f;  // Remap the sine wave output (-1 to 1) to the range 0 to 1.  This is the key fix.

        float newY = Mathf.Lerp(minHeight, maxHeight, normalizedHeight); //Calculate the height based on the remaped sine wave output.
       
        Vector3 newPosition = new Vector3(cube.position.x, newY, cube.position.z); // Update the cube's position 

        cube.position = newPosition;
    }

}