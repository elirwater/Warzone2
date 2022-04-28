/**
 * Class responsible for representing an Deploy Move
 */
public class DeployMoves
{
    public string toTerritory;
    public int armies;

    public DeployMoves(string toTerritory, int armies)
    {
        this.toTerritory = toTerritory;
        this.armies = armies;
    }
}