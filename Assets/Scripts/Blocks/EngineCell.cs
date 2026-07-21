using UnityEngine;

public class EngineCell : TankCell, Ihittable
{
    [Header("Engine property")]
    [SerializeField] private float engineOutput;
    public float EngineOutput => engineOutput;

    protected override void OnDisabled()
    {
        
    }
    protected override void OnRestored()
    {

    }
    protected override void OnActivate()
    {
        if(RootStatus == null)
        {
            Debug.Log("RootStatus null");
            return;
        }
        RootStatus.AddEngineOutput(engineOutput);
    }
    protected override void OnDeactivate()
    {
        if (RootStatus == null)
        {
            Debug.Log("RootStatus null");
            return;
        }
        RootStatus.RemoveEngineOutput(engineOutput);
    }

    public void TakeDamage(int damage)
    {
        throw new System.NotImplementedException();
    }
}
