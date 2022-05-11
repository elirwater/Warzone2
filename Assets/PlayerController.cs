using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour{

    private bool onDeployButtonPressed;
    private bool onAttackButtonPressed;
    public bool onCommitButtonPressed;
    

    private void Start()
    {
        onDeployButtonPressed = false;
        onAttackButtonPressed = false;
        onCommitButtonPressed = false;
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
            onAttackButtonPressed = false;
            onDeployButtonPressed = false;
            onCommitButtonPressed = false;
            return true;
        }
        return false;
    }
    

    private void logicController()
    {
        if (onDeployButtonPressed)
        {
            FindObjectOfType<PlayerAgent>().deployMode();
        }

        if (onAttackButtonPressed)
        {
            FindObjectOfType<PlayerAgent>().deployMode();
        }
    }
    

    public void onDeployButtonPress()
    {
        onDeployButtonPressed = true;
        onAttackButtonPressed = false;
        logicController();
        onDeployButtonPressed = false;
    }

    public void onAttackButtonPress()
    {
        onAttackButtonPressed = true;
        onDeployButtonPressed = false;
        logicController();
        onAttackButtonPressed = false;
    }

    public void onCommitButtonPress()
    {
        onCommitButtonPressed = true;
        logicController();
    }
    

}
