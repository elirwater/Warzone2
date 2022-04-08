using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pixel
{
    
    public string territoryName;
    public string regionName;
    public int value;
    public bool isBorder;
    public bool isOcean;

    public Pixel(int value)
    {
        this.value = value;
        this.territoryName = null;
        this.regionName = null;
        this.isBorder = false;
        this.isOcean = false;
    }

}
