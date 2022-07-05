using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class responsible for the right side bar display
 */
public class RightSideBar : MonoBehaviour
{
    
    public double percentageOfScreenWidth;
    public int agentDisplayPanelHeight;
    
    
    public GameObject panelDisplayInfo;
    private GameObject backgroundPanel;

    private List<GameObject> agentDisplayPanels;
    private int backgroundPanelWidth;
    
    
    
    /**
     * Instantiates the right side bar game components
     */
    public void instantiateUI()
    {
        backgroundPanel = GameObject.Find("BackgroundPanelRight");
        RectTransform backgroundPanelRectT = backgroundPanel.GetComponent<RectTransform>();

        // Setting it so it touches the border of the texture map neatly
        backgroundPanelWidth = (int) (Screen.width / percentageOfScreenWidth);

        // Have to use these weird setters because you can't change the position/size at runtime using the fields 
        backgroundPanelRectT.SetPositionAndRotation(
            new Vector3((int) Screen.width - (backgroundPanelWidth / 2), (int) (Screen.height / 2), 0), Quaternion.identity);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
        backgroundPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height);
    }
    
    /**
     * Called by the rendering class to update the info for the various players
     */
    public void updateDisplayInfo(List<(string, int, Color)> currentPlayerInfo, int roundNum)
    {
        // First need to destroy the previous panels (might be smarter to do this only when an agent dies / the game restarts...)
        if (agentDisplayPanels != null)
        {
            for (int j = 0; j < agentDisplayPanels.Count; j++)
            {
                Destroy(agentDisplayPanels[j]);
            }
        }
        agentDisplayPanels = new List<GameObject>();  
        
        instantiateAgentUIPanels(Screen.height - agentDisplayPanelHeight);
        agentDisplayPanels[0].GetComponent<Text>().text = "Round Number: " + roundNum;
        

        int i = 0;
        foreach (var agentInfo in currentPlayerInfo)
        {
            instantiateAgentUIPanels((Screen.height / 2) - (agentDisplayPanelHeight * i));
            GameObject agentDisplay = agentDisplayPanels[i + 1];
            
            Text inputText = panelDisplayInfo.GetComponent<Text>();
            StringBuilder s = new StringBuilder();
            s.Append(agentInfo.Item1);
            s.Append("\n");
            s.Append("Armies: " + agentInfo.Item2);
            
            inputText.text = s.ToString();
            inputText.color = agentInfo.Item3;
            i += 1;
        }
    }

    
    
    /**
     * Adds panels each time it is called to display certain things about each agent
     */
    private void instantiateAgentUIPanels(int yPos)
    {
        GameObject newPanel = Instantiate(panelDisplayInfo,
            new Vector3((int) Screen.width - (backgroundPanelWidth / 2), yPos, 0), Quaternion.identity,
            backgroundPanel.transform);
        newPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, backgroundPanelWidth);
        newPanel.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, agentDisplayPanelHeight);
        
        agentDisplayPanels.Add(newPanel);
    }

}
