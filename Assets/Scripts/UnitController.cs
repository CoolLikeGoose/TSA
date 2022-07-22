using UnityEngine;

public class UnitController : MonoBehaviour
{
    [HideInInspector] public Vector2Int currentUnitTile;
    public int speed = 2;

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
        
        //Really for debug
        while (true)
        {
            if (MapData.Instance.GetTile(currentUnitTile) != TerrainTileCode.Ocean &&
                MapData.Instance.GetUnit(currentUnitTile) == null) break;

            currentUnitTile.x++;
            if (currentUnitTile.x == MapData.Instance.GetMapWidth())
            {
                currentUnitTile.x = 0;
                currentUnitTile.y++;
            } 
            else if (currentUnitTile.y == MapData.Instance.GetMapHeight())
            {
                currentUnitTile.y = 0;
                currentUnitTile.x = 0;
            }
        }
        
        //Set position to tile in world
        transform.position = MapData.Instance.baseMap.GetCellCenterWorld((Vector3Int)currentUnitTile);
        
        MapData.Instance.SetUnit(currentUnitTile, this);
    }

    public void SelectUnit()
    {
        if (GameManager.Instance.isUnitSelected) MapManager.Instance.DeLightUpEverything();

        //Debug.Log("Unit selected" + gameObject.name);
        GameManager.Instance.selectedUnit = this;
        GameManager.Instance.isUnitSelected = true;
        
        MapManager.Instance.SetSelectedLightUp();
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
