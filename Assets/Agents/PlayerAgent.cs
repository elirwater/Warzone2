using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : Agents
{

    private List<DeployMoves> playerCreatedDeployMoves;
    private List<AttackMoves> playerCreatedAttackMoves;


    // Created to keep track of how many armies can be used to attack with from a given territory (if you 
    // are attacking multiple territories from one initial territory)
    private  IDictionary<string, int> remainingArmiesPerTerritory;
    
    
    public PlayerAgent()
    {
        agentName = "playerAgent";
    }
    
    public void playerNextRound()
    {
        playerCreatedDeployMoves = new List<DeployMoves>();
        playerCreatedAttackMoves = new List<AttackMoves>();
        remainingArmiesPerTerritory = new Dictionary<string, int>();
    }


    public bool playerOwnsTerritory(string territoryName)
    {
        return agentGameState.getTerritoryByName(territoryName).occupier == agentName;
    }


    public void addDeployMove(string territoryFrom, int armies)
    {
        //TODO: need to check that it doesn't already exist!
        playerCreatedDeployMoves.Add(new DeployMoves(territoryFrom, armies));

        remainingArmiesPerTerritory[territoryFrom] = armies;
    }
    

    public void removeDeployMove(int idx)
    {
        throw new NotImplementedException();
    }
    
    
    public void addAttackMove(string territoryFrom, string territoryTo, int armies)
    {
        //TODO: need to check that it doesn't already exist!
        playerCreatedAttackMoves.Add(new AttackMoves(territoryFrom, territoryTo, armies));
        
        // Subtract the number of armies used to attack from the total armies you can still attack with from this territory
        remainingArmiesPerTerritory[territoryFrom] -= armies;
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
        return remainingArmiesPerTerritory[inputTerritory];
    }
}
