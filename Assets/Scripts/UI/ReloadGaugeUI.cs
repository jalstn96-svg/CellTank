using UnityEngine;
using UnityEngine.UI;


// 인식용 component
public class ReloadGaugeUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    public void SetValue(float ratio)
    {
        slider.value = Mathf.Clamp01(ratio);
    }
}
