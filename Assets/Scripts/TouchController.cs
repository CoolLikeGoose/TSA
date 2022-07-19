using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

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
        MapManager.Instance.ProvideClicking(_touchPosWorld);
    }
}
