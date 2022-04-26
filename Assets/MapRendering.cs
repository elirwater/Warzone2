using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class MapRendering : MonoBehaviour{
    
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

            targetedTerritoryName = pixelMap[mapX, mapY].territoryName;
            targetedRegionName = pixelMap[mapX, mapY].regionName;

            Territories targetTerritory = Component.FindObjectOfType<MapGeneration>().findTerritoryByName(targetedTerritoryName);

            Regions territoryRegion = Component.FindObjectOfType<MapGeneration>()
                .findRegionsByName(targetTerritory.regionName);

            Component.FindObjectOfType<InfoPopup>().displayTerritoryInfo(targetedTerritoryName, targetTerritory.getOccupier(),
                targetTerritory.regionName, territoryRegion.regionalBonusValue, targetTerritory.neighbors.Count, targetTerritory.armies);
            
            FindObjectOfType<DisplayTerritoryInfo>().spawnTextFromMousePosition(worldPosition.x, worldPosition.z);
            
        }
    }



    public void updateMap(Pixel[,] inputPixelMap)
    {
        pixelMap = inputPixelMap;
    }



    //TODO: this is getting called too much (140 * 60 / s) -> not great
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


                    // :( old colorful code for region colors
                    // if (currentPixel.territoryName != null)
                    // {
                    //     string currentPixelsTerritoryName = pixelMap[i, j].territoryName;
                    //     Territories currentPixelsTerritory = Component.FindObjectOfType<MapGeneration>().findTerritoryByName(currentPixelsTerritoryName);
                    //     if (currentPixelsTerritory.regionName != null)
                    //     {
                    //         int territorySeed = currentPixelsTerritory.regionName.GetHashCode();
                    //         Random.seed = territorySeed;
                    //
                    //         float r = Random.Range(0f, 1f);
                    //         float g = Random.Range(0f, 1f);
                    //         float b = Random.Range(0f, 1f);
                    //
                    //         currentColor = new Color(r, g, b);
                    //     }
                    //
                    // }

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
