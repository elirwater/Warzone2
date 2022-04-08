﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour{


    private int numTerritories;
    private int numRegions;
    private int pointsPerTerritory;
    private int height;
    private int width;
    private int smoothIterations;
    [Range(0, 100)] private int randomFillPercent;
    private string seed;


    private Pixel[,] pixelMap;
    private List<Territories> allTerritories;
    private List<Regions> allRegions;

    void Start()
    {
        Controller.MapGenerationData mapGenerationInputData = FindObjectOfType<Controller>().mapGenerationData;
        
        numTerritories = mapGenerationInputData.numTerritories;
        numRegions = mapGenerationInputData.numRegions;
        width = mapGenerationInputData.width;
        height = mapGenerationInputData.height;
        pointsPerTerritory = mapGenerationInputData.pointsPerTerritory;     
        smoothIterations = mapGenerationInputData.smoothIterations;
        randomFillPercent = mapGenerationInputData.randomFillPercent;
        seed = mapGenerationInputData.seed;

        

        if (numTerritories < numRegions)
        {
            throw new Exception("Must specify more territories than regions");
        }


        NoiseGenerator noise = new NoiseGenerator(height, width, smoothIterations, randomFillPercent, seed);
        pixelMap = noise.generatePixelNoiseMap();

        allTerritories = new List<Territories>();
        allRegions = new List<Regions>();


        generateTerritories();
        generateTerritoryBordersAndNeighbors();
        generateRegions();
        calculateRegionalBonusValues();
        instantiateRegionNamesForPixels();


    }



    public Pixel[,] grabMapForRendering()
    {
        return pixelMap;
    }

    public List<Territories> grabMapForAgents()
    {
        return null;
    }



    // Generates all territories for this map
    private void generateTerritories()
    {

        System.Random r = new System.Random(seed.GetHashCode());

        for (int i = 0; i < numTerritories; i++)
        {
            // Generate random territory center
            int rx = r.Next(0, width);
            int ry = r.Next(0, height);
            Vector2 centerCord = new Vector2(rx, ry);



            string randomName = Component.FindObjectOfType<NameGenerator>().generateTerritoryName();
            Territories newTerritory = new Territories(centerCord, randomName);
            allTerritories.Add(newTerritory);
            
        }


        // Populating the pixelMap with our territories by iterating through all pixels and assigning the current pixels territory to be the closest territory center
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Pixel currentPixel = pixelMap[i, j];
                if (currentPixel.value == 0)
                {
                    int closest = int.MaxValue;
                    Territories closestTerritory = null;

                    foreach (Territories territory in allTerritories)
                    {
                        Vector2 centerCord = territory.centerCord;

                        int distance =
                            (int) Math.Sqrt((Math.Pow((i - centerCord.x), 2) + Math.Pow((j - centerCord.y), 2)));
                        if (distance <= closest)
                        {
                            closest = distance;
                            closestTerritory = territory;
                        }
                    }

                    if (closestTerritory == null || closest == int.MaxValue)
                    {
                        throw new Exception("Failed to properly generate territorial pixel");
                    }

                    currentPixel.territoryName = closestTerritory.territoryName;
                }
            }
        }
    }

    private void generateTerritoryBordersAndNeighbors()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Pixel currentPixel = pixelMap[i, j];

                if (!currentPixel.isOcean)
                {

                    string currentTerritoryName = currentPixel.territoryName;
                    Territories currentTerritory = findTerritoryByName(currentTerritoryName);


                    if (i + 1 < width)
                    {
                        if (pixelMap[i + 1, j].territoryName != currentTerritoryName && !pixelMap[i + 1, j].isOcean)
                        {
                            currentPixel.isBorder = true;
                            Territories t = findTerritoryByName(pixelMap[i + 1, j].territoryName);
                            currentTerritory.addNeighbor(t);
                        }

                        if (pixelMap[i + 1, j].isOcean)
                        {
                            currentPixel.isBorder = true;
                        }
                    }

                    if (j + 1 < height)
                    {
                        if (pixelMap[i, j + 1].territoryName != currentTerritoryName && !pixelMap[i, j + 1].isOcean)
                        {
                            currentPixel.isBorder = true;
                            Territories t = findTerritoryByName(pixelMap[i, j + 1].territoryName);
                            currentTerritory.addNeighbor(t);
                        }

                        if (pixelMap[i, j + 1].isOcean)
                        {
                            currentPixel.isBorder = true;
                        }
                    }

                    if (i - 1 >= 0)
                    {
                        if (pixelMap[i - 1, j].territoryName != currentTerritoryName && !pixelMap[i - 1, j].isOcean)
                        {
                            currentPixel.isBorder = true;
                            Territories t = findTerritoryByName(pixelMap[i - 1, j].territoryName);
                            currentTerritory.addNeighbor(t);
                        }

                        if (pixelMap[i - 1, j].isOcean)
                        {
                            currentPixel.isBorder = true;
                        }
                    }

                    if (j - 1 >= 0)
                    {
                        if (pixelMap[i, j - 1].territoryName != currentTerritoryName && !pixelMap[i, j - 1].isOcean)
                        {
                            currentPixel.isBorder = true;
                            Territories t = findTerritoryByName(pixelMap[i, j - 1].territoryName);
                            currentTerritory.addNeighbor(t);
                        }

                        if (pixelMap[i, j - 1].isOcean)
                        {
                            currentPixel.isBorder = true;
                        }
                    }
                }
            }
        }

    }



    private void generateRegions()
    {

        List<Territories> territoriesInRegions = new List<Territories>();


        System.Random r = new System.Random(seed.GetHashCode());
        List<Territories> frontline = new List<Territories>();
        List<int> fetchedIndexes = new List<int>();





        // Setting spawn territory for each new Region Center
        for (int i = 0; i < numRegions; i++)
        {

            int index = -1;
            while (index == -1)
            {
                int tempIndex = UnityEngine.Random.Range(0, (allTerritories.Count - 1));
                if (!fetchedIndexes.Contains(tempIndex))
                {
                    index = tempIndex;
                    fetchedIndexes.Add(tempIndex);

                    frontline.Add(allTerritories[index]);

                    string randomName = Component.FindObjectOfType<NameGenerator>().generateRegionName();
                    allTerritories[index].regionName = randomName;

                    // Adds our center spawn territory to a new region (and instantiates new region)
                    Regions newRegion = new Regions(randomName, 0); //Not great way to do this, should be a NULL value 
                    newRegion.territories.Add(allTerritories[index]);
                    allRegions.Add(newRegion);

                    territoriesInRegions.Add(allTerritories[index]);

                }
            }
        }




        // Propagates each region center outwards to include additional territories until all Territories are in a region
        while (territoriesInRegions.Count < allTerritories.Count)
        {
            List<Territories> tempFrontline = new List<Territories>();

            foreach (Territories territory in frontline)
            {
                foreach (Territories neighbor in territory.neighbors)
                {
                    if (neighbor.regionName == null)
                    {
                        neighbor.regionName = territory.regionName;
                        tempFrontline.Add(neighbor);
                        territoriesInRegions.Add(neighbor);

                        // Add it to our regional List TODO: replace this with a predicate find function that searches by name, too much looping
                        foreach (Regions region in allRegions)
                        {
                            if (region.regionName == neighbor.regionName)
                            {
                                region.territories.Add((territory));
                            }
                        }

                    }
                }

            }


            frontline = tempFrontline;

        }
    }
    
    
    // Used by the rendering class, avoids excessive cross-script calls
    private void instantiateRegionNamesForPixels()
    {
        foreach (Pixel p in pixelMap)
        {
            if (p.territoryName != null)
            {
                p.regionName = findTerritoryByName(p.territoryName).regionName;   
            }
        }
    }
    



    // Returns the territory based on the name passed in - instead of storing each territory in each pixel, we are just storing the name so we have to look it up
    // This is more space efficient and avoids mutation issues if multiple pixels try to modify one territory instance simaltaneously
    public Territories findTerritoryByName(string territoryName)
    {
        foreach(Territories territory in allTerritories)
        {
            if (territory.territoryName == territoryName)
            {
                return territory;
            }
        }
        throw new Exception("For some reason territory could not be located");

    }
    
    

    public Regions findRegionsByName(string regionName)
    {
        foreach(Regions region in allRegions)
        {
            if (region.regionName == regionName)
            {
                return region;
            }
        }
        throw new Exception("For some reason region could not be located");

    }



    public List<Territories> getTerritories()
    {
        return allTerritories;
    }


    public List<Regions> getRegions()
    {
        return allRegions;
    }

        //Not sure about this one lol, just generates the value for each region based on the number of territories in it
    private void calculateRegionalBonusValues()
    {
        foreach (Regions region in allRegions)
        {
            region.regionalBonusValue = region.territories.Count * pointsPerTerritory;
        }
    }

}
