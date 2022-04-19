using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regions
{
    public string regionName;
    
    public List<Territories> territories;
    public int regionalBonusValue;
    public string occupier;

    public Regions(string regionName, int regionalBonusValue)
    {
        this.regionName = regionName;
        this.regionalBonusValue = regionalBonusValue;
        this.territories = new List<Territories>();
        this.occupier = "unconquered";
    }

}
