using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    private GameState stateSave;
    public static int bestKillCount;
    public int killCount;
    int currentAlert;
    [SerializeField] public int waveUpgrade= 5;
    [SerializeField] TankStatus tankStatus;
    
    
    

    private GameState state;
    public GameState State => state;

    private void Awake()
    {
        Debug.Log("GameManager 작동");    //test
        tankStatus = GetComponent<TankStatus>();

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        killCount = 0;
        MobSpawner.instance.alert = 1;
        state = GameState.Playing;
        UIManager.instance.KillCountText(killCount);
        UIManager.instance.AlertCountText(1);
    }
    public void AddKillCount()   // 킬카운트로 교체
    {
        killCount++;
        UIManager.instance.KillCountText(killCount);
        
    }

    public void GameStart()
    {
        state = GameState.Playing;
        Time.timeScale = 1;
    }

    // Update is called once per frame
    public void GameOver()
    {
        Debug.Log("게임오버");
        state = GameState.GameOver;

        if(killCount > bestKillCount)
        {
            bestKillCount = killCount;
        }

        currentAlert = MobSpawner.instance.alert;
        UIManager.instance.ShowGameOver(killCount, currentAlert);
        

    }

    public void GamePause()
    {
        state = GameState.Pause;
        Time.timeScale = 0;
    }

    public void GameUnpause()
    {
        state = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void Restart() 
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartScene");
    }

    public void IncreaseAlert()
    {
        if (MobSpawner.instance.alert == 5)
        {
            return;
        }

        if (killCount % waveUpgrade == 0)
        {
            MobSpawner.instance.alert++;
            switch (MobSpawner.instance.alert)
            {
                case 1:
                    tankStatus.MaxWeight = 120;
                    break;
                case 2:
                    tankStatus.MaxWeight = 320;
                    break;
                case 3:
                    tankStatus.MaxWeight = 750;
                    break;
                case 4:
                    tankStatus.MaxWeight = 1600;
                    break;
                case 5:
                    tankStatus.MaxWeight = 3200;
                    break;
                default:
                    break;

            }
            UIManager.instance.AlertCountText(MobSpawner.instance.alert);
        }

    }
    public void EnterPause()
    {
        if (state == GameState.Pause)
        {
            return;
        }

        stateSave = state;
        state = GameState.Pause;
    }

    public void ExitPause()
    {
        if (state != GameState.Pause)
        {
            return;
        }

        state = stateSave;
    }
    public bool MaintenanceCall()
    {
        if(state != GameState.Playing)
        {
            return false;
        }

        state = GameState.MaintenanceCall;
        Debug.Log("정비 지원 호출");
        return true;
    }

    public void EnterMaintenance()
    {
        state = GameState.Maintenance;
        Debug.Log("정비 진입");
    }
    public void ExitMaintenance()
    {
        state = GameState.Playing;
        Debug.Log("정비 종료");
    }


}
