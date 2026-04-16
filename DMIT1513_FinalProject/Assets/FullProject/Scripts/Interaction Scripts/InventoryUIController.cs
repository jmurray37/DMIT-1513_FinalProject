using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    [Header("References")]
    public PlayerInventory inventory;
    public GameObject inventoryRoot;
    public TMP_Text inventoryListText;

    [Header("Input")]
    public InputAction toggleInventoryAction;

    [Header("Optional Cursor")]
    public bool unlockCursorWhenOpen = true;

    private bool isOpen = false;

    void OnEnable()
    {
        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.Enable();
            toggleInventoryAction.performed += OnToggleInventory;
        }

        if (inventory != null)
        {
            inventory.OnInventoryChanged += RefreshUI;
        }
    }

    void OnDisable()
    {
        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.performed -= OnToggleInventory;
            toggleInventoryAction.Disable();
        }

        if (inventory != null)
        {
            inventory.OnInventoryChanged -= RefreshUI;
        }
    }

    void Start()
    {
        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(false);
        }

        RefreshUI();
    }

    void OnToggleInventory(InputAction.CallbackContext context)
    {
        ToggleInventory();
    }

    public void ToggleInventory()
    {
        isOpen = !isOpen;

        if (inventoryRoot != null)
        {
            inventoryRoot.SetActive(isOpen);
        }

        if (unlockCursorWhenOpen)
        {
            if (isOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        RefreshUI();
    }

    public void RefreshUI()
    {
        if (inventoryListText == null || inventory == null)
        {
            return;
        }

        var items = inventory.Items;

        if (items == null || items.Count == 0)
        {
            inventoryListText.text = "Inventory is empty.";
            return;
        }

        StringBuilder sb = new StringBuilder();

        for (int i = 0; i < items.Count; i++)
        {
            sb.Append(items[i].itemId);
            sb.Append(" x");
            sb.Append(items[i].amount);

            if (i < items.Count - 1)
            {
                sb.AppendLine();
            }
        }

        inventoryListText.text = sb.ToString();
    }
}