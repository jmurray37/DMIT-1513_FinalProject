using UnityEngine;
using UnityEngine.UI;

public class BrightnessDifficultySliderUI : MonoBehaviour
{
    public Slider slider;

    void OnEnable()
    {
        if (slider != null && BrightnessDifficultyManager.Instance != null)
        {
            slider.SetValueWithoutNotify(BrightnessDifficultyManager.Instance.brightnessValue);
            slider.onValueChanged.RemoveListener(OnSliderChanged);
            slider.onValueChanged.AddListener(OnSliderChanged);
        }
    }

    void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(OnSliderChanged);
        }
    }

    void OnSliderChanged(float value)
    {
        if (BrightnessDifficultyManager.Instance != null)
        {
            BrightnessDifficultyManager.Instance.SetBrightnessDifficulty(value);
        }
    }
}