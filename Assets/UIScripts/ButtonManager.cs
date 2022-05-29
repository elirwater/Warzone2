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
    public GameObject troopSlider;
    public GameObject sliderText;


    private GameObject deployUIPanel;
    private GameObject deployUIToButton;

    private GameObject attackUIPanel;
    private GameObject attackUIFromButton;
    private GameObject attackUIToButton;

    private GameObject troopSliderText;
    private GameObject troopArmySlider;
    
    
    public int buttonHeight;
    
    private int sideBarWidth;
    private GameObject parentObject;
    
    
    
    private void Start()
    {
    }


    public void setupUI()
    {
        // Disables this script unless there exits a player in the game AND the game has been started
        if (!FindObjectOfType<Controller>().agentData.player)
        {
            this.GetComponent<ButtonManager>().enabled = false;
        }
        else
        {
            // https://docs.unity3d.com/Manual/class-MonoManager.html
            // Changed the script execution order so the start method for button manager will have completed by the time this is called
        
            RectTransform sideBarTransform = GameObject.Find("BackgroundPanelLeft").GetComponent<RectTransform>();
            sideBarWidth = (int) sideBarTransform.rect.width;
            parentObject = GameObject.Find("PrimaryCanvas");

            List<GameObject> buttons = new List<GameObject>() {commitButton, attackButton, deployButton};

            int i = 0;
            foreach (GameObject button in buttons)
            {
                FindObjectOfType<UISpawner>()
                    .instantiateGameObject(button, (buttonHeight / 2) + (buttonHeight * i), sideBarWidth, parentObject);
                i += 1;
            }

            deployUIPanel = null;
            deployUIToButton = null;
            attackUIPanel = null;
            attackUIFromButton = null;
            attackUIToButton = null;   
        }
    }


    private void Update()
    {
        // Updates the textual component of the troop slider when the player is scrolling with it
        if (troopSliderText != null && troopArmySlider != null)
        {
            int armies = (int) troopArmySlider.GetComponent<Slider>().value;
            troopSliderText.GetComponent<Text>().text = armies.ToString();
        }
    }
    
    
    /**
     * Sets up the deploy user interface and its various buttons, sliders, and text components
     */
    public void setupDeployUI()
    {
        int panelWidth = sideBarWidth;
        int panelHeight = (Screen.height / 2);
        
        GameObject deployPanel = FindObjectOfType<UISpawner>()
            .setupNewUIPanel(panelWidth, panelHeight, panelHeight, panelWidth / 2, parentObject);

        int panelTopYPos = (int) (panelHeight + (0.5 * panelHeight));
        
        int deployToButtonYPos = panelTopYPos - (buttonHeight / 2);
        GameObject deployToButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, deployToButtonYPos, panelWidth, deployPanel, "Deploy To: ");
        deployToButton.GetComponent<Button>().onClick.AddListener(onDeployToButtonPress);
        
        int sliderTopYPos = (int) (deployToButtonYPos - troopSlider.GetComponent<RectTransform>().rect.height) - 5;
        troopArmySlider = FindObjectOfType<UISpawner>()
            .instantiateGameObject(troopSlider, sliderTopYPos, panelWidth, deployPanel);
        
        int sliderTexTopYPos = sliderTopYPos - (int) (sliderText.GetComponent<RectTransform>().rect.height) - 5;
        troopSliderText = FindObjectOfType<UISpawner>()
            .instantiateGameObject(sliderText, sliderTexTopYPos, panelWidth, deployPanel);
        
        int prevAndNextTopYPos = sliderTexTopYPos - (buttonHeight / 2) - 5;
        GameObject prevButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, prevAndNextTopYPos, sideBarWidth / 2, deployPanel, "prev");
        prevButton.GetComponent<Button>().onClick.AddListener(onPrevButtonPress);
        
        GameObject nextButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, prevAndNextTopYPos, sideBarWidth / 2 + sideBarWidth, deployPanel, "next");
        nextButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth / 2);
        nextButton.GetComponent<Button>().onClick.AddListener(onNextButtonPress);

        deployUIPanel = deployPanel;
        deployUIToButton = deployToButton;
    }
    
    
    /**
     * Sets up the attack user interface and its various buttons, sliders, and text components
     */
    public void setupAttackUI()
    {
        int panelWidth = sideBarWidth;
        int panelHeight = (Screen.height / 2);
        
        GameObject attackPanel = FindObjectOfType<UISpawner>()
            .setupNewUIPanel(panelWidth, panelHeight, panelHeight, panelWidth / 2, parentObject);

        int panelTopYPos = (int) (panelHeight + (0.5 * panelHeight));
        
        int attackFromButtonYPos = panelTopYPos - (buttonHeight / 2);
        GameObject attackFromButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, attackFromButtonYPos, panelWidth, attackPanel, "Attack From: ");
        attackFromButton.GetComponent<Button>().onClick.AddListener(onAttackFromButtonPress);
        
        
        int attackToButtonYPos = attackFromButtonYPos - (buttonHeight);
        GameObject attackToButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, attackToButtonYPos, panelWidth, attackPanel, "Attack To: ");
        attackToButton.GetComponent<Button>().onClick.AddListener(onAttackToButtonPress);
        
        
        int sliderTopYPos = (int) (attackToButtonYPos - troopSlider.GetComponent<RectTransform>().rect.height) - 5;
        troopArmySlider = FindObjectOfType<UISpawner>()
            .instantiateGameObject(troopSlider, sliderTopYPos, panelWidth, attackPanel);
        
        int sliderTexTopYPos = sliderTopYPos - (int) (sliderText.GetComponent<RectTransform>().rect.height) - 5;
        troopSliderText = FindObjectOfType<UISpawner>()
            .instantiateGameObject(sliderText, sliderTexTopYPos, panelWidth, attackPanel);
        
        int prevAndNextTopYPos = sliderTexTopYPos - (buttonHeight / 2) - 5;
        GameObject prevButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, prevAndNextTopYPos, sideBarWidth / 2, attackPanel, "prev");
        prevButton.GetComponent<Button>().onClick.AddListener(onPrevButtonPress);
        
        GameObject nextButton = FindObjectOfType<UISpawner>()
            .instantiateButtonWithText(emptyButton, prevAndNextTopYPos, sideBarWidth / 2 + sideBarWidth, attackPanel, "next");
        nextButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, sideBarWidth / 2);
        nextButton.GetComponent<Button>().onClick.AddListener(onNextButtonPress);

        attackUIPanel = attackPanel;
        attackUIFromButton = attackFromButton;
        attackUIToButton = attackToButton;
    }
    
    
    /**
     * Updates the max value of the slider based on the territory that was clicked on
     */
    public void updateSliderMaxValue(int numArmies)
    {
        troopArmySlider.GetComponent<Slider>().maxValue = numArmies;
    }
    
    /**
     * Modifies the deployTo button to display which territory was selected to deploy to
     */
    public void modifyDeployToButton(string inputTerritoryName)
    {
        TMP_Text textComp = deployUIToButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Deploy To: " + inputTerritoryName;
    }
    
    /**
     * Modifies the attackFrom button to display which territory was selected to attack from
     */
    public void modifyAttackFromButton(string inputTerritoryName)
    {
        TMP_Text textComp = attackUIFromButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Attack From: " + inputTerritoryName;
    }

    /**
     * Modifies the attackTo button to display which territory was selected to attack to
     */
    public void modifyAttackToButton(string inputTerritoryName)
    {
        TMP_Text textComp = attackUIToButton.GetComponentInChildren<TMP_Text>();
        textComp.text = "Attack To: " + inputTerritoryName;
    }
    
    /**
     * Destroys deploy UI panel and every child component
     */
    public void destroyDeployUI()
    {
        if (deployUIPanel != null)
        {
            Destroy(deployUIPanel);   
        }
    }

    /**
     * Destroys attack UI panel and every child component
     */
    public void destroyAttackUI()
    {
        if (attackUIPanel != null)
        {
            Destroy(attackUIPanel);   
        }
    }
    
    

    /**
     * Called by the player controller to figure out how many armies were selected to deploy/attack with
     */
    public int getSliderArmies()
    {
        if (troopSlider != null)
        {
            return (int) troopArmySlider.GetComponent<Slider>().value;
        }
        throw new System.Exception("No troop amount selected through slider");
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

    
    public void onPrevButtonPress()
    {
        
    }
    
    public void onNextButtonPress()
    {
        FindObjectOfType<PlayerController>().onNext();

        //Also wipe the colors of each button
        if (deployUIPanel != null)
        {
            deployUIToButton.GetComponent<Button>().GetComponent<Image>().color = Color.gray;   
        }
        else
        {
            attackUIToButton.GetComponent<Button>().GetComponent<Image>().color = Color.gray;
            attackUIFromButton.GetComponent<Button>().GetComponent<Image>().color = Color.gray;   
        }
    }

}
