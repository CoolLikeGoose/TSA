using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class TouchController : MonoBehaviour
{
    private Vector2 _touchPosWorld;
    private void Update()
    {
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            Debug.Log("Finger");
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
            
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
            Debug.Log("Mouse");
            _touchPosWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CheckPointInWorld();
        }
    }
    
    //Remove this after release maybe...
    private void CheckPointInWorld()
    {
        RaycastHit2D hit = Physics2D.Raycast(_touchPosWorld, Camera.main.transform.forward);

        if (hit.collider != null)
        {
            GameObject touchedObj = hit.transform.gameObject;

            if (touchedObj.CompareTag("Units"))
                touchedObj.GetComponent<UnitController>().SelectUnit();
            else if (touchedObj.CompareTag("Tile"))
                touchedObj.GetComponent<TileController>().SelectTile();
        }
    }
}
