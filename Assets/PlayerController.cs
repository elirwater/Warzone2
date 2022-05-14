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

    //public GameObject targetedButton;
    
    
    private enum selectedButton
    {
        deployTo,
        attackFrom,
        attackTo
    }
    

    private void Start()
    {
        onCommitButtonPressed = false;
    }


    private void Update()
    {
        // TODO: should only be called when button is selected lol
        if (Input.GetMouseButtonDown(0))
        {
            string territoryName = FindObjectOfType<MapRendering>()
                .getTerritoryFromMousePos(Camera.main.ScreenToWorldPoint(Input.mousePosition));

            logicController(territoryName);
        }
    }


    public void playerNextRound()
    {
        FindObjectOfType<PlayerAgent>().playerNextRound();
    }

    public List<DeployMoves> getPlayerDeployMoves()
    {
        return FindObjectOfType<PlayerAgent>().playerCreatedDeployMoves;
    }

    public List<AttackMoves> getPlayerAttackMoves()
    {
        return FindObjectOfType<PlayerAgent>().playerCreatedAttackMoves;
    }
    

    // Controller instantiates a Coroutine and waits until this method evaluates true
    public bool playerRoundOver()
    {
        if (onCommitButtonPressed)
        {
            onCommitButtonPressed = false;
            return true;
        }
        return false;
    }
    

    private void logicController(string territoryName)
    {
        //TODO:  Pass this back to the player agent as well
        
        switch(targetField) 
        {
            case selectedButton.deployTo:
                FindObjectOfType<ButtonManager>().modifyDeployToButton(territoryName);
                break;
            case selectedButton.attackFrom:
                FindObjectOfType<ButtonManager>().modifyAttackFromButton(territoryName);
                break;
            case selectedButton.attackTo:
                FindObjectOfType<ButtonManager>().modifyAttackToButton(territoryName);
                break;
            
            default:
                break;
        }

    }
    

    public void onDeployButtonPress()
    {
        FindObjectOfType<ButtonManager>().destroyAttackUI();
        FindObjectOfType<ButtonManager>().setupDeployUI();
        onDeployButtonPressed = false;
    }
    
    

    public void onAttackButtonPress()
    {
        FindObjectOfType<ButtonManager>().destroyDeployUI();
        FindObjectOfType<ButtonManager>().setupAttackUI();
        onAttackButtonPressed = false;
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
    
    

}
