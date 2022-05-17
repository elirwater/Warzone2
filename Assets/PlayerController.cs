using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private bool onDeployButtonPressed;
    private bool onAttackButtonPressed;
    public bool onCommitButtonPressed;

    private selectedButton targetField;

    private PlayerAgent player;
    
    private string selectedTerritory;
    
    private string selectedFromTerritory;
    private string selectedToTerritory;
    //public GameObject targetedButton;
    
    
    private enum selectedButton
    {
        none,
        deployTo,
        attackFrom,
        attackTo
    }



    /**
     * CALLED BY the controller to setup the player with the playerAgent instance
     */
    public void instantiatePlayer(PlayerAgent playerAgent)
    {
        player = playerAgent;
    }
    

    private void Start()
    {
        targetField = selectedButton.none;
        onCommitButtonPressed = false;
    }


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedTerritory = FindObjectOfType<MapRendering>()
                .getTerritoryFromMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            

            if (selectedTerritory != "outOfBounds")
            {
                int maxArmies = FindObjectOfType<MapRendering>().getArmiesInTerritory(selectedTerritory);
                FindObjectOfType<ButtonManager>().addSliderUI(maxArmies);
                buttonSelectionController();
            }
        }
    }


    public void playerNextRound()
    {
        player.playerNextRound();
    }

    

    // Controller instantiates a Coroutine and waits until this method evaluates true
    public bool playerRoundOver()
    {
        if (onCommitButtonPressed)
        {
            print("done");
            onCommitButtonPressed = false;
            return true;
        }
        return false;
    }
    

    private void buttonSelectionController()
    {
        //TODO:  Pass this back to the player agent as well
        
        switch(targetField) 
        {
            case selectedButton.deployTo:
                FindObjectOfType<ButtonManager>().modifyDeployToButton(selectedTerritory);
                selectedFromTerritory = selectedTerritory;
                return;
            case selectedButton.attackFrom:
                FindObjectOfType<ButtonManager>().modifyAttackFromButton(selectedTerritory);
                selectedFromTerritory = selectedTerritory;
                return;
            case selectedButton.attackTo:
                selectedToTerritory = selectedTerritory;
                FindObjectOfType<ButtonManager>().modifyAttackToButton(selectedTerritory);
                return;
            
            default:
                targetField = selectedButton.none;
                break;
        }
    }

    
    

    public void onDeployButtonPress()
    {
        FindObjectOfType<ButtonManager>().destroyAttackUI();
        FindObjectOfType<ButtonManager>().setupDeployUI();
        onDeployButtonPressed = true;
        onAttackButtonPressed = false;
    }
    
    

    public void onAttackButtonPress()
    {
        FindObjectOfType<ButtonManager>().destroyDeployUI();
        FindObjectOfType<ButtonManager>().setupAttackUI();
        onAttackButtonPressed = true;
        onDeployButtonPressed = false;
    }

    public void onCommitButtonPress()
    {
        onCommitButtonPressed = true;
    }



    public void onDeployToButtonPress()
    {
        targetField = selectedButton.deployTo;
    }
    
    public void onAttackFromButtonPress()
    {
        targetField = selectedButton.attackFrom;
    }
    
    public void onAttackToButtonPress()
    {
        targetField = selectedButton.attackTo;
    }

    public void onNext()
    {
        print("onNext");
        int armies = FindObjectOfType<ButtonManager>().getSliderArmies();

        if (onDeployButtonPressed)
        {
            player.addDeployMove(selectedToTerritory, armies);
        }
        else
        {
            player.addAttackMove(selectedFromTerritory, selectedToTerritory, armies);
        }

    }
    
    //TODO: implement prev
    
    

}
