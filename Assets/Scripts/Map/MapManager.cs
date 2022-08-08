using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapData),
                  typeof(MapConstructor),
                  typeof(MapGenManager))]
public class MapManager : MonoBehaviour
{
    [HideInInspector] 
    public static MapManager Instance { get; private set; }

    [HideInInspector] public List<Vector2Int> availablePaths;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void LoadMap()
    {
        MapConstructor.Instance.ConstructBaseMap();
    }

    public void SetSelectedLightUp()
    {
        Vector2Int curTile = GameManager.Instance.selectedUnit.currentUnitTile;
        MapData.Instance.lightUpMap.SetTile(new Vector3Int(curTile.x, curTile.y, 1), 
            MapData.Instance.GetLightUpTileBase(LightUpTileCode.CurrentSector));
        
        if (GameManager.Instance.isUnitSelected) LightUpPossibleMoves(curTile);
    }

    private void LightUpPossibleMoves(Vector2Int pos)
    {
        availablePaths = MapData.Instance.GetConnections(pos, GameManager.Instance.selectedUnit.speed);
        for (int i = 0; i < availablePaths.Count; i++)    
        {
            if (MapData.Instance.GetUnit(availablePaths[i]) != null) continue;
            
            MapData.Instance.lightUpMap.SetTile((Vector3Int)availablePaths[i],
                MapData.Instance.GetLightUpTileBase(LightUpTileCode.AvailablePath));
        }
    }

    public void DeLightUpEverything()
    {
        MapData.Instance.lightUpMap.SwapTile(MapData.Instance.GetLightUpTileBase(LightUpTileCode.CurrentSector),
            null);
        MapData.Instance.lightUpMap.SwapTile(MapData.Instance.GetLightUpTileBase(LightUpTileCode.AvailablePath),
            null);
    }
}
