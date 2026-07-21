using UnityEngine;

public class PlayerCoreCell : CoreCell
{
    [SerializeField] private CorePhysics corePhysics;

    protected override void Awake()
    {
        base.Awake();
        if(corePhysics == null)
        {
            corePhysics = GetComponent<CorePhysics>();
        }
        if (corePhysics == null)
        {
            corePhysics = GetComponentInParent<CorePhysics>();
        }
        if(corePhysics == null)
        {
            Debug.Log("corePhysics 찾지 못함");
        }
    }

    protected override void CalcCoreDamage(int damage)
    {
        corePhysics.TakeDamage(damage);
    }
}
