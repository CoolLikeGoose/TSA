using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public void DeselectUnit()
    {
        isUnitSelected = false;
        selectedUnit = null;
    }
}
