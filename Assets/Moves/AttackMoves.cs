/**
 * Class responsible for representing an Attack Move
 */
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
