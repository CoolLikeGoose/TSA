using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    //[HideInInspector]
    public bool isAvaibleToMove = false;
    
    //Must be int[] with indexes
    public float[] coords;

    private void Start()
    {
        Vector2 pos = transform.position;
        coords = new float[] {pos.x, pos.y};
    }

    public void SelectTile()
    {
        if (GameManager.Instance.isUnitSelected)
        {
            if (!isAvaibleToMove)
            {
                GameManager.Instance.DeselectUnit();
                return;
            }
            
            //Debug.Log("Tile selected" + gameObject.name);
            //TODO: Check if there is no another troops and if they are then select them by index
            GameManager.Instance.selectedUnit.MoveToPos(coords);
        }
    }
}
