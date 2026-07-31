using UnityEngine;


public class MobSpawner : MonoBehaviour
{
    public static MobSpawner instance;
    private CameraMove cameraMove;
    

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
    int alertLv;
    int variation;
    string poolId;

    private void Awake()
    {
        mainCamera = Camera.main;
        cameraMove = mainCamera.GetComponent<CameraMove>();
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
        GameState state = GameManager.instance.State;

        EnemySpawnedCheck();
        if (isPaused || (state != GameState.Playing && state != GameState.MaintenanceCall))
        {
            return;
        }

        spawnTimer += Time.deltaTime;
        if (spawnTimer > spawnCoolTime)
        {
            spawnTimer = 0f;
            if (SummonEnemy() == true)
            {
                countSpawnedEnemy++;
            }
            
        }
    }


    private bool SummonEnemy()
    {

        Vector2 spawnPosition = SpawnPosition();

        alertLv = Mathf.Clamp(alert, 1, 5);

        variation = Random.Range(1, 5);
        if (alert == 5)
        {
            variation = 1;
        }

        poolId = $"Enemy_V{alertLv}_{variation}";


        GameObject enemyObject = EnemyPool.instance.GetObject(poolId); // prefab 이름    (추후 수정 필요)

        if (enemyObject == null)
        {
            Debug.Log("Enemy prefab not exist");
            return false;
        }

        enemyObject.transform.SetPositionAndRotation(spawnPosition, Quaternion.identity);
        //Instantiate(enemy, spawnPosition, Quaternion.identity);

        Enemy enemyComponent = enemyObject.GetComponent<Enemy>();
        enemyComponent.SetPoolId(poolId);

        return true;
        
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
        Vector3 cameraPosition = mainCamera.transform.position;

        float spawnX = cameraPosition.x + (x - 0.5f)  * cameraMove.MaxZoom * mainCamera.aspect;
        float spawnY = cameraPosition.y + (y - 0.5f)  * cameraMove.MaxZoom * mainCamera.aspect;
        //spawnPositionExpCamera.z = 0f;
        return new Vector2(spawnX, spawnY);


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
