using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradePanel : MonoBehaviour
{
    public InventoryItem[] array;
    public InventoryItem[] equipments;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>().array = equipments;
        transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>().index = 0;

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>().array = array;
            transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>().index = i;
        }
    }

    public void UpgradeItem()
    {

    }

    private void OnDestroy()
    {
        foreach (var item in equipments)
        {
            if (item.quantity > 0)
                Camera.main.GetComponent<PlayerController>().character.AddItemToInventory(item);
        }

        foreach (var item in array)
        {
            if (item.quantity > 0)
                Camera.main.GetComponent<PlayerController>().character.AddItemToInventory(item);
        }
    }
}
