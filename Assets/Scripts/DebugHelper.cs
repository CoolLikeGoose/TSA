using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
public class DebugHelper : MonoBehaviour
{
    public static DebugHelper Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    [SerializeField] private bool showTheTileCoords = false;

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.R))
    //     {
    //         SceneManager.LoadScene(0);
    //     }
    // }

    public void ShowMatrix(int[,] matrix)
    {
        string debugString = "";
        for (int x = 0; x < matrix.GetLength(0); x++)
        {
            for (int y = 0; y < matrix.GetLength(1); y++)
            {
                debugString += matrix[x, y];
            }

            debugString += '\n';
        }

        Debug.Log(debugString);
    }

    public void ShowList(List<Vector2Int> list)
    {
        if (list == null)
        {
            Debug.LogWarning("List is empty");
            return;
        }
        
        string debugString = "";

        foreach (Vector2Int el in list)
        {
            debugString += $"{el} ";
        }
        
        Debug.Log(debugString);
    }
    
    private void OnGUI()
    {
        if (showTheTileCoords)
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