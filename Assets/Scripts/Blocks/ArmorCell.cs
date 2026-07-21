using UnityEngine;

public enum ArmorHitResult
{
    Passed, //  0 = 
    Penetrated, // 관통
    Ricochet,   // 도탄
    Immuned,    // 면역
    Hitted,
}

public class ArmorCell : TankCell
{

    [Header("armor property")]
    [SerializeField] private float thickness;

    public float Thickness => thickness;    // 읽기만

    
    private const float perfectRicochet = 80f;


    //ContactPoint2D    => unity 전용 충돌 계산


    /// <summary>
    /// bullet과 cell 충돌판정 계산
    ///
    /// </summary>
    /// <param name="penetrate"> bullet의 관통력 ref 참조하여 변동 </param>
    /// <param name="power"> bullet의 power ref 참조하여 변동 </param>
    /// <param name="bulletDir"> bullet의 입사각도 (world rotation) </param>
    /// <param name="surfaceDir"> bullet과 충돌한 cell의 면에 대한 법선 각도 (world rotation)</param>
    /// <returns></returns>
    public ArmorHitResult ArmorHitCalculator(ref float penetrate, ref float power, Vector2 bulletDir, Vector2 surfaceDir)
    {
        // 이미 파괴된 cell
        if (IsDisabled)
        {
            return ArmorHitResult.Passed;
        }

        Vector2 direction = bulletDir.normalized;
        Vector2 surface = surfaceDir.normalized;

        // 두 벡터 사이 각도차(내적)
        float cosCal = Mathf.Abs(Vector2.Dot(direction, surface));

        // 각도차를 0~1로 정규화
        cosCal = Mathf.Clamp01(cosCal);

        // cos값을 각도로 변환
        float angle = Mathf.Acos(cosCal) * Mathf.Rad2Deg;

        // 완전 도탄
        if (angle >= perfectRicochet)
        {
            return ArmorHitResult.Immuned;
        }

        float realArmor = thickness / cosCal;
        float realPower = power * cosCal;

        // 관통
        if (penetrate >= realArmor)
        {
            // 음수 보정, 실제 적용값 계산
            power = Mathf.Max(0f, realPower * (1f - realArmor / penetrate));
            penetrate = Mathf.Max(0f, penetrate - realArmor);

            Disabled();

            return ArmorHitResult.Penetrated;
        }

        // 도탄
        TakeDamage(realPower);
        return ArmorHitResult.Ricochet;

    }
    public override ProjectileHitResult Hit(ref ProjectileHitInit hitInit)
    {
        ArmorHitResult result = ArmorHitCalculator(ref hitInit.penetration, ref hitInit.power, hitInit.direction, hitInit.cellSurface);
        
        switch(result)
        {
            case ArmorHitResult.Passed:
                return ProjectileHitResult.Passed;
            case ArmorHitResult.Penetrated:
                return ProjectileHitResult.Penetrated;
            case ArmorHitResult.Ricochet:
                return ProjectileHitResult.Ricochet;
            case ArmorHitResult.Immuned:
                return ProjectileHitResult.Immuned;
            default:
                return ProjectileHitResult.Hitted;
        }

        
        
    }


}