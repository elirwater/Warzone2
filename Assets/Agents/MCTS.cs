using System.Collections.Generic;
using Unity.Mathematics;

/**
 * Class for representing the Monte Carlo Tree Search Agent
 */
public class MCTSAgent : Agents
{
    private (DeployMoves, AttackMoves) currentRoundMove;

    public MCTSAgent()
    {
        agentName = "MCTSAgent";
    }

    
    public override List<DeployMoves> generateDeployMoves()
    {
        findNextMove(10);
        return new List<DeployMoves>(){currentRoundMove.Item1};
    }

    
    public override List<AttackMoves> generateAttackMoves()
    {
        return new List<AttackMoves>(){currentRoundMove.Item2};
    }


    /**
     * Enum for storing the different states of a given simulated playout
     */
    public enum SimulatedPlayoutStates
    {
        inProgress,
        outOfMoves,
        agentVictory
    }


    /**
     * Called each round to simulate the next (deploy, attack) tuple using MCTS
     */
    public void findNextMove(int iterations)
    {
        
        NodeT rootNode = new NodeT(new State(this.agentGameState, this.agentName, (null, null)), null);

        for (int i = 0; i < iterations; i++)
        {
            NodeT promisingNode = selection(rootNode);
            if (promisingNode.state.playoutStatus == SimulatedPlayoutStates.inProgress)
            {
                expansion(promisingNode);
            }

            NodeT nodeToExplore = promisingNode;
            if (promisingNode.children.Count > 0)
            {
                nodeToExplore = promisingNode.selectRandomChildNode();
            }
            
            string playoutResult = simulation(nodeToExplore);
            backPropagation(nodeToExplore, playoutResult);
        }
        NodeT winnerNode = rootNode.SelectChGetChildWithMaxScore();
        currentRoundMove = winnerNode.state.moveToState;
    }

    
    /**
     * Class for representing a given node in the MCTS search tree
     */
    public class NodeT
    {
        public State state;
        public NodeT parent;
        public List<NodeT> children;

        public NodeT(State state, NodeT parent)
        {
            this.state = state;
            this.parent = parent;
            children = new List<NodeT>();
        }
        

        /**
         * Selects a random child node
         */ 
        public NodeT selectRandomChildNode()
        {
            System.Random r = new System.Random();
            return children[r.Next(children.Count - 1)];
        }

        
        /**
         * Selects the child node with the highest number of wins
         */
        public NodeT SelectChGetChildWithMaxScore()
        {
            if (children.Count == 0)
            {
                return this;
            }
            
            int maxScore = int.MinValue;
            
            NodeT maxNode = children[0];

            foreach (var child in children)
            {
                if (child.state.numWins > maxScore)
                {
                    maxScore = (int) child.state.numWins;
                    maxNode = child;
                }
            }
            return maxNode;
        }
    }

    /**
     * Class for representing a given gameState with additional fields used by MCTS
     */
    public class State
    {
        public GameState.AbstractAgentGameState.AgentGameState g;
        public (DeployMoves, AttackMoves) moveToState;
        public string currentAgentName;
        public int visitCount;
        public double numWins;
        public SimulatedPlayoutStates playoutStatus;
        public string victoriousAgent;
        private List<string> opponents;
        private int opponentIndex;
        
        public State(GameState.AbstractAgentGameState.AgentGameState g, string agentName, (DeployMoves, AttackMoves) moveToState)
        {
            this.g = g;
            currentAgentName = agentName;
            this.moveToState = moveToState;
            visitCount = 0;
            numWins = int.MinValue;
            opponents = g.getOpponents(currentAgentName);
            playoutStatus = SimulatedPlayoutStates.inProgress;
            opponents.Add(currentAgentName);
        }

        /**
         * Grabs all possible states composed of each combination of tuple (deploy, attack) moves available from this state
         */
        public List<State> getAllPossibleStates()
        {
            List<State> possibleStates = new List<State>();
            
            List<(DeployMoves, AttackMoves)> legalMoves = g.generateLegalMoves(currentAgentName);
            
            foreach (var legalMove in legalMoves)
            {
                GameState.AbstractAgentGameState.AgentGameState newG = g.generateSuccessorGameState(legalMove.Item1, legalMove.Item2, currentAgentName);
                State s = new State(newG, currentAgentName, legalMove);
                possibleStates.Add(s);
            }
            return possibleStates;
        }

        
        /**
         * Randomly plays a a given move from the list of legal moves available from this state
         */
        public void randomPlay()
        {
            List<(DeployMoves, AttackMoves)> legalMoves = g.generateLegalMoves(currentAgentName);

            if (legalMoves.Count > 0)
            {
                System.Random r = new System.Random();
                (DeployMoves, AttackMoves) move = legalMoves[r.Next(0, legalMoves.Count - 1)];
            
                g = g.generateSuccessorGameState(move.Item1, move.Item2, currentAgentName);    
            }
            else
            {
                // Else clause triggers if the agent has no more legal moves left but isn't dead (essentially a loss)
                this.playoutStatus = SimulatedPlayoutStates.outOfMoves;
                return;
            }
            
            if (g.checkGameOverConditions())
            {
                playoutStatus = SimulatedPlayoutStates.agentVictory;
                victoriousAgent = g.findWinner();
            }
            
        }
        
        /**
         * Returns the current opponent, used for games with over 2 agents
         */
        public string getOpponent()
        {
            return opponents[opponentIndex];
        }

        // Toggles the current player
        public void toggleAgent()
        {
            if (opponentIndex == opponents.Count - 1)
            {
                opponentIndex = 0;
            }
            else
            {
                opponentIndex += 1;
            }
        }
    }
    


    /**
     * Class used for calculating upper confidence bound for MCTS in order to weigh exploration vs. exploitation
     * Formulas from: https://en.wikipedia.org/wiki/Monte_Carlo_tree_search
     */
    public class UCT
    {
        /**
         * Calculates the UCT value
         */
        public static double calculateUCTValue(int totalVisit, double nodeWinScore, int nodeVisit)
        {
            if (nodeVisit == 0)
            {
                return int.MaxValue;
            }
            
            return (nodeWinScore / nodeVisit) +
                   1.41 * math.sqrt(math.log(totalVisit) / (double) nodeVisit);
        }

        /**
         * Iterates through children nodes and finds the node with the highest UCT value
         */
        public static NodeT selectBestUCTNode(NodeT node)
        {
            int parentVisit = node.state.visitCount;

            double maxVal = int.MinValue;
            NodeT returnNode = null;

            foreach (var childNode in node.children)
            {
                double val = calculateUCTValue(parentVisit, childNode.state.numWins, childNode.state.visitCount);
                if (val >= maxVal)
                {
                    maxVal = val;
                    returnNode = childNode;
                }
            }

            return returnNode;
        }
    }
    
    private NodeT selection(NodeT rootNode) {
        NodeT node = rootNode;
        while (node.children.Count != 0) {
            node = UCT.selectBestUCTNode(node);
        }
        return node;
    }

    private void expansion(NodeT node)
    {
        List<State> possibleStates = node.state.getAllPossibleStates();
        foreach (var state in possibleStates)
        {
            NodeT newNode = new NodeT(state, node);
            newNode.state.currentAgentName = node.state.getOpponent();
            node.children.Add(newNode);
        }
    }
    private string simulation(NodeT node)
    {
        NodeT tempNode = node;
        State tempState = tempNode.state;
        SimulatedPlayoutStates playoutStatus = tempState.playoutStatus;
        if (playoutStatus == SimulatedPlayoutStates.agentVictory && tempState.victoriousAgent == tempState.getOpponent())
        {
            if (tempNode.parent != null)
            {
                tempNode.parent.state.numWins = int.MinValue;
                return tempState.victoriousAgent;     
            }
        }
        while (playoutStatus == SimulatedPlayoutStates.inProgress)
        {
            tempState.toggleAgent();
            tempState.randomPlay();
            playoutStatus = tempState.playoutStatus;
        }
        return tempState.victoriousAgent;
    }
    
    private void backPropagation(NodeT nodeToExplore, string agentName)
    {
        NodeT tempNode = nodeToExplore;
        while (tempNode != null)
        {
            tempNode.state.visitCount += 1;
            if (tempNode.state.playoutStatus == SimulatedPlayoutStates.outOfMoves)
            {
                tempNode.state.numWins -= 1;
            }
            if (tempNode.state.victoriousAgent == agentName)
            {
                tempNode.state.numWins += 1;
            }
            tempNode = tempNode.parent;
        }
    }
}