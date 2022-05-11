using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    

    public GameObject deployButton;
    public GameObject attackButton;
    public GameObject commitButton;
    public int buttonHeight;
    
    private int sideBarWidth;
    private GameObject parentObject;
    
    
    // Start is called before the first frame update
    private void Start()
    {
        // https://docs.unity3d.com/Manual/class-MonoManager.html
        // Changed the script execution order so the start method for button manager will have completed by the time this is called
        
        RectTransform sideBarTransform = GameObject.Find("BackgroundPanel").GetComponent<RectTransform>();
        sideBarWidth = (int) sideBarTransform.rect.width;
        parentObject = GameObject.Find("PrimaryCanvas");

        List<GameObject> buttons = new List<GameObject>() {commitButton, attackButton, deployButton};

        int i = 0;
        foreach (GameObject button in buttons)
        {
            instantiateButtons(button, i);
            i += 1;
        }
    }

    
    private void instantiateButtons(GameObject button, int buttonOrder)
    {
        GameObject initDeployButton = Instantiate(button, new Vector3((int) (sideBarWidth / 2), (int) (buttonHeight / 2) + (buttonHeight * buttonOrder), 1), Quaternion.identity, parentObject.transform);
        initDeployButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth);
        
    }



    public void onDeployButtonPress()
    {
        FindObjectOfType<PlayerController>().onDeployButtonPress();
    }
        
    public void onAttackButtonPress()
    {
        FindObjectOfType<PlayerController>().onAttackButtonPress();
    }
    
    public void onCommitButtonPress()
    {
        FindObjectOfType<PlayerController>().onCommitButtonPress();
    }

}
