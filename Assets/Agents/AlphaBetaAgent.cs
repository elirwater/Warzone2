using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class AlphaBetaAgent : Agents
{
    
    private int globalDepth = 1;
    private int round = 0;
    private List<Agents> agentsList = new List<Agents>();
    private int _agentIdx = -1;
    
    
    (DeployMoves, AttackMoves) roundMove = (null, null);
    

    public AlphaBetaAgent()
    {
        agentName = "AlphaBetaAgent";
        
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
        

        (int, DeployMoves, AttackMoves) move = miniMaxRecursive(agentGameState, 0, _agentIdx, null, null, int.MinValue, int.MaxValue);
        
        


        roundMove.Item1 = move.Item2;
        roundMove.Item2 = move.Item3;
        

        //print(agentName + " " + roundMove.Item1.toTerritory);
        

        return new List<DeployMoves>(){roundMove.Item1};


    }

    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){roundMove.Item2};
    }




    private (int, DeployMoves, AttackMoves) miniMaxRecursive(GameState.AbstractAgentGameState.AgentGameState gameState, int depth, int agentIdx, DeployMoves currentD, AttackMoves currentA, int alpha, int beta)
    {

        if (depth == globalDepth)
        {
            // ALSO DON"T KNOW IF THIS WORKS 
            int score = gameState.generateScore(agentName);
            
            // This could be VERY VERY WRONG
            // ALso -> should be checking win and loss conditions ahahaha
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
            
                (int, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2, alpha, beta);


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
                //print(agentsList[agentIdx].agentName + " " + move.Item1.toTerritory);
                
                GameState.AbstractAgentGameState.AgentGameState succGameState =
                    gameState.generateSuccessorGameState(move.Item1, move.Item2, agentsList[agentIdx].agentName);
                
                
                (int, DeployMoves, AttackMoves) actionPair = miniMaxRecursive(succGameState, newDepth, childIdx, move.Item1, move.Item2, alpha, beta);
            
                //So this action stays null

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