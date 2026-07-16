using UnityEngine;
using UnityEngine.EventSystems;


public class CellDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerClickHandler
{
    [SerializeField] private AssembleManager targetCell;

    private Camera mainCamera;
    private Collider2D cellCollider;
    private Vector3 startPosition;
    private Transform cellParent;

    private TankCell tankCell;

    private bool isAttached;

    private void Awake()
    {
        mainCamera = Camera.main;
        tankCell = GetComponent<TankCell>();
    }


    // drag start
    public void OnBeginDrag(PointerEventData eventData)
    {
        startPosition = this.transform.position;
        cellParent = this.transform.parent;

        // 이미 붙어있는걸 옮길 경우 => 집을 때 해제
        isAttached = tankCell.IsAttached;

        if(isAttached == true)
        {

            targetCell.RemoveCell(tankCell.CellPosition, tankCell);
        }
        
        

    }
    // in drag
    public void OnDrag(PointerEventData eventData)
    {

        // eventData.position => 화면 size 기반
        Vector3 dragPosition = new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z);

        this.transform.position = mainCamera.ScreenToWorldPoint(dragPosition);
    }

    // drop
    public void OnEndDrag(PointerEventData eventData)
    {

        Vector3 endPosition = new Vector3(eventData.position.x, eventData.position.y, -mainCamera.transform.position.z);
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(endPosition);

        Vector2Int targetPosition = targetCell.PositionWorldToLocal(this.transform.position);

        if (targetCell.IsAttachable(targetPosition))
        {
            
            targetCell.AttachCell(targetPosition, tankCell);
        }

        else if (isAttached == true)
        {
            targetCell.AttachCell(tankCell.CellPosition, tankCell);
        }
        else
        {
            transform.SetParent(cellParent);
            //this.transform.position = startPosition;    //  되돌아가는 것
        }

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

        // 우클릭 시 제거
        if(startPosition != null)
        {
            this.transform.position = startPosition;
        }
        
        targetCell.RemoveCell(tankCell.CellPosition, tankCell);
        
        




    }
}
