using System.Collections.Generic;

/**
 * Class for representing the ExpectiMax Agent
 */
public class ExpectiMaxAgent : Agents
{
    private int globalDepth = 1;
    private int round;
    private List<Agents> agentsList = new List<Agents>();
    private int _agentIdx = -1;
    (DeployMoves, AttackMoves) roundMove = (null, null);
    
    public ExpectiMaxAgent()
    {
        agentName = "ExpectiMaxAgent";
    }
    
    
    public override List<DeployMoves> generateDeployMoves()
    {
        if (round == 0)
        {
            agentsList = new List<Agents>(agentGameState.getAgents());
            
            for (int i = 0; i < agentsList.Count; i++)
            {
                if (agentsList[i].agentName == agentName)
                {
                    (agentsList[0], agentsList[i]) = (agentsList[i], agentsList[0]);
                    break;
                }
            }
            _agentIdx = 0;
            round += 1;
        }
        (double, DeployMoves, AttackMoves) move = expectiMaxRecursive(agentGameState, 0, _agentIdx, (null, null));
 
        roundMove.Item1 = move.Item2;
        roundMove.Item2 = move.Item3;
        return new List<DeployMoves>(){roundMove.Item1};
    }

    
    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){roundMove.Item2};
    }
    
    /**
     * Generates this rounds deploy and attack move using the expectimax algorithm
     */
    private (double, DeployMoves, AttackMoves) expectiMaxRecursive(GameState.AbstractAgentGameState.AgentGameState gameState, int depth, int agentIdx, (DeployMoves, AttackMoves) maxAction)
    {
        List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);

        if (depth == globalDepth || legalMoves.Count == 0 || gameState.checkGameOverConditions())
        {
            int score = gameState.generateScore(agentName);
            return (score, maxAction.Item1, maxAction.Item2);
        }

        double prOfAction = (double)1 / legalMoves.Count;
        double expectedVal = 0;
        
        // In min agent
        if (agentIdx != 0)
        {
            int childIdx;
            int newDepth = depth;
            
            if (agentsList.Count - 1 == agentIdx)
            {
                childIdx = 0;
                newDepth += 1;
            }
            else
            {
                childIdx = agentIdx + 1;
            }
            
            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
            
                (double, DeployMoves, AttackMoves) actionPair = expectiMaxRecursive(succGameState, newDepth, childIdx, maxAction);
                expectedVal += (actionPair.Item1 * prOfAction);
            }
        }
        else
        {
            int childIdx;
            int newDepth = depth;
            
            if (agentsList.Count - 1 == agentIdx)
            {
                childIdx = 0;
                newDepth += 1;
            }
            else
            {
                childIdx = agentIdx + 1;
            }

            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
                
                (double, DeployMoves, AttackMoves) actionPair = expectiMaxRecursive(succGameState, newDepth, childIdx, maxAction);
                
                if (actionPair.Item1 > expectedVal)
                {
                    expectedVal = actionPair.Item1;
                    maxAction = move;
                }
            }
        }
        return (expectedVal, maxAction.Item1, maxAction.Item2);
    }
}