using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomeScreen : MonoBehaviour
{
    private GameObject parentCanvas;
    public GameObject homeScreenText;

    private GameObject startGameText;
    
    
    private void Start()
    {
        
       parentCanvas = GameObject.Find("PrimaryCanvas");
       startGameText = Instantiate(homeScreenText, new Vector3(Screen.width / 2, Screen.height / 2, 1), Quaternion.identity, parentCanvas.transform);
    }

    public void startGame()
    {
        Destroy(startGameText);
    }

}
