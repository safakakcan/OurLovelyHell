using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public ItemSlot slot;

    public void Init(ItemSlot _slot)
    {
        slot = _slot;
        var index = slot.array[slot.index].index;
        transform.GetChild(1).GetComponent<RectTransform>().position = slot.GetComponent<RectTransform>().position;
        Item item = Camera.main.GetComponent<PlayerController>().gameData.items[index];
        transform.GetChild(1).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = item.sprite;
        transform.GetChild(1).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = item.itemName;
        transform.GetChild(1).GetChild(2).GetComponent<UnityEngine.UI.Text>().text = item.description + 
            "\nItem Type: " + (item.itemType == ItemType.Equipment ? item.itemType.ToString() + " (" + item.equipmentType + ")" : item.itemType.ToString());
    }

    public void Consume()
    {
        var index = slot.array[slot.index].index;
        Item item = Camera.main.GetComponent<PlayerController>().gameData.items[index];
        
        if (!string.IsNullOrEmpty(item.function))
        {
            Camera.main.SendMessage(item.function, slot.array[slot.index]);
        }
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}
