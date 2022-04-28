/**
 *  Class for representing a pixel on the map
 */
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
