using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

/**
 * Class responsible for controlling the progression of the game and calling every other script ->
 * Controller operates both on spacebar to progress the round, or a series of values telling it the number of games to
 * play and the number of rounds to attempt in each game
 */
public class Controller : MonoBehaviour
{
    private GameState gameStateObj;
    private MapGeneration mapState;
    private MapRendering mapRendering;
    private List<Agents> agents;
    private Agents inGameAgent;
    private Agents inGameAgent2;
    private Agents inGameAgent3;
    private Agents inGameAgent4;
    private Agents inGameAgent5;
    private bool isGameOver;
    

    /**
     * Menu in editor for selecting the map generation style
     */
    [System.Serializable]
    public class MapGenerationData
    {
        public int numTerritories;
        public int numRegions;
        public int pointsPerTerritory;
        public int height;
        public int width;
        public int smoothIterations;
        [Range(0, 100)] public int randomFillPercent;
    }
    public MapGenerationData mapGenerationData;
    
    /**
     * Menu in editor for selecting which agents are playing and how many of each
     */
    [System.Serializable]
    public class AgentData
    {
        public bool naiveAgent;
        public bool dfsAgent;
        public bool bfsAgent;
        public int testingAgent;
        public int alphaBetaAgent;
        public int miniMaxAgent;
        public int expectiMaxAgent;
        public int MCTSAgent;
    }
    public AgentData agentData;

    
    /**
     * Menu in editor for selecting which analytic mode you want to turn on
     */
    [System.Serializable]
    public class Analytics
    {
        public bool analyticsOn;
        public int numSimulatedGames;
        public int numPlayoutRounds;
    }
    public Analytics analytics;
    
    
    /**
     * Initializes the game based on the inputted editor menu parameters
     */
    private void Start()
    {
        initializeGame();
        
        if (analytics.analyticsOn)
        {
            if (analytics.numSimulatedGames != 0 && analytics.numPlayoutRounds != 0)
            {
                generateAnalyticForXGames(analytics.numSimulatedGames, analytics.numPlayoutRounds);
            }
            
            if (analytics.numPlayoutRounds > 0 && analytics.numSimulatedGames == 0)
            {
                generateAnalytics(analytics.numPlayoutRounds);
            }   
        }
    }
    
    
    /**
     * Initializes the the game, by instantiating our agents, GameState, and other important objects
     */
    private void initializeGame()
    {
        isGameOver = false;

        mapState = FindObjectOfType<MapGeneration>();
        mapRendering = FindObjectOfType<MapRendering>();
        
        List<Territories> territories = mapState.getTerritories();
        

        agents = new List<Agents>();
        instantiateAgentsFromEditor();
        
        // We then generate our GameState class which controls all aspects of the game
        gameStateObj = new GameState(new List<Territories>(territories), new List<Regions>(mapState.getRegions()), agents, false);
        
        // Now we assign our agent the same GameState object so then can make calls and query the gameState
        foreach (Agents agent in agents)
        {
            agent.setAbstractAgentGameState(new GameState.AbstractAgentGameState(gameStateObj));
        }
        
        
        updateMapForRendering();
        mapRendering.renderEntireMap();
    }



    /**
     * Whenever our GameState has been updated, we have to update the visual side of the gameState in the mapRendering
     * This passes a pixelMap to the rendering engine which figures out what to do with each pixel
     */
    private void updateMapForRendering()
    {
        mapRendering.updatePixelMap(mapState.grabMapForRendering());
    }
    
    /**
     * Checks if the game is over every tick and whether the spacebar has been pressed to manually advance the game
     */
    private void Update()
    {
        // A given round is progressed by hitting the space key (for now)
        if (Input.GetKeyDown("space") && !isGameOver)
        {
            nextRound();
        }

        if (isGameOver)
        {
            initializeGame();
        }
    }

    
    /**
     * Progressed the game to the next round
     * Order of execution:
     * 1. GameState gets updated
     * 2. Foreach agent
     *    1. Generate Deploy Moves
     *    2. Update the gameState w/ these moves
     *    3. Generate Attack Moves
     *    4. Update gameState w/ these moves
     * 3. Update map for rendering
     */
    private void nextRound()
    {
        gameStateObj.nextRound();
        
        foreach (Agents agent in agents)
        {
            agent.nextRound();
            List<DeployMoves> deployMovesTestingAgent = agent.generateDeployMoves();
            List<(string, List<DeployMoves>)> deployMoves = new List<(string, List<DeployMoves>)>();
            deployMoves.Add((agent.agentName, deployMovesTestingAgent));
            gameStateObj.updateDeploy(deployMoves);
        }
        
        List<(string, List<AttackMoves>)> attackMoves = new List<(string, List<AttackMoves>)>();
        foreach (Agents agent in agents)
        {
            List<AttackMoves> agentAttackMoves = agent.generateAttackMoves();
            attackMoves.Add((agent.agentName, agentAttackMoves));
        }
        gameStateObj.updateAttack(attackMoves);
        
        

        if (!analytics.analyticsOn)
        {
            // TODO: should be offloaded to the GameState class -> dumb to check it here (just do it in the updateAttack method) and make it a list with a method to grab that list 

            List<string> territoriesToBeUpdated = new List<string>();
            
            foreach ((string, List<AttackMoves>) attackMoveByAgent in attackMoves)
            {
                foreach (AttackMoves attackMove in attackMoveByAgent.Item2)
                {
                    try
                    {
                        string modifiedTerritory = attackMove.toTerritory;

                        if (!territoriesToBeUpdated.Contains(modifiedTerritory))
                        {
                            territoriesToBeUpdated.Add(modifiedTerritory);
                        }
                    }
                    // Attack move is sometimes null, this is usually handled in the gamestate
                    catch (Exception e)
                    {
                        
                    }
                }
            }
            
            mapRendering.renderMapByModifiedTerritories(territoriesToBeUpdated);
        }

        updateMapForRendering();
        
        gameStateObj.updateRegionalOccupiers();
        if (gameStateObj.checkGameOverConditions())
        {
            print("GAME OVER");
            isGameOver = true;
        }
    }
    
    
    /**
     * Generates analytics for a certain number of rounds
     * Info such as:
     * 1. Num territories conquered per agent
     * 2. Num regions conquered per agent
     * 2. Num points per agent
     * 3. 
     */
    private void generateAnalytics(int numRounds)
    {
        for (int i = 0; i < numRounds; i++)
        {
            nextRound();
            if (isGameOver)
            {
                print("Game lasted " + i + " rounds");
                break;
            }
        }

        gameStateObj.nextRound();
        foreach (var a in agents)
        {
            a.nextRound();
            print(a.getStringAnalytics());
        }
    }
    
    /**
     * Generates analytics for a certain number of games with a cap on a certain number of rounds
     */
    private void generateAnalyticForXGames(int numGames, int numRounds)
    {

        IDictionary<string, List<(int, int, int)>> gameInfoByAgent =
            new Dictionary<string, List<(int, int, int)>>();
        
        IDictionary<string, int> victoriesByAgent =
            new Dictionary<string, int>();
        
        IDictionary<string,  List<int>> roundsToWinByAgent =
            new Dictionary<string, List<int>>();
        
        foreach (var a in agents)
        {
            gameInfoByAgent.Add(a.agentName, new List<(int, int, int)>());
            victoriesByAgent.Add(a.agentName, 0);
            roundsToWinByAgent.Add(a.agentName, new List<int>());
        }
        
        for (int j = 0; j < numGames; j++)
        {
            try
            {
                for (int i = 0; i < numRounds; i++)
                {
                    nextRound();

                    if (isGameOver)
                    {
                        gameStateObj.nextRound();
                        
                        int maxArmies = 0;
                        Agents winningAgent = null;
                        foreach (var a in agents)
                        {
                            a.nextRound();
                            if (a.getAnalytics().Item1 > maxArmies)
                            {
                                maxArmies = a.getAnalytics().Item1;
                                winningAgent = a;
                            }
                        }
                        roundsToWinByAgent[winningAgent.agentName].Add(i);
                        victoriesByAgent[winningAgent.agentName] += 1;

                        break;
                    }
                }
            }
            // Sometimes the agents have edge-case errors that we need to prevent otherwise our data won't be collected
            catch (Exception e)
            {
            }
            initializeGame();
        }
        
        foreach (var data in victoriesByAgent)
        {
            print(data.Key);
            print(data.Value);
            // int sum = 0;
            // foreach (var dataPoint in data.Value)
            // {
            //     sum += dataPoint;
            // }
            // print(data.Key + " " + sum / data.Value.Count);
        }
    }
  
    
    /**
     * Instantiates the agents selected from the editor to be in this game
     */
    private void instantiateAgentsFromEditor()
    {
        System.Random r = new System.Random();

        if (agentData.dfsAgent)
        {
            agents.Add(new NonAdversarialDFS());
            return;
        }
        
        if (agentData.bfsAgent)
        {
            agents.Add(new NonAdversarialBFS());
            return;
        }
        if (agentData.naiveAgent)
        {
            agents.Add(new NaiveAgent());
        }

        if (agentData.testingAgent > 0)
        {
            for (int i = 0; i < agentData.testingAgent; i++)
            {
                Agents a = new TestingAgent();
                a.agentName = a.agentName + "_" + r.Next(1000);
                agents.Add(a);   
            }
        }
        
        if (agentData.alphaBetaAgent > 0)
        {
            for (int i = 0; i < agentData.alphaBetaAgent; i++)
            {
                Agents a = new AlphaBetaAgent();
                a.agentName = a.agentName;
                agents.Add(a);   
            }
        }
        
        if (agentData.miniMaxAgent > 0)
        {
            for (int i = 0; i < agentData.miniMaxAgent; i++)
            {
                Agents a = new MiniMaxAgent();
                a.agentName = a.agentName + "_" + r.Next(1000);
                agents.Add(a);   
            }
        }
        
        if (agentData.expectiMaxAgent > 0)
        {
            for (int i = 0; i < agentData.expectiMaxAgent; i++)
            {
                Agents a = new ExpectiMaxAgent();
                a.agentName = a.agentName + "_" + r.Next(1000);
                agents.Add(a);   
            }
        }
        if (agentData.MCTSAgent > 0)
        {
            for (int i = 0; i < agentData.MCTSAgent; i++)
            {
                Agents a = new MCTSAgent();
                a.agentName = a.agentName;
                agents.Add(a);   
            }
        }
        
    }
}
