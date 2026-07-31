using UnityEngine;

public abstract class TurretCell : TankCell, ITurret
{
    

    [Header("Turret")]
    [SerializeField] protected TurretManager turretManager;
    protected TurretManager newTurretManager;

    [Header("Bullet Stats")]
    [SerializeField] protected float bulletPower = 1f;
    [SerializeField] protected float bulletSpeed;
    [SerializeField] protected int bulletDamage;
    [SerializeField] protected float bulletLifeTime;
    [SerializeField] protected float penetration;

    [Header("Turret Stats")]
    [SerializeField] protected float reloadTimer;
    [SerializeField] protected float turretRotateSpeed = 30f;
    [SerializeField] protected float fireCoolDown;
    private float ratio;

    [Header("Reload Gauge")]
    [SerializeField] private GameObject reloadGauge;
    [SerializeField] private Transform reloadGaugeFill;
    [SerializeField] private SpriteRenderer reloadGaugeFillsr;
    [SerializeField] private Gradient reloadGradient;

    private Vector3 reloadFillScale;
    private Vector3 maxGaugeScale;
    private Vector3 scale;

    public float reloadRatio => Mathf.Clamp01(reloadTimer / fireCoolDown); // for reloading gauge ui

    public float BulletPower => bulletPower;
    public float BulletSpeed => bulletSpeed;
    public int BulletDamage => bulletDamage;
    public float BulletLifeTime => bulletLifeTime;
    public float Penetration => penetration;
    public float TurretRotateSpeed => turretRotateSpeed;
    public float FireCoolDown => fireCoolDown;

    public bool canFire => reloadTimer >= fireCoolDown;


    protected override void Awake()
    {
        base.Awake();

        maxGaugeScale = reloadGaugeFill.localScale;
        if(reloadGauge != null) 
        {
            reloadGauge.SetActive(false);
        }


       

    }

    protected virtual void LateUpdate()
    {
        if(!IsAttached || IsDisabled)
        {
            return;

        }
        UpdateReloadGauge();
    }

    private void UpdateReloadGauge()
    {
        if(reloadGaugeFill == null)
        {
            return;
        }

        ratio = reloadRatio;
        scale = maxGaugeScale;
        scale.x *= maxGaugeScale.x*ratio;

        reloadGaugeFill.localScale = scale;

        

    }


    protected override void OnActivate()
    {
        base.OnActivate();

        reloadTimer = 0;

        if(reloadGauge != null)
        {
            reloadGauge.SetActive(true);
        }
        UpdateReloadGauge();

        if(RootStatus == null)
        {
            Debug.Log("RootStatus 인식 불가");
            return;
        }

        newTurretManager = RootStatus.GetComponentInChildren<TurretManager>(true);


        if (newTurretManager == null)
        {
            
            
            Debug.Log("turretManager 인식 불가");
            return;

            
        }
        if(turretManager != null && turretManager != newTurretManager)
        {
            turretManager.UnregisterTurret(this);
        }
        turretManager = newTurretManager;
        turretManager.RegisterTurret(this);
        Debug.Log("새로운 turretManager 호출 완료");

    }

   
    protected override void OnDeactivate()
    {

        if(reloadGauge != null)
        {
            reloadGauge.SetActive(false);

        }
        

        
        if(turretManager == null)
        {
            return;
        }
        turretManager.UnregisterTurret(this);
        turretManager = null;
        base.OnDeactivate();
    }
    public abstract void Aim(Vector2 targetPosition);

    public abstract void TryFire();

}
