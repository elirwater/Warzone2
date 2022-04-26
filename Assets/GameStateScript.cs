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

    public List<Territories> currentMapState;
    private List<Regions> regions;
    private List<Agents> agentsList;

    private List<int> agentSpawnTerritoryIndexes = new List<int>();
    private bool isFakeGameState;

    //Dict that stores the agentID as a key and the List<Territories for that agent, makes for faster indexing
    IDictionary<string, List<Territories>> territoriesByAgent = new Dictionary<string, List<Territories>>();
    
    
    public GameState(List<Territories> startingMapState, List<Regions> startingRegionState, List<Agents> agents, bool fakeGameState)
    {
        currentMapState = startingMapState;
        regions = startingRegionState;
        agentsList = agents;
        isFakeGameState = fakeGameState;


        //Legit terrible solution
        if (isFakeGameState)
        {
            updateAgentTerritoryLists();
            nextRound();
        }
        else
        {
            populateAgentsOnMap();
            updateAgentTerritoryLists();
        }
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
                    agentSpawnTerritoryIndexes.Add(tempIdx);
                    idx = tempIdx;
                }
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
        updateAgentTerritoryLists();
        updateRegionalOccupiers();
        checkDeadAgents();

    }



    /**
     * CALLED BY the controller every round
     * this checks if an agent has lost all it territory, and removes it from the game if so
     */
    private void checkDeadAgents()
    {
        if (!isFakeGameState)
        {
            for (int i = 0; i < agentsList.Count; i++)
            {
                if (territoriesByAgent[agentsList[i].agentName].Count == 0)
                {
                    agentsList.RemoveAt(i);
                }
            }   
        }
    }


    /**
     * CALLED BY the controller every round
     * this updates our territoriesByAgent dict to contain the newest territories
     */
    private void updateAgentTerritoryLists()
    {
        // Firstly, re-instantiate our territoryBy Agent dict
        territoriesByAgent = new Dictionary<string, List<Territories>>();

        foreach (Agents agent in agentsList)
        {
            territoriesByAgent.Add(agent.agentName, new List<Territories>());
        }
        
        
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
     *
     * THIS METHOD MIGHT NOT BE FUNCTIONAL
     */
    public void updateRegionalOccupiers()
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


    public bool checkGameOverConditions()
    {
        
        List<string> occupiers = new List<string>();
        foreach (Territories t in currentMapState)
        {
            occupiers.Add(t.occupier);
        }
        int uniqueOccupiers = occupiers.Distinct().Count();
        if (uniqueOccupiers == 1)
        {
            return true;
        }

        return false;

    }


    public string findWinner()
    {
         
        List<string> occupiers = new List<string>();
        foreach (Territories t in currentMapState)
        {
            occupiers.Add(t.occupier);
        }
        int uniqueOccupiers = occupiers.Distinct().Count();
        if (uniqueOccupiers == 1)
        {
            return occupiers[0];
        }
        else
        {
            throw new SystemException("Winner failed to be found");
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
                foreach (string neighborTerritoryName in territory.neighbors)
                {
                    Territories t = getTerritoryByName(neighborTerritoryName);
                    if (t.occupier != agentName)
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
            * Helper function to get a territory if given the territory name -> used when the transition from neighbors being
            * stored as territories to strings
             */
        public Territories getTerritoryByName(string name)
        {
            List<Territories> territories = gameState.currentMapState;
            foreach (Territories t in territories)
            {
                // Prolly should be returning a copy 
                if (t.territoryName == name)
                {
                    return t;
                }
            }

            throw new System.Exception("Could not locate territory");

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
            private AbstractAgentGameState abstractAgentGameState;
            
            public AgentGameState(AbstractAgentGameState gameState)
            {
                this.gameState = gameState.gameState;
                this.abstractAgentGameState = gameState;
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
            
            
            public List<string> getOpponents(string agentName)
            {
                List<string> opponents = new List<string>();
                foreach (var agent in gameState.agentsList)
                {
                    if (agent.agentName != agentName)
                    {
                        opponents.Add(agent.agentName);
                    }
                }
            
                return opponents;
            }
            
            
            /**
            * Returns a score based on the total number of armies this agent has plus the number of conquered territories
            * This should be tweaked heuristically
            * CALLED BY miniMaxAgent
             */
            public int generateScore(string agentName)
            {
                // REPEATED CODE, already exists in protected abstract agent class!
                int finalArmiesValue = 5;
                foreach (Agents agent in gameState.agentsList)
                {

                    if (agent.agentName == agentName)
                    {
                        foreach (Regions region in gameState.regions) 
                        {
                            if (agent.agentName == region.occupier)
                            {
                                finalArmiesValue += region.regionalBonusValue;
                            }
                        }
                    }
                }

                return finalArmiesValue + gameState.territoriesByAgent[agentName].Count;
            }


            
            public AgentGameState generateSuccessorGameState(DeployMoves dMove, AttackMoves aMove, string agentName)
            {

                List<Territories> newList = new List<Territories>(gameState.currentMapState.Count);

                foreach (var t in gameState.currentMapState)
                {
                    newList.Add(new Territories(t.centerCord, t.territoryName, t.neighbors, t.regionName, t.occupier, t.armies));
                }

                GameState g = new GameState(newList, gameState.regions, gameState.agentsList, true);

                g.updateDeploy(new List<(string, List<DeployMoves>)>(){(agentName, new List<DeployMoves>(){dMove})});
                g.updateAttack(new List<(string, List<AttackMoves>)>(){(agentName, new List<AttackMoves>(){aMove})});

                // A bit convuluded
                AbstractAgentGameState abs = new AbstractAgentGameState(g);
                return new AgentGameState(abs);
            }

            public List<Agents> getAgents()
            {
                return gameState.agentsList;
            }


            public bool checkGameOverConditions()
            {
                return gameState.checkGameOverConditions();
            }

            public string findWinner()
            {
                return gameState.findWinner();
            }
            
            
            public List<(DeployMoves, AttackMoves)> generateLegalMoves(string agentName)
            {

                // Generate a list of neighboring enemy territories (we are excluding internal troop transfers for recursive load issues)
                // List is structured as fromTerritory -> toTerritory
                List<(Territories, Territories)> neighboringEnemyTerritoriesTuple = new List<(Territories, Territories)>();
                // Keeps track of attacking territories so only a given territory we own cannot attack multiple territories for deployment and attack simplicity
                List<Territories> frontlineAttackingTerritories = new List<Territories>();
                
                foreach (Territories territory in abstractAgentGameState.getFrontLine(agentName))
                {
                    foreach (string neighboringTerritoryName in territory.neighbors)
                    {
                        Territories t = getTerritoryByName(neighboringTerritoryName);
                        if (!frontlineAttackingTerritories.Contains(territory) && t.occupier != agentName)
                        {
                            frontlineAttackingTerritories.Add(territory);
                            neighboringEnemyTerritoriesTuple.Add((territory, t));
                        }
                        
                    }
                }

                List<(DeployMoves, AttackMoves)> moves = new List<(DeployMoves, AttackMoves)>();
                foreach ((Territories, Territories) territory in neighboringEnemyTerritoriesTuple)
                {
                    if (territory.Item1.armies + abstractAgentGameState.getArmies(agentName) >= territory.Item2.armies)
                    {
                        // Doing it like this to make everything much simpler

                        DeployMoves dMove = new DeployMoves(territory.Item1.territoryName,
                            abstractAgentGameState.getArmies(agentName));

                        AttackMoves aMove = new AttackMoves(territory.Item1.territoryName, territory.Item2.territoryName,
                            territory.Item1.armies + abstractAgentGameState.getArmies(agentName));
                        moves.Add((dMove, aMove));   
                    }
                }
                
                
                return moves;
           
            }


            /**
            * Helper function to get a territory if given the territory name -> used when the transition from neighbors being
            * stored as territories to strings
             */
            public Territories getTerritoryByName(string name)
            {
                List<Territories> territories = gameState.currentMapState;
                foreach (Territories t in territories)
                {
                    // Prolly should be returning a copy 
                    if (t.territoryName == name)
                    {
                        return t;
                    }
                }

                throw new System.Exception("Could not locate territory");

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
            if (validateAgent(currentAgent, "updateDeploy"))
            {
                foreach (DeployMoves deployMove in move.Item2)
                {
                    bool legalMove = false;
                    int territoryIndex = -1;
                    // Some agents fail to properly instantiate moves, that is caught here to make the gameState more robust
                    try
                    {
                        territoryIndex = getTerritoryIndex(deployMove.toTerritory);
                        Territories moveTerritory = currentMapState[territoryIndex];
                        legalMove = territoriesByAgent[currentAgent].Contains(moveTerritory);
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                    
                    if (legalMove)
                    {
                        currentMapState[territoryIndex].armies += deployMove.armies;
                    }
                    else
                    {
                        if (isFakeGameState)
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
                    // Some agents fail to properly instantiate moves, that is caught here to make the gameState more robust
                    int fromIndex = -1;
                    int toIndex = -1;
                    try
                    {
                        fromIndex = getTerritoryIndex(move.Item2[currentMoveIdx].fromTerritory);
                        toIndex = getTerritoryIndex(move.Item2[currentMoveIdx].toTerritory);
                    }
                    catch
                    {
                        return;
                    }
                    
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
            return false;
        }
    }
    
    
    


    
    
    
    
    
    
    
    
    
    
    

}
