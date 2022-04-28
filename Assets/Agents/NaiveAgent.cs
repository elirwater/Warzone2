using System.Collections.Generic;

/**
 * Class for representing the Naive Agent, this agent randomly selects a deploy and attack move tuple
 * from the list of available moves and uses it
 */
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

    /**
     * Generates random deploy,attack tuple from this agents legal moves for this round
     */
    private void generateMovePair()
    {
        System.Random r = new System.Random();
        List<(DeployMoves, AttackMoves)> legalMoves = agentGameState.generateLegalMoves(agentName);
        int idx = r.Next(legalMoves.Count - 1);
        currentRoundMove = legalMoves[idx];
    }
}
