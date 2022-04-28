using System.Collections.Generic;
using Unity.Mathematics;

/**
 * Class for representing the Alpha Beta Pruning Agent
 */
public class AlphaBetaAgent : Agents
{
    
    private int globalDepth = 1;
    private int round;
    private List<Agents> agentsList = new List<Agents>();
    private int _agentIdx = -1;
    
    
    (DeployMoves, AttackMoves) roundMove = (null, null);
    

    public AlphaBetaAgent()
    {
        agentName = "AlphaBetaAgent";
    }


    public override List<DeployMoves> generateDeployMoves()
    {
        // Sets up agent info
        if (round == 0)
        {
            agentsList = new List<Agents>(agentGameState.getAgents());
            
            for (int i = 0; i < agentsList.Count; i++)
            {
                if (agentsList[i].agentName == this.agentName)
                {
                    (agentsList[0], agentsList[i]) = (agentsList[i], agentsList[0]);
                    break;
                }
            }
            _agentIdx = 0;
            round += 1;
        }
        
        (int, DeployMoves, AttackMoves) move = alphaBetaRecursive(agentGameState, 0, _agentIdx, null, null, int.MinValue, int.MaxValue);
        
        roundMove.Item1 = move.Item2;
        roundMove.Item2 = move.Item3;
        
        return new List<DeployMoves>(){roundMove.Item1};
    }

    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){roundMove.Item2};
    }
    
    /**
     * Generates this rounds deploy and attack move using alpha beta pruning
     */
    private (int, DeployMoves, AttackMoves) alphaBetaRecursive(GameState.AbstractAgentGameState.AgentGameState gameState, int depth, int agentIdx, DeployMoves currentD, AttackMoves currentA, int alpha, int beta)
    {
        if (depth == globalDepth || gameState.checkGameOverConditions())
        {
            int score = gameState.generateScore(agentName);
            return (score, currentD, currentA);
        }

        // In min agent
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
            
            int v = int.MaxValue;

            List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);
            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
            
                (int, DeployMoves, AttackMoves) actionPair = alphaBetaRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2, alpha, beta);
                
                v = math.min(v, actionPair.Item1);

                if (v == actionPair.Item1)
                {
                    minAction = move;
                }

                if (v < alpha)
                {
                    return (v, move.Item1, move.Item2);
                }

                beta = math.min(beta, v);

            }
            return (v, minAction.Item1, minAction.Item2);
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
            
            int v = int.MinValue;
            
            List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);

            foreach ((DeployMoves, AttackMoves) move in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                exploredGameStates += 1;
                
                (int, DeployMoves, AttackMoves) actionPair = alphaBetaRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2, alpha, beta);
                
                v = math.max(v, actionPair.Item1);

                if (v == actionPair.Item1)
                {
                    maxAction = move;
                }

                if (v > beta)
                {
                    return (v, move.Item1, move.Item2);
                }

                alpha = math.max(alpha, v);
            }

            return (v, maxAction.Item1, maxAction.Item2);
        }
    }
}