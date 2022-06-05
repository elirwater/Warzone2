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
    
    
    private GameState gameStateObj;
    
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

    /**
     * CALLED BY the controller to give the player controller access to the current gameState
     */
    public void updateGameStateObj(GameState g)
    {
        gameStateObj = g;
    }
    

    private void Start()
    {
        // Disables this script unless there exits a player in the game (stops the update method and all other methods from
        // being called/triggered
        if (!FindObjectOfType<Controller>().agentData.player)
        {
            this.GetComponent<PlayerController>().enabled = false;
        }
        
        targetField = selectedButton.none;
        onCommitButtonPressed = false;
    }
    
    


    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            selectedTerritory = FindObjectOfType<MapRendering>()
                .getTerritoryFromMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // This stuff is easy to break, needs to be changed
            if (selectedTerritory != "outOfBounds")
            {

                bool playerOwnsTerritory = player.playerOwnsTerritory(selectedTerritory);
                
                int maxArmies = 0;
                
                if (targetField == selectedButton.deployTo && playerOwnsTerritory)
                {
                    maxArmies = player.getArmies();
                    FindObjectOfType<ButtonManager>().updateSliderMaxValue(maxArmies); 
                    buttonSelectionController();
                    return;
                } 
                
                
                maxArmies = FindObjectOfType<MapRendering>().getArmiesInTerritory(selectedTerritory);
                
                // We check if we are in attack mode, and if we are, we grab the number of armies we have already deployed as well
                if (targetField == selectedButton.attackFrom && playerOwnsTerritory)
                {
                    maxArmies += player.getDeployMoveArmies(selectedTerritory);
                    buttonSelectionController();
                }
                
                // We don't need to update the slider if we are attacking to a given territory
                if (targetField != selectedButton.attackTo)
                {
                    FindObjectOfType<ButtonManager>().updateSliderMaxValue(maxArmies);
                }
                
                if (targetField == selectedButton.attackTo)
                {
                    // Make sure its a neighboring territory so you can't attack to anywhere on the map
                    if (player.neighboringTerritory(selectedTerritory))
                    {
                        buttonSelectionController();    
                    }
                }
            }
        }
    }


    public void playerNextRound()
    {
        onCommitButtonPressed = false;
        FindObjectOfType<ButtonManager>().destroyDeployUI();
        FindObjectOfType<ButtonManager>().destroyAttackUI();
        player.playerNextRound();
    }

    

    // Controller instantiates a Coroutine and waits until this method evaluates true
    public bool playerRoundOver()
    {
        return onCommitButtonPressed;
    }
    

    private void buttonSelectionController()
    {
        //TODO:  Pass this back to the player agent as well
        
        switch(targetField) 
        {
            case selectedButton.deployTo:
                FindObjectOfType<ButtonManager>().modifyDeployToButton(selectedTerritory);
                selectedToTerritory = selectedTerritory;
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
    
    
    
    //TODO: need to keep track of armies already deployed to, so you can attack with all of those troops (but they haven't actually been deployed yet so just use a dict here
    
    

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
        int armies = FindObjectOfType<ButtonManager>().getSliderArmies();

        if (onDeployButtonPressed)
        {
            player.addDeployMove(selectedToTerritory, armies);
        }
        else
        {
            player.addAttackMove(selectedFromTerritory, selectedToTerritory, armies);
        }
        
        //TODO: re-do this implementation, its fairly messy
        
        //Clear all the button text fields when the next button is pressed
        selectedTerritory = "";
        buttonSelectionController();

        // Need to clear both buttons if in attack mode
        if (onAttackButtonPressed)
        {
            targetField = selectedButton.attackFrom;
            buttonSelectionController();
            targetField = selectedButton.attackTo;
            buttonSelectionController();
        }

        targetField = selectedButton.none;
        buttonSelectionController();

    }
    
    //TODO: implement prev
    
    

}
