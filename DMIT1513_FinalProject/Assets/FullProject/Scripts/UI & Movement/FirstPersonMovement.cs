using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public PauseMenuController pauseMenu;

    [Header("Movement")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 7f;
    public float jumpHeight = 1.2f;

    [Header("Gravity")]
    public float gravity = -20f;
    public float groundedStickForce = -2f;

    [Header("Input")]
    public InputAction moveAction;
    public InputAction jumpAction;
    public InputAction sprintAction;

    private CharacterController characterController;
    private Vector2 moveInput;
    private float verticalVelocity;
    private bool isSprinting;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
            moveAction.performed += OnMove;
            moveAction.canceled += OnMove;
        }

        if (jumpAction != null)
        {
            jumpAction.Enable();
            jumpAction.performed += OnJump;
        }

        if (sprintAction != null)
        {
            sprintAction.Enable();
            sprintAction.performed += OnSprintPerformed;
            sprintAction.canceled += OnSprintCanceled;
        }
    }

    void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.performed -= OnMove;
            moveAction.canceled -= OnMove;
            moveAction.Disable();
        }

        if (jumpAction != null)
        {
            jumpAction.performed -= OnJump;
            jumpAction.Disable();
        }

        if (sprintAction != null)
        {
            sprintAction.performed -= OnSprintPerformed;
            sprintAction.canceled -= OnSprintCanceled;
            sprintAction.Disable();
        }
    }

    void Start()
    {
        if (pauseMenu == null)
        {
            pauseMenu = PauseMenuController.Instance;
        }

        if (orientation == null)
        {
            orientation = transform;
        }

        if (pauseMenu != null)
        {
            pauseMenu.OnPauseToggle += HandlePauseStateChanged;
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
            moveInput = Vector2.zero;
            isSprinting = false;
            return;
        }

        HandleMovement();
    }

    void HandleMovement()
    {
        bool isGrounded = characterController.isGrounded;

        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = groundedStickForce;
        }

        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        Vector3 moveDirection = (orientation.forward * moveInput.y) + (orientation.right * moveInput.x);
        moveDirection.Normalize();

        Vector3 horizontalVelocity = moveDirection * currentSpeed;
        Vector3 finalVelocity = new Vector3(horizontalVelocity.x, verticalVelocity, horizontalVelocity.z);

        characterController.Move(finalVelocity * Time.deltaTime);

        verticalVelocity += gravity * Time.deltaTime;
    }

    void OnMove(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            return;
        }

        if (characterController != null && characterController.isGrounded)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void OnSprintPerformed(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            isSprinting = false;
            return;
        }

        isSprinting = true;
    }

    void OnSprintCanceled(InputAction.CallbackContext context)
    {
        isSprinting = false;
    }

    void HandlePauseStateChanged(bool paused)
    {
        moveInput = Vector2.zero;
        isSprinting = false;

        if (paused)
        {
            if (moveAction != null) moveAction.Disable();
            if (jumpAction != null) jumpAction.Disable();
            if (sprintAction != null) sprintAction.Disable();
        }
        else
        {
            if (moveAction != null) moveAction.Enable();
            if (jumpAction != null) jumpAction.Enable();
            if (sprintAction != null) sprintAction.Enable();
        }
    }
}