using UnityEngine;

public class TurretCell_C1 : TurretCell, ITurret
{
    [SerializeField] Transform turretBody;
    [SerializeField] Transform bulletStartPosition;

    [Header("Bullet Stats")]
    
    [SerializeField] float bulletPower = 1f;
    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletLifeTime;
    [SerializeField] float penetration;

    [Header("Turret Stats")]
    [SerializeField] float reloadTimer;
    [SerializeField] float turretRotateSpeed = 30f;
    [SerializeField] float fireCoolDown;

    

    GameObject mainGun;
    int bulletLayer;    // 탄 레이어 => 타격 판정
    LayerMask targetLayer;  // 맞을 판정 타겟 레이어

    public float reloadRatio => Mathf.Clamp01(reloadTimer / fireCoolDown); // for reloading gauge ui

    public bool canFire => reloadTimer >= fireCoolDown;

    protected override void Awake()
    {
        base.Awake();

        

        reloadTimer = fireCoolDown;
        
    }


    // Update is called once per frame
    void Update()
    {
        if(!IsAttached || IsDisabled)
        {
            return;
        }

        if (reloadTimer < fireCoolDown)
        {
            reloadTimer += Time.deltaTime;
        }
    }
    protected override void OnActivate()
    {
        mainGun = RootStatus.gameObject;
        SetLayer();

        base.OnActivate();// 본래 TurretCell.cs에서 실행
    }

    protected override void OnDeactivate()
    {
        
        base.OnDeactivate();
    }

    public override void Aim(Vector2 targetPosition)
    {
        if(!IsAttached || IsDisabled)
        {
            return;
        }

        Vector2 direction = targetPosition - (Vector2)turretBody.position;
        if(direction == Vector2.zero)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float nextAngle = Mathf.MoveTowardsAngle(turretBody.eulerAngles.z, targetAngle, turretRotateSpeed * Time.deltaTime);

        turretBody.rotation = Quaternion.Euler(0f, 0f, nextAngle);

    }
    public override void TryFire()
    {
        if(!IsAttached || IsDisabled)
        {
            return;
        }
        if (!canFire)
        {
            return;
        }
        GameObject bulletObject = ObjectPool.instance.GetObject("Bullet");

        bulletObject.transform.SetPositionAndRotation(bulletStartPosition.position, bulletStartPosition.rotation);

        bulletObject.layer = bulletLayer;

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        bullet.Init(bulletStartPosition.up, bulletSpeed, bulletPower, bulletDamage, bulletLifeTime, penetration, targetLayer, mainGun);

        reloadTimer = 0f;

    }


    private void SetLayer()
    {
        if (mainGun.layer == LayerMask.NameToLayer("Player"))
        {
            bulletLayer = LayerMask.NameToLayer("PlayerBullet");
            targetLayer = LayerMask.GetMask("Enemy");

        }
        else if (mainGun.layer == LayerMask.NameToLayer("Enemy"))
        {
            bulletLayer = LayerMask.NameToLayer("EnemyBullet");
            targetLayer = LayerMask.GetMask("Player", "Enemy");

        }

    }
    public void SetRoot(GameObject root)
    {
        mainGun = root;
        SetLayer();
    }

}
