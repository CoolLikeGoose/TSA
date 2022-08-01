using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    //Unit controller
    [HideInInspector] public bool isUnitSelected;
    [HideInInspector] public UnitController selectedUnit;
    
    public TextMeshProUGUI fpsLabel;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        DebugHelper.Instance.InitializeDestroyed();
        MapManager.Instance.LoadMap();
    }

    public void DeselectUnit()
    {
        isUnitSelected = false;
        selectedUnit = null;
        
        MapManager.Instance.DeLightUpEverything();
    }
    
    public void ReloadScene()
    {
        DebugHelper.ReloadScene();
    }
}
