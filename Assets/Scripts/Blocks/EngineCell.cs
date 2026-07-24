using UnityEngine;

public class EngineCell : TankCell
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
        Debug.Log($"Engine 활성화됨 {RootStatus},{engineOutput}");

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

  
}
