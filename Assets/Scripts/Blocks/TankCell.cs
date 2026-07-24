using UnityEngine;

public abstract class TankCell : MonoBehaviour, Ihittable
{
    [Header("Cell property")]

    [SerializeField] float maxDurability; // 실질적인 cell 체력
    [SerializeField] float weight;

    [SerializeField] private Vector2Int gridPosition;
    public Vector2Int GridPosition => gridPosition;
    

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
    }


    public void SetAttached(Vector2Int cellPosition)
    {
        if (IsAttached == true)
        {
            return;
        }

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

    
    public void TakeDamage(int damage)
    {
        
    }

    public virtual ProjectileHitResult Hit(ref ProjectileHitInit hitInit)
    {

        if (IsDisabled)
        {
            return ProjectileHitResult.Passed;
        }
        TakeDamage(hitInit.power);

        return ProjectileHitResult.Hitted;
    }
}