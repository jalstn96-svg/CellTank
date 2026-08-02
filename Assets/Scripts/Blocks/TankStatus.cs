using UnityEngine;

public class TankStatus : MonoBehaviour
{


    [SerializeField]private float maxWeight=120f;
    
    [SerializeField]private float currentWeight = 0f;
    private float engineOutput;


    private float baseSpeed = 10f;
    private float baseRotateSpeed = 45f;
    private float speed;
    private float rotateSpeed;
    private float speedRatioAtMaxWeight = 0.6f;
    private float weightSpeedRatio;
    private float weightRotateRatio;
    private float weightRatio;
    //public bool canAttach;

    private Rigidbody2D rb;

    public float RotateSpeed => rotateSpeed;
    public float Speed => speed;
    
    public float CurrentWeight => currentWeight;
    public float EngineOutput => engineOutput;

    public float MaxWeight 
    {
        get { return maxWeight; } 
        
        set { maxWeight = value; } 
    }
    



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
        weightRatio = engineOutput / currentWeight;

        // 1부터 0.5(최대하중 속도비)까지 weightRatio의 비율로 나눔.
        weightSpeedRatio = Mathf.Sqrt( weightRatio / speedRatioAtMaxWeight);
        weightSpeedRatio = Mathf.Clamp(weightSpeedRatio, 0.35f, 1.25f);


        speed = baseSpeed * weightSpeedRatio;
        rotateSpeed = baseRotateSpeed * weightSpeedRatio;
        // 아래는 rigidbody mass
        rb.mass = 1f + currentWeight;
        
    }

    public bool CanAttach(float cellWeight)
    {
       
         return currentWeight + cellWeight <= maxWeight;
        
    }

    
}
