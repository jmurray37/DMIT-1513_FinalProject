using UnityEngine;

public class KeyPickup : MonoBehaviour, IInteractable, IHighlightable
{
    [Header("Key Info")]
    public string keyId = "DayRoomKey";
    public string pickupText = "pick up key";

    [Header("Highlight")]
    public GameObject outlineObject;
    public Renderer[] glowRenderers;
    public Color glowColor = Color.yellow;
    [Range(0f, 5f)]
    public float glowIntensity = 1.5f;

    private bool isHighlighted = false;

    void Awake()
    {
        if (outlineObject != null && outlineObject == gameObject)
        {
            outlineObject = null;
        }
    }

    void Start()
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }

        SetHighlighted(false);
    }

    public void Interact(PlayerInteractor interactor)
    {
        if (interactor == null || interactor.inventory == null)
        {
            return;
        }

        interactor.inventory.AddKey(keyId);
        Debug.Log("Picked up key: " + keyId);
        gameObject.SetActive(false);
    }

    public string GetInteractText(PlayerInteractor interactor)
    {
        return pickupText;
    }

    public void SetHighlighted(bool highlighted)
    {
        if (isHighlighted == highlighted)
        {
            return;
        }

        isHighlighted = highlighted;

        if (outlineObject != null)
        {
            outlineObject.SetActive(highlighted);
        }

        if (glowRenderers != null && glowRenderers.Length > 0)
        {
            for (int i = 0; i < glowRenderers.Length; i++)
            {
                if (glowRenderers[i] == null)
                {
                    continue;
                }

                Material[] materials = glowRenderers[i].materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    if (materials[j] == null)
                    {
                        continue;
                    }

                    if (materials[j].HasProperty("_EmissionColor"))
                    {
                        if (highlighted)
                        {
                            materials[j].EnableKeyword("_EMISSION");
                            materials[j].SetColor("_EmissionColor", glowColor * glowIntensity);
                        }
                        else
                        {
                            materials[j].SetColor("_EmissionColor", Color.black);
                        }
                    }
                }
            }
        }
    }
}