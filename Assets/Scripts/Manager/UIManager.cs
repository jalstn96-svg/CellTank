using UnityEngine;
using TMPro;
public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI alertCountText;



    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
    }

    void Start()
    {
        killCountText.text = "kill count : 0";
        alertCountText.text = $"alert : 1";
    }


    // Update is called once per frame
    public void KillCountText(int killCount)
    {
        killCountText.text = $"kill count : {killCount}";
    }
    public void AlertCountText(int alert)
    {
        alertCountText.text = $"alert : {alert}";
    }
}
