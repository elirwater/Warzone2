﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territories
{
    public Vector2 centerCord;
    public string territoryName;

    public List<string> neighbors;

    public string regionName;

    public string occupier;
    public int armies;

    public Territories(Vector2 centerCord, string territoryName)
    {
        this.centerCord = centerCord;
        this.territoryName = territoryName;
        this.neighbors = new List<string>();
        this.regionName = null;
        this.occupier = "unconquered";
        this.armies = 3;
    }
    
    
    
    
    //For Deepcloning territories
    public Territories(Vector2 centerCord, string territoryName, List<string> neighbors, string regionName,
        string occupier, int armies)
    {
        this.centerCord = centerCord;
        this.territoryName = territoryName;
        this.neighbors = neighbors;
        this.regionName = regionName;
        this.occupier = occupier;
        this.armies = armies;
    }


    // Adds a neigbor and checks that it isn't already in this territories list of neighbors
    public void addNeighbor(string territoryName)
    {
        if (!neighbors.Contains(territoryName))
        {
            neighbors.Add(territoryName);
        }
    }


    public string getOccupier()
    {
        if (occupier == null)
        {
            return "Unconquered";
        }
        else
        {
            return occupier;
        }
    }
}
