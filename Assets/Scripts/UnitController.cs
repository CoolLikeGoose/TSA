using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public Vector2Int currentUnitTile;

    private void Start()
    {
        currentUnitTile = Vector2Int.zero;
        PlaceUnit();
    }

    public void PlaceUnit()
    {
        //Just for debug
        //TODO: delete this later or maybe don't delete this
        
        //Get position on tilemap
        currentUnitTile = (Vector2Int)MapData.Instance.baseMap.WorldToCell(transform.position);
        //Set position to tile in world
        transform.position = MapData.Instance.baseMap.GetCellCenterWorld((Vector3Int)currentUnitTile);
        
        MapData.Instance.SetUnit(currentUnitTile, this);
    }

    public void SelectUnit()
    {
        if (GameManager.Instance.isUnitSelected) {} //TODO: Add here the delight-upping event
        
        //Debug.Log("Unit selected" + gameObject.name);
        GameManager.Instance.selectedUnit = this;
        GameManager.Instance.isUnitSelected = true;
    }

    public void MoveToPos(float[] pos)
    {
        GameManager.Instance.DeselectUnit();

        Vector3 futurePos = Vector3.back; //gives -1 on z coordinate
        futurePos.x = pos[0];
        futurePos.y = pos[1];

        transform.position = futurePos;
    }
}
