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
        MapGenManager.Instance.InitializeGenerationDependencies();
            
        for (int x = 0; x < MapData.Instance.GetMapWidth(); x++)
        {
            for (int y = 0; y < MapData.Instance.GetMapHeight()/2; y++)
            {
                TerrainTileCode terrainTileCode = GetTileCode(x, y);
                ProcessTile(x, y, terrainTileCode);
                ProcessTile(x, MapData.Instance.GetMapHeight() - y - 1, terrainTileCode);
            }
        }
        
        CompleteGraphDependencies();
        MapGenManager.Instance.ShowDebugDiagram();
    }

    //Add tile to array and set to field
    private void ProcessTile(int x, int y, TerrainTileCode terrainTileCode)
    {
        MapData.Instance.SetTile(new Vector2Int(x, y), terrainTileCode);
        MapData.Instance.baseMap.SetTile(new Vector3Int(x, y, 0),
            MapData.Instance.GetLandscapeTileBase(terrainTileCode));
    }
    
    private TerrainTileCode GetTileCode(int x, int y)
    {
        return MapGenManager.Instance.GetProceduralTileCode(x, y);
    }

    private void CompleteGraphDependencies()
    {
        for (int i = 0; i < MapData.Instance.GetMapWidth(); i++)
        {
            for (int j = 0; j < MapData.Instance.GetMapHeight(); j++)
            {
                //RESTRICTION!
                if (i == 0 && j == 0 || 
                    MapData.Instance.GetTile(new Vector2Int(i, j)) == TerrainTileCode.Ocean) continue;
                
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
