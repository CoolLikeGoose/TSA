using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapData : MonoBehaviour
{
    public static MapData Instance { get; private set; }

    [Header("Tilemaps")]
    public Tilemap baseMap;
    public Tilemap buildingMap;
    public Tilemap lightUpMap;

    [Header("Map settings")]
    [SerializeField] private int mapHeight;
    [SerializeField] private int mapWidth;

    [Header("Tiles")] [Tooltip("Change also enum after adding new tiles!")]
    [SerializeField] private List<TileBase> landscapeTiles;
    [SerializeField] private List<TileBase> lightUpTiles;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        InitializePublicArrays();
    }
    
    public int ModifyAccessor(int accessor)
    {
        return mapHeight - 1 - accessor;
    }
    //--------------------Public variables--------------------//
    //Storage for tiles
    private int[,] _tilesArray;
    public TerrainTileCode GetTile(Vector2Int position)
    {
        return (TerrainTileCode)_tilesArray[position[0], ModifyAccessor(position[1])];
    }
    public void SetTile(Vector2Int position, TerrainTileCode terrainTileCode)
    {
        _tilesArray[position[0], ModifyAccessor(position[1])] = (int)terrainTileCode;
    }
    
    //Storage for units
    private UnitController[,] _unitsArray;
    public UnitController GetUnit(Vector2Int position)
    {
        return _unitsArray[position[0], ModifyAccessor(position[1])];
    }
    public void SetUnit(Vector2Int position, UnitController unit)
    {
        Vector2Int curPos = unit.currentUnitTile;
        _unitsArray[curPos[0], ModifyAccessor(curPos[1])] = null;
        _unitsArray[position[0], ModifyAccessor(position[1])] = unit;
        unit.currentUnitTile = position;
    }
    
    //Storage for buildings
    private BuildingController[,] _buildingsArray;
    public BuildingController GetBuilding(Vector2Int position)
    {
        return _buildingsArray[position[0], ModifyAccessor(position[1])];
    }
    public void SetBuilding(Vector2Int position, BuildingController building)
    {
        _buildingsArray[position[0], ModifyAccessor(position[1])] = building;
    }
    
    //Graph
    private Dictionary<Vector2Int, List<Vector2Int>> _dependencyGraph;
    public List<Vector2Int> GetGraphNode(Vector2Int nodePosition)
    {
        if (_dependencyGraph.ContainsKey(nodePosition))
            return _dependencyGraph[nodePosition];

        Debug.LogWarning($"Wrong access position in GraphNode: {nodePosition}");
        return null;
    }
    public void AddGraphNode(Vector2Int nodePosition, Vector2Int addedNode)
    {
        //Debug.Log($"{nodePosition} ---> {addedNode}");
        if (GetTile(nodePosition) == TerrainTileCode.Ocean || GetTile(addedNode) == TerrainTileCode.Ocean) return;
        
        if (_dependencyGraph.ContainsKey(nodePosition))
        {
            if (!_dependencyGraph[nodePosition].Contains(addedNode))
                _dependencyGraph[nodePosition].Add(addedNode);
        }
        else _dependencyGraph.Add(nodePosition, new List<Vector2Int>() {addedNode});
        
        if (_dependencyGraph.ContainsKey(addedNode))
        {
            if (!_dependencyGraph[addedNode].Contains(nodePosition))
                _dependencyGraph[addedNode].Add(nodePosition);
        }
        else _dependencyGraph.Add(addedNode, new List<Vector2Int>() {nodePosition});
    }
    //------------------Public variables end------------------//

    private void InitializePublicArrays()
    {
        _tilesArray = new int[mapWidth, mapHeight];
        _unitsArray = new UnitController[mapWidth, mapHeight];
        _buildingsArray = new BuildingController[mapWidth, mapHeight];
        _dependencyGraph = new Dictionary<Vector2Int, List<Vector2Int>>();
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

    public TileBase GetLandscapeTileBase(TerrainTileCode terrainTileCode)
    {
        return landscapeTiles[(int) terrainTileCode];
    }

    public TileBase GetLightUpTileBase(LightUpTileCode lightUpTileCode)
    {
        return lightUpTiles[(int)lightUpTileCode];
    }

    public HashSet<Vector2Int> GetConnectedTiles(Vector2Int position, int steps)
    {
        if (steps == 0) return new HashSet<Vector2Int>();
        HashSet<Vector2Int> temp = new HashSet<Vector2Int>(_dependencyGraph[position]);
        if (steps == 1) return temp;

        HashSet<Vector2Int> output = new HashSet<Vector2Int>(temp);
        foreach (Vector2Int connection in temp)
        {
            output.UnionWith(GetConnectedTiles(connection, steps-1));
        }

        return output;
    }
}
