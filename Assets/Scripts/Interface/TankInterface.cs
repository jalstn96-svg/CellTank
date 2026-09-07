using UnityEngine;

public enum ProjectileHitResult
{
    Passed, // 0
    Hitted,
    Penetrated,
    Ricochet,
    Immuned,
}

public struct ProjectileHitInit
{
    public int damage;  // core에 대한 피해량
    public float power; // Cell의 내구도에 대한 피해량
    public float penetration; // ArmorCell에 대한 관통력
    public Vector2 direction; // 투사체 방향 vector
    public Vector2 cellSurface; // 투사체가 입사한 cell의 법선 각도

    public ProjectileHitInit(int _damage, float _power, float _penetration, Vector2 _direction, Vector2 _cellSurface)
    {
        damage = _damage;
        power = _power;
        penetration = _penetration;
        direction = _direction;
        cellSurface = _cellSurface;

    }
}




public interface Ihittable
{
    ProjectileHitResult Hit(ref ProjectileHitInit hitInit);

}

