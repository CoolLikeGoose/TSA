using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MapGenManager : MonoBehaviour
{
    public static MapGenManager Instance { get; private set; }
    
    [Range(4.0f, 20.0f)]
    [SerializeField] private float magnification = 7.0f;

    private int _biasX = 0;
    private int _biasY = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _biasX = Random.Range(-1_000_000, 1_000_000);
        _biasY = Random.Range(-1_000_000, 1_000_000);
    }

    public TerrainTileCode GetTileCodeByNoise(int x, int y)
    {
        float raw = Mathf.PerlinNoise((x + _biasX) / magnification, 
            (y + _biasY) / magnification);
        float clamp = Mathf.Clamp(raw, 0.0f, .99f);
        float scale = clamp * MapData.Instance.GetTilesCount();
        return (TerrainTileCode)Mathf.Floor(scale);
    }
}