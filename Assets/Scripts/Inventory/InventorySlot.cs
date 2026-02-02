using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    public Item item;
    public int stackSize;

    public InventorySlot(Item _item, int _stackSize)
    {
        item = _item;
        stackSize = _stackSize;
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void ClearSlot()
    {
        item = null;
        stackSize = 0;
    }
}