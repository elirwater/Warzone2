using System.Collections.Generic;

/**
 * Class for representing the MiniMax Agent
 */
public class MiniMaxAgent : Agents
{
    private int globalDepth = 1;
    private int round = 0;
    private List<Agents> agentsList = new List<Agents>();
    private int _agentIdx = -1;
    (DeployMoves, AttackMoves) roundMove = (null, null);

    public MiniMaxAgent()
    {
        agentName = "MiniMaxAgent";
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
        
        (int, DeployMoves, AttackMoves) move = miniMaxRecursive(agentGameState, 0, _agentIdx, null, null);
        roundMove.Item1 = move.Item2;
        roundMove.Item2 = move.Item3;
        return new List<DeployMoves>(){roundMove.Item1};
    }

    
    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){roundMove.Item2};
    }

    /**
     * Generates this rounds deploy and attack move using the mini max algorithm
     */
    private (int, DeployMoves, AttackMoves) miniMaxRecursive(GameState.AbstractAgentGameState.AgentGameState gameState, int depth, int agentIdx, DeployMoves currentD, AttackMoves currentA)
    {
        if (depth == globalDepth || gameState.checkGameOverConditions())
        {
            int score = gameState.generateScore(agentName);
            return (score, currentD, currentA);
        }

        int currentMin = int.MaxValue;
        int currentMax = int.MinValue;
        
        if (agentIdx != 0)
        {
            int childIdx;
            int newDepth = depth;
            (DeployMoves, AttackMoves) minAction = (null, null);

            if (agentsList.Count - 1 == agentIdx)
            {
                childIdx = 0;
                newDepth += 1;
            }
            else
            {
                childIdx = agentIdx + 1;
            }

            List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);
            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
                
                (int, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2);
                
                if (actionPair.Item1 < currentMin)
                {
                    currentMin = actionPair.Item1;
                    minAction = move;
                }
            }
            return (currentMin, minAction.Item1, minAction.Item2);
        }
        else
        {
            int childIdx;
            int newDepth = depth;
            (DeployMoves, AttackMoves) maxAction = (null, null);
            
            if (agentsList.Count - 1 == agentIdx)
            {
                childIdx = 0;
                newDepth += 1;
            }
            else
            {
                childIdx = agentIdx + 1;
            }
            
            List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);

            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
                
                (int, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2);
                
                if (actionPair.Item1 > currentMax)
                {
                    currentMax = actionPair.Item1;
                    maxAction = move;
                }
            }
            return (currentMax, maxAction.Item1, maxAction.Item2);
        }
    }
}