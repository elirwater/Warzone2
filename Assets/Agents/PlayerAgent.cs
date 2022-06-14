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

    private int moveListPos;
    
    
    public PlayerAgent()
    {
        agentName = "playerAgent";
    }
    
    public void playerNextRound()
    {
        playerCreatedDeployMoves = new List<DeployMoves>();
        playerCreatedAttackMoves = new List<AttackMoves>();
        remainingArmiesPerTerritory = new Dictionary<string, int>();
        moveListPos = 0;
    }


    public bool playerOwnsTerritory(string territoryName)
    {
        return agentGameState.getTerritoryByName(territoryName).occupier == agentName;
    }

    public bool neighboringTerritory(string territoryName)
    {
        //Inefficient way to do this, should be changed or moved to the GameState class, also everything should
        //be a string here
        foreach (Territories territory in territories)
        {
            foreach (string neighbor in territory.neighbors)
            {
                if (agentGameState.getTerritoryByName(neighbor).territoryName == territoryName)
                {
                    return true;
                }
            }
        }

        return false;
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
        if (!playerCreatedAttackMoves.Contains(new AttackMoves(territoryFrom, territoryTo, armies)))
        {
            playerCreatedAttackMoves.Add(new AttackMoves(territoryFrom, territoryTo, armies));
            moveListPos += 1;
        }

        // Subtract the number of armies used to attack from the total armies you can still attack with from this territory
        if (remainingArmiesPerTerritory.ContainsKey(territoryFrom))
        {
            remainingArmiesPerTerritory[territoryFrom] -= armies;   
        }
    }

    


    public AttackMoves prev()
    {

        if (playerCreatedAttackMoves.Count == moveListPos)
        {
            return playerCreatedAttackMoves[0];
        }
        
        AttackMoves info = playerCreatedAttackMoves[moveListPos - 1];
        //We first wipe this move from the list of moves (so it can be modified and added again)
        playerCreatedAttackMoves.RemoveAt(moveListPos - 1);

        
        
        //TODO: NEEDS TO BE FIXED: armies not being replenished to territory
        remainingArmiesPerTerritory[playerCreatedAttackMoves[moveListPos - 1].fromTerritory] +=
            playerCreatedAttackMoves[moveListPos - 1].armies;
        
        moveListPos -= 1;
        return info;
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
        if (remainingArmiesPerTerritory.ContainsKey(inputTerritory))
        {
            return remainingArmiesPerTerritory[inputTerritory]; 
        }

        return 0;
    }
}
