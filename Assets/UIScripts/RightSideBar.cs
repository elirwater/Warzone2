using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class RightSideBar : MonoBehaviour
{
    
    public double percentageOfScreenWidth;
    private GameObject backgroundPanel;
    private GameObject panelDisplayInfo;

    public void instantiateUI()
    {
        backgroundPanel = GameObject.Find("BackgroundPanelRight");
        panelDisplayInfo = GameObject.Find("PanelDisplayInfoRight");
        RectTransform backgroundPanelRectT = backgroundPanel.GetComponent<RectTransform>();
        RectTransform panelDisplayInfoRectT = panelDisplayInfo.GetComponent<RectTransform>();

        // Setting it so it touches the border of the texture map neatly
        int backgroundPanelWidth = (int) (Screen.width / percentageOfScreenWidth);

        // Have to use these weird setters because you can't change the position/size at runtime using the fields 
        backgroundPanelRectT.SetPositionAndRotation(
            new Vector3((int) Screen.width - (backgroundPanelWidth / 2), (int) (Screen.height / 2), 0), Quaternion.identity);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);

        panelDisplayInfoRectT.SetPositionAndRotation(
            new Vector3((int)Screen.width - (backgroundPanelWidth / 2), (int) (Screen.height / 2), 0), Quaternion.identity);
        panelDisplayInfoRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
    }


    /**
     * Called by the rendering class to update the info for the various players
     */
    public void updateDisplayInfo(List<(string, int, Color)> currentPlayerInfo, string currentPlayer)
    {
        Text inputText = panelDisplayInfo.GetComponent<Text>();
        
        StringBuilder s = new StringBuilder();
        s.Append("Current Player: " + currentPlayer);
        s.Append("\n");

        inputText.text = s.ToString();
    }
    
}
