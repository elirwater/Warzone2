using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HomeScreen : MonoBehaviour
{
    private GameObject parentCanvas;
    public GameObject homeScreenText;

    private GameObject hsText;
    

    public void startGame()
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
        hsText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
        hsText.GetComponent<TMP_Text>().text = "PRESS SPACE TO START";
    }

    public void destroyHS()
    {
        hsText.GetComponent<TMP_Text>().text = ""; 
    }
    

    public void endGame(string winningAgent)
    {
        parentCanvas = GameObject.Find("PrimaryCanvas");
        hsText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
        hsText.GetComponent<TMP_Text>().text = "GAME OVER: " + winningAgent + " WINS (press space to restart)";
  
    }
    

}
