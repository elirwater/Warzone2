using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territories
{
    public Vector2 centerCord;
    public string territoryName;

    public List<Territories> neighbors;

    public string regionName;

    public string occupier;
    public int armies;

    public Territories(Vector2 centerCord, string territoryName)
    {
        this.centerCord = centerCord;
        this.territoryName = territoryName;
        this.neighbors = new List<Territories>();
        this.regionName = null;
        this.occupier = "unconquered";
        this.armies = 3;
    }


    // Adds a neigbor and checks that it isn't already in this territories list of neighbors
    public void addNeighbor(Territories territory)
    {
        if (!neighbors.Contains(territory))
        {
            neighbors.Add(territory);
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
