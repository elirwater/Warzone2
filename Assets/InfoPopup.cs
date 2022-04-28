using UnityEngine;
using TMPro;
using System.Text;

/**
 * Class responsible for displaying the territory info popup you see when a territory is clicked on
 */
[RequireComponent(typeof(TMP_Text))]
public class InfoPopup : MonoBehaviour
{
    [HideInInspector]
    public string displayText;
    
    void Start()
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();
        tmp_text.text = "";
    }
    

    public void displayTerritoryInfo(string name, string occupier, string region, int RegionalBonusValue, int numNeighbors, int numArmies)
    {
        TMP_Text tmp_text = GetComponent<TMP_Text>();

        StringBuilder s = new StringBuilder();
        s.Append("Territory Name: " + name);
        s.Append("\n");
        s.Append("Occupier Name: " + occupier);
        s.Append("\n");
        s.Append("Region Name: " + region);
        s.Append("\n");
        s.Append("RegionalBonusValue: " + RegionalBonusValue);
        s.Append("\n");
        s.Append("Number of Neighbors: " + numNeighbors);
        s.Append("\n");
        s.Append("Number of Armies: " + numArmies);
        s.Append("\n");
        
        tmp_text.text = s.ToString();
    }
}