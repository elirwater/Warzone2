using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agents
{

    private List<DeployMoves> playerCreatedDeployMoves;
    private List<AttackMoves> playerCreatedAttackMoves;
    


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
        //TODO: need to check that it doesn't already exist!
        playerCreatedDeployMoves.Add(new DeployMoves(territoryFrom, armies));
    }
    

    public void removeDeployMove(int idx)
    {
        throw new NotImplementedException();
    }
    
    
    public void addAttackMove(string territoryFrom, string territoryTo, int armies)
    {
        //TODO: need to check that it doesn't already exist!
        playerCreatedAttackMoves.Add(new AttackMoves(territoryFrom, territoryTo, armies));
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
    
    
    /**
     * Fetches the number of additional deployed armies to a territory (as the gameState hasn't been updated with these
     * deploy moves until commit is pressed, and you want to be able to attack with the armies you have deployed
     */
    public int getDeployMoveArmies(string inputTerritory)
    {
        foreach (DeployMoves territoryMove in playerCreatedDeployMoves)
        {
            if (territoryMove.toTerritory == inputTerritory)
            {
                return territoryMove.armies;
            }
        }

        return 0;
    }
}
