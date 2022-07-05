using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

/**
 * Class responsible for all interactions between the physical player and its representation class playerAgent
 */
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
    
    private int numMoves;

    /**
     *  Which button is currently selected represented in an easier to use fashion
     */
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
        numMoves = 0;
    }
    
    
    /**
     * Enables the player controller and sets up the necessary parameters
     */
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
    
    

    /**
     * Handles the logic of the player clicking
     */
    private void Update()
    {
        // If the player has clicked
        if (Input.GetMouseButtonDown(0))
        {
            // We find the territory the player has clicked on (using the map rendering class)
            selectedTerritory = FindObjectOfType<MapRendering>()
                .getTerritoryFromMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            // If the selected territory isn't out of bounds, proceed
            if (selectedTerritory != "outOfBounds")
            {
                
                // We establish whether the player owns this territory
                // (used to figure out whether the displays should allow deployment to this territory)
                bool playerOwnsTerritory = player.playerOwnsTerritory(selectedTerritory);
                
                int maxArmies = 0;
                
                // If the selected button is deployTO
                if (targetField == selectedButton.deployTo && playerOwnsTerritory)
                {
                    // We find the number of armies the player can deploy in total, then we update the max value of
                    // of the slider, and then we call the button logic controller
                    maxArmies = player.getArmies();
                    FindObjectOfType<ButtonManager>().updateSliderMaxValue(maxArmies); 
                    buttonSelectionController();
                    return;
                } 
                
                // Now we set the max armies to be the number of armies in the selected territory, because both of the
                // following clauses are for the attacking UI
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

    /**
     * Advances the player to the next round, called by the controller
     */
    public void playerNextRound()
    {
        onCommitButtonPressed = false;
        FindObjectOfType<ButtonManager>().destroyDeployUI();
        FindObjectOfType<ButtonManager>().destroyAttackUI();
        player.playerNextRound();
    }

    /**
     * Primary controller instantiates a Coroutine that waits until this method evaluates to be true,
     * which is only the case if the player has clicked the commitButton, and then the rest of the controller logic
     * proceeds
     */
    public bool playerRoundOver()
    {
        return onCommitButtonPressed;
    }
    
    /**
     * Handles the basic logic of the primary buttons
     */
    private void buttonSelectionController()
    {
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

    /**
     * Sends information to the playerAgent when the next button is pressed, such as the move the player selected
     */
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


    public void onPrev()
    {
        AttackMoves a = player.prev();
        targetField = selectedButton.attackFrom;
        selectedTerritory = a.fromTerritory;
        buttonSelectionController();
        
        targetField = selectedButton.attackTo;
        selectedTerritory = a.toTerritory;
        buttonSelectionController();
        
        FindObjectOfType<ButtonManager>().setSliderValue(a.armies);
    }
}
