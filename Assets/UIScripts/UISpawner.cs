using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISpawner : MonoBehaviour
{
    
    /**
     * Instantiates an empty button with text
     */
    public GameObject instantiateButtonWithText(GameObject obj, int yPanelPos, int width, GameObject parent, string text)
    {
        GameObject newObj = instantiateGameObject(obj, yPanelPos, width, parent);
        newObj.GetComponentInChildren<TMP_Text>().text = text;
        newObj.GetComponentInChildren<TMP_Text>().fontSize = 12;
        return newObj;

    }
    
    /**
     * Instantiates a gameObject
     */
    public GameObject instantiateGameObject(GameObject obj, int yPanelPos, int width, GameObject parent)
    {
        GameObject newObj = Instantiate(obj, new Vector3(width / 2, yPanelPos, 1), Quaternion.identity, parent.transform);
        newObj.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        return newObj;
    }


    /**
     * Instantiates a new UI panel
     */
    public GameObject setupNewUIPanel(int panelWidth, int panelHeight, int panelYPos, int panelXPos, GameObject parent)
    {
        // First instantiate a new UI panel, middle of UI
        GameObject inputPanel = GameObject.Find("BackgroundPanel");

        GameObject newPanel = Instantiate(inputPanel, Vector3.one, Quaternion.identity, parent.transform);
        newPanel.GetComponent<Image>().color = Color.black;
        
        RectTransform deployPanelRectT = newPanel.GetComponent<RectTransform>();
        deployPanelRectT.SetPositionAndRotation(new Vector3(panelXPos, panelYPos, 0), Quaternion.identity);
        deployPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
        deployPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  panelHeight);

        return newPanel;
    }
    
}
