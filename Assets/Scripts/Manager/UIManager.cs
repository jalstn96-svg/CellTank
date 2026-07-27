using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [Header("Combat HUD")]
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI alertCountText;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI resultKillText;
    [SerializeField] private TextMeshProUGUI resultAlertText;

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
    }

    void Start()
    {
        KillCountText(0);
        AlertCountText(1);
        // 이미 존재하는 gameover 패널 종료
        if(gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }


    // Update is called once per frame
    public void KillCountText(int killCount)
    {
        killCountText.text = $"kill count : {killCount}";
    }
    public void AlertCountText(int alert)
    {
        alertCountText.text = $"Alert : {alert}";
    }

    public void ShowGameOver(int killCount, int maxAlert)
    {
        gameOverPanel.SetActive(true);

        resultKillText.text = $"{killCount} kill";
        resultAlertText.text = $"Highest Alert : {maxAlert}";


    }

}
