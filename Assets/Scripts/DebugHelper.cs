using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class DebugHelper : MonoBehaviour
{
    [SerializeField] private bool ShowTheTileCoords = false;

    private void OnGUI()
    {
        if (ShowTheTileCoords)
        {
            for (int i = 0; i < MapData.Instance.GetMapWidth(); i++)
            {
                for (int j = 0; j < MapData.Instance.GetMapHeight(); j++)
                {
                    Handles.Label(MapData.Instance.baseMap.GetCellCenterWorld(new Vector3Int(i, j , 1)), 
                        $"{i}:{j}");
                }
            }
        }
    }
}

#endif