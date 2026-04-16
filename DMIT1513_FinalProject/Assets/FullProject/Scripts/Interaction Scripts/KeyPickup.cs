using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable
{
    [Header("Key Info")]
    public string keyId = "MainKey";

    public void Interact(PlayerInteractor interactor)
    {
        if (interactor == null || interactor.inventory == null)
        {
            return;
        }

        interactor.inventory.AddKey(keyId);
        Destroy(gameObject);
    }

    public string GetInteractText(PlayerInteractor interactor)
    {
        return "Pick Up Key";
    }
}