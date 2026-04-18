using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    public static PauseMenuController Instance;

    [Header("Input")]
    public InputAction pauseAction;

    [Header("UI")]
    public GameObject pauseMenuUI;

    [Header("Scenes")]
    public string titleSceneName = "Title";

    private bool isPaused = false;

    public event Action<bool> OnPauseToggle;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPausePressed;
    }

    void OnDisable()
    {
        pauseAction.performed -= OnPausePressed;
        pauseAction.Disable();
    }

    void Start()
    {
        Resume();
    }

    void OnPausePressed(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        isPaused = true;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        AudioListener.pause = true;

        OnPauseToggle?.Invoke(true);
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        isPaused = false;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        AudioListener.pause = false;

        OnPauseToggle?.Invoke(false);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(titleSceneName);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Application.Quit();
        Debug.Log("Quit Game pressed");
    }
}