using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class InventoryEntry
    {
        public string itemId;
        public int amount;

        public InventoryEntry(string newItemId, int newAmount)
        {
            itemId = newItemId;
            amount = newAmount;
        }
    }

    [Header("Inventory Data")]
    [SerializeField] private List<InventoryEntry> items = new List<InventoryEntry>();

    public IReadOnlyList<InventoryEntry> Items => items;

    public event Action OnInventoryChanged;
    public event Action<string, int> OnItemAdded;

    public void AddItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (amount <= 0)
        {
            return;
        }

        InventoryEntry existingEntry = GetEntry(itemId);

        if (existingEntry != null)
        {
            existingEntry.amount += amount;
        }
        else
        {
            items.Add(new InventoryEntry(itemId, amount));
        }

        Debug.Log("Added item: " + itemId + " x" + amount);
        OnItemAdded?.Invoke(itemId, amount);
        OnInventoryChanged?.Invoke();
    }

    public bool RemoveItem(string itemId, int amount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        if (amount <= 0)
        {
            return false;
        }

        InventoryEntry existingEntry = GetEntry(itemId);

        if (existingEntry == null)
        {
            return false;
        }

        if (existingEntry.amount < amount)
        {
            return false;
        }

        existingEntry.amount -= amount;

        if (existingEntry.amount <= 0)
        {
            items.Remove(existingEntry);
        }

        Debug.Log("Removed item: " + itemId + " x" + amount);
        OnInventoryChanged?.Invoke();
        return true;
    }

    public bool HasItem(string itemId, int requiredAmount = 1)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        if (requiredAmount <= 0)
        {
            return true;
        }

        InventoryEntry existingEntry = GetEntry(itemId);

        if (existingEntry == null)
        {
            return false;
        }

        return existingEntry.amount >= requiredAmount;
    }

    public int GetItemAmount(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return 0;
        }

        InventoryEntry existingEntry = GetEntry(itemId);

        if (existingEntry == null)
        {
            return 0;
        }

        return existingEntry.amount;
    }

    public void AddKey(string keyId)
    {
        AddItem(keyId, 1);
    }

    public bool HasKey(string keyId)
    {
        return HasItem(keyId, 1);
    }

    public bool UseKey(string keyId)
    {
        return RemoveItem(keyId, 1);
    }

    public void ClearInventory()
    {
        items.Clear();
        Debug.Log("Inventory cleared.");
        OnInventoryChanged?.Invoke();
    }

    private InventoryEntry GetEntry(string itemId)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].itemId == itemId)
            {
                return items[i];
            }
        }

        return null;
    }
}