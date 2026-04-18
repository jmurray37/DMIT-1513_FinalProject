using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

public class TitleScreenController : MonoBehaviour
{
    [Header("Scene")]
    public string gameSceneName = "GameWorld";

    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject controlsPanel;
    public GameObject introPanel;

    [Header("Intro Text")]
    [TextArea(3, 10)]
    public string introMessage =
        "The brighter your surroundings become, the easier it is to see.\n\n" +
        "But the monsters can see more clearly too.\n\n" +
        "Use the darkness carefully.\n\n" +
        "Not every light is your friend.";
    public TMP_Text introText;

    [Header("Intro Scroll")]
    public float introScrollSpeed = 80f;
    public float introBottomPadding = 100f;
    public float introTopPadding = 100f;

    private bool introPlaying = false;
    private RectTransform introRect;
    private RectTransform introPanelRect;

    void Start()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (introText != null)
        {
            introText.text = introMessage;
            introRect = introText.GetComponent<RectTransform>();
        }

        if (introPanel != null)
        {
            introPanelRect = introPanel.GetComponent<RectTransform>();
        }

        ShowMainMenu();
    }

    void Update()
    {
        if (introPlaying && introRect != null && introPanelRect != null)
        {
            Vector2 pos = introRect.anchoredPosition;
            pos.y += introScrollSpeed * Time.deltaTime;
            introRect.anchoredPosition = pos;

            
            if (AnyInputPressedThisFrame())
            {
                StartGame();
                return;
            }
        }
    }

    bool AnyInputPressedThisFrame()
    {
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            return true;
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame ||
                Mouse.current.middleButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonSouth.wasPressedThisFrame ||
                Gamepad.current.buttonNorth.wasPressedThisFrame ||
                Gamepad.current.buttonEast.wasPressedThisFrame ||
                Gamepad.current.buttonWest.wasPressedThisFrame ||
                Gamepad.current.startButton.wasPressedThisFrame)
            {
                return true;
            }
        }

        return false;
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(false);

        introPlaying = false;
    }

    public void ShowControls()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(true);
        if (introPanel != null) introPanel.SetActive(false);

        introPlaying = false;
    }

    public void StartIntro()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (controlsPanel != null) controlsPanel.SetActive(false);
        if (introPanel != null) introPanel.SetActive(true);

        if (introText != null)
        {
            introText.text = introMessage;
            introText.ForceMeshUpdate();
        }

        if (introRect != null && introPanelRect != null)
        {
            float textHalfHeight = introRect.rect.height * 0.5f;
            float panelHalfHeight = introPanelRect.rect.height * 0.5f;

            float startY = -(panelHalfHeight + textHalfHeight + introBottomPadding);
            introRect.anchoredPosition = new Vector2(0f, startY);
        }

        introPlaying = true;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}