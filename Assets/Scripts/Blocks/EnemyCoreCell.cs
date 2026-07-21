using UnityEngine;

public class EnemyCoreCell : CoreCell
{
    [SerializeField] private Enemy enemy;


    protected override void Awake()
    {
        base.Awake();
        if (enemy == null)
        {
            enemy = GetComponent<Enemy>();
        }
        if (enemy == null)
        {
            enemy = GetComponentInParent<Enemy>();
        }
        if (enemy == null)
        {
            Debug.Log("Enemy 찾지 못함");
        }
    }

    protected override void CalcCoreDamage(int damage)
    {
        enemy.TakeDamage(damage);
    }

    
}
