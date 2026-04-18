using UnityEngine;

[RequireComponent(typeof(AudioSource))]
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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public AudioClip unlockSound;
    public AudioClip lockedSound;
    [Range(0f, 1f)]
    public float openVolume = 1f;
    [Range(0f, 1f)]
    public float closeVolume = 1f;
    [Range(0f, 1f)]
    public float unlockVolume = 1f;
    [Range(0f, 1f)]
    public float lockedVolume = 1f;

    private bool isOpen;
    private float targetYRotation;

    private void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (doorPivot == null)
        {
            doorPivot = transform;
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
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

            if (!string.IsNullOrWhiteSpace(requiredKeyId) && interactor != null && interactor.inventory != null)
            {
                hasRequiredKey = interactor.inventory.HasKey(requiredKeyId);
            }

            if (!hasRequiredKey)
            {
                PlayClip(lockedSound, lockedVolume);
                Debug.Log("Door is locked.");
                return;
            }

            isLocked = false;
            PlayClip(unlockSound, unlockVolume);
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

            return "Door is locked";
        }

        return isOpen ? "Close Door" : "Open Door";
    }

    private void ToggleDoor(PlayerInteractor interactor)
    {
        bool wasOpen = isOpen;
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

            if (!wasOpen)
            {
                PlayClip(openSound, openVolume);
            }
        }
        else
        {
            targetYRotation = closedYRotation;
            PlayClip(closeSound, closeVolume);
        }
    }

    private void PlayClip(AudioClip clip, float volume)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        audioSource.pitch = Random.Range(0.96f, 1.04f);
        audioSource.PlayOneShot(clip, volume);
    }
}