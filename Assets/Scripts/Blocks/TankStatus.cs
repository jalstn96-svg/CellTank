using UnityEngine;

public class TankStatus : MonoBehaviour
{


    private float maxWeight;
    
    private float currentWeight;
    private float engineOutput;


    private float baseSpeed;
    private float speed;
    private float speedRatioAtMaxWeight = 0.5f;
    private float weightSpeedRatio;
    private float weightRatio;

    private Rigidbody2D rb;

    public float MaxWeight => maxWeight;
    public float CurrentWeight => currentWeight;
    public float EngineOutput => engineOutput;
    



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        RefreshStatus();

    }

    public void AddWeight(float weight)
    {
        
        currentWeight += weight;
        RefreshStatus();
    }

    public void RemoveWeight(float weight)
    {
        currentWeight -= weight;
        RefreshStatus();
    }

    public void AddEngineOutput(float enginePower)
    {
        engineOutput += enginePower;
        RefreshStatus();
    }
    public void RemoveEngineOutput(float enginePower)
    {
        engineOutput -= enginePower;
        RefreshStatus();
    }
    
    public void SetMaxWeight(float newMaxWeight)
    {
        maxWeight = newMaxWeight;
        RefreshStatus();
    }

    private void RefreshStatus()
    {
        // 추중비
        weightRatio = currentWeight / maxWeight;

        // 1부터 0.5(최대하중 속도비)까지 weightRatio의 비율로 나눔.
        weightSpeedRatio = Mathf.Lerp(1f, speedRatioAtMaxWeight, weightRatio);

        speed = (baseSpeed + engineOutput) * weightSpeedRatio;

        // 아래는 rigidbody mass
        rb.mass = currentWeight;
        
    }

    
}
