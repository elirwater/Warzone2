using System.Collections;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.UI;

public class InfoSpawning : MonoBehaviour
{
    // Start is called before the first frame update
    Text _guitext;
    void Start()
    {

        CreateText(100, 100, "LOL", 100, Color.white);

    }
    
    
    GameObject CreateText(float x, float y, string text_to_print, int font_size, Color text_color)
    {
        GameObject UItextGO = new GameObject("Text2");
        //UItextGO.transform.SetParent(canvas_transform);

        RectTransform trans = UItextGO.AddComponent<RectTransform>();
        trans.anchoredPosition = new Vector2(x, y);

        Text text = UItextGO.AddComponent<Text>();
        text.text = text_to_print;
        text.fontSize = font_size;
        text.color = text_color;

        return UItextGO;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
