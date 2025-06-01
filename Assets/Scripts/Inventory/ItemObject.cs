using System;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    
    public InventoryItemData itemData;

    public void OnHandlePickup()
    {
        InventorySystem.Instance.AddItem(itemData);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnHandlePickup();
        }
    }
}
