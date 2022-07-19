using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [HideInInspector] 
    public static MapManager Instance { get; private set; }
    
    [Header("Tilemaps")]
    public Tilemap BaseMap;
    public Tilemap BuildingMap;

    [Header("Map settings")]
    [SerializeField] private int mapHeight;
    [SerializeField] private int mapWidth;
    //private int _accessingX;
    //private int _accessingY;

    [Header("Tiles")] [Tooltip("Change also enum after adding new tiles!")]
    [SerializeField] private List<TileBase> landscapeTiles;
    
    //--------------------Public arrays--------------------//
    private int[,] _tilesArray;
    public TileCode GetTile(Vector2Int position)
    {
        Debug.Log($"Actual tile accessor: " +
                  $"{(TileCode)_tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]]}\n" +
                  $"Without tile cast: " +
                  $"{_tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]]}\n" +
                  $"Array without modification: " +
                  $"{_tilesArray[position[0], position[1]]}\n" +
                  $"Access position: " +
                  $"{position}\n" +
                  $"Modificated access position: " +
                  $"{position[0]}:{_tilesArray.GetLength(1)-1 - position[1]}");
        return (TileCode)_tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]];
    }

    public void SetTile(Vector2Int position, TileCode tileCode)
    {
        _tilesArray[position[0], _tilesArray.GetLength(1)-1 - position[1]] = (int)tileCode;
    }
    
    private UnitController[,] _unitsArray;
    public UnitController GetUnit(Vector2Int position)
    {
        //Debug.Log(position);
        //Debug.Log(_unitsArray[position[0], _unitsArray.Length - position[1]]);
        return _unitsArray[position[0], _unitsArray.GetLength(1)-1 - position[1]];
    }
    public void SetUnit(Vector2Int position, UnitController unit)
    {
        Vector2Int curPos = unit.currentUnitTile;
        _unitsArray[curPos[0], _unitsArray.GetLength(1)-1 - curPos[1]] = null;
        _unitsArray[position[0], _unitsArray.GetLength(1)-1 - position[1]] = unit;
        //Debug.Log($"{position[0]},{_unitsArray.Length - position[1]}");
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
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        _tilesArray = new int[mapWidth, mapHeight];
        _unitsArray = new UnitController[mapWidth, mapHeight];
        _buildingsArray = new BuildingController[mapWidth, mapHeight];

        //_accessingX = -mapWidth / 2;
        //_accessingY = -mapHeight / 2;
    }

    private void Start()
    {
        ConstructBaseMap();
    }

    private void ConstructBaseMap()
    {
        for (int i = 0; i < mapWidth; i++)
        {
            for (int j = 0; j < mapHeight; j++)
            {
                TileCode tileCode = GetTileCode(i, j);
                SetTile(new Vector2Int(i, j), tileCode);
                BaseMap.SetTile(new Vector3Int(i, j, 0),
                    landscapeTiles[(int)tileCode]);
            }
        }
    }
    
      // private void OnGUI()
      // {
      //     for (int i = 0; i < mapWidth; i++)
      //     {
      //         for (int j = 0; j < mapHeight; j++)
      //         {
      //             Handles.Label(BaseMap.GetCellCenterWorld(new Vector3Int(i, j , 1)), 
      //                 $"{i}:{j}");
      //         }
      //     }
      // }

    private TileCode GetTileCode(int x, int y)
    {
        return (TileCode)UnityEngine.Random.Range(0, landscapeTiles.Count);
    }
    
    public void ProvideClicking(Vector2 pos)
    {
        Vector2Int gridPos = (Vector2Int)BaseMap.WorldToCell(pos);
        if (gridPos[0] < 0 || gridPos[1] < 0) return;
        Debug.Log(gridPos); 

        if (GetUnit(gridPos))
        {
            GetUnit(gridPos).SelectUnit();
        }
        else if (GetBuilding(gridPos))
        {
            //TODO: update for buildings
        }
        else if (GameManager.Instance.isUnitSelected)
        {
            //Check if we can move to this position
            if (GetTile(gridPos) == TileCode.Ocean)//or another strange thing
            {
                GameManager.Instance.DeselectUnit();
                return;
            }
            //TODO: add here the instruction for attacking
            
            //Change the unit prg pos
            SetUnit(gridPos, GameManager.Instance.selectedUnit);
            
            //Move the unit
            Vector2 worldPos = BaseMap.GetCellCenterWorld((Vector3Int)gridPos);
            GameManager.Instance.selectedUnit.MoveToPos(new float[] {worldPos[0], worldPos[1]});
        }
    }
}
