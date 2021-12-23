using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInfo : MonoBehaviour
{
    public void Init(int index)
    {
        Item item = Camera.main.GetComponent<PlayerController>().gameData.items[index];
        transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = item.sprite;
        transform.GetChild(0).GetChild(1).GetComponent<UnityEngine.UI.Text>().text = item.itemName;
        transform.GetChild(0).GetChild(2).GetComponent<UnityEngine.UI.Text>().text = item.description + 
            "\nItem Type: " + (item.itemType == ItemType.Equipment ? item.itemType.ToString() + " (" + item.equipmentType + ")" : item.itemType.ToString());
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}
