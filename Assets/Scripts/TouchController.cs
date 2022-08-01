using System;
using System.Collections;
using UnityEngine;

public class TouchController : MonoBehaviour
{
    [Header("Zoom settings")]
    [SerializeField] private float zoomInLim = 5;
    private float zoomOutLim;
    [SerializeField] private float zoomSpeed = .01f;
    private float _curZoom;

    [Header("Camera constrains")] 
    [SerializeField] private float zoomYMax = 24.5f;
    [SerializeField] private float zoomXMax = 14.6f;
    private float zoomYMin = 3.4f;
    private float zoomXMin = 1.2f;
    private float[] _cameraBordersX;
    private float[] _cameraBordersY;
    
    private float _percentOfSize;
    private float _curLerp;

    private Vector3 _touchPosWorld;
    
    private Camera _mainCamera;
    private Transform _cameraTransform;
    private Vector3 _cameraPos;

    private bool _multiTouch = false;

    private float _mapWidthH;
    private float _mapHeightH;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
        _cameraPos = _cameraTransform.position;
    }

    private void Start()
    {
        _mapHeightH = MapData.Instance.GetMapHeight()/2f;
        _mapWidthH = MapData.Instance.GetMapWidth()/2f;
        
        zoomOutLim = (MapData.Instance.GetMapWidth()+1) * 1.1f;
        _curZoom = _mainCamera.orthographicSize;
        
        _percentOfSize = (zoomOutLim-zoomInLim);
        _cameraBordersX = new[] {0f, 0f};
        _cameraBordersY = new[] {0f, 0f};
        
        _cameraPos = new Vector3(
            _mapWidthH,
            _mapHeightH,
            -10);
        
        CountCameraBorders();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _touchPosWorld = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _multiTouch = false;
            StartCoroutine(CheckForClick());
        }

        if (Input.touchCount == 2)
        {
            _multiTouch = true;
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;
            
            ProvideZoom(difference * zoomSpeed);
        } else if (!_multiTouch && Input.GetMouseButton(0))
        {
            Vector3 direction = _touchPosWorld - _mainCamera.ScreenToWorldPoint(Input.mousePosition);
            _cameraPos += direction;
            ClampCamera();
        }
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
            ProvideZoom(Input.GetAxis("Mouse ScrollWheel")*10);
    }

    private IEnumerator CheckForClick()
    {
        //Set delay to check tocuh
        yield return new WaitForSeconds(.15f);
        
        if (!Input.GetMouseButton(0))
            ProcessPoint();
    }

    private void ProvideZoom(float x)
    {
        _curZoom = Mathf.Clamp(_mainCamera.orthographicSize - x, zoomInLim, zoomOutLim);
        _mainCamera.orthographicSize = _curZoom;
        CountCameraBorders();
    }

    private void CountCameraBorders()
    {
        _curLerp = (_curZoom-zoomInLim) / _percentOfSize;
        
        _cameraBordersY[1] = Mathf.Lerp(zoomYMax, _mapHeightH, _curLerp); // from 24 to 14
        _cameraBordersY[0] = Mathf.Lerp(zoomYMin, _mapHeightH, _curLerp); // from 3 to 14
        _cameraBordersX[1] = Mathf.Lerp(zoomXMax, _mapWidthH, _curLerp);
        _cameraBordersX[0] = Mathf.Lerp(zoomXMin, _mapWidthH, _curLerp);
        
        ClampCamera();
    }

    private void ClampCamera()
    {
        _cameraPos.y = Mathf.Clamp(_cameraPos.y, _cameraBordersY[0], _cameraBordersY[1]);
        _cameraPos.x = Mathf.Clamp(_cameraPos.x, _cameraBordersX[0], _cameraBordersX[1]);
        _cameraTransform.position = _cameraPos;
    }

    //Remove this after release maybe...
    private void ProcessPoint()
    {
        Vector2Int gridPos = (Vector2Int)MapData.Instance.baseMap.WorldToCell(_touchPosWorld);
        if (gridPos[0] < 0 || gridPos[1] < 0 
                           || gridPos[0] >= MapData.Instance.GetMapWidth() 
                           || gridPos[1] >= MapData.Instance.GetMapHeight()) return;
        //DebugHelper.Instance.ShowList(MapData.Instance.GetGraphNode(gridPos));

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
            if (!MapManager.Instance.avaiblePaths.Contains(gridPos))//or another strange thing
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
