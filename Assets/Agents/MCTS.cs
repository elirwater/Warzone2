using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Unity.Mathematics;
using UnityEditor.Experimental.GraphView;
using UnityEngine.Rendering;

public class MCTSAgent : Agents
{
    
    private int WIN_SCORE = 10;

    private (DeployMoves, AttackMoves) currentRoundMove;
    

    // So, each node in the tree is the gameState this node produces, and the (Deploy, AttacK) tuple that created it
    // I'm assuming playouts will use opponents?

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



    public void findNextMove(int iterations)
    {
        
        NodeT rootNode = new NodeT(new State(this.agentGameState, this.agentName, (null, null)), null);  //TODO: issue is coming from here, but these should all be populated unfortunately 

        for (int i = 0; i < iterations; i++)
        {
            NodeT promisingNode = selectPromisingNode(rootNode);
            if (promisingNode.state.boardStatus == "unfinished")
            {
                expandNode(promisingNode);
            }

            NodeT nodeToExplore = promisingNode;
            if (promisingNode.children.Count > 0)
            {
                nodeToExplore = promisingNode.getRandomChildNode();
            }

            string playoutResult = simulateRandomPlayout(nodeToExplore);
            //TODO: also modified here
            if (playoutResult != "hanging")
            {
                backPropagation(nodeToExplore, playoutResult);   
            }
        }

        NodeT winnerNode = rootNode.getChildWithMaxScore();

        this.currentRoundMove = winnerNode.state.moveToState;
        
        
        // print(agentName);
        // print(currentRoundMove.Item1.toTerritory);
        // print(currentRoundMove.Item2.fromTerritory);
        // print(currentRoundMove.Item2.toTerritory);
        // print("done");
        
    }
    
    
    
    
    

    public class NodeT
    {
        public State state;
        
        
        // TODO: need some way to keep track of origin move along path, right now we can only return a novel gameState that has been updated by MCTS
        
        public NodeT parent;
        public List<NodeT> children;

        public NodeT(State state, NodeT parent)
        {
            this.state = state;
            this.parent = parent;
            this.children = new List<NodeT>();
        }


        public NodeT(NodeT node)
        {
            //TODO COULD BE SOME REAL REFERENCING ISSUES HERE -> recall deep cloning issues faced earlier
            state = node.state;
            parent = node.parent;
            children = new List<NodeT>(node.children);
        }


        public NodeT getRandomChildNode()
        {
            System.Random r = new System.Random();
            return children[r.Next(children.Count - 1)];
        }


        public NodeT getChildWithMaxScore()
        {
            
            //TODO: not sure about this one
            if (children.Count == 0)
            {
                return this;
            }
            
            int maxScore = int.MinValue;
            
            
            NodeT maxNode = children[0];

            foreach (var child in children)
            {
                if (child.state.winScore > maxScore)
                {
                    maxScore = (int) child.state.winScore;
                    maxNode = child;
                }
            }

            return maxNode;
        }
    }

    public class State
    {
        public GameState.AbstractAgentGameState.AgentGameState g;
        //implemented because some states don't have a playout ?????
        public int numRandomPlayouts;
        
        public (DeployMoves, AttackMoves) moveToState;
        public string currentAgentName;
        public int visitCount;
        public double winScore;
        public string boardStatus;
        private List<string> opponents;
        private int opponentIndex;
        
        public State(GameState.AbstractAgentGameState.AgentGameState g, string agentName, (DeployMoves, AttackMoves) moveToState)
        {
            this.g = g;
            this.currentAgentName = agentName;
            this.moveToState = moveToState;
            this.visitCount = 0;
            this.winScore = int.MinValue;
            this.opponents = g.getOpponents(currentAgentName);
            this.boardStatus = "unfinished";  
            // so we can just iterate through the opponents list
            this.opponents.Add(currentAgentName);
            this.numRandomPlayouts = 0;
        }

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

        public void randomPlay()
        {
            //TODO Could possibly get stuck, we want to avoid this
            numRandomPlayouts += 1;
            if (numRandomPlayouts > 10000)
            {
                this.boardStatus = "hanging";
            }
            
            List<(DeployMoves, AttackMoves)> legalMoves = g.generateLegalMoves(currentAgentName);

            if (legalMoves.Count > 0)
            {
                System.Random r = new System.Random();
                (DeployMoves, AttackMoves) move = legalMoves[r.Next(0, legalMoves.Count - 1)];
            
                g = g.generateSuccessorGameState(move.Item1, move.Item2, currentAgentName);    
            }
            else
            {
                //TODO: WILL ONLY WORK IN A 2v2!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // So what do we do if there aren't any legal moves left?
                
                // We have 2 basic options
                // 1 ) Select a random winner from the remaining agents
                // 2 ) somehow remove an agent from the game?
                //print("About to be a null ref exception");
                //this.boardStatus = "hanging";
                this.boardStatus = opponents[1];
                return;
            }
            
            
            
            if (g.checkGameOverConditions())
            {
                this.boardStatus = g.findWinner();
                print("here");
            }
            
        }

        public string getOpponent()
        {
            return this.opponents[opponentIndex];
        }

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
    
    


    public class UCT
    {
        public static double uctValue(int totalVisit, double nodeWinScore, int nodeVisit)
        {
            if (nodeVisit == 0)
            {
                return int.MaxValue;
            }

            return (nodeWinScore / nodeVisit) +
                   1.41 * math.sqrt(math.log(totalVisit) / (double) nodeVisit);
        }

        public static NodeT findBestNodeWithUCT(NodeT node)
        {
            int parentVisit = node.state.visitCount;

            double maxVal = int.MinValue;
            NodeT returnNode = null;

            foreach (var childNode in node.children)
            {
                double val = uctValue(parentVisit, childNode.state.winScore, childNode.state.visitCount);
                if (val >= maxVal)
                {
                    maxVal = val;
                    returnNode = childNode;
                }
            }

            return returnNode;
        }
    }

    
    
    private NodeT selectPromisingNode(NodeT rootNode) {
        NodeT node = rootNode;
        while (node.children.Count != 0) {
            node = UCT.findBestNodeWithUCT(node);
        }
        return node;
    }


    private void expandNode(NodeT node)
    {
        List<State> possibleStates = node.state.getAllPossibleStates();
        foreach (var state in possibleStates)
        {
            NodeT newNode = new NodeT(state, node);
            newNode.state.currentAgentName = node.state.getOpponent();
            node.children.Add(newNode);
        }
    }


    private void backPropagation(NodeT nodeToExplore, string agentName)
    {
        NodeT tempNode = nodeToExplore;
        while (tempNode != null)
        {
            tempNode.state.visitCount += 1;
            if (tempNode.state.currentAgentName == agentName)
            {
                tempNode.state.winScore += WIN_SCORE;
            }
            tempNode = tempNode.parent;
        }
    }


    private string simulateRandomPlayout(NodeT node)
    {
        NodeT tempNode = node;
        State tempState = tempNode.state;
        string boardStatus = tempState.boardStatus;
        
        if (boardStatus == tempState.getOpponent())
        {
            if (tempNode.parent != null)
            {
                tempNode.parent.state.winScore = int.MinValue;
                return boardStatus;     
            }

        }

        while (boardStatus == "unfinished")
        {
            tempState.toggleAgent();
            tempState.randomPlay();
            boardStatus = tempState.boardStatus;
        }

        return boardStatus;
    }
    
    





}