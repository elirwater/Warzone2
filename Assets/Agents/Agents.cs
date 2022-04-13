using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Agents : MonoBehaviour
{
    public string agentName;
    
    private GameState.AbstractAgentGameState abstractAgentGameState;


    protected GameState.AbstractAgentGameState.AgentGameState agentGameState;
    protected List<Territories> territories;
    protected List<Territories> frontLine;
    protected List<Regions> regions;
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
        regions = abstractAgentGameState.getRegions();

    }
    
    
    public abstract List<DeployMoves> generateDeployMoves();
    public abstract List<AttackMoves> generateAttackMoves();


    protected Regions getRegionByName(string name)
    {
        foreach (Regions region in regions)
        {
            if (region.regionName == name)
            {
                return region;
            }
        }
        throw new System.Exception("Failed to find target region");
    }
    
    protected Territories getTerritoryByName(string name)
    {
        foreach (Territories territory in territories)
        {
            if (territory.territoryName == name)
            {
                return territory;
            }
        }
        throw new System.Exception("Failed to find target territory");
    }

    
}
