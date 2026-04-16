using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private readonly HashSet<string> keyItems = new HashSet<string>();

    public void AddKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId))
        {
            return;
        }

        keyItems.Add(keyId);
        Debug.Log("Picked up key: " + keyId);
    }

    public bool HasKey(string keyId)
    {
        if (string.IsNullOrWhiteSpace(keyId))
        {
            return false;
        }

        return keyItems.Contains(keyId);
    }
}