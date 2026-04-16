using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PauseMenuController pauseMenu;
    public PlayerInventory inventory;
    public InteractionPromptUI promptUI;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public float interactSphereRadius = 0.15f;
    public LayerMask interactionLayers = ~0;
    public InputAction interactAction;

    [Header("Target Stability")]
    public float loseTargetDelay = 0.1f;

    private IInteractable currentInteractable;
    private IHighlightable currentHighlightable;
    private Collider currentHitCollider;
    private float loseTimer = 0f;

    void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }

    void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.Enable();
            interactAction.performed += OnInteractPerformed;
        }
    }

    void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.Disable();
        }

        ClearCurrentInteractable();
    }

    void Start()
    {
        if (pauseMenu == null)
        {
            pauseMenu = PauseMenuController.Instance;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (promptUI != null)
        {
            promptUI.HidePrompt();
        }
    }

    void Update()
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            ClearCurrentInteractable();

            if (promptUI != null)
            {
                promptUI.HidePrompt();
            }

            return;
        }

        UpdateHoverTarget();
    }

    void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            return;
        }

        if (currentInteractable != null)
        {
            currentInteractable.Interact(this);
            UpdateHoverTarget();
        }
    }

    void UpdateHoverTarget()
    {
        if (playerCamera == null)
        {
            ClearCurrentInteractable();

            if (promptUI != null)
            {
                promptUI.HidePrompt();
            }

            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.SphereCast(ray, interactSphereRadius, out RaycastHit hit, interactDistance, interactionLayers, QueryTriggerInteraction.Collide))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();
            }

            IHighlightable highlightable = hit.collider.GetComponent<IHighlightable>();

            if (highlightable == null)
            {
                highlightable = hit.collider.GetComponentInParent<IHighlightable>();
            }

            if (interactable != null)
            {
                loseTimer = 0f;

                bool changedTarget = currentInteractable != interactable || currentHitCollider != hit.collider;

                if (changedTarget)
                {
                    ClearCurrentInteractable();

                    currentInteractable = interactable;
                    currentHighlightable = highlightable;
                    currentHitCollider = hit.collider;

                    if (currentHighlightable != null)
                    {
                        currentHighlightable.SetHighlighted(true);
                    }
                }

                if (promptUI != null)
                {
                    string interactText = interactable.GetInteractText(this);
                    promptUI.ShowPrompt(interactText);
                }

                return;
            }
        }

        loseTimer += Time.deltaTime;

        if (loseTimer >= loseTargetDelay)
        {
            ClearCurrentInteractable();

            if (promptUI != null)
            {
                promptUI.HidePrompt();
            }
        }
    }

    void ClearCurrentInteractable()
    {
        if (currentHighlightable != null)
        {
            currentHighlightable.SetHighlighted(false);
        }

        currentInteractable = null;
        currentHighlightable = null;
        currentHitCollider = null;
    }
}