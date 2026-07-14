using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, Ihittable
{
    [Header("Move")]
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotateSpeed = 45f;

    float moveInput;
    float rotateInput;

    [Header("HP")]
    [SerializeField] int maxHp = 5;
    int currentHp;

    [Header("Turret")]
    [SerializeField] Transform Turret;
    [SerializeField] Transform BulletStart;
    [SerializeField] float turretRotateSpeed = 45f;

    [Header("Fire")]
    [SerializeField] TurretManager turretManager;
    [SerializeField] GameObject Bullet;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float fireCooldown = 0.3f;

    Rigidbody2D rb;
    Vector2 dir;
    float lastFireTime;

    private Camera mainCamera;
    
    
    void Awake()
    {
        mainCamera = Camera.main; 

        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;

        currentHp = maxHp;
    }

    //void Start()
    //{
        
    //}

    
    void Update()
    {
        MoveInput();
        TurretInput();
        Fire();
    }

    void FixedUpdate()
    {
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
        float rotate = rotateInput * rotateSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation + rotate);

        // 차체 전후진
        Vector2 dir = transform.up;
        rb.linearVelocity = dir * moveInput * moveSpeed;
    }

    void Fire()
    {
        if (Mouse.current.leftButton.isPressed == true)
        {
            turretManager.TryFire();

        }
        

    }
    //void TurretInput()
    //{
    //    Vector2 aimDir = Vector2.zero;

    //    if (Keyboard.current.upArrowKey.isPressed)
    //    {
    //        aimDir.x = 1f;
    //    }
    //    if (Keyboard.current.downArrowKey.isPressed)
    //    {
    //        aimDir.x = -1f;
    //    }
    //    if (Keyboard.current.leftArrowKey.isPressed)
    //    {
    //        aimDir.y = 1f;
    //    }
    //    if (Keyboard.current.rightArrowKey.isPressed)
    //    {
    //        aimDir.y = -1f;
    //    }
    //    if(aimDir == Vector2.zero)
    //    {
    //        return;
    //    }

    //    aimDir = aimDir.normalized;
    //    float targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg;
    //    float currentAngle = Turret.eulerAngles.z;

    //    float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turretRotateSpeed*Time.deltaTime);

    //    Turret.rotation = Quaternion.Euler(0f, 0f, angle);

    //}

    void TurretInput()
    {
        Vector2 aimDir = Vector2.zero;


        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(mousePos);

        aimDir = mouseWorldPos - (Vector2)Turret.position;

        //if (Keyboard.current.upArrowKey.isPressed)
        //{
        //    aimDir.x = 1f;
        //}
        //if (Keyboard.current.downArrowKey.isPressed)
        //{
        //    aimDir.x = -1f;
        //}
        //if (Keyboard.current.leftArrowKey.isPressed)
        //{
        //    aimDir.y = 1f;
        //}
        //if (Keyboard.current.rightArrowKey.isPressed)
        //{
        //    aimDir.y = -1f;
        //}
        //if (aimDir == Vector2.zero)
        //{
        //    return;
        //}

        aimDir = aimDir.normalized;
        float targetAngle = Mathf.Atan2(aimDir.y, aimDir.x) * Mathf.Rad2Deg -90f;
        float currentAngle = Turret.eulerAngles.z;

        float angle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, turretRotateSpeed * Time.deltaTime);

        Turret.rotation = Quaternion.Euler(0f, 0f, angle);

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
        GameManager.instance.GamePause();
        Destroy(gameObject);
    }

}
