using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject settingPanel;

    [Header("Scene")]
    [SerializeField] private string mainScene = "StartScene";

    [Header("Sound Settings")]
    [SerializeField] private Slider fireVolumeSlider;
    [SerializeField] private Slider ricochetVolumeSlider;
    [SerializeField] private Slider hitVolumeSlider;
    [SerializeField] private Slider destroyedVolumeSlider;

    private bool isOpen;
    private float previousTimeScale = 1f;

    


    private void Awake()
    {
        if(settingPanel != null)
        {
            settingPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if(Keyboard.current == null)
        {
            return;

        }

        if (!Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }
        if (CanOpenSetting())
        {
            OpenSetting();
        }

    }

    private bool CanOpenSetting()
    {
        GameState state = GameManager.instance.State;

        return state == GameState.Playing || state == GameState.Maintenance || state == GameState.MaintenanceCall;
    }

    public void OpenSetting()
    {
        if (settingPanel == null)
        {
            Debug.Log("설정창 지정 안됨");
            return;
        }
        GameManager.instance.EnterPause();
        isOpen = true;

        previousTimeScale = Time.timeScale;

        
        

        settingPanel.SetActive(true);
        if (SFXManager.instance != null)
        {
            fireVolumeSlider.SetValueWithoutNotify(SFXManager.instance.FireVolume);
            ricochetVolumeSlider.SetValueWithoutNotify(SFXManager.instance.RicochetVolume);
            hitVolumeSlider.SetValueWithoutNotify(SFXManager.instance.HitVolume);
            destroyedVolumeSlider.SetValueWithoutNotify(SFXManager.instance.DestroyedVolume);
        }


        Time.timeScale = 0f;
    }

    public void CloseSetting()
    {
        PlayerPrefs.Save();

        settingPanel.SetActive(false);

        isOpen = false;
        GameManager.instance.ExitPause();
        Time.timeScale = previousTimeScale;
    }

    public void ReturnToMain()
    {
        PlayerPrefs.Save();

        Time.timeScale = 1f;
        isOpen = false;
        settingPanel.SetActive(false);

        SceneManager.LoadScene(mainScene);
    }
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}
