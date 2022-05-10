using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class ArmiesInTerritory : MonoBehaviour
{
    public GameObject textObject;
    private GameObject parentCanvas;

    private void Start()
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
    }

    public void displayArmyInfoInTerritory(Territories territory)
    {
        
        //TODO Shouldn't be called like this
        (int, int) texturePos = FindObjectOfType<MapRendering>()
            .mapPosToMapPos(160, 100);
        
        
        Vector3 pos = new Vector3(texturePos.Item1, texturePos.Item2);
        GameObject newText = Instantiate(textObject, pos, quaternion.identity);
        
        // THIS MESSES WITH POSITION< somehow we are getting negative positions (which is good), but not sure how
        newText.GetComponent<RectTransform>().SetParent(parentCanvas.transform, false);
        //newText.transform.parent = parentCanvas.transform;
        
        
        newText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 30);
        newText.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  30);

        newText.GetComponent<Text>().text = territory.armies.ToString();
        //RectTransform textRectTransform = backgroundPanel.GetComponent<RectTransform>();
        
        
        
        
        
    }
    
}
