using System;
using UnityEngine;
using Random = UnityEngine.Random;

/**
 * Class responsible for rendering the map using Unity Gizmos
 */
public class MapRendering : MonoBehaviour
{
    
    private static Pixel[,] pixelMap;
    private static int width;
    private static int height;
    private string targetedTerritoryName;
    private string targetedRegionName;
    private MapGeneration mapGenerationClass;
    
    
    private void Start()
    {
        Controller.MapGenerationData mapGenerationInputData = FindObjectOfType<Controller>().mapGenerationData;
        mapGenerationClass = FindObjectOfType<MapGeneration>();
        width = mapGenerationInputData.width;
        height = mapGenerationInputData.height;
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
    }

    /**
     * Draws our gizmos for each pixel in the map
     */
    void OnDrawGizmos()
    {
        if (pixelMap != null && Application.isPlaying)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Pixel currentPixel = pixelMap[i, j];

                    //placeholder color to instantiate the variable
                    Color currentColor = Color.red;
                    
                    if (currentPixel.value != 1)
                    {
                        int territorySeed = mapGenerationClass.findTerritoryByName(currentPixel.territoryName).occupier.GetHashCode();
                        Random.seed = territorySeed;

                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        float b = Random.Range(0f, 1f);

                        currentColor = new Color(r, g, b);
                    }

                    
                    if (currentPixel.value == 1)
                    {
                        currentColor = new Color(0.353f, 0.733f, 0.812f);
                    }
                    
                    if (currentPixel.isBorder)
                    {
                        currentColor = Color.black;
                    }
                    
                    if (currentPixel.territoryName == targetedTerritoryName)
                    {
                        currentColor = currentColor * 2;
                    }
                    if (currentPixel.regionName == targetedRegionName)
                    {
                        currentColor = currentColor * 2;
                    }
                    
                    Gizmos.color = currentColor;
                    Vector3 pos = new Vector3(-width / 2 + i + .5f, -10, -height / 2 + j + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
