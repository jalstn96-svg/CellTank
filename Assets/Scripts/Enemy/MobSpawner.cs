using UnityEngine;


public class MobSpawner : MonoBehaviour
{
    public static MobSpawner instance;

    

    public GameObject enemy;


    [Header("스폰 타이머")]
    [SerializeField] float spawnCoolTime = 5f;     //계획은 40f,
    float spawnTimer;
    bool isPaused;

    [Header("스폰 영역")]
    [SerializeField] float maxSpawnDistance;
    [SerializeField] float minSpawnDistance;
    Camera mainCamera;
    float cameraEdge;
    float x, y;

    [Header("스폰 개체 제한")]
    [SerializeField] int maxSpawnedEnemy = 3;
    public int countSpawnedEnemy;

    [Header("경계도")]
    [SerializeField] public int alert;



    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        mainCamera = Camera.main;
    }

    private void Start()
    {
        countSpawnedEnemy = 0;
        alert = 1;
    }

    private void Update()
    {
        EnemySpawnedCheck();
        if (isPaused)
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnCoolTime)
        {
            spawnTimer = 0f;
            SummonEnemy();
            countSpawnedEnemy++;
        }
    }


    private void SummonEnemy()
    {

        Vector2 spawnPosition = SpawnPosition();


        GameObject enemyObject = EnemyPool.instance.GetObject("Enemy3"); // prefab 이름    (추후 수정 필요)

        enemyObject.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        //Instantiate(enemy, spawnPosition, Quaternion.identity);

        
    }

    private Vector2 SpawnPosition()
    {
        while (true)
        {
            x = Random.Range(-1f, 2f);
            y = Random.Range(-1f, 2f);
            if (x <= -0.5f || x >= 1.5f || y >= 1.5f || y <= -0.5f)
            {
                break;
            }
            
            
        }

        //카메라 범위는 좌측하단(0,0)부터 우측 상단(1,1)까지로 정규화
        Vector3 spawnPositionExpCamera = mainCamera.ViewportToWorldPoint(new Vector3(x , y));    // 카메라 외곽 경계선 이후 x만큼, y만큼 결정
        //spawnPositionExpCamera.z = 0f;
        return spawnPositionExpCamera;


    }

    public void ResetTimer()    // 타이머'만' 초기화
    {
        spawnTimer = 0f;
    }

    public void PauseTimer()  // 타이머'만' 정지 == 스폰 중지
    {
        isPaused = true;
    }
    public void ResumeTimer()
    {
        isPaused = false;
    }
    private void EnemySpawnedCheck()
    {
        if (countSpawnedEnemy >= 3)
        {
            PauseTimer();
        }
        else if(isPaused = true && countSpawnedEnemy<3)
        {
            ResumeTimer();
        }
    }
    public void DecreaseEnemyCount()
    {
        if (countSpawnedEnemy > 0)
        {
            countSpawnedEnemy--;
        }
        else Debug.Log("몹 개체 카운트 오류");
    }

    


}
