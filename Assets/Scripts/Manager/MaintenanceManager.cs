using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class MaintenanceManager : MonoBehaviour
{
    [Header("call wait time")]
    [SerializeField] private float waitTime = 10f;

    [Header("Player")]
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private CorePhysics corePhysics;
    private RigidbodyConstraints2D previousConstraints;

    [Header("Maintenance Grid")]
    [SerializeField] private GameObject maintenanceBasePrefab;
    [SerializeField] private GameObject attachablePointRoot;

    [Header("UI")]
    [SerializeField] private GameObject maintenancePanel;

    [Header("Parts Area")]
    [SerializeField] private BoxCollider2D detachedCellArea;
    [SerializeField] private float scatterSpeed = 1.5f;
    [SerializeField] private float maxAngularSpeed = 180f;

    [Header("Maintenance Exit")]
    [SerializeField] private GameObject playingPanel;
    [SerializeField] private float baseReturnTime = 1.5f;
    [SerializeField] private string baseReturnTrigger = "Return";

    

    private Animator maintenanceBaseAnimator;
    private bool isReturning;

    float timer;
    // parts

    private GameObject maintenanceBaseObject;
    private Coroutine coroutine;

    private IEnumerator Start()
    {
        yield return null;  // 프레임 대기

        EnterMaintenance(false);
    }

    public void CallMaintenance()
    {
        if(coroutine != null)
        {
            return;
        }
        if(GameManager.instance == null)
        {
            Debug.Log("gamemanager 인식 안됨");
            return;
        }
        if (GameManager.instance.MaintenanceCall() == false)
        {
            Debug.Log("gameManager 내에 maintenanceCall() 연결안됨.");
            return;
        }
        UIManager.instance.CallMaintenance(waitTime);

        coroutine = StartCoroutine(CallStart());

    }

    private IEnumerator CallStart()
    {
        timer = waitTime;
        while (timer > 0f)
        {
            
            if(GameManager.instance.State == GameState.Pause)
            {
                yield return null;
                continue; 
            }

            if(GameManager.instance.State != GameState.MaintenanceCall)
            {
                coroutine = null;
                yield break;
            }

            timer -= Time.unscaledDeltaTime;
            UIManager.instance.UpdateMaintenanceCall(timer, waitTime);

            yield return null;
        }
        EnterMaintenance();
        coroutine = null;

    }

    private void EnterMaintenance(bool scatterCells = true)
    {

        GameManager.instance.EnterMaintenance();
        MobSpawner.instance.PauseTimer();
        StopPlayer();
        RemoveEnemies();
        BuildBase();
        EnableAttach();
        GatherDetachedCells(scatterCells);
        CellInteraction(true);

        UIManager.instance.ShowMaintenanceUI();
        

        Debug.Log("정비 시스템 진입");

    }

    private void StopPlayer()
    {
        previousConstraints = playerRb.constraints;

        playerRb.linearVelocity = Vector2.zero;
        playerRb.angularVelocity = 0f;
        playerRb.SetRotation(0f);
        playerRb.constraints |= RigidbodyConstraints2D.FreezeRotation;

        //클릭 및 포탑움직임이 마우스 영향 안받도록 할 것

    }

    private void RemoveEnemies()
    {
        Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach(Enemy enemy in enemies)
        {
            enemy.RemoveByAirDrop();
        }

        
    }

    private void BuildBase()
    {
        maintenanceBaseObject = Instantiate(maintenanceBasePrefab, playerRoot.position, Quaternion.identity);

        Transform partsArea = maintenanceBaseObject.transform.Find("PartsArea");

        detachedCellArea = partsArea.GetComponent<BoxCollider2D>();

        maintenanceBaseAnimator = maintenanceBaseObject.GetComponentInChildren<Animator>();

    }
    private void RemoveBase()
    {
        if (maintenanceBaseObject != null)
        {
            Destroy(maintenanceBaseObject);
            maintenanceBaseObject = null;

        }
        detachedCellArea = null;
    }

    private void EnableAttach()
    {
        attachablePointRoot.SetActive(true);
        AssembleManager assembleManager = playerRoot.GetComponentInChildren<AssembleManager>();

        assembleManager.RefreshGrid();
    }

    private void GatherDetachedCells(bool scatterCells)
    {
        AssembleManager playerAssembleManager = playerRoot.GetComponent<AssembleManager>();

        TankCell[] tankCells = FindObjectsByType<TankCell>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        // Bounds => Collider2D의 중심을 기준으로 경계 영역 => 특정 범위 인식 가능
        Bounds areaBounds = detachedCellArea.bounds;

        foreach (TankCell cell in tankCells)
        {
            if (cell == null)
            {
                continue;
            }
            if (cell.IsAttached==true)
            {
                continue;
            }
            if (cell.IsDisabled==true)
            {
                continue;
            }

            CellDrag cellDrag = cell.GetComponent<CellDrag>();

            if(cellDrag != null)
            {
                cellDrag.SetTarget(playerAssembleManager);
            }

            Vector2 randomPosition = new Vector2(Random.Range(areaBounds.min.x, areaBounds.max.x), Random.Range(areaBounds.min.y, areaBounds.max.y));
            ThrowCell(cell, randomPosition);
        }

    }

    private void ThrowCell(TankCell cell, Vector2 targetPosition)
    {
        Rigidbody2D cellRb = cell.GetComponent<Rigidbody2D>();

        if(cellRb == null)
        {
            cellRb = cell.gameObject.AddComponent<Rigidbody2D>();

            cellRb.bodyType = RigidbodyType2D.Dynamic;
            cellRb.gravityScale = 0f;
            cellRb.simulated = true;
            cellRb.interpolation = RigidbodyInterpolation2D.Interpolate;
            cellRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }


        cellRb.linearVelocity = Vector2.zero;
        cellRb.angularVelocity = 0f;

        cellRb.position = targetPosition;
        cellRb.rotation = Random.Range(0f, 360f);

        cellRb.WakeUp();

    }
    // 오버로딩 => 전달
    public void ThrowCell(TankCell cell)
    {
        if (cell == null || detachedCellArea == null) 
        { 
            return;
        }

        Bounds areaBounds = detachedCellArea.bounds;

        Vector2 randomPosition = new Vector2(Random.Range(areaBounds.min.x, areaBounds.max.x), Random.Range(areaBounds.min.y, areaBounds.max.y));

        ThrowCell(cell, randomPosition);
    }

    private void CellInteraction(bool isEnable)
    {

        // FindObjectsOfType => instanceID 정렬이 필요할 때만
        // FindObjectsByType => FindObjectsSortMode.None을 사용해 instanceID 정렬 배제
        // FindObjectsInactive.Exclude => 비활성화 오브젝트 배제
        CellDrag[] cellDrags = FindObjectsByType<CellDrag>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);

        foreach (CellDrag cellDrag in cellDrags)
        {
            cellDrag.enabled = isEnable;
        }


    }

    public void ExitMaintenance()
    {
        // 중복클릭 방지
        if (isReturning==true)
        {
            return;
        }

        if(GameManager.instance.State != GameState.Maintenance)
        {
            return;
        }
        isReturning = true;
        StartCoroutine(ReturnBaseRoutine());

    }

    private IEnumerator ReturnBaseRoutine()
    {
        

        CellInteraction(false);
        attachablePointRoot.SetActive(false);

        DestroyDetachedCells();

        if(maintenanceBaseAnimator != null)
        {
            maintenanceBaseAnimator.SetTrigger(baseReturnTrigger);
        }

        yield return new WaitForSeconds(baseReturnTime);

        FinishMaintenance();

        isReturning = false;


    }

    private void DestroyDetachedCells()
    {
        TankCell[] tankCells = FindObjectsByType<TankCell>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        
        foreach(TankCell cell in tankCells)
        {
            if (cell.IsAttached)
            {
                continue;
            }

            Destroy(cell.gameObject);
        }

    }

    private void FinishMaintenance()
    {


        // 정비 시스템 관련 전부 제거
        

        RepairAttachedCells();

        RemoveBase();

        maintenanceBaseAnimator = null;

        UIManager.instance.ShowPlayingUI();

        GameManager.instance.ExitMaintenance();

        playerRb.constraints = previousConstraints;

        MobSpawner.instance.ResumeTimer();

        Debug.Log("정비 종료");

    }

    private void RepairAttachedCells()
    {
        TankCell[] tankCells = playerRoot.GetComponentsInChildren<TankCell>(true);

        foreach(TankCell cell in tankCells)
        {
            if(cell.IsAttached==false)
            {
                continue;
            }
            if(cell.IsDisabled == true)
            {
                continue;
            }
            if(cell is CoreCell)
            {
                continue;
            }

            cell.Repaired();
        }
    }

}
