using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agents
{

    public List<DeployMoves> playerCreatedDeployMoves = new List<DeployMoves>();
    public List<AttackMoves> playerCreatedAttackMoves = new List<AttackMoves>();
    


    public PlayerAgent()
    {
        agentName = "playerAgent";
    }
    
    public void playerNextRound()
    {
        playerCreatedDeployMoves = new List<DeployMoves>();
        playerCreatedAttackMoves = new List<AttackMoves>();
    }


    public void addDeployMove(string territoryFrom, int armies)
    {
        print("deployMove added");
        playerCreatedDeployMoves.Add(new DeployMoves(territoryFrom, armies));
    }

    public void removeDeployMove(int idx)
    {
        throw new NotImplementedException();
    }
    
    
    public void addAttackMove(string territoryFrom, string territoryTo, int armies)
    {
        playerCreatedAttackMoves.Add(new AttackMoves(territoryFrom, territoryTo ,armies));
    }

    public void removeAttackMove(int idx)
    {
        throw new NotImplementedException();
    }

    public override List<DeployMoves> generateDeployMoves()
    {
        return playerCreatedDeployMoves;
    }

    public override List<AttackMoves> generateAttackMoves()
    {
        return playerCreatedAttackMoves;
    }
}
