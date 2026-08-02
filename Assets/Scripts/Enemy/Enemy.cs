using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Hp")]
    [SerializeField] private int maxHp = 3;
    int currentHp;

    [Header("Movement")]
    float moveSpeed = 4f;
    float rotateSpeed = 30f;
    float attachRange = 6f;   // 사거리는 탄 성능에 맞게 => 포탑의 bullet 인식
    float turretRotateSpeed = 30f;

    [Header("Target")]
    [SerializeField] Transform playerPosition;
    float inRange = 20f;        // 사격 사거리
    [SerializeField] LayerMask playerLayer;



    [Header("Fire")]
    [SerializeField] TurretManager turretManager;


    [Header("Status")]
    [SerializeField] private TankStatus tankStatus;
    private string poolId;
    private bool isDead;
    private TankCell[] cells;

    [Header("Cell Drop")]
    
    [SerializeField] private float cellDropMove = 0.1f;


    
    
    //[Header("Drop")]
    //[SerializeField] GameObject CoinPrefab;


    Rigidbody2D rb;
    float distance;
    //float fireTimer;


    private void Awake()    // ObjectPool 쓸시 교체
    {
        currentHp = maxHp;
        rb = GetComponent<Rigidbody2D>();
        cells = GetComponentsInChildren<TankCell>(true);
        tankStatus = GetComponent<TankStatus>();
    }
    void Start()
    {
        if (playerPosition != null)
        {
            return;

        }

        CorePhysics player = FindAnyObjectByType<CorePhysics>();

        if(player != null)
        {
            playerPosition = player.transform;
        }

        


    }

    void OnEnable()
    {
        isDead = false;
        currentHp = maxHp;
        //fireTimer = 0f;
        ResetEnemyCells();
        if(rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

    }

    void Update()
    {
        if (playerPosition == null)
        {
            return;
        
        }

        turretManager.Aim(playerPosition.position);

        if (PlayerInRange() == false)
        {
            return;
        }

        turretManager.TryFire();
    }

    private void ResetEnemyCells()
    {
        foreach (TankCell cell in cells)
        {
            cell.ResetCell();
        }
    }
    private void FixedUpdate()
    {
        if (playerPosition == null)
        {
            return;
        }

        RotateBody();
        MoveForward();  // 플레이어 추적, 거리조절
    }

    void RotateBody()
    {
        if(playerPosition == null)
        {
            return;
        }

        Vector2 dir = playerPosition.position - transform.position; // Vector2로 player 방향 추적

        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;    // PreFab 이미지가 위를 보고 생성됨..

        float nextAngle = Mathf.MoveTowardsAngle(rb.rotation, targetAngle, tankStatus.RotateSpeed*Time.fixedDeltaTime);    //이동할 각도방향 및 속도 설정

        rb.MoveRotation(nextAngle); //실제 next angle로 차체 회전 시작

    }

    
    void MoveForward()
    {
        
        float pveDistance = Vector2.Distance(transform.position, playerPosition.position);

        moveSpeed = tankStatus.Speed;

        if(pveDistance <= inRange-1f)
        {
            rb.linearVelocity = transform.up * moveSpeed * -1;
            
        }
        else if(pveDistance> inRange)    // 여지를 줘야 움직임이 이상해지지 않음.
        {
            rb.linearVelocity = transform.up * moveSpeed;
        }
        else
            rb.linearVelocity = Vector2.zero;


    }


    bool PlayerInRange()
    {
        Vector2 myPos = transform.position;
        Vector2 dir = (Vector2)playerPosition.position - myPos;

        distance = dir.magnitude;

        if (distance > inRange)
        {
            return false;
        }

        if (dir == Vector2.zero)
        {
            return false;
        }

        //dir = dir.normalized;
        //RaycastHit2D shotRay = Physics2D.Raycast(myPos, dir, inRange, playerLayer);

        //if(shotRay.collider == null)
        //{
        //    return false;
        //}

        //CorePhysics player = shotRay.collider.GetComponentInParent<CorePhysics>();

        return true;
    }

    public ProjectileHitResult Hit(ref ProjectileHitInit hitInit)
    {
        TakeDamage(hitInit.damage);

        return ProjectileHitResult.Hitted;
    }

    public void TakeDamage(int damage)  // 코어 데미지 계산
    {
        currentHp -= damage;

        Debug.Log($"적 체력 : {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }

    //void Aim()
    //{
    //    Vector2 dir = playerPosition.position - Turret.position;

    //    if (dir == Vector2.zero)    // 유저 위치 확인 안됨 -> 반환
    //    {
    //        return; 
    //    }

    //    float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg-90f;    // 현재 이미지가 위를 보고있음.

    //    float currentAngle = Turret.eulerAngles.z;
    //    float targetAngle = Mathf.MoveTowardsAngle(currentAngle, angle, turretRotateSpeed * Time.deltaTime);


    //    Turret.rotation = Quaternion.Euler(0f, 0f, targetAngle);
    //}

    //void FireTimer()
    //{
    //    fireTimer += Time.deltaTime;

    //    if (fireTimer >= fireCooldown)
    //    {
    //        fireTimer = 0f;
    //        Fire();
    //    }
    //}

    //void Fire()
    //{


    //    GameObject BulletObj = Instantiate(BulletPrefab, BulletStart.position, BulletStart.rotation);
        

    //    Bullet bullet = BulletObj.GetComponent<Bullet>();
    //    bullet.Init(BulletStart.up, bulletDamage, LayerMask.GetMask("Player","Enemy"),gameObject);

    //}

    public void SetPoolId(string _poolId)
    {
        poolId = _poolId;
    }

    private void Die()
    {
        if(isDead == true)
        {
            return;
        }

        Debug.Log("적 사망");
        isDead = true;
        DropCell();

        //if(CoinPrefab != null)
        //{
        //    Instantiate(CoinPrefab, transform.position, Quaternion.identity);
        //}

        MobSpawner.instance.DecreaseEnemyCount();
        GameManager.instance.AddKillCount();
        GameManager.instance.IncreaseAlert();


        EnemyPool.instance.ReturnObject(poolId, gameObject);  // 추후 수정 필요
        //Destroy(gameObject);
    }

    public void RemoveByAirDrop()
    {
        MobSpawner.instance.DecreaseEnemyCount();
        EnemyPool.instance.ReturnObject(poolId, gameObject);
    }

    // pool로 돌아가는 enemy의 cell을 복사하여 투척
    private void DropCell()
    {
        foreach(TankCell cell in cells)
        {
            if (cell == null)
            {
                continue;
            }

            // 코어는 드랍x
            if(cell is CoreCell)
            {
                continue;


            }

            // 파괴된 cell은 드랍x
            if (cell.IsDisabled == true)
            {
                continue;
            }

            GameObject droppedObject = Instantiate(cell.gameObject, cell.transform.position, cell.transform.rotation);

            

            droppedObject.transform.SetParent(null);

            TankCell droppedCell = droppedObject.GetComponent<TankCell>();

            if(droppedCell == null)
            {
                Destroy(droppedObject);
                continue;
            }

            droppedCell.PrepareAsDrop();

            
            Rigidbody2D droppedRb = droppedObject.AddComponent<Rigidbody2D>();
            


            droppedRb.bodyType = RigidbodyType2D.Dynamic;
            droppedRb.gravityScale = 0f;
            droppedRb.interpolation = RigidbodyInterpolation2D.Interpolate;

            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            droppedRb.linearVelocity = randomDirection * cellDropMove;



        }
    }

    



}