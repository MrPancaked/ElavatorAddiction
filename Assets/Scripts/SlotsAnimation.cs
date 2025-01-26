using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{
    // SORRY TRIED TO FIX IT BUT NOT WORKY :(((( BWOMP
    // ISSUE IS THE NEW MATERIAL THINGY BTW

    ///public float AnimationSpeed = 0.1f;
    ///
    ///private Renderer renderer;
    ///private Material material;
    ///private int mainTexOffsetID;
    ///
    ///void Awake()
    ///{
    ///    renderer = GetComponent<Renderer>();
    ///    material = renderer.sharedMaterial;
    ///    material = new Material(material);
    ///    renderer.material = material;
    ///    mainTexOffsetID = Shader.PropertyToID("_MainTex_ST"); // Or "_MainTexOffset"
    ///}
    ///
    ///void Update()
    ///{
    ///    material.mainTextureOffset += new Vector2(0, AnimationSpeed * Time.deltaTime);
    ///}
}