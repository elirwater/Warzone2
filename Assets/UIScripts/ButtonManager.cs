using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    

    public GameObject deployButton;
    public GameObject attackButton;
    public GameObject commitButton;
    public GameObject emptyButton;


    private GameObject deployUIPanel;
    private GameObject deployUIToButton;

    private GameObject attackUIPanel;
    private GameObject attackUIFromButton;
    private GameObject attackUIToButton;
    
    
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
            instantiatePrimaryButtons(button, i);
            i += 1;
        }

        deployUIPanel = null;
        deployUIToButton = null;
        attackUIPanel = null;
        attackUIFromButton = null;
        attackUIToButton = null;
    }

    
    private void instantiatePrimaryButtons(GameObject button, int buttonOrder)
    {
        GameObject initDeployButton = Instantiate(button, new Vector3((int) (sideBarWidth / 2), (int) (buttonHeight / 2) + (buttonHeight * buttonOrder), 1), Quaternion.identity, parentObject.transform);
        initDeployButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth);
        
    }

    public void setupDeployUI()
    {

        GameObject deployPanel = setupNewUIPanel();
        RectTransform deployPanelRectT = deployPanel.GetComponent<RectTransform>();
        int panelTopPos = (int) (deployPanelRectT.transform.position.y + (deployPanelRectT.rect.height / 2));
        
        
        GameObject deployToButton = Instantiate(emptyButton, new Vector3((int) (sideBarWidth / 2), panelTopPos, 1), Quaternion.identity, deployPanel.transform);
        deployToButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth);
        deployToButton.GetComponentInChildren<TMP_Text>().text = "Deploy To: ";
        deployToButton.GetComponentInChildren<TMP_Text>().fontSize = 12;
        // Add the button listener through the script
        deployToButton.GetComponent<Button>().onClick.AddListener(onDeployToButtonPress);
        
        
        
        // Save these GameObjects as fields so we can modify / destroy them as needed in the PlayerController
        deployUIPanel = deployPanel;
        deployUIToButton = deployToButton;

    }

    public void modifyDeployToButton(string inputTerritoryName)
    {
        TMP_Text textComp = deployUIToButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Deploy To: " + inputTerritoryName;
    }

    public void destroyDeployUI()
    {
        if (deployUIPanel != null)
        {
            Destroy(deployUIPanel);   
        }
    }
    
    
    
    public void setupAttackUI()
    {
        GameObject attackPanel = setupNewUIPanel();
        RectTransform attackPanelRectT = attackPanel.GetComponent<RectTransform>();
        int panelTopPos = (int) (attackPanelRectT.transform.position.y + (attackPanelRectT.rect.height / 2));
        
        // TODO: needs to be abstracted!!!!!!!!!!!!!!!!!!!
        
        GameObject attackFromButton = Instantiate(emptyButton, new Vector3((int) (sideBarWidth / 2), panelTopPos, 1), Quaternion.identity, attackPanel.transform);
        attackFromButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth);
        attackFromButton.GetComponentInChildren<TMP_Text>().text = "Attack From: ";
        attackFromButton.GetComponentInChildren<TMP_Text>().fontSize = 12;
        attackFromButton.GetComponent<Button>().onClick.AddListener(onAttackFromButtonPress);

        GameObject attackToButton = Instantiate(emptyButton, new Vector3((int) (sideBarWidth / 2), panelTopPos - buttonHeight, 1), Quaternion.identity, attackPanel.transform);
        attackToButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth);
        attackToButton.GetComponentInChildren<TMP_Text>().text = "Attack To: ";
        attackToButton.GetComponentInChildren<TMP_Text>().fontSize = 12;
        attackToButton.GetComponent<Button>().onClick.AddListener(onAttackToButtonPress);
        
        // Save these GameObjects as fields so we can modify / destroy them as needed in the PlayerController
        attackUIPanel = attackPanel;
        attackUIFromButton = attackFromButton;
        attackUIToButton = attackToButton;
    }
    
    public void modifyAttackFromButton(string inputTerritoryName)
    {
        TMP_Text textComp = attackUIFromButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Attack From: " + inputTerritoryName;
    }

    public void modifyAttackToButton(string inputTerritoryName)
    {
        TMP_Text textComp = attackUIToButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Attack To: " + inputTerritoryName;
    }

    public void destroyAttackUI()
    {
        if (attackUIPanel != null)
        {
            Destroy(attackUIPanel);   
        }
    }
    


    // Sets up a UI panel in the middle of the main UI (used for deploy and attacking UI panels)
    private GameObject setupNewUIPanel()
    {
        int panelWidth = sideBarWidth;
        int panelHeight = (Screen.height / 2);
        
        // First instantiate a new UI panel, middle of UI
        GameObject inputPanel = GameObject.Find("BackgroundPanel");
        GameObject newPanel = Instantiate(inputPanel, Vector3.one, Quaternion.identity, parentObject.transform);
        RectTransform deployPanelRectT = newPanel.GetComponent<RectTransform>();
        deployPanelRectT.SetPositionAndRotation(new Vector3((panelWidth / 2), panelHeight, 0), Quaternion.identity);
        deployPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, panelWidth);
        deployPanelRectT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,  panelHeight);

        return newPanel;
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


    public void onDeployToButtonPress()
    {
        Button b = deployUIToButton.GetComponent<Button>();
        b.GetComponent<Image>().color  = Color.red;
        FindObjectOfType<PlayerController>().onDeployToButtonPress();
    }
    
    public void onAttackFromButtonPress()
    {
        Button b = attackUIFromButton.GetComponent<Button>();
        Button prevB = attackUIToButton.GetComponent<Button>();
        b.GetComponent<Image>().color  = Color.red;
        prevB.GetComponent<Image>().color  = Color.gray;
        FindObjectOfType<PlayerController>().onAttackFromButtonPress();
    }
    
    public void onAttackToButtonPress()
    {
        Button b = attackUIToButton.GetComponent<Button>();
        Button prevB = attackUIFromButton.GetComponent<Button>();
        b.GetComponent<Image>().color  = Color.red;
        prevB.GetComponent<Image>().color  = Color.gray;
        FindObjectOfType<PlayerController>().onAttackToButtonPress();
    }

}
