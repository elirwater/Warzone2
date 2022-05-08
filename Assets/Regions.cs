using System.Collections.Generic;

/**
 * Class for representing a region
 */
public class Regions
{
    public string regionName;
    
    public List<string> territories;
    public int regionalBonusValue;
    public string occupier;

    public Regions(string regionName, int regionalBonusValue)
    {
        this.regionName = regionName;
        this.regionalBonusValue = regionalBonusValue;
        this.territories = new List<string>();
        this.occupier = "unconquered";
    }

}
