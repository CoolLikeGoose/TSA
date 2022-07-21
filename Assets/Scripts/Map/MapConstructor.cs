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
        
        CompleteGraphDependencies();
    }
    
    private TileCode GetTileCode(int x, int y)
    {
        return (TileCode)UnityEngine.Random.Range(0, MapData.Instance.GetTilesCount());
    }

    private void CompleteGraphDependencies()
    {
        for (int i = 0; i < MapData.Instance.GetMapWidth(); i++)
        {
            for (int j = 0; j < MapData.Instance.GetMapHeight(); j++)
            {
                //RESTRICTION!
                if (i == 0 && j == 0 || 
                    MapData.Instance.GetTile(new Vector2Int(i, j)) == TileCode.Ocean) continue;
                
                if (i != 0 && j != 0)
                {
                    AddGraphDependency(i, j, 1, 1);
                    AddGraphDependency(i, j, 1, 0);
                    AddGraphDependency(i, j, 0, 1);
                    AddGraphDependency(i, j, -1, 1); //diagonal down
                }
                else if (i == 0)
                {
                    AddGraphDependency(i, j, -1, 1); //diagonal down
                    AddGraphDependency(i, j, 0, 1);
                }
                else if (j == 0)
                    AddGraphDependency(i, j, 1, 0);
            }
        }
    }
    
    //надо что-то думать с этим говнокодом, хотя он оптимизирован
    private void AddGraphDependency(int x, int y, int biasX = 0, int biasY = 0)
    {
        if (x - biasX >= MapData.Instance.GetMapWidth()) return;
        //if (biasX == 1 && biasY == 1) Debug.Log($"({x},{y}) ---> ({x-biasX},{y-biasY})");
        MapData.Instance.AddGraphNode(new Vector2Int(x, y), new Vector2Int(x-biasX, y-biasY));
    }
}