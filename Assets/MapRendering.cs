using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * Class responsible for rendering the map using Unity Gizmos
 */
public class MapRendering : MonoBehaviour{
    public GameObject test;

    private static Pixel[,] pixelMap;
    private static int width;
    private static int height;
    private string targetedTerritoryName;
    private string targetedRegionName;
    private MapGeneration mapGenerationClass;


    private Texture2D texture;


    //TODO: SOLUTION -> JUST STORE a tuple of cords that make up each territory and only update it based on that

    private void Start()
    {
        Controller.MapGenerationData mapGenerationInputData = FindObjectOfType<Controller>().mapGenerationData;
        mapGenerationClass = FindObjectOfType<MapGeneration>();
        width = mapGenerationInputData.width;
        height = mapGenerationInputData.height;




        texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB565, true);
        texture.name = "Procedural Texture";

        var scale = (Screen.height / 2.0) / Camera.main.orthographicSize;

        transform.localScale = new Vector3((float) (texture.width) / 3, (float) (texture.height) / 3, 1);


        GetComponent<MeshRenderer>().material.mainTexture = texture;
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");



        //texture.Apply();


        //float xTile = Screen.width / T1.width, yTile = Screen.height / T1.height;


       // Invoke("FillTexture", 5f);




        //FillTexture2();

    }



    private void FillTexture2()
    {
        int resWidth = Screen.width - 1;
        int resHeight = Screen.height - 1;


        for (int y = 0; y < resWidth; y++)
        {
            for (int x = 0; x < resHeight; x++)
            {
                texture.SetPixel(x, y, Color.red);
            }
        }

        texture.Apply();
    }

    
    // Only runs on instantiation

    private void FillTexture()
    {


        //print(Screen.width);
        //print(Screen.height);

        // (int)Math.Ceiling((double)nItems / (double)nItemsPerPage);

        double adjustedWidth = (double) texture.width / width;
        double adjustedHeight = (double) texture.height / height;


        //print(adjustedWidth);
        //print(adjustedHeight);





        //float xTile = Screen.width / T1.width, yTile = Screen.height / T1.height;!!!!!!!!!!!!!!!!


        // TODO: need a way to lower the resolution
        
        // TODO: figure out a way to do this in chunks (combine both methods)

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {

                int pixelX = (int) (i / adjustedWidth);
                int pixelY = (int) (j / adjustedHeight);




                Pixel currentPixel = pixelMap[i, j];

                //placeholder color to instantiate the variable
                Color color = findMapPixelColor(currentPixel);
                
                for (int x = 0; x < adjustedWidth; x++)
                {
                    for (int y = 0; y < adjustedHeight; y++)
                    {
                        texture.SetPixel((int) (x + (i * adjustedWidth)), (int) (y + (j * adjustedHeight)), color);
                    }
                }
            }
        }

        texture.Apply();
    }


    public void updateByTerritory(List<string> territoriesToBeUpdated)
    {
        double adjustedWidth = (double) texture.width / width;
        double adjustedHeight = (double) texture.height / height;
        
        
        foreach (string territoryName in territoriesToBeUpdated)
        {
            List<(int, int)> mapPixelsToBeUpdates = mapGenerationClass.getPixelsByTerritory(territoryName);

            foreach ((int, int) mapPixel in mapPixelsToBeUpdates)
            {
                Pixel p = pixelMap[mapPixel.Item1, mapPixel.Item2];
                Color color = findMapPixelColor(p);
                
                //now for the chunking part, updating the chunk of pixels by the adjusted texture size
                for (int i = 0; i < adjustedWidth; i++)
                {
                    for (int j = 0; j < adjustedHeight; j++)
                    {
                        // Updates the map in chuncks based on the original texture size
                        texture.SetPixel((int) (i + (mapPixel.Item1 * adjustedWidth)), (int) (j + (mapPixel.Item2 * adjustedHeight)), color);
                    }
                }
                
            }
        }
        texture.Apply();
    }




    private Color findMapPixelColor(Pixel p)
    {
        Color currentColor = Color.red;

        if (p.value != 1)
        {
            int territorySeed = mapGenerationClass.findTerritoryByName(p.territoryName).occupier
                .GetHashCode();
            Random.seed = territorySeed;

            float r = Random.Range(0f, 1f);
            float g = Random.Range(0f, 1f);
            float b = Random.Range(0f, 1f);

            currentColor = new Color(r, g, b);
        }


        if (p.value == 1)
        {
            currentColor = new Color(0.353f, 0.733f, 0.812f);
        }

        if (p.isBorder)
        {
            currentColor = Color.black;
        }

        if (p.territoryName == targetedTerritoryName)
        {
            currentColor = currentColor * 2;
        }

        if (p.regionName == targetedRegionName)
        {
            currentColor = currentColor * 2;
        }

        return currentColor;
    }







private void Update()
    {
        //Checks if we have clicked on a given territory
        if (Input.GetMouseButtonDown(0) && pixelMap != null)
        {
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            int mapX = 0;
            int mapY = 0;
            

            if (worldPosition.x >= 0)
            {
                mapX = (int)(worldPosition.x + width / 2);
            }
            if (worldPosition.x < 0)
            {
                mapX = (int)((width / 2) + worldPosition.x);
            }
            if (worldPosition.z >= 0)
            {
                mapY = (int)(worldPosition.z + height / 2);
            }
            if (worldPosition.z < 0)
            {
                mapY = (int)((height / 2) + worldPosition.z);
            }

   
            // Click outside of map to unselect a territory/region
            try
            {
                targetedTerritoryName = pixelMap[mapX, mapY].territoryName;
                targetedRegionName = pixelMap[mapX, mapY].regionName;
                
                Territories targetTerritory = Component.FindObjectOfType<MapGeneration>().findTerritoryByName(targetedTerritoryName);

                Regions territoryRegion = Component.FindObjectOfType<MapGeneration>()
                    .findRegionsByName(targetTerritory.regionName);

                Component.FindObjectOfType<InfoPopup>().displayTerritoryInfo(targetedTerritoryName, targetTerritory.getOccupier(),
                    targetTerritory.regionName, territoryRegion.regionalBonusValue, targetTerritory.neighbors.Count, targetTerritory.armies);
            }
            catch (IndexOutOfRangeException e)
            {
                targetedRegionName = "";
                targetedTerritoryName = "";
            }
        }
    }

    /**
     * Called by the controller to update the pixel map
     */
    public void updateMap(Pixel[,] inputPixelMap)
    {
        pixelMap = inputPixelMap;
        FillTexture();
    }
    
    
    public void updateMap2(Pixel[,] inputPixelMap, List<string> territoriesToBeUpdated)
    {
        pixelMap = inputPixelMap;
        updateByTerritory(territoriesToBeUpdated);
    }

    /**
     * Draws our gizmos for each pixel in the map
     */
    // void OnDrawGizmos()
    // {
    //     if (pixelMap != null && Application.isPlaying)
    //     {
    //         for (int i = 0; i < width; i++)
    //         {
    //             for (int j = 0; j < height; j++)
    //             {
    //                 Pixel currentPixel = pixelMap[i, j];
    //
    //                 //placeholder color to instantiate the variable
    //                 Color currentColor = Color.red;
    //                 
    //                 if (currentPixel.value != 1)
    //                 {
    //                     int territorySeed = mapGenerationClass.findTerritoryByName(currentPixel.territoryName).occupier.GetHashCode();
    //                     Random.seed = territorySeed;
    //
    //                     float r = Random.Range(0f, 1f);
    //                     float g = Random.Range(0f, 1f);
    //                     float b = Random.Range(0f, 1f);
    //
    //                     currentColor = new Color(r, g, b);
    //                 }
    //
    //                 
    //                 if (currentPixel.value == 1)
    //                 {
    //                     currentColor = new Color(0.353f, 0.733f, 0.812f);
    //                 }
    //                 
    //                 if (currentPixel.isBorder)
    //                 {
    //                     currentColor = Color.black;
    //                 }
    //                 
    //                 if (currentPixel.territoryName == targetedTerritoryName)
    //                 {
    //                     currentColor = currentColor * 2;
    //                 }
    //                 if (currentPixel.regionName == targetedRegionName)
    //                 {
    //                     currentColor = currentColor * 2;
    //                 }
    //                 
    //                 
    //                 Gizmos.color = currentColor;
    //                 Vector3 pos = new Vector3(-width / 2 + i + .5f, 0, -height / 2 + j + .5f);
    //
    //                 
    //                 //Instantiate(test, pos, Quaternion.identity, test.transform);
    //                 
    //                 
    //                 Gizmos.DrawCube(pos, Vector3.one);
    //                 
    //             }
    //         }
    //     }
    // }
}
