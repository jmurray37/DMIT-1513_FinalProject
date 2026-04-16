using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteractor interactor);
    string GetInteractText(PlayerInteractor interactor);
}