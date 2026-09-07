using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMove : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] private Camera mainCamera;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 1f;
    [SerializeField] private float minZoom = 10f;
    [SerializeField] private float maxZoom = 40f;
    Vector3 targetPos;

    public float MaxZoom => maxZoom;
    void Start()
    {
        
    }

    private void Update()
    {
        if(GameManager.instance == null || Mouse.current == null)
        {
            return;
        }
        GameState state = GameManager.instance.State;

        if(state != GameState.Playing && state != GameState.MaintenanceCall && state != GameState.Maintenance)
        {
            return;
        }
        float scroll = Mouse.current.scroll.ReadValue().y;

        if(scroll >0f)
        {
            mainCamera.orthographicSize -= zoomSpeed;
        }
        else if (scroll < 0f)
        {
            mainCamera.orthographicSize += zoomSpeed;
        }
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize, minZoom, maxZoom);
    }
    
    void LateUpdate()
    {
        if(target == null) 
        {
            return; 
        }
        targetPos=new Vector3(target.position.x,target.position.y,-10f);

        transform.position = targetPos;
    }
}
