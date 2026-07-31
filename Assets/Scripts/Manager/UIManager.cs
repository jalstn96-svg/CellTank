using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
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

    [Header("Playing UI")]
    [SerializeField] private GameObject playingPanel;

    //[Header("Turret Reload UI")]
    //[SerializeField] private GameObject reloadGaugePrefab;
    //[SerializeField] private Vector3 reloadUIOffset = new Vector3(0f, 1.5f, 0f);
    //private Dictionary<TurretCell, Slider> reloadSliders = new Dictionary<TurretCell, Slider>();



    [Header("MaintenanceCall UI")]
    [SerializeField] private Button maintenanceCallButton;
    [SerializeField] private GameObject waitForSupportUI;
    [SerializeField] private Slider waitForSupportSlider;
    [SerializeField] private TextMeshProUGUI waitForSupportTimeText;

    [Header("Maintenance UI")]
    [SerializeField] private GameObject maintenancePanel;
    [SerializeField] private TankStatus tankStatus;
    [SerializeField] private TextMeshProUGUI weightCompareText;

    [Header("Cell Info UI")]
    [SerializeField] private GameObject cellInfoPanel;
    [SerializeField] private TextMeshProUGUI cellNameText;
    [SerializeField] private TextMeshProUGUI cellTypeText;
    [SerializeField] private TextMeshProUGUI durabilityText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI cellStateText;
    [SerializeField] private TextMeshProUGUI detailText;

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
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        ResetMaintenanceCallUI();
        HideCellInfo();

        if(maintenancePanel != null)
        {
            maintenancePanel.SetActive(false);
        }
        

        
    }

    private void Update()
    {
        if (maintenancePanel.activeInHierarchy==true)
        {
            UpdateWeightUI();
        }
        
    }

    public void UpdateWeightUI()
    {
        weightCompareText.text = $"하중 : {tankStatus.CurrentWeight:0} / {tankStatus.MaxWeight}";
            
    }
    public void HideCellInfo()
    {
        cellInfoPanel.SetActive(false);
    }
    public void ShowCellInfo(TankCell cell)
    {
        if(cell == null)
        {
            HideCellInfo();
            return;
        }

        cellInfoPanel.SetActive(true);
        cellNameText.text = cell.CellName;
        cellTypeText.text = $"Type : {cell.CellTypeName}";

        durabilityText.text = $"HP : {cell.CurrentDurability:0} / {cell.MAxDurability:0}";
        weightText.text = $"무게:{cell.Weight:0}";

        detailText.text = "";

        if(cell is ArmorCell armor)
        {
            detailText.text = $"두께 : {armor.Thickness : 0}";

        }
        if(cell is EngineCell engine)
        {
            detailText.text = $"출력 : {engine.EngineOutput:0}";

        }
        if(cell is TurretCell turret)
        {
            detailText.text = $"cell 공격력 : {turret.BulletPower:0}\n" +
                $"코어 공격력 : {turret.BulletDamage:0}\n" +
                $"관통력 : {turret.Penetration:0}\n" +
                $"탄속 : {turret.BulletSpeed:0}\n" +
                $"재장전 : {turret.FireCoolDown:0}초";
        }


        cellStateText.text = cell.IsDisabled ? "Condition : Broken" : "Condition : Usuable"; // true : false



        

    }


    
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

    public void CallMaintenance(float waitTime)
    {
        //버튼 비활성화
        maintenanceCallButton.interactable = false;
        waitForSupportUI.SetActive(true);

        waitForSupportSlider.minValue = 0f;
        waitForSupportSlider.maxValue = 1f;
        waitForSupportSlider.value = 0f;
        waitForSupportTimeText.text = $"지원까지 {waitTime:F1}초";


    }
    public void UpdateMaintenanceCall(float remainTime, float totalTime)
    {
        float progress = 1f - remainTime / totalTime;

        waitForSupportSlider.value = Mathf.Clamp01(progress);
        waitForSupportTimeText.text = $"지원까지 {remainTime:F1}초";



    }

    public void ShowMaintenanceUI()
    {
        waitForSupportUI.SetActive(false);
        playingPanel.SetActive(false);
        maintenancePanel.SetActive(true);
        UpdateWeightUI();
    }

    public void ShowPlayingUI()
    {
        HideCellInfo();
        maintenancePanel.SetActive(false);
        playingPanel.SetActive(true);

        ResetMaintenanceCallUI();
    }

    public void ResetMaintenanceCallUI()
    {
        waitForSupportUI.SetActive(false);
        waitForSupportSlider.value = 0f;
        waitForSupportTimeText.text = string.Empty;

        maintenanceCallButton.interactable = true;
    }

    //public void RegisterTurretReloadUI(TurretCell turret)
    //{
    //    if(turret == null)
    //    {
    //        return;
    //    }
    //    if (reloadSliders.ContainsKey(turret)==true)
    //    {
    //        return;
    //    }
    //    GameObject gaugeObject = Instantiate(reloadGaugePrefab, turret.transform, false);

    //    gaugeObject.transform.localPosition = reloadUIOffset;
    //    gaugeObject.transform.localRotation = Quaternion.identity;

    //    Slider slider = gaugeObject.GetComponentInChildren<Slider>();

    //    slider.minValue = 0f;
    //    slider.maxValue = 1f;
    //    slider.value = turret.reloadRatio;
    //    slider.interactable = false;

    //    reloadSliders.Add(turret, slider);

    //    }

    //public void UnregisterTurretReloadUI(TurretCell turret)
    //{
    //    if(turret == null)
    //    {
    //        return;
    //    }
    //    if(!reloadSliders.TryGetValue(turret, out Slider slider))
    //    {
    //        return;

    //    }

    //    reloadSliders.Remove(turret);
        
    //    if(slider != null)
    //    {
    //        Destroy(slider.gameObject);
    //    }



    //}
    //private void UpdateReloadUI()
    //{
    //    foreach(KeyValuePair<TurretCell, Slider> pair in reloadSliders)
    //    {
    //        TurretCell turret = pair.Key;
    //        Slider slider = pair.Value;

    //        if(turret == null || slider == null)
    //        {
    //            continue;
    //        }

    //        bool canShow = turret.IsAttached && !turret.IsDisabled;

    //        slider.gameObject.SetActive(canShow);
    //        if (canShow)
    //        {
    //            slider.value = turret.reloadRatio;
    //        }

    //    }

        
    //}

}
