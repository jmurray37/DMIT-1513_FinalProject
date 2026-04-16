using UnityEngine;

public interface IInteractable
{
    void Interact(PlayerInteractor interactor);
    string GetInteractText(PlayerInteractor interactor);
}

public interface IHighlightable
{
    void SetHighlighted(bool highlighted);
}