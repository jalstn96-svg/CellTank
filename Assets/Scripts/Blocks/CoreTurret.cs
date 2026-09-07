using UnityEngine;
using UnityEngine.UIElements;

public class CoreTurret : MonoBehaviour, ITurret
{
    [SerializeField] private Transform bulletStartPosition;

    [Header("Turret Stats")]
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private float fireCoolDown = 3f;

    [Header("Bullet Stats")]
    [SerializeField] private float bulletPower;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private int bulletDamage;
    [SerializeField] private float bulletLifeTime = 3f;
    [SerializeField] private float penetration = 10f;

    [Header("Reload Gauge")]
    [SerializeField] private GameObject reloadGauge;
    [SerializeField] private Transform reloadGaugeFill;
    [SerializeField] private SpriteRenderer reloadGaugeFillsr;
    [SerializeField] private Gradient reloadGradient;
    public float reloadRatio => Mathf.Clamp01(reloadTimer / fireCoolDown); // for reloading gauge ui
    private float ratio;

    private Vector3 reloadFillScale;
    private Vector3 maxGaugeScale;
    private Vector3 scale;

    private TurretManager turretManager;
    private GameObject mainGun;
    private float reloadTimer;

    private int bulletLayer;
    private LayerMask targetLayer;

    

    private void Awake()
    {


        turretManager = GetComponentInParent<TurretManager>();
        Rigidbody2D rootRb = GetComponentInParent<Rigidbody2D>();
        
        
        if(rootRb != null)
        {
            mainGun = rootRb.gameObject;
            SetLayer();
        }
        maxGaugeScale = reloadGaugeFill.localScale;
        reloadGauge.SetActive(true);
        reloadTimer = fireCoolDown;
    }

    private void Start()
    {
        turretManager.RegisterTurret(this);
    }

    public void Update()
    {
        if (reloadTimer < fireCoolDown)
        {
            reloadTimer += Time.deltaTime;
        }
        UpdateReloadGauge();
    }

    private void UpdateReloadGauge()
    {
        if (reloadGaugeFill == null)
        {
            return;
        }

        ratio = reloadRatio;
        scale = maxGaugeScale;
        scale.x *= maxGaugeScale.x * ratio;

        reloadGaugeFill.localScale = scale;



    }

    public void Aim(Vector2 targetPosition)
    {
        
        Vector2 direction = targetPosition - (Vector2)transform.position;
        if(direction == Vector2.zero)
        {
            return;
        }

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;

        float nextAngle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);

        transform.rotation = Quaternion.Euler(0f, 0f, nextAngle);

    }
    public void TryFire()
    {
        if(reloadTimer < fireCoolDown)
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

}
