
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI bestKillCountText;


    private void Start()
    {
        if(bestKillCountText != null)
        {
            bestKillCountText.text = $"Best Kill Count : {GameManager.bestKillCount}";
        }
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");

    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif

    }


}
