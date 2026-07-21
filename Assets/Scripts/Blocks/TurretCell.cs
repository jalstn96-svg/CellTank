using UnityEngine;

public abstract class TurretCell : TankCell, Ihittable
{

    [Header("Turret")]
    [SerializeField] protected TurretManager turretManager;

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }

    protected override void Awake()
    {
        base.Awake();

        if (turretManager == null)
        {
            Debug.Log("turretManager 인식 안됨. 호출 완료");
            turretManager = GetComponentInChildren<TurretManager>();
        }


    }
    protected override void OnActivate()
    {
        turretManager.SetRoot(RootStatus.gameObject);
        turretManager.SetAbled(true);
    }

    protected override void OnDeactivate()
    {
        turretManager.SetAbled(false);
    }
}
