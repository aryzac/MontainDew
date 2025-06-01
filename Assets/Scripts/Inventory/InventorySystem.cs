using System.Collections.Generic;
using UnityEngine;

public class InventorySystem :MonoBehaviour
{
    
    public static InventorySystem Instance;
    
    public delegate void onInventoryChangedEvent();
    public event onInventoryChangedEvent onInventoryChangedEventCallback;

    private Dictionary<InventoryItemData, InventoryItem> _itemDictionary;
    public List<InventoryItem> inventory;
    
    private void Awake()
    {
        inventory = new List<InventoryItem>();
        _itemDictionary = new Dictionary<InventoryItemData, InventoryItem>();

        Instance = this;
    }

    public void AddItem(InventoryItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData, out InventoryItem value))
        {
            Debug.Log("El item ha sido actualizado");
            value.AddStack();
            
            onInventoryChangedEventCallback.Invoke();
        }
        else
        {
            Debug.Log("El nuevo item se ha agregado");
            InventoryItem newItem = new InventoryItem(itemData);
            inventory.Add(newItem);
            _itemDictionary.Add(itemData, newItem);
            
            onInventoryChangedEventCallback.Invoke();
        }
    }

    public void RemoveItem(InventoryItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                _itemDictionary.Remove(itemData);
            }
        }
        
        onInventoryChangedEventCallback.Invoke();
    }
    
}
