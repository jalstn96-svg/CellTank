using UnityEngine;



public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    public int killCount;
    [SerializeField] public int waveUpgrade= 5;
    

    private GameState state;
    

    private void Awake()
    {
        Debug.Log("GameManager 작동");    //test

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
    }

    public void GamePause()
    {
        state = GameState.Pause;
        Time.timeScale = 0;
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
            UIManager.instance.AlertCountText(MobSpawner.instance.alert);
        }

    }



}
