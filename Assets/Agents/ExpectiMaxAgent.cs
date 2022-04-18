using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Rendering;

public class ExpectiMaxAgent : Agents
{
    
    private int globalDepth = 1;
    private int round = 0;
    private List<Agents> agentsList = new List<Agents>();
    private int _agentIdx = -1;
    
    
    (DeployMoves, AttackMoves) roundMove = (null, null);
    

    public ExpectiMaxAgent()
    {
        agentName = "ExpectiMaxAgent";
        
    }


    public override List<DeployMoves> generateDeployMoves()
    {

        //TODO I really hate this solution, super akward to not have like an on-game-start method in the agents 
        if (round == 0)
        {
            agentsList = new List<Agents>(agentGameState.getAgents());


            for (int i = 0; i < agentsList.Count; i++)
            {
                if (agentsList[i].agentName == this.agentName)
                {
                    Agents tempAgent = agentsList[0];
                    agentsList[0] = agentsList[i];
                    agentsList[i] = tempAgent;
                    break;
                }
            }

            _agentIdx = 0;
            round += 1;
            
        }
        

        (double, DeployMoves, AttackMoves) move = miniMaxRecursive(agentGameState, 0, _agentIdx, (null, null));
        
        


        roundMove.Item1 = move.Item2;
        roundMove.Item2 = move.Item3;
        

        //print(agentName + " " + roundMove.Item1.toTerritory);
        

        return new List<DeployMoves>(){roundMove.Item1};


    }

    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){roundMove.Item2};
    }




    private (double, DeployMoves, AttackMoves) miniMaxRecursive(GameState.AbstractAgentGameState.AgentGameState gameState, int depth, int agentIdx, (DeployMoves, AttackMoves) maxAction)
    {
        
        List<(DeployMoves, AttackMoves)> legalMoves = gameState.generateLegalMoves(agentsList[agentIdx].agentName);

        if (depth == globalDepth || legalMoves.Count == 0)
        {
            // ALSO DON"T KNOW IF THIS WORKS 
            int score = gameState.generateScore(agentName);
            
            // This could be VERY VERY WRONG
            // ALso -> should be checking win and loss conditions ahahaha
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
            
                (double, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, maxAction);

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
                //print(agentsList[agentIdx].agentName + " " + move.Item1.toTerritory);
                
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                
                
                (double, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, maxAction);
            
                //So this action stays null
                
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