using System;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    //Unit controller
    [HideInInspector] public bool isUnitSelected;
    [HideInInspector] public UnitController selectedUnit;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        MapManager.Instance.LoadMap();
    }

    public void DeselectUnit()
    {
        isUnitSelected = false;
        selectedUnit = null;
        
        MapManager.Instance.DeLightUpEverything();
    }
}
