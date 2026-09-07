
using UnityEngine;
using System.Collections.Generic;

public class TurretManager : MonoBehaviour
{

    
    private bool isAbled = true;

    private readonly List<ITurret> activeTurrets = new List<ITurret>();

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

        
    }

    

   

    public void TryFire()
    {
        foreach(ITurret turret in activeTurrets)
        {
            turret.TryFire();
        }



        //GameObject bulletObject = ObjectPool.instance.GetObject("Bullet");  //추후 탄 구분 시 수정(탄 넘버링)
        
        //if(bulletObject == null)    // object pool에 탄 없음(버그대비)
        //{
        //    Debug.Log("objectpool empty error");
        //    return;
        //}

        //bulletObject.transform.SetPositionAndRotation(bulletStartPosition.position, bulletStartPosition.rotation);
        //bulletObject.layer = bulletLayer;

        //Bullet bullet = bulletObject.GetComponent<Bullet>();

        //bullet.Init(bulletStartPosition.up, bulletSpeed, bulletPower,bulletDamage, bulletLifeTime, penetration, targetLayer, mainGun);

        //reloadTimer = 0f;

    }
    

    

    
    

    public void RegisterTurret(ITurret turret)
    {
        if(turret == null)
        {
            Debug.Log("turret 인식안됨.");
            return;
        }
        if (activeTurrets.Contains(turret))
        {
            Debug.Log("활성화 목록에 이미 존재함.");
            return;
        }
        activeTurrets.Add(turret);

    }



    public void UnregisterTurret(ITurret turret)
    {
        if(turret == null)
        {
            Debug.Log("turret 인식안됨.");
            return;
        }
        activeTurrets.Remove(turret);

    }

    public void Aim(Vector2 targetPosition)
    {
        foreach(ITurret turret in activeTurrets)
        {
            turret.Aim(targetPosition);
        }
    }
    

}
