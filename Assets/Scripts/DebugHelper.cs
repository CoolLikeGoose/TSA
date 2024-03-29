using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugHelper : MonoBehaviour
{
    public static DebugHelper Instance { get; private set; }

    [Header("MapDebugging")]
    [Header("Perlin noise")]
    [Range(4.0f, 20.0f)] [Tooltip("Change the amount of islands")]
    public float magnification = 7.0f;

    [Range(20f, 45f)] [Tooltip("Change the average hikes")]
    public float maxHikes = 50f;
    
    [Header("Voronoi noise")]
    public int detalization;

    [Header("FPS")] 
    private TextMeshProUGUI fpsLabel;
    private float _fps;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        InitializeDestroyed();
    }

    public void InitializeDestroyed()
    {
        fpsLabel = GameManager.Instance.fpsLabel;
    }

    //[SerializeField] private bool showTheTileCoords = false;
    private float _deltatime = 0;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
        
        _deltatime += (Time.deltaTime - _deltatime) * .1f;
        _fps = 1.0f / _deltatime;
        fpsLabel.SetText(Mathf.Ceil(_fps).ToString());
    }
    
    public static void ReloadScene()
    {
        SceneManager.LoadScene(0);
    }

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
    
    // private void OnGUI()
    // {
    //     if (!showTheTileCoords) return;
    //     for (int i = 0; i < MapData.Instance.GetMapWidth(); i++)
    //     {
    //         for (int j = 0; j < MapData.Instance.GetMapHeight(); j++)
    //         {
    //             Handles.Label(MapData.Instance.baseMap.GetCellCenterWorld(new Vector3Int(i, j , 1)), 
    //                 $"{i}:{j}");
    //         }
    //     }
    // }
}