using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/**
 * Class responsible for the main screen UI (such as start screen and end screen)
 */
public class HomeScreen : MonoBehaviour
{
    private GameObject parentCanvas;
    public GameObject homeScreenText;
    public GameObject homeScreenAgentText;

    private GameObject hsText;
    private GameObject agentText;
    
    

    /**
     * Instantiates the start screen UI
     */
    public void startGame()
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
        hsText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
        hsText.GetComponent<TMP_Text>().text = "PRESS SPACE TO START\n";
        
        
        parentCanvas = GameObject.Find("PrimaryCanvas");
        agentText = Instantiate(homeScreenAgentText, new Vector3(Screen.width / 2, Screen.height / 2 - (homeScreenText.GetComponent<RectTransform>().rect.height / 2), 1), Quaternion.identity, parentCanvas.transform);
        agentText.GetComponent<TMP_Text>().fontSize = 35;

        StringBuilder activeAgents = new StringBuilder();
        activeAgents.Append("CURRENT PlAYERS: ");
        
        foreach (string activeAgent in GameConfiguration.getActiveAgents())
        {
            activeAgents.Append(activeAgent + ", ");
        }
        
        
        agentText.GetComponent<TMP_Text>().text = activeAgents.ToString();
        
        
        
       // NEEDS TO READ IN PREVIOUS CONFIGURATION OF AGENTS FOR A DEfAULT START FROM CONFIG CLASS 
    }

    
    
    
    
    
    
    
    
    // EVERYTHING BELOW HERE WILL NEED TO BE CHANGED!
    
    
    /**
     * Cleans the home screen when the game begins (doesn't remove canvas because that is used for the end screen)
     */
    public void destroyHS()
    {
        hsText.GetComponent<TMP_Text>().text = ""; 
    }
    
    /**
     * Displays the winner of the game
     */
    public void endGame(string winningAgent)
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
        hsText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
        hsText.GetComponent<TMP_Text>().text = "GAME OVER: " + winningAgent + " WINS (press space to restart)";
  
    }
    

}
