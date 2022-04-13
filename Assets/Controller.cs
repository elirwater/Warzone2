using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = Unity.Mathematics.Random;

public class Controller : MonoBehaviour


{
    private GameState gameStateObj;
    private MapGeneration mapState;
    private MapRendering mapRendering;
    private List<Agents> agents;


    private Agents inGameAgent;

    

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
        public string seed;
        
    }

    public MapGenerationData mapGenerationData;
    
    
    private void Start()
    {
        initializeGame();
    }
    
    
    /**
     * Initializes the the game, by instantiating our agents, GameState, and other important objects
     */
    private void initializeGame()
    {
        // Note: will need to change in the future when there are multiple agents simultaneously
        
        // We first instantiate the two of the core objets used by the controller, the map and the mapRenderer
        mapState = FindObjectOfType<MapGeneration>();
        mapRendering = FindObjectOfType<MapRendering>();
        
        // We grab our map, which is a list of territories and feed it into the GameState 
        List<Territories> territories = mapState.getTerritories();
        
        // We invoke the rendering aspect of the map after 1 second (due to how long the map takes to generate)
        Invoke("updateMapForRendering", 1f);
        
        // We instantiate our agent
        inGameAgent = new NonAdversarialBFS();


        
        agents = new List<Agents>(){inGameAgent};
        
        
        // We then generate our GameState class which controls all aspects of the game
        gameStateObj = new GameState(territories, mapState.getRegions(), agents);
        
        
        // Now we assign our agent the same GameState object so then can make calls and query the gameState
        foreach (Agents agent in agents)
        {
            agent.setAbstractAgentGameState(new GameState.AbstractAgentGameState(gameStateObj));
        }
    }


    /**
     * Whenever our GameState has been updated, we have to update the visual side of the gameState in the mapRendering
     * This passes a pixelMap to the rendering engine which figures out what to do with each pixel
     */
    private void updateMapForRendering()
    {
        mapRendering.updateMap(mapState.grabMapForRendering());
    }
    
    
    private void Update()
    {
        // A given round is progressed by hitting the space key (for now)
        if (Input.GetKey ("space"))
        {
            nextRound();
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

        updateMapForRendering();

    }
}
