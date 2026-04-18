using UnityEngine;
using UnityEngine.UI;

public class SensitivitySlider : MonoBehaviour
{
    public FirstPersonLook firstPersonLook;
    public Slider sensitivitySlider;

    void Start()
    {
        if (firstPersonLook == null || sensitivitySlider == null)
        {
            return;
        }

        sensitivitySlider.minValue = 0.1f;
        sensitivitySlider.maxValue = 10f;

        sensitivitySlider.value = firstPersonLook.GetSensitivity();
        sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
    }

    void OnSensitivityChanged(float value)
    {
        if (firstPersonLook != null)
        {
            firstPersonLook.SetSensitivity(value);
        }
    }
}