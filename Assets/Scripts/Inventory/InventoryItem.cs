using UnityEngine;

[System.Serializable]
public class InventoryItem 
{
    public InventoryItemData data;
    public int stackSize;

    public InventoryItem(InventoryItemData itemData)
    {
        data = itemData;
        AddStack();
    }

    public void AddStack()
    {
        stackSize++;
    }

    public void RemoveFromStack()
    {
        stackSize--;
    }

}
