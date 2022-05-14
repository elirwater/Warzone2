using System;
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


    public void addDeployMove()
    {

        throw new NotImplementedException();
    }

    public void removeDeployMove(int idx)
    {
        throw new NotImplementedException();
    }
    
    
    public void addAttackMove()
    {
        throw new NotImplementedException();
    }

    public void removeAttackMove(int idx)
    {
        throw new NotImplementedException();
    }

}
