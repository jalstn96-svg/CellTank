using UnityEngine;

public abstract class TankCell : MonoBehaviour, Ihittable
{
    [Header("Cell property")]

    [SerializeField] float maxDurability; // 실질적인 cell 체력
    [SerializeField] float weight;

    [SerializeField] private Vector2Int gridPosition;
    public Vector2Int GridPosition => gridPosition;
    [SerializeField] private CellPhysics cellPhysics;

    // get
    public float CurrentDurability { get; private set; }
    public float Weight => weight;

    // get set
    public Vector2Int CellPosition {
        get;
        private set; }
    public bool IsAttached { get; private set; }
    public bool IsDisabled { get; private set; }
    protected TankStatus RootStatus { get; private set; }

    protected virtual void Awake()
    {
        CurrentDurability = maxDurability;
        if(cellPhysics == null)
        {
            cellPhysics = GetComponent<CellPhysics>();
        }
        if(cellPhysics == null)
        {
            cellPhysics = GetComponentInParent<CellPhysics>();

        }
        if(cellPhysics == null)
        {
            cellPhysics = GetComponentInChildren<CellPhysics>();
        }
    }



    public void SetAttached(Vector2Int cellPosition)
    {
        if (IsAttached == true)
        {
            return;
        }
        gameObject.layer = transform.parent.gameObject.layer;

        TankStatus rootParent = GetComponentInParent<TankStatus>();

        
        RootStatus = rootParent;
        CellPosition = cellPosition;
        gridPosition = cellPosition;
        IsAttached = true;

        RootStatus.AddWeight(weight);
        if (IsDisabled == false)
        {
            OnActivate();
        }
        
    }

    public void SetDetached()
    {
        if (IsAttached == false)
        {
            return;
        }
        if(IsDisabled == false)
        {
            OnDeactivate();
        }

        RootStatus.RemoveWeight(weight);
        IsAttached = false;
        RootStatus = null;
        
    }

    public virtual void TakeDamage(float damage)
    {
        if (IsDisabled == true)
        {
            return;
        }
        CurrentDurability -= damage;
        if (CurrentDurability <= 0)
        {
            Disabled();
        }


    }

    public void Repaired()
    {
        if(IsDisabled == true)
        {
            return;
        }

        CurrentDurability = maxDurability;

        OnRestored();   // 애니메이션이나 효과 추가
    }
    public void Disabled()
    {
        if (IsDisabled == true)
        {
            return;
        }
        CurrentDurability = 0f; // 관통 시 즉시 disable
        IsDisabled = true;
        if (IsAttached)
        {
            OnDeactivate();
        }

        cellPhysics.SetDisabledVisual();

        OnDisabled();   //애니메이션이나 효과 추가
    }

   


    protected virtual void OnDisabled()
    {

    }
    protected virtual void OnRestored()
    {

    }
    protected virtual void OnActivate()
    {

    }
    protected virtual void OnDeactivate()
    {

    }
    /// <summary>
    /// 파괴된 cell을 가진 enemy가 pool에서 재소환 될 때 복구용도
    /// </summary>
    //public void ResetCell()
    //{
    //    bool wasDisabled = IsDisabled;
    //    CurrentDurability = maxDurability;
    //    IsDisabled = false;
    //    cellPhysics.RestorVisual();
    //    if(wasDisabled && IsAttached)
    //    {

    //    }
    //}
   

    public virtual ProjectileHitResult Hit(ref ProjectileHitInit hitInit)
    {

        if (IsDisabled)
        {
            return ProjectileHitResult.Passed;
        }
        TakeDamage(hitInit.power);

        return ProjectileHitResult.Hitted;
    }

    public void ResetCell()
    {
        if(cellPhysics == null)
        {
            cellPhysics = GetComponent<CellPhysics>();
        }

        if(cellPhysics == null)
        {
            cellPhysics = GetComponentInChildren<CellPhysics>();

        }

        bool wasDisabled = IsDisabled;
        

        CurrentDurability = maxDurability;
        IsDisabled = false;

        if (cellPhysics != null)
        {
            cellPhysics.RestoreVisual();
        }

        
        if (wasDisabled&&IsAttached)
        {
            OnActivate();
        }
    }
}