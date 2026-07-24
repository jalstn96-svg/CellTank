using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    //[SerializeField] int damage;
    [SerializeField] float lifeTime;
    //[SerializeField] float penetration;
    //[SerializeField] float power;
    Rigidbody2D rb;
    LayerMask targetLayer;
    Vector2 shotDir;
    bool isInit;
    private GameObject mainGun;    // 오브젝트 변수명 검토
    float lifeTimer;

    private ProjectileHitInit hitInit;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
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
        lifeTimer += Time.deltaTime;
        if (lifeTimer >= lifeTime)
        {
            ObjectPool.instance.ReturnObject("Bullet", gameObject);
        }

    }

    void FixedUpdate()
    {
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
        
        targetLayer = _targetLayer;
        isInit = true;
        mainGun = rootObject;

        lifeTimer = 0f;

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(mainGun != null && other.transform.root.gameObject == mainGun.transform.root.gameObject)
        {
            return;
        }


        if ((targetLayer.value & (1 << other.gameObject.layer)) == 0)
        {
            return;
        }

        
        if (other.TryGetComponent(out Ihittable target)==false)
        {

            Debug.Log("Ihittable not founded");
            return;

           
        }

        hitInit.direction = shotDir;
        hitInit.cellSurface = Vector2.zero; // 추후 변하도록 수정
        ProjectileHitResult result = target.Hit(ref hitInit);
        CheckHitResult(result);


        //Monster monster = other.GetComponent<Monster>();

        //if (monster != null)
        //{
        //    monster.TakeDamage(damage);
        //    Destroy(gameObject);
        //    return;
        //}


    }

    private void CheckHitResult(ProjectileHitResult result)
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
                ObjectPool.instance.ReturnObject("Bullet", gameObject);
                //도탄 상태
                break;
        }
            

    }

}
