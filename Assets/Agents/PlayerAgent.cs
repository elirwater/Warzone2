using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAgent : MonoBehaviour
{

    public List<DeployMoves> playerCreatedDeployMoves = new List<DeployMoves>();
    public List<AttackMoves> playerCreatedAttackMoves = new List<AttackMoves>();
    


    public void playerNextRound()
    {
        playerCreatedDeployMoves = new List<DeployMoves>();
        playerCreatedAttackMoves = new List<AttackMoves>();
    }


    public void deployMode()
    {
        // Call Deploy UI, add moves to deploy list, etc.
        
    }
    
    
    public void attackMode()
    {
        // Call Attack UI, add moves to attack list, etc.
        
    }

}
