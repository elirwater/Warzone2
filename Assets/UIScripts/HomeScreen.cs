using System.Collections;
using System.Collections.Generic;
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

    private GameObject hsText;
    

    /**
     * Instantiates the start screen UI
     */
    public void startGame()
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
        hsText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
        hsText.GetComponent<TMP_Text>().text = "PRESS SPACE TO START";
    }

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
