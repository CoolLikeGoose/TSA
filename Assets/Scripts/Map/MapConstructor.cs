using UnityEngine;

public class MapConstructor : MonoBehaviour
{
    public static MapConstructor Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void ConstructBaseMap()
    {
        for (int i = 0; i < MapData.Instance.GetMapWidth(); i++)
        {
            for (int j = 0; j < MapData.Instance.GetMapHeight(); j++)
            {
                TileCode tileCode = GetTileCode(i, j);
                MapData.Instance.SetTile(new Vector2Int(i, j), tileCode);
                MapData.Instance.baseMap.SetTile(new Vector3Int(i, j, 0),
                    MapData.Instance.GetTileBase(tileCode));
            }
        }
    }
    
    private TileCode GetTileCode(int x, int y)
    {
        return (TileCode)UnityEngine.Random.Range(0, MapData.Instance.GetTilesCount());
    }
}
