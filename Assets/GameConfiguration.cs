using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConfiguration
{
    
    // All of the variables should be stored in this class, then when spacebar is pressed from homescreen class, the startGame method should
    // be called in the controller which will fetch the configuration data from this class

    private static readonly List<String> agentNames = new List<string>()
    {
        "player", "naiveAgent", "dfsAgent", "bfsAgent", "testingAgent",
        "alphaBetaAgent", "miniMaxAgent", "expectiMaxAgent", "MCTSAgent"
    };

    public static List<String> getActiveAgents()
    {
        List<string> activeAgents = new List<string>();
        
        foreach (string agentName in agentNames)
        {
            int numSpecificAgents = PlayerPrefs.GetInt(agentName);
            if (numSpecificAgents > 0)
            {
                for (int i = 0; i < numSpecificAgents; i++)
                {
                    activeAgents.Add(agentName);
                }
            }
        }

        return activeAgents;
    }


    public static void test()
    {
        setPlayer(1);
        setNaiveAgent(2);
    }
    
    

    public static void setPlayer(int value)
    {
        PlayerPrefs.SetInt("player", value);
    }
    public static void setNaiveAgent(int value)
    {
        PlayerPrefs.SetInt("naiveAgent", value);
    }
    public static void setDfsAgent(int value)
    {
        PlayerPrefs.SetInt("dfsAgent", value);
    }
    public static void setBfsAgent(int value)
    {
        PlayerPrefs.SetInt("bfsAgent", value);
    }
    public static void setTestingAgent(int value)
    {
        PlayerPrefs.SetInt("testingAgent", value);
    }
    public static void setAlphaBetaAgent(int value)
    {
        PlayerPrefs.SetInt("alphaBetaAgent", value);
    }
    public static void setMiniMaxAgent(int value)
    {
        PlayerPrefs.SetInt("miniMaxAgent", value);
    }
    public static void setExpectiMaxAgent(int value)
    {
        PlayerPrefs.SetInt("expectiMaxAgent", value);
    }
    public static void setMCTSAgent(int value)
    {
        PlayerPrefs.SetInt("MCTSAgent", value);
    }
    
    
    
    
    public static int getPlayer()
    {
        return PlayerPrefs.GetInt("player");;
    }
    public static int getNaiveAgent()
    {
        return PlayerPrefs.GetInt("naiveAgent");;
    }
    public static int getDfsAgent()
    {
        return PlayerPrefs.GetInt("dfsAgent");
    }
    public static int getBfsAgent()
    {
        return PlayerPrefs.GetInt("bfsAgent");
    }
    public static int getTestingAgent()
    {
        return PlayerPrefs.GetInt("testingAgent");
    }
    public static int getAlphaBetaAgent()
    {
        return PlayerPrefs.GetInt("alphaBetaAgent");
    }
    public static int getMiniMaxAgent()
    {
        return PlayerPrefs.GetInt("miniMaxAgent");
    }
    public static int getExpectiMaxAgent()
    {
        return PlayerPrefs.GetInt("expectiMaxAgent");
    }
    public static int getMCTSAgent()
    {
        return PlayerPrefs.GetInt("MCTSAgent");
    }
    
}
