using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NameGenerator : MonoBehaviour
{




    List<string> usedTerritoryNames;
    List<string> usedRegionNames;


    string[] inputRegionNames;
    string[] inputTerritoryNames;


    public bool filesRead;
    


    private void Start()
    {
        this.filesRead = false;
    }
    
    
    
    // Created because the scripts all instantiate at slightly different times and this needs to be read in separetely
    // Start method was not working properly
    private void asynchronousStart()
    {
        this.inputTerritoryNames = readFileInput("territoryNames.txt");
        this.inputRegionNames = readFileInput("regionNames.txt");
        this.usedTerritoryNames = new List<string>();
        this.usedRegionNames = new List<string>();
        filesRead = true;
    }


    public string generateTerritoryName()
    {
        
        if (!filesRead)
        {
            asynchronousStart();
        }

        System.Random r = new System.Random();
        string outputName = null;

        while (outputName == null) {
            int index = r.Next(0, inputTerritoryNames.Length - 1);

            string tempString = inputTerritoryNames[index];

            if (!usedTerritoryNames.Contains(tempString))
            {
                usedTerritoryNames.Add(tempString);
                return tempString;
            }
        }

        throw new System.Exception("Failed to generate territory name");
    }

    public string generateRegionName()
    {
        if (!filesRead)
        {
            asynchronousStart();
        }

        System.Random r = new System.Random();
        string outputName = null;

        while (outputName == null)
        {
            int index = r.Next(0, inputRegionNames.Length - 1);

            string tempString = inputTerritoryNames[index];

            if (!usedRegionNames.Contains(tempString))
            {
                usedRegionNames.Add(tempString);
                return tempString;
            }
        }

        throw new System.Exception("Failed to generate territory name");

    }



    private string[] readFileInput(string fileName)
    {

        string currDirectory = Directory.GetCurrentDirectory();
        string targetDirectory = currDirectory + "\\Assets\\GameAssests\\";
        targetDirectory = targetDirectory + fileName;

        string[] lines = System.IO.File.ReadAllLines(@targetDirectory);

        return lines;

    }


}
