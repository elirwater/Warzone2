/**
 * Class responsible for generating the noise map used by our map generation class
 */
public class NoiseGenerator
{
    
    private int height;
    private int width;
    private int smoothIterations;
    private int randomFillPercent;
    private string seed;
    private int[,] map;

    public NoiseGenerator(int height, int width, int smoothIterations, int randomFillPercent, string seed)
    {
        this.height = height;
        this.width = width;
        this.smoothIterations = smoothIterations;
        this.randomFillPercent = randomFillPercent;
        this.seed = seed;
    }


    /**
     * Generates the initial pixel noise map
     */
    public Pixel[,] generatePixelNoiseMap()
    {
        GenerateMap();
        
        Pixel[,] pixelMap = new Pixel[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Pixel p = new Pixel(map[i, j]);

                if (map[i,j] == 1)
                {
                    p.isOcean = true;
                }

                pixelMap[i, j] = p;

            }
        }
        return pixelMap;
    }
    
    /**
     * Fills the map based on the smoothing function
     */
    void GenerateMap()
    {
        map = new int[width, height];
        RandomFillMap();

        for (int i = 0; i < smoothIterations; i++)
        {
            SmoothMap();
        }
    }
    
    /**
     * Function that populates the map depth
     */
    void RandomFillMap()
    {

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
    
    /**
     * Smooths the noise map
     */
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

    /**
     * Finds surrounding walls
     */
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
}
