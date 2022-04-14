using System.Collections.Generic;


/**
 * Generic DFS Agent that attacks and deploys on the map based on the order the search algorithm visited nodes
 * This agent is not capable of interacting with other agents
 */
public class NonAdversarialDFS : Agents
{
    
    // A list of territories in  the order that DFS visits them, equivalent to visited
    private List<Territories> DFSOrderedTerritories = new List<Territories>();


    public NonAdversarialDFS()
    {
        agentName = "DFSAgent";
    }
    
    
    public override List<DeployMoves> generateDeployMoves()
    {
        // We check if its the first round, and then populate our list of territories to visit by using DFS
        if (DFSOrderedTerritories.Count == 0 && territories.Count == 1)
        {
            // Run DFS with our starting node (starting territory)
            DFS(territories[0]);
        }
        
        List<DeployMoves> moves = new List<DeployMoves>();
        DeployMoves move = new DeployMoves(DFSOrderedTerritories[0].territoryName, armies);
        moves.Add(move);
        return moves;
    }

    public override List<AttackMoves> generateAttackMoves()
    {
        List<AttackMoves> moves = new List<AttackMoves>();

        AttackMoves move = new AttackMoves(DFSOrderedTerritories[0].territoryName,
            DFSOrderedTerritories[1].territoryName, DFSOrderedTerritories[0].armies);
        moves.Add(move);
        
        DFSOrderedTerritories.RemoveAt(0);
        return moves;
    }


    private void DFS(Territories territory)
    { 
        Stack<Territories> stack = new Stack<Territories>();
        stack.Push(territory);
        DFSOrderedTerritories.Add(territory);

        while (stack.Count > 0)
        {
            Territories vertex = stack.Pop();

            foreach (Territories neighbor in vertex.neighbors)
            {
                if (!DFSOrderedTerritories.Contains(neighbor))
                {
                    stack.Push(neighbor);
                    DFSOrderedTerritories.Add(neighbor);
                }
            }
        }
    }
    
    
    
}