using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agents
{
    public string agentName;
    
    private GameState.AbstractAgentGameState abstractAgentGameState;


    protected GameState.AbstractAgentGameState.AgentGameState agentGameState;
    protected List<Territories> territories;
    protected List<Territories> frontLine;
    protected int armies;


    public void setAbstractAgentGameState(GameState.AbstractAgentGameState abstractAgentGameState)
    {
        this.abstractAgentGameState = abstractAgentGameState;
        this.agentGameState = new GameState.AbstractAgentGameState.AgentGameState(this.abstractAgentGameState);

    }
    

    public void nextRound()
    {
        territories = abstractAgentGameState.getTerritories(agentName);
        armies = abstractAgentGameState.getArmies(agentName);
        frontLine = abstractAgentGameState.getFrontLine(agentName);

    }
    
    public abstract List<DeployMoves> generateDeployMoves();
    public abstract List<AttackMoves> generateAttackMoves();

    
}
