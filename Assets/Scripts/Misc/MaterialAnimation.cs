using UnityEngine;

public class MaterialAnimation : MonoBehaviour
{
    public Material targetMaterial;

    public float minValue = 1f;
    public float maxValue = 1.2f;
    public float speed = 1f; // Reduced speed to compensate for no longer using PI

    private float initialPropertyValue; // Store the initial value
    private string propertyName = "Vector1_E8746023"; // Color Precision

    void Start()
    {
        initialPropertyValue = targetMaterial.GetFloat(propertyName); // Store the initial value when the game starts
    }

    void Update()
    {
        float pingPongValue = Mathf.PingPong(Time.time * speed, maxValue - minValue) + minValue; // PingPong between 0 and maxValue-minValue, then offset by minValue
        targetMaterial.SetFloat(propertyName, pingPongValue); // Set the property on the material
    }

    void OnDestroy() 
    {
        targetMaterial.SetFloat(propertyName, initialPropertyValue);
    }
}