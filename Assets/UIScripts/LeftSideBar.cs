using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LeftSideBar : MonoBehaviour
{

    public double percentageOfScreenWidth;
    private GameObject backgroundPanel;
    private GameObject panelDisplayInfo;
    void Start()
    {
        //TODO THIS SHOULD BE ATTACHED VIA SCRIPT, not found by name
        
        backgroundPanel = GameObject.Find("BackgroundPanel");
        panelDisplayInfo = GameObject.Find("PanelDisplayInfo");
        RectTransform backgroundPanelRectT = backgroundPanel.GetComponent<RectTransform>();
        RectTransform panelDisplayInfoRectT = panelDisplayInfo.GetComponent<RectTransform>();

        // Setting it so it touches the border of the texture map neatly
        int backgroundPanelWidth = (int) (Screen.width / percentageOfScreenWidth);
        
        // Have to use these weird setters because you can't change the position/size at runtime using the fields 
        backgroundPanelRectT.SetPositionAndRotation(new Vector3((int) (backgroundPanelWidth / 2), (int) (Screen.height / 2), 0), Quaternion.identity);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  Screen.height);
        
        panelDisplayInfoRectT.SetPositionAndRotation(new Vector3((int) (backgroundPanelWidth / 2), (int) (Screen.height / 2), 0), Quaternion.identity);
        panelDisplayInfoRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
    }

    // Update is called once per frame
    void Update()
    {

    }



    public void displayTerritoryInfo(string name, string occupier, string region, int RegionalBonusValue, int numNeighbors, int numArmies)
    {
        Text inputText = panelDisplayInfo.GetComponent<Text>();
        
        StringBuilder s = new StringBuilder();
        s.Append("Territory Name: " + name);
        s.Append("\n");
        s.Append("Occupier Name: " + occupier);
        s.Append("\n");
        s.Append("Region Name: " + region);
        s.Append("\n");
        s.Append("RegionalBonusValue: " + RegionalBonusValue);
        s.Append("\n");
        s.Append("Number of Neighbors: " + numNeighbors);
        s.Append("\n");
        s.Append("Number of Armies: " + numArmies);
        s.Append("\n");
        
        inputText.text = s.ToString();
    }
}
