using System.Collections.Generic;
using System.Linq;
/**
 * Class for representing the Breadth First Search Agent
 */
public class BFSAgent : Agents
{
    private Regions targetRegion = null;
    private Territories targetRegionStartingTerritory = null;
    private List<Territories> bfsOrderedTerritories = new List<Territories>();
    private List<(Territories, Territories)> targetFromToTerritories = new List<(Territories, Territories)>();
    private int roundNumber;
    
    public BFSAgent()
    {
        agentName = "BFSAgent";
    }

 
    /**
     * Generates which from which territories other territories can be attacked from on the frontline
     */
    private void generateRoundTargetFromAndToTerritories()
    {
        targetFromToTerritories = new List<(Territories, Territories)>();
        List<Territories> alreadyTargeted = new List<Territories>();

        foreach (Territories territory in frontLine)
        {
            foreach (string neighborName in territory.neighbors)
            {
                Territories t = getTerritoryByName(neighborName);
                if (bfsOrderedTerritories.Contains(t) && !alreadyTargeted.Contains(t))
                {
                    targetFromToTerritories.Add((territory, t));
                    bfsOrderedTerritories.Remove(t);
                    alreadyTargeted.Add(t);
                }
            }
        }
    }
    
    public override List<DeployMoves> generateDeployMoves()
    {
        if (bfsOrderedTerritories.Count == 0 && targetFromToTerritories.Count == 0)
        {
            generateTargetRegion();
            BFS();
        }

        generateRoundTargetFromAndToTerritories();
        roundNumber += 1;
        
        int currentArmies = armies;
        List<DeployMoves> moves = new List<DeployMoves>();

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
            moves.Add(new AttackMoves(territoryTuple.Item1.territoryName, territoryTuple.Item2.territoryName, 4));
        }

        return moves;
    }

    /**
     * Runs BFS on the map and generates a list of visited territories that the agent will attack in order
     */
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
            
            foreach (string territoryName in targetTerritory.neighbors)
            {
                Territories t = getTerritoryByName(territoryName);
                if (t.regionName == targetRegion.regionName)
                {
                    neighbors.AddLast(t);
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

    /**
     * Generates a region for this BFS agent to target
     */
    private void generateTargetRegion()
    {
        int maxRegionalBonusVal = 0;
        Regions tempTargetRegion = null;
        Territories tempTargetRegionStartingTerritory = null;
        
        
        foreach (Territories territory in frontLine)
        {
            foreach (string frontLineNeighborName in territory.neighbors)
            {
                Territories t = agentGameState.getTerritoryByName(frontLineNeighborName);
                if (t.occupier != agentName)
                {
                    Regions extractedRegion = getRegionByName(t.regionName);
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
