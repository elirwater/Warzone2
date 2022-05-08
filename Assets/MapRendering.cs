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
    private static int mapWidth;
    private static int mapHeight;

    private static double textureWidth;
    private static double textureHeight;
    private static double mapWidthToTextureWidthRatio;
    private static double mapHeightToTextureHeightRatio;

    private static int textureToScreenSizeScalingFactor;
    
    private string targetedTerritoryName;
    private string targetedRegionName;
    private MapGeneration mapGenerationClass;
    
    private Texture2D textureMap;

    private void Start()
    {
        Controller.MapGenerationData mapGenerationInputData = FindObjectOfType<Controller>().mapGenerationData;
        mapGenerationClass = FindObjectOfType<MapGeneration>();
        mapWidth = mapGenerationInputData.width;
        mapHeight = mapGenerationInputData.height;
        
        // NOTE: arbitrarily choosing 3 so there is room to display additional info on the sides of the game window
        textureToScreenSizeScalingFactor = 3;

        textureWidth = (double) Screen.width / textureToScreenSizeScalingFactor;
        textureHeight = (double) Screen.height / textureToScreenSizeScalingFactor;
        

        // Texture is originally set to be the resolution of the game window, then a scaling factor is applied
        textureMap = new Texture2D((int) textureWidth, (int)textureHeight, TextureFormat.RGB565, true);

        transform.localScale = new Vector3((float) (textureMap.width), (float) (textureMap.height), 1);
        transform.localPosition = Camera.main.ScreenToViewportPoint(Vector3.one * 0.5f);
        
        
        textureWidth = textureMap.width;
        textureHeight = textureMap.height;
        
        mapWidthToTextureWidthRatio = textureWidth / mapWidth;
        mapHeightToTextureHeightRatio = textureHeight / mapHeight;
        
        GetComponent<MeshRenderer>().material.mainTexture = textureMap;
        GetComponent<MeshRenderer>().material.shader = Shader.Find("Unlit/Texture");
    }
    
    
    /**
     * Creates the texture map on game start of our game map
     */
    public void renderMapOnGameStart()
    {
        
        print(mapWidthToTextureWidthRatio);
        print(mapHeightToTextureHeightRatio);
        
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                Pixel currentPixel = pixelMap[i, j];
                
                Color color = findMapPixelColor(currentPixel);
                
                // Uses chunking system based on texture dimensions to update multiple texture pixels for 1 map pixel
                for (int x = 0; x < mapWidthToTextureWidthRatio; x++)
                {
                    for (int y = 0; y < mapHeightToTextureHeightRatio; y++)
                    {
                        textureMap.SetPixel((int) (x + (i * mapWidthToTextureWidthRatio)), 
                            (int) (y + (j * mapHeightToTextureHeightRatio)), color);
                    }
                }
            }
        }
        textureMap.Apply();
    }

    
    /**
     * Updates the map texture based on which territories were modified to save computational resources (instead
     * of iterating through a massive array of pixels when most of them remain unchanged)
     */
    public void renderMapByModifiedTerritories(List<string> territoriesToBeUpdated)
    {
        foreach (string territoryName in territoriesToBeUpdated)
        {
            List<(int, int)> mapPixelsToBeUpdates = mapGenerationClass.getPixelsByTerritory(territoryName);

            foreach ((int, int) mapPixel in mapPixelsToBeUpdates)
            {
                Pixel p = pixelMap[mapPixel.Item1, mapPixel.Item2];
                Color color = findMapPixelColor(p);
                
                // Uses chunking system based on texture dimensions to update multiple texture pixels for 1 map pixel
                for (int i = 0; i < mapWidthToTextureWidthRatio; i++)
                {
                    for (int j = 0; j < mapHeightToTextureHeightRatio; j++)
                    {
                        textureMap.SetPixel((int) (i + (mapPixel.Item1 * mapWidthToTextureWidthRatio)), 
                            (int) (j + (mapPixel.Item2 * mapHeightToTextureHeightRatio)), color);
                    }
                }
            }
        }
        textureMap.Apply();
    }

    
    /**
     * Determines the color of a given map pixel
     */
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

            GameObject o = GameObject.Find("MapTextureQuad");
            RectTransform rt = o.GetComponent<RectTransform>();
            
            int mapX = 0;
            int mapY = 0;
            
            
            if (worldPosition.x >= 0)
            {
                mapX = (int)((worldPosition.x + textureWidth / 2) / mapWidthToTextureWidthRatio);
            }
            if (worldPosition.x < 0)
            {
                mapX = (int)(((textureWidth / 2) + worldPosition.x) / mapWidthToTextureWidthRatio);
            }
            if (worldPosition.z >= 0)
            {
                mapY = (int)((worldPosition.z + textureHeight / 2) / mapHeightToTextureHeightRatio);
            }
            if (worldPosition.z < 0)
            {
                mapY = (int)(((textureHeight / 2) + worldPosition.z) / mapHeightToTextureHeightRatio);
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
            
            renderMapOnGameStart();
        }
    }

    private void selectTerritoryAndRegion(Vector3 mouseWorldPosition)
    {
        
    }


    /**
     * Called by the controller to update the pixel map
     */
    public void updatePixelMap(Pixel[,] inputPixelMap)
    {
        pixelMap = inputPixelMap;
    }
    
}
