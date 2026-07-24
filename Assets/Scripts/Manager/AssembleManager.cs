using UnityEngine;
using System.Collections.Generic;

public class AssembleManager : MonoBehaviour
{


    [SerializeField] private Transform cellRoot;
    [SerializeField] private AssembleGrid assembleGrid;
    [SerializeField] public TankCell testCell;
    [SerializeField] private TankCell coreCell;

    // 부착 가능 미리보기
    [SerializeField] private bool showAttachableView = true;
    [SerializeField] private SpriteRenderer attachableSign;
    [SerializeField] private Transform attachableSignPosition;

    
    
    private Vector2 CellSize => assembleGrid.CellSize;

    private TankStatus tankStatus;

    // cell 부착 기록 및 cell 상태 관리
    private Dictionary<Vector2Int, TankCell> installedCells = new Dictionary<Vector2Int, TankCell>();

    // cell 부착 가능 공간 제한 => 이미 부착된 cell 인근
    private HashSet<Vector2Int> attachablePositions = new HashSet<Vector2Int>();
    private Vector2Int[] checkAttachableDir = { Vector2Int.up, Vector2Int.down, Vector2Int.right, Vector2Int.left };

    // cell 부착 가능 공간 preview list
    private List<SpriteRenderer> attachableSigns = new List<SpriteRenderer>();

    // get
    public IEnumerable<Vector2Int> AttachablePositions => attachablePositions;

    private void Awake()
    {
        tankStatus = GetComponentInParent<TankStatus>();
    }



    private void Start()
    {

        InstalledCellsInit();



        RefreshGrid();
        RefreshAttachablePositions();
        
    }

    
    // IsAttachable 빈칸 한정으로 변경
    public bool IsAttachable(Vector2Int cellPosition) 
    {
        if (cellPosition == Vector2Int.zero)
        {
            return false;
        }

        if (installedCells.ContainsKey(cellPosition))
        {
            return false;
        }

        
        return attachablePositions.Contains(cellPosition);
    }

    private void InstalledCellsInit()
    {
        installedCells.Clear();
        TankCell[] cells = cellRoot.GetComponentsInChildren<TankCell>(true);
        foreach (TankCell cell in cells)
        {
            Vector2Int cellPosition;
            if(cell == coreCell)
            {
                cellPosition = Vector2Int.zero;
            }
            else
            {
                cellPosition = PositionWorldToLocal(cell.transform.position);
            }

            if (installedCells.ContainsKey(cellPosition))
            {
                Debug.Log($"Cell 위치 중복{cellPosition}");
                continue;
            }

            cell.SetAttached(cellPosition);
            installedCells.Add(cellPosition, cell);

        }
    }

    public bool TryAttach(Vector2Int targetPosition, TankCell newCell, bool wasAttached, Vector2Int previousPosition, Vector3 startPosition)
    {
        if (targetPosition == Vector2Int.zero)
        {
            return false;
        }

        if(installedCells.TryGetValue(targetPosition, out TankCell oldCell) == true)
        {
            // 1. 장착된 cell 간 위치 변경
            if (wasAttached)
            {
                RemoveCell(targetPosition, oldCell);
                AttachCell(targetPosition, newCell);
                AttachCell(previousPosition, oldCell);
                return true;
            }

            // 2. 필드 위의 cell과 장착된 cell 교체
            float resultWeight = tankStatus.CurrentWeight - oldCell.Weight + newCell.Weight;
            if(resultWeight > tankStatus.MaxWeight)
            {
                Debug.Log("cell 교체 하중 초과");
                return false;
            }

            RemoveCell(targetPosition, oldCell);
            AttachCell(targetPosition, newCell);

            return true;
        }

        if (IsAttachable(targetPosition) == false)
        {
            Debug.Log("장착 불가능한 위치");
            return false;
        }
        if (tankStatus.CanAttach(newCell.Weight) == false)
        {
            Debug.Log("하중 초과");
            return false;
        }

        //3. 빈 cell에 장착
        AttachCell(targetPosition, newCell);

        return true;

        
    }

    public void AttachCell(Vector2Int cellPosition, TankCell cell)
    {

        if (IsAttachable(cellPosition) == false)
        {
            Debug.Log("설치 가능 위치 없음");
            // 설치 불가 알림? 애니메이션?
            return;
        }
        if (tankStatus.CanAttach(cell.Weight) == false)
        {
            Debug.Log("하중 초과");
            return;
        }
        

        cell.transform.SetParent(cellRoot);

        cell.transform.localPosition = new Vector2(cellPosition.x * CellSize.x, cellPosition.y * CellSize.y);
        cell.transform.localRotation = Quaternion.identity;


        cell.SetAttached(cellPosition);
        installedCells.Add(cellPosition, cell);

        RefreshGrid();
        RefreshAttachablePositions();
    }

    public void RemoveCell(Vector2Int cellPosition, TankCell cell)
    {


        if(cell.IsAttached == false)    // 붙어있지 않은 cell은 return
        {
            Debug.Log("해당 cell은 not attached");
            return;
        }


        if (cellPosition == Vector2Int.zero)    // 코어는 unattachable
        {
            Debug.Log("해당 cell은 core");

            return;
        }

        cell.SetDetached();
        installedCells.Remove(cellPosition);
        cell.transform.SetParent(null);

        
        RefreshGrid();
        RefreshAttachablePositions();

    }

    public void RefreshGrid()
    {
        Debug.Log("RefreshGrid in Manager Start");
        int maxX=0;
        int maxY=0;

        foreach(Vector2Int cellPosition in installedCells.Keys)
        {
            maxX = Mathf.Max(maxX,Mathf.Abs(cellPosition.x));
            maxY = Mathf.Max(maxY,Mathf.Abs(cellPosition.y));
        }
        

        
        assembleGrid.RefreshGrid(maxX,maxY);
        
    }
    

    private void RefreshAttachablePositions()
    {
        // 청소
        attachablePositions.Clear();

        foreach(Vector2Int installedPosition in installedCells.Keys)
        {
            foreach(Vector2Int direction in checkAttachableDir)
            {
                Vector2Int canInstalledPosition = installedPosition + direction;

                if (installedCells.ContainsKey(canInstalledPosition))
                {
                    continue;
                }
                attachablePositions.Add(canInstalledPosition);

            }
        }
        RefreshAttachableView();

    }

    private void RefreshAttachableView()
    {
        // view 초기화
        foreach(SpriteRenderer previousView in attachableSigns)
        {
            Destroy(previousView.gameObject);
        }

        attachableSigns.Clear();

        // 부착 가능 표시 on/off
        if(showAttachableView == false)
        {
            return;
        }
        
        

        // view 그리기
        foreach(Vector2Int cellPosition in attachablePositions)
        {
            SpriteRenderer attachableView = Instantiate(attachableSign, attachableSignPosition);


            attachableView.transform.localPosition = new Vector3(cellPosition.x * CellSize.x, cellPosition.y * CellSize.y, 0f);

            attachableView.transform.localRotation = Quaternion.identity;

            attachableSigns.Add(attachableView);

        }


    }

    public Vector2Int PositionWorldToLocal(Vector3 worldPosition)
    {
        Vector3 localPosition = cellRoot.InverseTransformPoint(worldPosition);

        
        return new Vector2Int(Mathf.RoundToInt(localPosition.x/CellSize.x), Mathf.RoundToInt(localPosition.y/CellSize.y));

    }

    public bool IsConnected()
    {

        return false;
    }

    
    public void EnemyPresetInit()
    {
        installedCells.Clear();

        TankCell[] presetCells = cellRoot.GetComponentsInChildren<TankCell>(true);

        foreach(TankCell cell in presetCells)
        {
            installedCells.Add(cell.GridPosition, cell);
            cell.SetAttached(cell.GridPosition);

        }

        RefreshGrid();

    }
  

}
