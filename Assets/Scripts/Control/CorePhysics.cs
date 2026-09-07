using UnityEngine;
using UnityEngine.InputSystem;

public class CorePhysics : MonoBehaviour
{
    [Header("MinimalMovement")]
    [SerializeField] float moveSpeed = 2f;
    [SerializeField] float rotateSpeed = 30f;

    float moveInput;
    float rotateInput;

    [Header("HP")]
    [SerializeField] int maxHp = 5;
    int currentHp;

    //[Header("Turret")]
    //[SerializeField] Transform Turret;
    //[SerializeField] Transform BulletStart;
    //[SerializeField] float turretRotateSpeed = 45f;

    [Header("Fire")]
    [SerializeField] TurretManager turretManager;
    [SerializeField] GameObject Bullet;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float fireCooldown = 0.3f;

    [Header("Status")]
    [SerializeField] public TankStatus tankStatus;

    //[SerializeField] AssembleManager assembleManager;
    [SerializeField] private Rigidbody2D playerRb;
    //Vector2 dir;
    //float lastFireTime;

    
    private Camera mainCamera;


    private void Awake()
    {
        playerRb = GetComponentInParent<Rigidbody2D>();
        tankStatus = GetComponentInParent<TankStatus>();
        //assembleManager = GetComponentInChildren<AssembleManager>();
        playerRb.gravityScale = 0f;

        currentHp = maxHp;
        mainCamera = Camera.main;
    }

    


    private void Update()
    {
        if(GameManager.instance.State == GameState.MaintenanceCall)
        {
            TurretInput();
            Fire();
            return;
        }

        if(GameManager.instance.State != GameState.Playing)
        {
            return;
        }

        MoveInput();
        TurretInput();
        Fire();
    }

    private void FixedUpdate()
    {
        if (GameManager.instance.State != GameState.Playing)
        {
            return;
        }

        MoveTank();
    }


    void MoveInput()
    {
        moveInput = 0f;
        rotateInput = 0f;

        if (Keyboard.current.aKey.isPressed)
        {
            rotateInput = 1f;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            rotateInput = -1f;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            moveInput = 1f;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            moveInput = -1f;
        }



    }
    void MoveTank()
    {
        // 차체 회전
        float rotate = rotateInput * tankStatus.RotateSpeed * Time.fixedDeltaTime;
        playerRb.MoveRotation(playerRb.rotation + rotate);

        // 차체 전후진
        Vector2 dir = transform.up;
        playerRb.linearVelocity = dir * moveInput * tankStatus.Speed;
    }

    void Fire() // 사격 => 상속으로
    {
        if (Mouse.current.leftButton.isPressed == true)
        {
            turretManager.TryFire();
            //Time.timeScale = 1;   // for test(RemoveCell)
            //assembleManager.RemoveCell(new Vector2Int(10,1),assembleManager.testCell);
            
        }
    

        //lastFireTime = Time.time;

        // 이하 수정
        //GameObject BulletObj = Instantiate(Bullet, BulletStart.position, BulletStart.rotation);
        //BulletObj.layer = LayerMask.NameToLayer("PlayerBullet");    // 유저가 쏜 탄 레이어
        //Bullet bullet = BulletObj.GetComponent<Bullet>();
        //bullet.Init(BulletStart.right, bulletDamage, LayerMask.GetMask("Enemy"),gameObject);

    }
    void TurretInput()
    {
        //Vector2 aimDir = Vector2.zero;


        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

        turretManager.Aim(mouseWorldPos);

    }



    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"플레이어 Hp : {currentHp}");

        if (currentHp <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        Debug.Log("Player 사망");
        GameManager.instance.GameOver();
        
        Destroy(gameObject);

        //GameManager.instance.GamePause();
    }

    
}
