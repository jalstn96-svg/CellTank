using UnityEngine;

public abstract class CoreCell : TankCell
{
    //public void TakeDamage(int damage)
    //{
    //    CalcCoreDamage(damage);
    //}
    public override ProjectileHitResult Hit(ref ProjectileHitInit hitInit)
    {
        CalcCoreDamage(hitInit.damage);

        return ProjectileHitResult.Hitted;
    }


    public override void TakeDamage(float damage)
    {
        Debug.Log("core는 일반적인 데미지를 받지 않음.");
    }


    protected abstract void CalcCoreDamage(int damage);

}
