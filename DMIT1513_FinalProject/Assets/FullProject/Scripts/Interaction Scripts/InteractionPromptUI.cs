using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("References")]
    public GameObject rootObject;
    public Image keyIconImage;
    public TMP_Text promptText;

    [Header("Display")]
    public Sprite keySprite;
    public string prefixText = "Press";
    public string suffixText = "to";

    void Start()
    {
        HidePrompt();

        if (keyIconImage != null && keySprite != null)
        {
            keyIconImage.sprite = keySprite;
        }
    }

    public void ShowPrompt(string actionText)
    {
        if (string.IsNullOrWhiteSpace(actionText))
        {
            HidePrompt();
            return;
        }

        if (rootObject != null)
        {
            rootObject.SetActive(true);
        }

        if (keyIconImage != null)
        {
            keyIconImage.enabled = true;
        }

        if (promptText != null)
        {
            promptText.enabled = true;
            promptText.text = prefixText + " " + suffixText + " " + actionText;
        }
    }

    public void HidePrompt()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(false);
        }

        if (keyIconImage != null)
        {
            keyIconImage.enabled = false;
        }

        if (promptText != null)
        {
            promptText.enabled = false;
            promptText.text = "";
        }
    }
}