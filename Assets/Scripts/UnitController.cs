using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitController : MonoBehaviour
{
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

        Vector3 futurePos = transform.position;
        futurePos.x = pos[0];
        futurePos.y = pos[1];

        transform.position = futurePos;
    }
}
