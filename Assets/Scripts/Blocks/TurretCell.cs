using UnityEngine;

public abstract class TurretCell : TankCell, ITurret
{
    

    [Header("Turret")]
    [SerializeField] protected TurretManager turretManager;

    
    protected override void Awake()
    {
        base.Awake();

       

    }
    protected override void OnActivate()
    {
        if (turretManager == null)
        {
            Debug.Log("turretManager 인식 안됨. 호출 시도");
            turretManager = RootStatus.GetComponentInChildren<TurretManager>();
            if (turretManager == null)
            {
                Debug.Log("turretManager 인식 불가");
                return;
            }
            Debug.Log("turretManager 호출 완료");
        }

        turretManager.RegisterTurret(this);
        
    }

    protected override void OnDeactivate()
    {
        if(turretManager == null)
        {
            return;
        }
        turretManager.UnregisterTurret(this);
    }
    public abstract void Aim(Vector2 targetPosition);

    public abstract void TryFire();

}
