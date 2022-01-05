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
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        string playerName = Camera.main.name;
        transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>().array = equipments;
        transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>().index = 0;
        transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>().id = string.Format("users/{0}/upgrade/equipments/{1}", playerName, 0);

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>().array = array;
            transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>().index = i;
            transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>().id = string.Format("users/{0}/upgrade/recipes/{1}", playerName, i);
        }
    }

    public void UpgradeItem()
    {

    }

    private void OnDestroy()
    {
        var equipmentSlot = transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>();

        if (equipmentSlot.source != null)
        {
            equipmentSlot.source.clone = null;
            equipmentSlot.source.Refresh(false);
        }

        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            var slot = transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>();

            if (slot.source != null)
            {
                slot.source.clone = null;
                slot.source.Refresh(false);
            }
        }
    }
}
