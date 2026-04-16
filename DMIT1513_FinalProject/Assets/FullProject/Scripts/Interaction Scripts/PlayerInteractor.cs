using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("References")]
    public Camera playerCamera;
    public PauseMenuController pauseMenu;
    public PlayerInventory inventory;

    [Header("Interaction")]
    public float interactDistance = 3f;
    public LayerMask interactionLayers = ~0;
    public InputAction interactAction;

    private void Awake()
    {
        if (inventory == null)
        {
            inventory = GetComponent<PlayerInventory>();
        }
    }

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.Enable();
            interactAction.performed += OnInteractPerformed;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractPerformed;
            interactAction.Disable();
        }
    }

    private void Start()
    {
        if (pauseMenu == null)
        {
            pauseMenu = PauseMenuController.Instance;
        }

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if (pauseMenu != null && pauseMenu.IsPaused())
        {
            return;
        }

        TryInteract();
    }

    public void TryInteract()
    {
        if (playerCamera == null)
        {
            return;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactionLayers, QueryTriggerInteraction.Ignore))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();

            if (interactable == null)
            {
                interactable = hit.collider.GetComponentInParent<IInteractable>();
            }

            if (interactable != null)
            {
                interactable.Interact(this);
            }
        }
    }
}