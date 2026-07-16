using UnityEngine;

public abstract class TankCell : MonoBehaviour
{
    [Header("Cell property")]

    [SerializeField] float maxDurability; // 실질적인 cell 체력
    [SerializeField] float weight; 

    private float currentDurability;

    // get
    public float CurrentDurability => currentDurability;
    public float Weight => weight;

    // get set
    public Vector2Int CellPosition { get; private set; }
    public bool IsAttached { get; private set; }
    public bool IsDisabled { get; private set; }
    protected TankStatus RootStatus { get; private set; }

    public void Awake()
    {
        currentDurability = maxDurability;
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
        IsAttached = true;

        RootStatus.AddWeight(weight);
    }

    public void SetDetached()
    {

        RootStatus.RemoveWeight(weight);
        IsAttached = false;
        RootStatus = null;
    }

    public virtual void TakeDamage()
    {

    }

    public virtual void Disable()
    {

    }
    public virtual void Restore()
    {

    }



}