using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class GameState : MonoBehaviour
{

    private List<Territories> currentMapState;
    private List<Regions> regions;
    private List<Agents> agentsList;

    private List<int> agentSpawnTerritoryIndexes = new List<int>();
    

    //Dict that stores the agentID as a key and the List<Territories for that agent, makes for faster indexing
    IDictionary<string, List<Territories>> territoriesByAgent = new Dictionary<string, List<Territories>>();
    
    
    public GameState(List<Territories> startingMapState, List<Regions> startingRegionState, List<Agents> agents)
    {
        currentMapState = startingMapState;
        regions = startingRegionState;
        agentsList = agents;
        populateAgentsOnMap();
        populateInitialTerritoriesByAgentDict();
    }
    
    
    /**
     * Foreach agent in the game, this assigns that agent a starting territory and updates
     * that territory to contain 5 initial armies
     */
    private void populateAgentsOnMap()
    {
        System.Random r = new System.Random();
        
        foreach (Agents agent in agentsList)
        {
            int idx = -1;
            while (idx == -1)
            {
                // This is to prevent the agents from spawning in the same territory, and thus crashing the game :/
                int tempIdx = r.Next(currentMapState.Count - 1);
                if (!agentSpawnTerritoryIndexes.Contains(idx))
                {
                    Territories startingTerritory = currentMapState[tempIdx];
                    startingTerritory.occupier = agent.agentName;
                    startingTerritory.armies = 5;
                    idx = tempIdx;
                }
            }
        }
    }
    
    /**
     * This initializes our territoriesByAgent dict
     * This dict was created in order to avoid having to iterate through every territory every time an agent
     * requested its frontline territories, or territories in general
     */
    private void populateInitialTerritoriesByAgentDict()
    {
        foreach (Territories territory in currentMapState)
        {
            if (!territoriesByAgent.ContainsKey(territory.occupier) && territory.occupier != "unconquered")
            {
                territoriesByAgent.Add(territory.occupier, new List<Territories>());
            }
        }
    }


    /**
     * Updates the gameState for the round
     * Re-populates agentTerritoryDict
     * Updates each region to see if there is an occupier
     * Checks Game-over conditions
     */

    public void nextRound()
    {
        populateTerritoriesByAgent();
        updateRegionalOccupiers();
        checkGameOverConditions();
    }


    /**
     * CALLED BY the controller every round
     * this updates our territoriesByAgent dict to contain the newest territories
     */
    private void populateTerritoriesByAgent()
    {
        foreach (Territories territory in currentMapState)
        {
            if (territoriesByAgent.ContainsKey(territory.occupier))
            {
                if (!territoriesByAgent[territory.occupier].Contains(territory))
                {
                    territoriesByAgent[territory.occupier].Add(territory);      
                }
            }
        }
    }

    /**
     * CALLED BY the controller every round
     * this updates each region to see if an agent holds a region or not, and if so, assigns it to be the occupier
     */
    private void updateRegionalOccupiers()
    {
        foreach (Regions region in regions)
        {
            List<string> occupiers = new List<string>();
            foreach (Territories territory in region.territories)
            {
                occupiers.Add(territory.occupier);
            }
            int uniqueOccupiers = occupiers.Distinct().Count();
            if (uniqueOccupiers == 1)
            {
                region.occupier = occupiers[0];
            }

        }
    }


    private void checkGameOverConditions()
    {
        List<string> occupiers = new List<string>();
        foreach (Regions region in regions)
        {
            occupiers.Add(region.occupier);
        }
        int uniqueOccupiers = occupiers.Distinct().Count();
        if (uniqueOccupiers == 1 && occupiers[0] != "unconquered")
        {
            EditorApplication.ExecuteMenuItem("Edit/Play");
        }
    }



    /**
     * Class abstract agent has access to, used to update all fields of abstract agent each round
     * NOTE: individual agents DO NOT have access to this class -> in addition, because the gameState field is private
     * there is no way an agent can modify the gameState
     */
    public class AbstractAgentGameState
    {
        
        private GameState gameState;

        public AbstractAgentGameState(GameState gameState)
        {
            this.gameState = gameState;
        }

        /**
         * CALLED BY an agent requesting to see their territories
         * return COPY of territory list for this abstractAgent, preserving gameState
         */
        public List<Territories> getTerritories(string agentName)
        {
            try
            {
                List<Territories> copyTerritories = new List<Territories>(gameState.territoriesByAgent[agentName]);
                return copyTerritories;
            }
            catch (Exception e)
            {
                throw new System.Exception("Attempted access of getTerritories by an unknown Agent");
            }
            
        }

        /**
         * CALLED BY an agent requesting to see the regions on the map
         * return a COPY of the region list for this abstractAgent, preserving gameState
         **/
        public List<Regions> getRegions()
        {
            List<Regions> regionsList = new List<Regions>(gameState.regions);
            return regionsList;
        }



        /**
         * CALLED BY an agent requesting to see their frontline, i.e. the territories that touch unconquered or enemy occupied territories
         * return COPY of territory list for this abstractAgent, preserving gameState
         */
        public List<Territories> getFrontLine(string agentName)
        {
            List<Territories> agentTerritories;
            try
            { 
                agentTerritories = gameState.territoriesByAgent[agentName];
            }
            catch (Exception e)
            {
                throw new System.Exception("Attempted access of getFrontlines by an unknown Agent");
            }

            List<Territories> frontLine = new List<Territories>();
            foreach (Territories territory in agentTerritories)
            {
                foreach (Territories neighborTerritory in territory.neighbors)
                {
                    if (neighborTerritory.occupier != agentName)
                    {
                        if (!frontLine.Contains(territory))
                        {
                            frontLine.Add(territory);   
                        }
                    }
                }
            }
            return frontLine;
        }

        
        /**
         * CALLED BY an abstractAgent requesting to see how many armies they currently have based on regional bonuses
         */
        public int getArmies(string agentName)
        {
            int finalArmiesValue = 5;
            foreach (Agents agent in gameState.agentsList)
            {

                if (agent.agentName == agentName)
                {
                    foreach (Regions region in gameState.regions) 
                    {
                        // TODO: this obviously doesn't make any sense, armies will grow infinetely
                        // TODO: also getting negative army values, so make sure the attack is fine
                        // need to re-calculate every round dumbass
                        if (agent.agentName == region.occupier)
                        {
                            finalArmiesValue += region.regionalBonusValue;
                        }
                    }
                }
            }

            return finalArmiesValue;
        }
        
        
        
        /**
        * Class which contains all the methods agents dynamically have access to -> the rest of the methods are handled
        * in the abstractAgentGameState class and are protected from individual agents calling them
        */
        public class AgentGameState
        {
            
            private GameState gameState;
            
            public AgentGameState(AbstractAgentGameState gameState)
            {
                this.gameState = gameState.gameState;
            }
        
        
            /**
            * CALLED BY an agent
            * Grabs the distance between two territories 
            */
            public float getDistance(string territory1, string territory2)
            {
                Territories t1 = gameState.currentMapState[gameState.getTerritoryIndex(territory1)];
                Territories t2 = gameState.currentMapState[gameState.getTerritoryIndex(territory2)];
                return Vector2.Distance(t1.centerCord, t2.centerCord);
            }
        }
        
    }





    // TODO: GAME STATE SHOULD BE CHECKING THAT
    // 1) Moves are valid
    // 2) Moves are from the correctPlayer
    // 3) Illegal number of armies being deployed
    // FOR ALL OF THESE< SUPER BIG ISSUE WITH NOT CHECKING, one agent can mess up the entire map
    // THIS IS ALL SUPER HARD, IGNORE IT FOR NOW

    /**
     * Input parameters: List<agentName, List<DeployMoves>)
     * CALLED BY the controller after the agents have all made their deployment moves
     * Updates the currentMapStates to include the new deployments of armies
     */
    public void updateDeploy(List<(string, List<DeployMoves>)> movesPerAgent)
    {
        foreach ((string, List<DeployMoves>) move in movesPerAgent)
        {
            string currentAgent = move.Item1;
            validateAgent(currentAgent, "updateDeploy");

            foreach (DeployMoves deployMove in move.Item2)
            {
                bool legalMove = false;
                int territoryIndex = getTerritoryIndex(deployMove.toTerritory);
                Territories moveTerritory = currentMapState[territoryIndex];
                legalMove = territoriesByAgent[currentAgent].Contains(moveTerritory);
                

                if (legalMove)
                {
                    currentMapState[territoryIndex].armies += deployMove.armies;
                }
                else
                {
                    throw new System.Exception(currentAgent + " has attempted to deploy troops to territories unconquered by this agent, " +
                                               "please check your generateDeploy() method in " + currentAgent);
                }
            }
        }
    }
    

    /**
     * Input parameters: List<agentName, List<AttackMoves>)
     * CALLED BY the controller after the agents have all made their attack moves
     * Moves through each agent attack by a priority assigned at the beginning of the round, updating
     * the currentMapState to reflect which agent won/lost that attack
     */
    public void updateAttack(List<(string, List<AttackMoves>)> movesPerAgent)
    {
        //TODO All of this needs to be thoroughly tested
        System.Random rand = new System.Random();

        // List<(AgentName, List<AttackMoves)>
        List<(string, List<AttackMoves>)> shuffledList = movesPerAgent.OrderBy(_ => rand.Next()).ToList();


        int remainingMoves = 0;
        foreach ((string, List<AttackMoves>) move in shuffledList)
        {
            remainingMoves += move.Item2.Count;
        }

        int currentMoveIdx = 0;

        // Not sure if we are leaving out the last move or not, needs to be tested
        while (remainingMoves > 0)
        {
            foreach ((string, List<AttackMoves>) move in shuffledList)
            {
                
                if (move.Item2.Count > currentMoveIdx)
                {
                    int fromIndex = getTerritoryIndex(move.Item2[currentMoveIdx].fromTerritory);
                    int toIndex = getTerritoryIndex(move.Item2[currentMoveIdx].toTerritory);

                    Territories fromTerritory = currentMapState[fromIndex];
                    Territories toTerritory = currentMapState[toIndex];
                    int attackingArmies = move.Item2[currentMoveIdx].armies;


                    if (fromTerritory.armies > 0 && fromTerritory.armies >= attackingArmies)
                    {
                        // If agent is attacking itself -> now becomes a transfer
                        if (fromTerritory.occupier == toTerritory.occupier)
                        {
                            fromTerritory.armies -= attackingArmies;
                            toTerritory.armies += attackingArmies;
                        }
                    
                        // If agent is attacking another territory and has enough armies to overtake it
                        else if (attackingArmies >= toTerritory.armies)
                        {
                            fromTerritory.armies -= attackingArmies;
                            toTerritory.armies = attackingArmies - toTerritory.armies;
                            toTerritory.occupier = move.Item1;
                        }
                        // Attack fails
                        else
                        {
                            fromTerritory.armies -= attackingArmies;
                        }   
                    }

                    remainingMoves -= 1;
                }
            }

            currentMoveIdx += 1;
        }
    }


    /**
     * Helper function for updateAttack which grabs the index of a given territory in the currentMapState
     */
    private int getTerritoryIndex(string territoryName)
    {
        for (int i = 0; i < currentMapState.Count; i++)
        {
            if (currentMapState[i].territoryName == territoryName)
            {
                return i;
            }
        }
        throw new System.Exception("Attempted access of currentMapState to update Territory that doesn't exist");
    }


    /**
     * Helper function that validates that the agent name exists and is in the agentTerritories dict
     */
    private bool validateAgent(string agentName, string functionName)
    {
        List<Territories> agentTerritories = new List<Territories>();
        try
        {
            agentTerritories = territoriesByAgent[agentName];
            return true;
        }
        catch (Exception e)
        {
            throw new System.Exception("Unknown Agent Name passed in in function: " + functionName);
        }
    }
    
    
    


    
    
    
    
    
    
    
    
    
    
    

}
