using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayTerritoryInfo : MonoBehaviour{

    private int x;
    private int y;
    
    void OnGUI()
    {
        //print("here");
        //GUI.Label(new Rect(x, (int)y, 100, 20), "Hello World!");
    }




    public void spawnTextFromMousePosition(float x, float y)
    {
        this.x = (int) x;
        this.y = (int) y;
    }
    
}
