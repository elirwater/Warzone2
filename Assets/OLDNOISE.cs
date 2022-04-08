using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class OLDNOISE : MonoBehaviour{

    public int width;
    public int height;

    public string seed;
    public bool useRandomSeed;

    public string territorySeed;

    public int territories;




    public int smoothIterations;



    [Range(0, 100)] public int randomFillPercent;



    private int[,] map;



    private List<Vector3> _territoryCenters = new List<Vector3>();
    private List<Vector3> _regionCenters = new List<Vector3>();
    private List<Regions> _regions = new List<Regions>();
    private List<Territories> _territories = new List<Territories>();
    private int currentRegion = 0;
    private int currentTerritory = 0;


    private Vector2[,] map2;
    //vect.x = territory
    //vect.y = region


    void Start()
    {
        GenerateMap();
        populateTerritoryAndRegionCenters(10, 200);
        setRegions();
    }

    void GenerateMap()
    {
        map = new int[width, height];
        map2 = new Vector2[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }
    }

    void RandomFillMap()
    {
        if (useRandomSeed)
        {
            seed = Time.time.ToString();
        }

        System.Random prng = new System.Random(seed.GetHashCode());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1;
                }

                map[x, y] = (prng.Next(0, 100) < randomFillPercent) ? 1 : 0;
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighbourWallTiles = GetSurroundingWallCount(x, y);
                if (neighbourWallTiles > 4)
                    map[x, y] = 1;
                else if (neighbourWallTiles < 4)
                    map[x, y] = 0;
            }
        }
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
        {
            for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                {
                    if (neighbourX != gridX || neighbourY != gridY)
                    {
                        wallCount += map[neighbourX, neighbourY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }

        return wallCount;
    }

    

    // FOR NOW, territories and regions will be disjoint in number, meaning one region can have far more territories
    // And then the number of points that regions is worth should correspond to the number of territories within it
    // currentRegion and currentTerritory are serving as names

    void populateTerritoryAndRegionCenters(int numRegions, int numTerritories)
    {
        System.Random r = new System.Random(territorySeed.GetHashCode());

        for (int i = numRegions; i < width; i++)
        {
            while (true)
            {
                int rx = r.Next(0, width);
                int ry = r.Next(0, height);
                Vector3 regionCenterCandidate = new Vector3(rx, ry, currentRegion);

                if (!_regionCenters.Contains(regionCenterCandidate))
                {
                    _regionCenters.Add(regionCenterCandidate);
                    currentRegion += 1;
                    break;
                }
            }
        }

        for (int j = 0; j < numTerritories; j++)
        {
            while (true)
            {
                int tx = r.Next(0, width);
                int ty = r.Next(0, height);
                Vector3 regionCenterCandidate = new Vector3(tx, ty, currentTerritory);
                if (!_territoryCenters.Contains(regionCenterCandidate))
                {
                    currentTerritory += 1;
                    _territoryCenters.Add(regionCenterCandidate);
                    break;
                }
            }
        }
    }

    //TODO: MAKE SURE TO CHECK THAT CLOSEST TERRITORY CENTER IS IN SAME REGION
    
    //TODO: make sure not conflicitng with walls/ocean in original noise map 
    void setRegions()
    {

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j] != 1)
                {
                    if (map[i, j] == 0)
                    {
                        Vector2 regionAndTerritory = new Vector2();
                        int closest = int.MaxValue;
                        Vector3 closestRegionCenter = new Vector3();
                        foreach (var regionCenter in _regionCenters)
                        {

                            int distance = (int) Math.Sqrt((Math.Pow((i - regionCenter.x), 2) +
                                                            Math.Pow((j - regionCenter.y), 2)));
                            if (distance <= closest)
                            {
                                closest = distance;
                                closestRegionCenter = regionCenter;
                            }
                        }
                        
                        
                        // TODO: need to make sure territories are assigned in the same region idiot

                        regionAndTerritory.x = closestRegionCenter.z;

                        int closest2 = int.MaxValue;
                        Vector3 closestTerritoryCenter = new Vector3();
                        foreach (var territoryCenter in _territoryCenters)
                        {


                            int distance = (int) Math.Sqrt((Math.Pow((i - territoryCenter.x), 2) +
                                                            Math.Pow((j - territoryCenter.y), 2)));
                            if (distance <= closest2)
                            {
                                closest2 = distance;
                                closestTerritoryCenter = territoryCenter;
                            }
                        }

                        regionAndTerritory.y = closestTerritoryCenter.z;

                        map2[i, j] = regionAndTerritory;

                    }
                }
            }
        }

    }



    //TODO: this is getting called too much (140 * 60 / s) -> not great
    void OnDrawGizmos()
    {
    

        if (map2 != null)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {

                    if (map[i, j] != 1)
                    {
                        
                        int c = (int) map2[i, j].x;
                        int d = (int) map2[i, j].y;
                    
                        Random.seed = c;

                        float r = Random.Range(0f, 1f);
                        float g = Random.Range(0f, 1f);
                        
                        Random.seed = d;

                        float b = Random.Range(0f, 1f);
                        
                        Gizmos.color = new Color( r, g,b);

                    }
                    else
                    {
                        Gizmos.color = new Color(0.353f, 0.733f, 0.812f);
                    }
                    
                    Vector3 pos = new Vector3(-width / 2 + i + .5f, 0, -height / 2 + j + .5f);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }

        }
    }
    
    
    
    //TODO: need to put constraints on the actual territory propogation so they stay within a given region
    //TODO: BIG ISSUE WIH REGION RENDERING, NUM REGIONS NOT AFFECTED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
}

    
