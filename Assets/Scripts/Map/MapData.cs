using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapData : MonoBehaviour
{
    public static MapData Instance { get; private set; }

    [Header("Tilemaps")]
    public Tilemap baseMap;
    public Tilemap buildingMap;

    [Header("Map settings")]
    [SerializeField] private int mapHeight;
    [SerializeField] private int mapWidth;

    [Header("Tiles")] [Tooltip("Change also enum after adding new tiles!")]
    [SerializeField] private List<TileBase> landscapeTiles;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        InitializePublicArrays();
    }
    
    //--------------------Public arrays--------------------//
    private int[,] _tilesArray;
    public TileCode GetTile(Vector2Int position)
    {
        return (TileCode)_tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]];
    }
    public void SetTile(Vector2Int position, TileCode tileCode)
    {
        _tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]] = (int)tileCode;
    }
    
    private UnitController[,] _unitsArray;
    public UnitController GetUnit(Vector2Int position)
    {
        return _unitsArray[position[0], _unitsArray.GetLength(1)-1 - position[1]];
    }
    public void SetUnit(Vector2Int position, UnitController unit)
    {
        Vector2Int curPos = unit.currentUnitTile;
        _unitsArray[curPos[0], _unitsArray.GetLength(1)-1 - curPos[1]] = null;
        _unitsArray[position[0], _unitsArray.GetLength(1)-1 - position[1]] = unit;
        unit.currentUnitTile = position;
    }
    
    private BuildingController[,] _buildingsArray;
    public BuildingController GetBuilding(Vector2Int position)
    {
        return _buildingsArray[position[0], _buildingsArray.GetLength(1)-1 - position[1]];
    }
    public void SetBuilding(Vector2Int position, BuildingController building)
    {
        _buildingsArray[position[0], _buildingsArray.GetLength(1)-1 - position[1]] = building;
    }
    //------------------Public arrays end------------------//

    private void InitializePublicArrays()
    {
        _tilesArray = new int[mapWidth, mapHeight];
        _unitsArray = new UnitController[mapWidth, mapHeight];
        _buildingsArray = new BuildingController[mapWidth, mapHeight];
    }

    public int GetMapWidth()
    {
        return mapWidth;
    }

    public int GetMapHeight()
    {
        return mapHeight;
    }

    public int GetTilesCount()
    {
        return landscapeTiles.Count;
    }

    public TileBase GetTileBase(TileCode tileCode)
    {
        return landscapeTiles[(int) tileCode];
    }
}
