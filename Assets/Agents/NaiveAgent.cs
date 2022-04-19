using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Transactions;

public class NaiveAgent : Agents
{
    private (DeployMoves, AttackMoves) currentRoundMove;


    public NaiveAgent()
    {
        
        agentName = "NaiveAgent";
    }
    
    public override List<DeployMoves> generateDeployMoves()
    {
        generateMovePair();
        List<DeployMoves> moves = new List<DeployMoves>();
        moves.Add(currentRoundMove.Item1);
        return moves;

    }
    

    public override List<AttackMoves> generateAttackMoves()
    {
        List<AttackMoves> moves = new List<AttackMoves>();
        moves.Add(currentRoundMove.Item2);
        return moves;

    }



    private void generateMovePair()
    {
        System.Random r = new System.Random();
        
        List<(DeployMoves, AttackMoves)> legalMoves = agentGameState.generateLegalMoves(agentName);
        int idx = r.Next(legalMoves.Count - 1);

        currentRoundMove = legalMoves[idx];


    }
    
    
    
}
