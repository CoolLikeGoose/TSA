using UnityEngine;

[RequireComponent(typeof(MapData),
                  typeof(MapConstructor))]
public class MapManager : MonoBehaviour
{
    [HideInInspector] 
    public static MapManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        MapConstructor.Instance.ConstructBaseMap();
    }
}
