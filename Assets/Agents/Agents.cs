using System.Collections.Generic;
using UnityEngine;

/**
 * Abstract class for representing an agent
 */
public abstract class Agents : MonoBehaviour
{
    public string agentName;
    private GameState.AbstractAgentGameState abstractAgentGameState;
    
    protected GameState.AbstractAgentGameState.AgentGameState agentGameState;
    protected List<Territories> territories;
    protected List<Territories> frontLine;
    protected List<Regions> regions;
    protected int armies;
    protected int exploredGameStates = 0;


    /**
     * Sets this agent's gameState object as well as its agentGameState object
     */
    public void setAbstractAgentGameState(GameState.AbstractAgentGameState abstractAgentGameState)
    {
        this.abstractAgentGameState = abstractAgentGameState;
        this.agentGameState = new GameState.AbstractAgentGameState.AgentGameState(this.abstractAgentGameState);

    }
    
    
    /**
     * Progresses this agent forward 1 round, updating its territories, armies, frontline, and regions
     * CALLED BY: controller every round
     */
    public void nextRound()
    {
        territories = abstractAgentGameState.getTerritories(agentName);
        armies = abstractAgentGameState.getArmies(agentName);
        frontLine = abstractAgentGameState.getFrontLine(agentName);
        regions = abstractAgentGameState.getRegions();

    }
    
    /**
     * Generates this rounds deploy moves using whatever algorithm this agent is using
     * CALLED BY: controller every round
     */
    public abstract List<DeployMoves> generateDeployMoves();
    
    /**
     * Generates this rounds attack moves using whatever algorithm this agent is using
     * CALLED BY: controller every round
     */
    public abstract List<AttackMoves> generateAttackMoves();


    /**
     * Fetches the given region given its name
     */
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
    
    /**
     * Fetches the given territory given its name
     */
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

    /**
     * Fetches some useful data about our agent for testing and analytics as a string
     */
    public string getStringAnalytics()
    {
        return "AgentName: " + agentName + ", Armies: " + armies + ", Territories: " + territories.Count + ", Explored GameStates: " + exploredGameStates;
    }


    /**
     * Fetches some useful data about our agent for testing and analytics
     */
    public (int, int, int) getAnalytics()
    {
        return (armies, territories.Count, exploredGameStates);
    }
}
