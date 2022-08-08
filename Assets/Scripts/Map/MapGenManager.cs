using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MapGenManager : MonoBehaviour
{
    public static MapGenManager Instance { get; private set; }
    
    [Header("Perlin noise")]
    //[Range(4.0f, 20.0f)] [Tooltip("Change the amount of islands")] [SerializeField] 
    private float magnification = 7.0f;

    //[Range(30f, 50f)] [Tooltip("Change the average hikes")] [SerializeField]
    private float maxHikes = 50f;
    
    [Header("Voronoi noise")]
    //[SerializeField] 
    private int detalization;
    private int _tilesCount;
    private Vector2[] _focalPoints;
    private Color[] _pixelColors;
    [SerializeField] private Image outputImage;

    private int _mapHeight;
    private int _mapWidth;
    
    private int _biasX = 0;
    private int _biasY = 0;
    
    

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _biasX = Random.Range(-1_000_000, 1_000_000);
        _biasY = Random.Range(-1_000_000, 1_000_000);
    }

    public void InitializeGenerationDependencies()
    {
        magnification = DebugHelper.Instance.magnification;
        maxHikes = DebugHelper.Instance.maxHikes;
        detalization = DebugHelper.Instance.detalization;
        InitializeVoronoiDiagram();
        CheckValidOfPerlin();
    }

    public TerrainTileCode GetProceduralTileCode(int x, int y)
    {
        if (GetPerlinBinary(x, y))
            return GetTileCodeByVoronoi(x, y);
        if (_focalPoints.Contains(new Vector2(x, y)))
            _pixelColors[x + y * _mapWidth] = new Color(1, 0, 0);
        return TerrainTileCode.Ocean;
    }

    private bool GetPerlinBinary(int x, int y)
    {
        float raw = Mathf.PerlinNoise((x + _biasX) / magnification,
            (y + _biasY) / magnification);
        return Mathf.Clamp(raw, 0, 1) >= maxHikes/100f;
    }
    
    //Rework perlin noise if there are the lack of ocean tiles
    private void CheckValidOfPerlin()
    {
        if (GetValidInPercentage() < 15)
        {
            _biasX = Random.Range(-1_000_000, 1_000_000);
            _biasY = Random.Range(-1_000_000, 1_000_000);
            CheckValidOfPerlin();
        }
    }

    private int GetValidInPercentage()
    {
        int tileCount = _mapHeight * _mapWidth;
        int invalidTiles = 0;
        for (int x = 0; x < _mapWidth; x++)
        {
            for (int y = 0; y < _mapHeight; y++)
            {
                if (!GetPerlinBinary(x, y)) invalidTiles++;
            }
        }

        return (invalidTiles * 100) / tileCount;
    }

    private TerrainTileCode GetTileCodeByVoronoi(int x, int y)
    {
        float distance = float.MaxValue;
        int value = 0;

        for (int i = 0; i < detalization; i++)
        {
            if (Vector2.Distance(new Vector2(x, y), _focalPoints[i]) < distance)
            {
                distance = Vector2.Distance(new Vector2(x, y), _focalPoints[i]);
                value = i;
            }
        }
        
        AddPixelDetail(x, y, value % _tilesCount);
        return (TerrainTileCode)(value % _tilesCount);
    }

    private void InitializeVoronoiDiagram()
    {
        _mapHeight = MapData.Instance.GetMapHeight();
        _mapWidth = MapData.Instance.GetMapWidth();

        _pixelColors = new Color[_mapHeight * _mapWidth];
        _tilesCount = MapData.Instance.GetTilesCount() - 1; //because tile[0] - ocean
        _focalPoints = new Vector2[detalization];
        
        for (int i = 0; i < detalization; i++)
        {
            _focalPoints[i] = new Vector2(Random.Range(0, _mapWidth), Random.Range(0, _mapHeight));
        }
    }

    private void AddPixelDetail(int x, int y, int value)
    {
        if (_focalPoints.Contains(new Vector2(x, y)))
            _pixelColors[x + y * _mapWidth] = new Color(1, 0, 0);
        else
            _pixelColors[x + y * _mapWidth] = new Color((value + 1) / 2f, (value + 1) / 2f, (value + 1) / 2f);
    }

    public void ShowDebugDiagram()
    {
        Texture2D newTexture2D = new Texture2D(_mapWidth, _mapHeight);
        newTexture2D.SetPixels(_pixelColors);
        newTexture2D.Apply();
        outputImage.sprite = Sprite.Create(
            newTexture2D, 
            new Rect(0, 0, newTexture2D.width, newTexture2D.height),
            new Vector2(.5f, .5f));
        outputImage.transform.localScale = new Vector3(_mapWidth / 8f, _mapHeight / 8f, 1);
    }

    //Legacy
    /*w
    private void ConstructVoronoiNoise()
    {
        int mapHeight = MapData.Instance.GetMapHeight();
        int mapWidth = MapData.Instance.GetMapWidth();

        Vector2[] points = new Vector2[detalization];
        Color[] regionColors = new Color[regionColorAmount];

        //Main points
        for (int i = 0; i < detalization; i++)
        {
            points[i] = new Vector2(Random.Range(0, size), Random.Range(0, size));
        }

        //Tile count
        for (int i = 0; i < regionColorAmount; i++)
        {
            regionColors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f),1);
        }

        //We can calculate point color depends on perlin noise to boost performance
        //Matrix of colors
        Color[] pixelColors = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float distance = float.MaxValue;
                int value = 0;

                for (int i = 0; i < detalization; i++)
                {
                    if (Vector2.Distance(new Vector2(x, y), points[i]) < distance)
                    {
                        distance = Vector2.Distance(new Vector2(x, y), points[i]);
                        value = i;
                    }
                }

                pixelColors[x + y * size] = regionColors[value % regionColorAmount];
             }
        }
        
        //DebugHelper.Instance.ShowList(pixelColors);

        Texture2D newTexture2D = new Texture2D(size, size);
        newTexture2D.SetPixels(pixelColors);
        newTexture2D.Apply();
        outputQuad.GetComponent<Renderer>().material.mainTexture = newTexture2D;
        // output2D.GetComponent<SpriteRenderer>().sprite = Sprite.Create(newTexture2D,
        //     new Rect(0, 0, newTexture2D.width, newTexture2D.height), new Vector2(.5f, .5f));
    } */
    
    //Legacy
    /*
    private TerrainTileCode GetTileCodeByPerlin(int x, int y)
    {
        float raw = Mathf.PerlinNoise((x + _biasX) / magnification, 
            (y + _biasY) / magnification);
        float clamp = Mathf.Clamp(raw, 0.0f, .99f);
        float scale = clamp * MapData.Instance.GetTilesCount();
        return (TerrainTileCode)Mathf.Floor(scale);
    } */
}