using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackMoves
{
    public string fromTerritory;
    public string toTerritory;
    public int armies;

    public AttackMoves(string fromTerritory, string toTerritory, int armies)
    {
        this.fromTerritory = fromTerritory;
        this.toTerritory = toTerritory;
        this.armies = armies;
    }
}
