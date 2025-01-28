using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    // SORRY TRIED TO FIX IT BUT NOT WORKY :(((( BWOMP
    // ISSUE IS THE NEW MATERIAL THINGY BTW

    public float AnimationSpeed = 0f;
    
    private Material rendererMaterial;
    
    void Awake()
    {
        rendererMaterial = GetComponent<Renderer>().material;
    }
    
    void Update()
    {
        rendererMaterial.mainTextureOffset += new Vector2(0, AnimationSpeed * Time.deltaTime);
    }
}