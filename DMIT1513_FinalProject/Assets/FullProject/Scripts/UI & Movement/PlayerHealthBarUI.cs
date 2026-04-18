using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBarUI : MonoBehaviour
{
    [Header("References")]
    public PlayerHealth playerHealth;
    public RectTransform fillRect;
    public Image fillImage;

    [Header("Sizing")]
    public float maxBarWidth = 200f;
    public bool smoothResize = true;
    public float resizeSpeed = 8f;

    [Header("Low Health Pulse")]
    public bool enableLowHealthPulse = true;
    public float lowHealthThreshold = 0.35f;
    public float pulseSpeed = 6f;
    public float pulseScaleAmount = 0.08f;
    public float pulseColorAmount = 0.25f;

    private float displayedWidth;
    private Vector3 originalScale;
    private Color baseColor;

    void Start()
    {
        if (fillRect != null)
        {
            displayedWidth = maxBarWidth;
            fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, displayedWidth);
            originalScale = fillRect.localScale;
        }

        if (fillImage != null)
        {
            baseColor = fillImage.color;
        }

        if (playerHealth != null)
        {
            float startWidth = maxBarWidth * playerHealth.HealthPercent;
            displayedWidth = startWidth;

            if (fillRect != null)
            {
                fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, displayedWidth);
            }
        }
    }

    void Update()
    {
        if (playerHealth == null || fillRect == null)
        {
            return;
        }

        UpdateBarWidth();
        UpdatePulse();
    }

    void UpdateBarWidth()
    {
        float targetWidth = maxBarWidth * playerHealth.HealthPercent;

        if (smoothResize)
        {
            displayedWidth = Mathf.Lerp(displayedWidth, targetWidth, resizeSpeed * Time.deltaTime);

            if (Mathf.Abs(displayedWidth - targetWidth) < 0.5f)
            {
                displayedWidth = targetWidth;
            }
        }
        else
        {
            displayedWidth = targetWidth;
        }

        fillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, displayedWidth);
    }

    void UpdatePulse()
    {
        if (!enableLowHealthPulse || fillImage == null)
        {
            if (fillRect != null)
            {
                fillRect.localScale = originalScale;
            }

            if (fillImage != null)
            {
                fillImage.color = baseColor;
            }

            return;
        }

        float hp = playerHealth.HealthPercent;

        if (hp <= lowHealthThreshold && hp > 0f)
        {
            float pulse = Mathf.Sin(Time.time * pulseSpeed);

            float scaleOffset = pulse * pulseScaleAmount;
            fillRect.localScale = originalScale + new Vector3(scaleOffset, scaleOffset, 0f);

            float colorOffset = (pulse * 0.5f + 0.5f) * pulseColorAmount;
            Color pulseColor = baseColor;
            pulseColor.r = Mathf.Clamp01(baseColor.r + colorOffset);
            fillImage.color = pulseColor;
        }
        else
        {
            fillRect.localScale = originalScale;
            fillImage.color = baseColor;
        }
    }
}