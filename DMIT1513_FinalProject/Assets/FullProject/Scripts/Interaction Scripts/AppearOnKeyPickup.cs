using UnityEngine;

public class AppearOnKeyPickup : MonoBehaviour
{
    [Header("Inventory Check")]
    public PlayerInventory playerInventory;
    public string requiredKeyId = "MorgueKey";

    [Header("Target")]
    public GameObject targetToEnable;

    [Header("Optional Audio")]
    public AudioSource audioSource;
    public AudioClip appearSound;
    [Range(0f, 1f)]
    public float appearSoundVolume = 1f;

    [Header("Optional Animation Refresh")]
    public bool rebindAnimatorOnAppear = true;

    private bool hasTriggered = false;
    private bool hadKeyLastFrame = false;

    void Start()
    {
        if (playerInventory != null)
        {
            hadKeyLastFrame = playerInventory.HasKey(requiredKeyId);
        }

        if (targetToEnable != null)
        {
            targetToEnable.SetActive(false);
        }
    }

    void Update()
    {
        if (hasTriggered)
        {
            return;
        }

        if (playerInventory == null)
        {
            return;
        }

        bool hasKeyNow = playerInventory.HasKey(requiredKeyId);

        if (!hadKeyLastFrame && hasKeyNow)
        {
            hasTriggered = true;
            TriggerAppear();
        }

        hadKeyLastFrame = hasKeyNow;
    }

    void TriggerAppear()
    {
        if (targetToEnable == null)
        {
            return;
        }

        targetToEnable.SetActive(true);

        if (rebindAnimatorOnAppear)
        {
            Animator animator = targetToEnable.GetComponentInChildren<Animator>(true);

            if (animator != null)
            {
                animator.Rebind();
                animator.Update(0f);
            }
        }

        if (audioSource != null && appearSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(appearSound, appearSoundVolume);
        }
    }
}