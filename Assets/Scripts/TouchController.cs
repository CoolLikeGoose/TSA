using UnityEngine;

public class TouchController : MonoBehaviour
{
    private Vector2 _touchPosWorld;
    private Camera _mainCamera;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.GetTouch(0).position);
            
            CheckPointInWorld();
            // RaycastHit2D hit = Physics2D.Raycast(_touchPosWorld, Camera.main.transform.forward);
            //
            // if (hit.collider != null)
            // {
            //     GameObject touchedObj = hit.transform.gameObject;
            //
            //     if (touchedObj.CompareTag("Units")) 
            //         touchedObj.GetComponent<UnitController>().SelectUnit();
            //     else if (touchedObj.CompareTag("Tile") && GameManager.Instance.isUnitSelected) 
            //         touchedObj.GetComponent<TileController>().SelectTile();
            // }
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            CheckPointInWorld();
        }
    }
    
    //Remove this after release maybe...
    private void CheckPointInWorld()
    {
        Vector2Int gridPos = (Vector2Int)MapData.Instance.baseMap.WorldToCell(_touchPosWorld);
        if (gridPos[0] < 0 || gridPos[1] < 0 
                           || gridPos[0] >= MapData.Instance.GetMapWidth() 
                           || gridPos[1] >= MapData.Instance.GetMapHeight()) return;
        DebugHelper.Instance.ShowList(MapData.Instance.GetGraphNode(gridPos));

        if (MapData.Instance.GetUnit(gridPos))
        {
            MapData.Instance.GetUnit(gridPos).SelectUnit();
        }
        else if (MapData.Instance.GetBuilding(gridPos))
        {
            //TODO: update for buildings
        }
        else if (GameManager.Instance.isUnitSelected)
        {
            //Check if we can move to this position
            if (MapData.Instance.GetTile(gridPos) == TileCode.Ocean)//or another strange thing
            {
                GameManager.Instance.DeselectUnit();
                return;
            }
            //TODO: add here the instruction for attacking
            
            //Change the unit prg pos
            MapData.Instance.SetUnit(gridPos, GameManager.Instance.selectedUnit);
            
            //Move the unit
            Vector2 worldPos = MapData.Instance.baseMap.GetCellCenterWorld((Vector3Int)gridPos);
            GameManager.Instance.selectedUnit.MoveToPos(new float[] {worldPos[0], worldPos[1]});
        }
    }
}
