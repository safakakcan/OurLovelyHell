using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryItem
{
    public int index = 0;
    public int quantity = 0;
    public float durability = 1;
    public ItemSocket socket = null;

    public InventoryItem(int _index, int _quantity)
    {
        index = _index;
        quantity = _quantity;
        durability = Camera.main.GetComponent<PlayerController>().gameData.items[index].maxDurability;
    }
}

[System.Serializable]
public class ItemSocket
{
    public int index;
    //public float durability;
}

[System.Serializable]
public class ItemQuantity
{
    public int index = 0;
    public int quantity = 0;

    public ItemQuantity(int _index, int _quantity)
    {
        index = _index;
        quantity = _quantity;
    }
}