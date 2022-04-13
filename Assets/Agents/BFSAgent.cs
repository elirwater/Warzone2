using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Transactions;

public class UCSAgent : Agents
{
    private Regions targetRegion = null;
    private Territories targetRegionStartingTerritory = null;

    private List<Territories> bfsOrderedTerritories = new List<Territories>();
    private List<Territories> currentRoundTargetTerritories = new List<Territories>();


    private List<(Territories, Territories)> targetFromToTerritories = new List<(Territories, Territories)>();

    private int roundNumber = 0;
    


    public UCSAgent()
    {
        agentName = "UCSAgent";
    }

    //TODO: need to enforce in gamestate that you can't add more armies then u have!!!!!!!

    private void generateRoundTargetFromAndToTerritories()
    {
        targetFromToTerritories = new List<(Territories, Territories)>();
        List<Territories> alreadyTargeted = new List<Territories>();
        
        // Hopefully frontline is working here
        foreach (Territories territory in frontLine)
        {
            foreach (Territories neighbor in territory.neighbors)
            {
                // Not sure how valid these contain calls are...
                if (bfsOrderedTerritories.Contains(neighbor) && !alreadyTargeted.Contains(neighbor))
                {
                    targetFromToTerritories.Add((territory, neighbor));
                    bfsOrderedTerritories.Remove(neighbor);
                    alreadyTargeted.Add(neighbor);
                }
            }
        }
    }
    
    public override List<DeployMoves> generateDeployMoves()
    {
        print("bfs territories count: " + bfsOrderedTerritories.Count);
        print("targetTerritoriesCount " + targetFromToTerritories.Count);
        
        if (bfsOrderedTerritories.Count == 0 && targetFromToTerritories.Count == 0)
        {
            generateTargetRegion();
            print(targetRegion.regionName);
            BFS();
        }
        

        generateRoundTargetFromAndToTerritories();


        // foreach (var t in targetFromToTerritories)
        // {
        //     print(roundNumber + " from " + t.Item1.territoryName);
        //     print(roundNumber + " to " + t.Item2.territoryName);
        // }
        //
        // //TODO: number of armies being deployed is wrong
        //
        

        roundNumber += 1;
        
        int currentArmies = armies;
        List<DeployMoves> moves = new List<DeployMoves>();

        
        
        //TODO  You are literally not deploying any troops??????????

        while (currentArmies > 0)
        {
            if (targetFromToTerritories.Count == 0)
            {
                break;
            }
            foreach ((Territories, Territories) territoryTuple in targetFromToTerritories)
            {
                territoryTuple.Item1.armies += 1;
                currentArmies -= 1;

            }   
        }

        return moves;
    }
    

    public override List<AttackMoves> generateAttackMoves()
    {
        List<AttackMoves> moves = new List<AttackMoves>();
        
        foreach ((Territories, Territories) territoryTuple in targetFromToTerritories)
        {
            
            // ATTACK IS SENDING ALL THE TROOPS, should be sending just a fraction of them
            // And once regions are conquered, it still isn't working 
            moves.Add(new AttackMoves(territoryTuple.Item1.territoryName, territoryTuple.Item2.territoryName, 4));

        }

        return moves;

    }
    
    
    // Returns a list of territories ordered by the order in which they are to be taken
    // Then up to another function to parse through the legal moves for this round
    private void BFS()
    {
        
        List<Territories> visited = new List<Territories>(){targetRegionStartingTerritory};

        LinkedList<Territories> queue = new LinkedList<Territories>();
        queue.AddLast(targetRegionStartingTerritory);

        while (queue.Any())
        {
            Territories targetTerritory = queue.First();
            queue.RemoveFirst();

            LinkedList<Territories> neighbors = new LinkedList<Territories>();
            
            // Need to make sure we only add neighbors that are in this region
            foreach (Territories territory in targetTerritory.neighbors)
            {
                if (territory.regionName == targetRegion.regionName)
                {
                    neighbors.AddLast(territory);
                }
            }

            foreach (Territories territory in neighbors)
            {
                if (!visited.Contains(territory))
                {
                    queue.AddLast(territory);
                    visited.Add(territory);
                }
            }
            

        }

        visited.RemoveAt(0);
        bfsOrderedTerritories = visited;

    }



    // TODO: this guy won't work with other agents because its not checking it still holds all territories in that region after a round has passed
    // So, should run BFS on all partially occupied regions and the target region?
    
    
    
    // TODO: I BELIEVE EVERYTHING IS OFF BY A ROUND!
    private void generateTargetRegion()
    {
        
        
        int maxRegionalBonusVal = 0;
        Regions tempTargetRegion = null;
        Territories tempTargetRegionStartingTerritory = null;
        
        
        foreach (Territories territory in frontLine)
        {
            foreach (Territories frontLineNeighbor in territory.neighbors)
            {
                if (frontLineNeighbor.occupier != agentName)
                {
                    Regions extractedRegion = getRegionByName(frontLineNeighbor.regionName);
                    if (extractedRegion.regionalBonusValue > maxRegionalBonusVal)
                    {
                        maxRegionalBonusVal = extractedRegion.regionalBonusValue;
                        tempTargetRegion = extractedRegion;
                        tempTargetRegionStartingTerritory = territory;

                    }
                }   
            }
        }
        
        

        if (tempTargetRegion == null || tempTargetRegionStartingTerritory == null)
        {
            throw new System.Exception("Failed to create a new target Region");
        }

        targetRegion = tempTargetRegion;
        targetRegionStartingTerritory = tempTargetRegionStartingTerritory;
    }

    
    
    
    
}
