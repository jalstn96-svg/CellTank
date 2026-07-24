using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    //[SerializeField] int damage;
    [SerializeField] float lifeTime;
    //[SerializeField] float penetration;
    //[SerializeField] float power;
    [SerializeField] private Transform rayOrigin;

    [SerializeField] private float ricochetSpeedRatio = 2f;
    Rigidbody2D rb;
    LayerMask targetLayer;
    Vector2 shotDir;
    bool isInit;
    private GameObject mainGun;    // 오브젝트 변수명 검토
    float lifeTimer;

    private ProjectileHitInit hitInit;


    [SerializeField] private Collider2D bulletCollider;
    private bool isRicochet;
    private float ricochetTimer;
    private const float ricochetReturnTime = 0.3f;
    private RigidbodyConstraints2D defaultConstraints;
    private bool defaultIsTrigger;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        if(bulletCollider == null)
        {
            bulletCollider = GetComponent<Collider2D>();
        }
        

        // 각도나 발사방향 초기화
        defaultConstraints= rb.constraints ;
        defaultIsTrigger = bulletCollider.isTrigger;
    }

    void Start()
    {
        
        if(isInit == false)
        {
            shotDir = transform.up;
        }
    }
    void Update()
    {
        if(isRicochet == true)
        {
            ricochetTimer += Time.deltaTime;
            if (ricochetTimer >= ricochetReturnTime)
            {
                ObjectPool.instance.ReturnObject("Bullet", gameObject);
            }
            return;
        }

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            ObjectPool.instance.ReturnObject("Bullet", gameObject);
        }

    }

    void FixedUpdate()
    {
        if (isRicochet == true)
        {
            return;
        }
        float rayDistance = speed * Time.fixedDeltaTime;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, shotDir, rayDistance, targetLayer);

        if (hit.collider != null)
        {
            CheckSensorHit(hit);
            if(gameObject.activeSelf == false)
            {
                return;
            }
        }

        rb.linearVelocity = shotDir * speed;
    }

    public void Init(Vector2 dir, float _speed, float _power, int _damage, float _lifeTime,float _penetration, LayerMask _targetLayer, GameObject rootObject)
    {
        hitInit = new ProjectileHitInit
        {
            damage = _damage,
            power = _power,
            penetration = _penetration,
            direction = dir.normalized,
            cellSurface = Vector2.zero

        };

        shotDir = dir.normalized;
        speed = _speed;
        lifeTime = _lifeTime;
        bulletCollider.isTrigger = true;

        targetLayer = _targetLayer;
        isInit = true;
        mainGun = rootObject;

        lifeTimer = 0f;

        isRicochet = false;
        ricochetTimer = 0f;
        rb.constraints = defaultConstraints;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;

    }
    public void CheckSensorHit(RaycastHit2D hit)
    {
        Collider2D hitCollider = hit.collider;

        if (mainGun != null && hitCollider.transform.root.gameObject == mainGun.transform.root.gameObject)
        {
            return;
        }

        // Ihittable == TankCell 상속된 cell
        if (hitCollider.TryGetComponent(out Ihittable target) == false)
        {
            Debug.Log($"인식 가능한 cell 아님");
            return;
        }
        
        hitInit.direction = shotDir;

        // raycast와 충돌한 표면 법선
        hitInit.cellSurface = hit.normal;

        ProjectileHitResult result = target.Hit(ref hitInit);

        CheckHitResult(result,hit);
        

    }
   
    //void OnTriggerEnter2D(Collider2D other)
    //{
    //    if(mainGun != null && other.transform.root.gameObject == mainGun.transform.root.gameObject)
    //    {
    //        return;
    //    }


    //    if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
    //    {
    //        return;
    //    }

        
    //    if (other.TryGetComponent(out Ihittable target)==false)
    //    {

    //        Debug.Log("Ihittable not founded");
    //        return;

           
    //    }

    //    hitInit.direction = shotDir;
    //    hitInit.cellSurface = Vector2.zero; // 추후 변하도록 수정
    //    ProjectileHitResult result = target.Hit(ref hitInit);
    //    CheckHitResult(result);


    //    //Monster monster = other.GetComponent<Monster>();

    //    //if (monster != null)
    //    //{
    //    //    monster.TakeDamage(damage);
    //    //    Destroy(gameObject);
    //    //    return;
    //    //}
    //}

    private void CheckHitResult(ProjectileHitResult result, RaycastHit2D hit)
    {
        switch (result)
        {
            case ProjectileHitResult.Passed:
            case ProjectileHitResult.Penetrated:
                //pass
                break;
            case ProjectileHitResult.Hitted:
                ObjectPool.instance.ReturnObject("Bullet", gameObject);
                break;

            case ProjectileHitResult.Immuned:
            case ProjectileHitResult.Ricochet:
                StartRicochet(hit.normal,hit.point);
                //도탄 상태
                break;
        }
            

    }

    private void StartRicochet(Vector2 surfaceNormal,Vector2 hitPoint)
    {
        if (isRicochet == true)
        {
            return;
        }

        isRicochet = true;
        ricochetTimer = 0f;

        rb.linearVelocity = Vector2.zero;
        

        bulletCollider.isTrigger = false;
        // z축 freeze 해제
        rb.constraints &= ~RigidbodyConstraints2D.FreezeRotation;

        Vector2 reflectDir = Vector2.Reflect(shotDir, surfaceNormal.normalized).normalized;

        float ricochetSpeed = speed * ricochetSpeedRatio;
        Vector2 impulse = reflectDir * ricochetSpeed;
        rb.AddForceAtPosition(impulse, hitPoint, ForceMode2D.Impulse);
    }

}
