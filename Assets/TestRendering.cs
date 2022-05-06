using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestRendering : MonoBehaviour
{
    public int resolution = 256;
    public Texture2D texture;


    


    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {

        

    }
    
    
    
    private void Awake () {
        
        
        texture = new Texture2D(resolution, resolution, TextureFormat.RGB565, true);
        texture.name = "Procedural Texture";
        
        GetComponent<MeshRenderer>().material.mainTexture = texture;
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
        
        
        //float xTile = Screen.width / T1.width, yTile = Screen.height / T1.height;

        
        FillTexture();
    }
    
    private void FillTexture () {
        for (int y = 0; y < resolution; y++) {
            for (int x = 0; x < resolution; x++) {
                texture.SetPixel(x, y, Color.red);
            }
        }
        texture.Apply();
    }
}
