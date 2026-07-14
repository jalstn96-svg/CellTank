using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float speed = 10f;
    [SerializeField] int damage;
    [SerializeField] float lifeTime;
    [SerializeField] float penetration;
    
    Rigidbody2D rb;
    LayerMask targetLayer;
    Vector2 shotDir;
    bool isInit;
    private GameObject mainGun;    // 오브젝트 변수명 검토
    float lifeTimer;

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

    public void Init(Vector2 dir, float _speed, int _damage, float _lifeTime,float _penetration, LayerMask _targetLayer, GameObject rootObject)
    {
        shotDir = dir.normalized;
        speed = _speed;
        damage = _damage;
        lifeTime = _lifeTime;
        penetration = _penetration;
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

        
        if (other.TryGetComponent(out Ihittable target))
        {
            target.TakeDamage(damage);
            ObjectPool.instance.ReturnObject("Bullet",gameObject);
            return;
        }

        //Monster monster = other.GetComponent<Monster>();

        //if (monster != null)
        //{
        //    monster.TakeDamage(damage);
        //    Destroy(gameObject);
        //    return;
        //}

        
    }


}
