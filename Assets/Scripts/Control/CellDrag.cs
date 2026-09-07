using UnityEngine;
using UnityEngine.EventSystems;


public class CellDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AssembleManager targetCell;

    private Camera mainCamera;
    private Collider2D cellCollider;
    private Vector3 startPosition;
    private Transform cellParent;
    private Vector2Int previousPosition;

    private TankCell tankCell;

    private bool isAttached;
    private bool isEngineering=false;
    private bool canDrag;

    private MaintenanceManager maintenanceManager;

    private void Awake()
    {
        mainCamera = Camera.main;
        tankCell = GetComponent<TankCell>();
        cellCollider = GetComponent<Collider2D>();
        maintenanceManager = FindAnyObjectByType<MaintenanceManager>();
    }


    // drag start
    public void OnBeginDrag(PointerEventData eventData)
    {
        canDrag = false;

        if (isEngineering == true)
        {
            if (GameManager.instance.State != GameState.Maintenance)
            {
                Debug.Log("정비 타임 아님.");
                return;
            }
        }

        
        if (tankCell.IsAttached == true)
        {
            Debug.Log("장착된 cell은 드래그 불가");
            return;
        }

        if (tankCell.IsDisabled)
        {
            Debug.Log("파괴된 cell은 교체만 가능.");
            return;
        }

        canDrag = true;

        startPosition = this.transform.position;
        cellParent = this.transform.parent;

        // 이미 붙어있는걸 옮길 경우 => 못잡게
        

        

        if (cellCollider != null)
        {
            cellCollider.enabled = false;

        }
        else Debug.Log("cell Collider 에러");
        

    }
    // in drag
    public void OnDrag(PointerEventData eventData)
    {
        if (canDrag==false)
        {
            return;
        }
        // eventData.position => 화면 size 기반
        Vector3 dragPosition = new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z);

        this.transform.position = mainCamera.ScreenToWorldPoint(dragPosition);
    }

    // drop
    public void OnEndDrag(PointerEventData eventData)
    {
        if(canDrag == false) { return; }

        canDrag = false;
        Vector3 endPosition = new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z);
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(endPosition);

        Vector2Int targetPosition = targetCell.PositionWorldToLocal(this.transform.position);

        bool isPlaced = targetCell.TryAttach(targetPosition, tankCell,isAttached,previousPosition,startPosition);

        if(isPlaced == false)
        {
            
            transform.SetParent(cellParent);

            transform.position = startPosition;    //  되돌아가는 것
            

        }
        cellCollider.enabled = true;
        // 레이어 설정 추가할 것
                        
        

        //this.transform.position = mousePosition;    //  마우스 위치에 놓는 것.

        
        
        // cell 장착 파트

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //eventData.button 
        //PointerEventData.InputButton.Right (or Left or Middle)

        // 우클릭이 아니면 이 class는 패스
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            return;
        }
        // 붙은게 아니면 패스
        if(tankCell.IsAttached == false)
        {
            return;
        }
        // 코어는 금지
        if(tankCell.CellPosition == Vector2Int.zero)
        {
            return;
        }
        targetCell.RemoveByRClick(tankCell.CellPosition, tankCell);

        // 우클릭 시 제거
        if (tankCell.IsAttached==true) 
        {
            
            return; 
        }
        

        maintenanceManager.ThrowCell(tankCell);


    }

    private void SetLayerChildren(GameObject target, int layer)
    {
        target.layer = layer;

        foreach (Transform child in target.transform)
        {
            SetLayerChildren(child.gameObject, layer);
        }
    }

    public void SetTarget(AssembleManager assembleManager)
    {
        targetCell = assembleManager;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (UIManager.instance == null)
        {
            return;
        }
        UIManager.instance.ShowCellInfo(tankCell);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (UIManager.instance == null)
        {
            return;
        }
        UIManager.instance.HideCellInfo();
    }
}
