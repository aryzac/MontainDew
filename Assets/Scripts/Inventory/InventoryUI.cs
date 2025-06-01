using System;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemsSlotPrefab;
    public Transform slotParent;

    private void Start()
    {
        // InventorySystem.Instance.onInventoryChangedEventCallback += OnUpdateInventory;
        
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.onInventoryChangedEventCallback += OnUpdateInventory;
        }

        // Llenar al iniciar si ya hay ítems
        OnUpdateInventory();
    }

    public void OnUpdateInventory()
    {
        
        Debug.Log("Actualizando inventario UI...");
        
        // Limpia todos los slots viejos
        foreach (Transform child in slotParent)
        {
            Destroy(child.gameObject);
        }

        // Dibuja todos los ítems actuales
        foreach (InventoryItem item in InventorySystem.Instance.inventory)
        {
            AddInventorySlot(item);
        }
        
        // foreach (Transform t in transform)
        // {
        //     Destroy(t.transform.gameObject);
        // }
        //
        // DrawInventory();
    }

    // public void DrawInventory()
    // {
    //     foreach (InventoryItem item in InventorySystem.Instance.inventory)
    //     {
    //         AddInventorySlot(item);
    //     }
    // }

    public void AddInventorySlot(InventoryItem item)
    {
        // GameObject obj = Instantiate(itemsSlotPrefab);
        // obj.transform.SetParent(transform, false);
        //
        // ItemSlot slot = obj.GetComponent<ItemSlot>();
        // slot.Set(item);
        
        GameObject obj = Instantiate(itemsSlotPrefab, slotParent); // usa slotParent como padre
        ItemSlot slot = obj.GetComponent<ItemSlot>();
        slot.Set(item);
    }
}
