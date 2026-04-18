using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonLook : MonoBehaviour
{
    [Header("References")]
    public Transform playerBody;
    public Transform cameraPivot;
    public PauseMenuController pauseMenu;

    [Header("Settings")]
    public float mouseSensitivity = 2.5f;
    public float verticalClamp = 80f;

    [Header("Start Snap")]
    public bool snapToForwardOnStart = true;
    public float startSnapSpeed = 6f;
    public float forcedStartYaw = 0f;

    [Header("Input")]
    public InputAction lookAction;

    private float xRotation = 0f;
    private Vector2 lookInput;

    private bool snappingToStartRotation = false;
    private Quaternion targetPlayerRotation;
    private Quaternion targetCameraRotation;

    void Awake()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("MouseSensitivity", mouseSensitivity);
    }

    void OnEnable()
    {
        if (lookAction != null)
        {
            lookAction.Enable();
            lookAction.performed += OnLook;
            lookAction.canceled += OnLook;
        }
    }

    void OnDisable()
    {
        if (lookAction != null)
        {
            lookAction.performed -= OnLook;
            lookAction.canceled -= OnLook;
            lookAction.Disable();
        }
    }

    void Start()
    {
        if (pauseMenu == null)
        {
            pauseMenu = PauseMenuController.Instance;
        }

        if (pauseMenu != null)
        {
            pauseMenu.OnPauseToggle += HandlePauseStateChanged;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        xRotation = 0f;
        lookInput = Vector2.zero;

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }

        if (playerBody != null)
        {
            targetPlayerRotation = Quaternion.Euler(0f, forcedStartYaw, 0f);
            playerBody.rotation = targetPlayerRotation;
        }

        targetCameraRotation = Quaternion.Euler(0f, 0f, 0f);

        if (snapToForwardOnStart)
        {
            snappingToStartRotation = true;
        }
        else
        {
            snappingToStartRotation = false;

            if (cameraPivot != null)
            {
                cameraPivot.localRotation = targetCameraRotation;
            }
        }
    }

    void OnDestroy()
    {
        if (pauseMenu != null)
        {
            pauseMenu.OnPauseToggle -= HandlePauseStateChanged;
        }
    }

    void Update()
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            lookInput = Vector2.zero;
            return;
        }

        if (snappingToStartRotation)
        {
            UpdateStartSnap();
            return;
        }

        float mouseX = lookInput.x * mouseSensitivity;
        float mouseY = lookInput.y * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -verticalClamp, verticalClamp);

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }

    void UpdateStartSnap()
    {
        bool playerDone = true;
        bool cameraDone = true;

        if (playerBody != null)
        {
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetPlayerRotation, startSnapSpeed * Time.deltaTime);

            float playerAngle = Quaternion.Angle(playerBody.rotation, targetPlayerRotation);
            playerDone = playerAngle < 0.1f;

            if (playerDone)
            {
                playerBody.rotation = targetPlayerRotation;
            }
        }

        if (cameraPivot != null)
        {
            cameraPivot.localRotation = Quaternion.Slerp(cameraPivot.localRotation, targetCameraRotation, startSnapSpeed * Time.deltaTime);

            float cameraAngle = Quaternion.Angle(cameraPivot.localRotation, targetCameraRotation);
            cameraDone = cameraAngle < 0.1f;

            if (cameraDone)
            {
                cameraPivot.localRotation = targetCameraRotation;
            }
        }

        xRotation = 0f;

        if (playerDone && cameraDone)
        {
            snappingToStartRotation = false;
        }
    }

    void OnLook(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            lookInput = Vector2.zero;
            return;
        }

        if (snappingToStartRotation)
        {
            lookInput = Vector2.zero;
            return;
        }

        lookInput = context.ReadValue<Vector2>();
    }

    void HandlePauseStateChanged(bool paused)
    {
        lookInput = Vector2.zero;

        if (lookAction == null)
        {
            return;
        }

        if (paused)
        {
            lookAction.Disable();
        }
        else
        {
            lookAction.Enable();
        }
    }

    public void SetSensitivity(float newSensitivity)
    {
        mouseSensitivity = newSensitivity;
        PlayerPrefs.SetFloat("MouseSensitivity", mouseSensitivity);
        PlayerPrefs.Save();
    }

    public float GetSensitivity()
    {
        return mouseSensitivity;
    }
}