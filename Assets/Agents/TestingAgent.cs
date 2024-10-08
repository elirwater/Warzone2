﻿using System;
using System.Collections.Generic;

/**
 * Manual but intelligent agent that is hardcoded to play as I would
 * Can attack multiple territories per round, targets regions
 */
public class TestingAgent : Agents
{
    
    public TestingAgent()
    {
        agentName = "testingAgent";
    }
    
    public override List<DeployMoves> generateDeployMoves()
    {
        List<DeployMoves> moves = new List<DeployMoves>();
        int targetTerritoryIdx = 0;
        
        while (armies > 0)
        {
            foreach (Territories territory in frontLine)
            {
                moves.Add(new DeployMoves(territory.territoryName, 1));
                armies -= 1;
            }
        }
        return moves;
    }
    
    
    public override List<AttackMoves> generateAttackMoves()
    {
        List<AttackMoves> moves = new List<AttackMoves>();
        
        System.Random random = new Random();
        foreach (Territories territory in frontLine)
        {

            int attackArmies = territory.armies;
            Territories choosenTerritory = null;

            List<Territories> neighboringEnemyTerritories = new List<Territories>();
            foreach (string neighborName in territory.neighbors)
            {
                Territories t = agentGameState.getTerritoryByName(neighborName);
                if (t.occupier != agentName)
                {
                    neighboringEnemyTerritories.Add(t);
                }
            }

            int attempts = 0;
            while (choosenTerritory == null)
            {
                if (attempts >= neighboringEnemyTerritories.Count)
                {
                    break;
                }
                // Pick a random territory that isn't your own on the front line
                int idx = random.Next(neighboringEnemyTerritories.Count - 1);
                Territories tempChoosenTerritory = neighboringEnemyTerritories[idx];

                if (tempChoosenTerritory.armies < attackArmies)
                {
                    choosenTerritory = tempChoosenTerritory;
                }

                attempts += 1;
            }

            if (choosenTerritory != null)
            {
                moves.Add(new AttackMoves(territory.territoryName, choosenTerritory.territoryName, attackArmies));    
            }
        }
        return moves;
    }
}
