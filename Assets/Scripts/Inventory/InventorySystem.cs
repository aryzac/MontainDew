using System.Collections.Generic;
using UnityEngine;

public class InventorySystem :MonoBehaviour
{
    
    public static InventorySystem Instance;
    
    public delegate void onInventoryChangedEvent();
    public event onInventoryChangedEvent onInventoryChangedEventCallback;

    private Dictionary<string, InventoryItem> _itemDictionary;
    public List<InventoryItem> inventory;
    
    private void Awake()
    {
        inventory = new List<InventoryItem>();
        _itemDictionary = new Dictionary<string, InventoryItem>();

        Instance = this;
    }

    public void AddItem(InventoryItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData.id, out InventoryItem value))
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
            _itemDictionary.Add(itemData.id, newItem);
            
            onInventoryChangedEventCallback.Invoke();
        }
    }

    public void RemoveItem(InventoryItemData itemData)
    {
        if (_itemDictionary.TryGetValue(itemData.id, out InventoryItem value))
        {
            value.RemoveFromStack();

            if (value.stackSize == 0)
            {
                inventory.Remove(value);
                _itemDictionary.Remove(itemData.id);
            }
        }
        
        onInventoryChangedEventCallback.Invoke();
    }
    
}
