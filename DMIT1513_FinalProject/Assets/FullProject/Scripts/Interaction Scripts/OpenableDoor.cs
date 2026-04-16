using UnityEngine;

public class OpenableDoor : MonoBehaviour, IInteractable
{
    [Header("Door Setup")]
    public Transform doorPivot;
    public bool canPlayerOpen = true;
    public bool startsOpen = false;

    [Header("Locking")]
    public bool isLocked = false;
    public string requiredKeyId = "";

    [Header("Rotation")]
    public float closedYRotation = 0f;
    public float openYRotation = 90f;
    public float openSpeed = 3f;

    [Header("Auto Direction")]
    public bool openAwayFromPlayer = true;

    private bool isOpen;
    private float targetYRotation;

    private void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        isOpen = startsOpen;
        targetYRotation = isOpen ? openYRotation : closedYRotation;

        Vector3 currentEuler = doorPivot.localEulerAngles;
        currentEuler.y = targetYRotation;
        doorPivot.localEulerAngles = currentEuler;
    }

    private void Update()
    {
        if (doorPivot == null)
        {
            return;
        }

        Vector3 currentEuler = doorPivot.localEulerAngles;
        float newY = Mathf.LerpAngle(currentEuler.y, targetYRotation, Time.deltaTime * openSpeed);
        doorPivot.localEulerAngles = new Vector3(currentEuler.x, newY, currentEuler.z);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (!canPlayerOpen)
        {
            Debug.Log("This door cannot be opened.");
            return;
        }

        if (isLocked)
        {
            bool hasRequiredKey = false;

            if (string.IsNullOrWhiteSpace(requiredKeyId))
            {
                hasRequiredKey = false;
            }
            else if (interactor != null && interactor.inventory != null)
            {
                hasRequiredKey = interactor.inventory.HasKey(requiredKeyId);
            }

            if (!hasRequiredKey)
            {
                Debug.Log("Door is locked.");
                return;
            }

            isLocked = false;
            Debug.Log("Unlocked door with key: " + requiredKeyId);
        }

        ToggleDoor(interactor);
    }

    public string GetInteractText(PlayerInteractor interactor)
    {
        if (!canPlayerOpen)
        {
            return "";
        }

        if (isLocked)
        {
            if (interactor != null && interactor.inventory != null && interactor.inventory.HasKey(requiredKeyId))
            {
                return "Unlock Door";
            }

            return "Locked Door";
        }

        return isOpen ? "Close Door" : "Open Door";
    }

    private void ToggleDoor(PlayerInteractor interactor)
    {
        isOpen = !isOpen;

        if (isOpen)
        {
            float chosenOpenAngle = openYRotation;

            if (openAwayFromPlayer && interactor != null)
            {
                Vector3 toPlayer = interactor.transform.position - doorPivot.position;
                Vector3 localToPlayer = doorPivot.InverseTransformDirection(toPlayer.normalized);

                if (localToPlayer.x > 0f)
                {
                    chosenOpenAngle = -Mathf.Abs(openYRotation);
                }
                else
                {
                    chosenOpenAngle = Mathf.Abs(openYRotation);
                }
            }

            targetYRotation = chosenOpenAngle;
        }
        else
        {
            targetYRotation = closedYRotation;
        }
    }
}