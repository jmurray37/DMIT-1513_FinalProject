using UnityEngine;

public class DisappearOnKeyPickup : MonoBehaviour
{
    [Header("Inventory Check")]
    public PlayerInventory playerInventory;
    public string requiredKeyId = "ExitKey";

    [Header("Target")]
    public GameObject targetToDisable;

    [Header("Optional")]
    public bool destroyInsteadOfDisable = false;

    private bool hasTriggered = false;
    private bool hadKeyLastFrame = false;

    void Start()
    {
        if (targetToDisable == null)
        {
            targetToDisable = gameObject;
        }

        if (playerInventory != null)
        {
            hadKeyLastFrame = playerInventory.HasKey(requiredKeyId);
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
            TriggerDisappear();
        }

        hadKeyLastFrame = hasKeyNow;
    }

    void TriggerDisappear()
    {
        if (targetToDisable == null)
        {
            return;
        }

        if (destroyInsteadOfDisable)
        {
            Destroy(targetToDisable);
        }
        else
        {
            targetToDisable.SetActive(false);
        }
    }
}