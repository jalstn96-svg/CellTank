using Unity.VisualScripting;
using UnityEngine;

public class TurretManager : MonoBehaviour
{
    [SerializeField]  Transform bulletStartPosition;

    [Header("Bullet Stats")]
    [SerializeField] float fireCoolDown;

    [SerializeField] float bulletSpeed;
    [SerializeField] int bulletDamage;
    [SerializeField] float bulletLifeTime;
    [SerializeField] float penetration;
    float reloadTimer;

    GameObject mainGun;
    int bulletLayer;    // 탄 레이어 => 타격 판정
    LayerMask targetLayer;  // 맞을 판정 타겟 레이어

    public float reloadRatio => Mathf.Clamp01(reloadTimer / fireCoolDown); // for reloading gauge ui

    public bool canFire => reloadTimer >= fireCoolDown;
    private bool isAbled = true;

    void Awake()
    {
        //mainGun = transform.root.gameObject;  // object pool 사용시 최상위 오브젝트 = 오브젝트풀.

        // 하이어라키 구조 수정 => 폐기
        //Enemy enemy = GetComponentInParent<Enemy>();
        //CorePhysics player = GetComponentInParent<CorePhysics>();

        //if(enemy != null)
        //{
        //    mainGun = enemy.gameObject;
        //}
        //else if(player != null)
        //{
        //    mainGun = player.gameObject;
        //}

        Rigidbody2D rb = GetComponentInParent<Rigidbody2D>();

        if(rb == null)
        {
            Debug.Log("최상위 rigidbody 찾지 못함");
            return;
        }

        mainGun = rb.gameObject;



        reloadTimer = fireCoolDown;
        SetLayer();
    }

    void Update()
    {
        if (isAbled == false)
        {
            return;
        }

        if (reloadTimer < fireCoolDown)
        {
            reloadTimer += Time.deltaTime;
        }
    }

    public void TryFire()
    {
        if (isAbled != true)
        {
            return;
        }
        if (canFire != true) //사격 불가능
        {
            return;
        }

        GameObject bulletObject = ObjectPool.instance.GetObject("Bullet");  //추후 탄 구분 시 수정(탄 넘버링)
        
        if(bulletObject == null)    // object pool에 탄 없음(버그대비)
        {
            Debug.Log("objectpool empty error");
            return;
        }

        bulletObject.transform.SetPositionAndRotation(bulletStartPosition.position, bulletStartPosition.rotation);
        bulletObject.layer = bulletLayer;

        Bullet bullet = bulletObject.GetComponent<Bullet>();

        bullet.Init(bulletStartPosition.up, bulletSpeed, bulletDamage, bulletLifeTime, penetration, targetLayer, mainGun);

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
            targetLayer = LayerMask.GetMask("Player","Enemy");

        }

    }

    public void SetAbled(bool isBool)
    {
        isAbled = isBool;
    }

    public void SetRoot(GameObject root)
    {
        mainGun = root;
        SetLayer();
    }

}
