using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public Transform[] equipments;
    [HideInInspector]
    public Transform[] slots;

    [Header("Trash")]
    public Transform trashSlot;
    [HideInInspector]
    public InventoryItem[] trash = new InventoryItem[1];
    

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
        string playerName = Camera.main.name;
        equipments = new Transform[transform.GetChild(1).childCount];

        for (int i = 0; i < equipments.Length; i++)
        {
            equipments[i] = transform.GetChild(1).GetChild(i);
            equipments[i].GetComponent<ItemSlot>().index = i;
            equipments[i].GetComponent<ItemSlot>().array = Camera.main.GetComponent<PlayerController>().character.equipments; //
            equipments[i].GetComponent<ItemSlot>().id = string.Format("users/{0}/equipments/{1}", playerName, i);
        }

        slots = new Transform[transform.GetChild(0).childCount];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = transform.GetChild(0).GetChild(i);
            slots[i].GetComponent<ItemSlot>().Init(i, string.Format("users/{0}/inventory/{1}", playerName, i), Camera.main.GetComponent<PlayerController>().character.inventory, Camera.main.GetComponent<PlayerController>().character.inventorySlotCount < i + 1);
        }

        trashSlot.GetComponent<ItemSlot>().index = 0;
        trashSlot.GetComponent<ItemSlot>().array = trash;
        //trashSlot.GetComponent<ItemSlot>().id = string.Format("users/{0}/trash", playerName);

        Refresh();
    }

    public void Refresh()
    {
        for (int i = 0; i < equipments.Length; i++)
        {
            var itemData = equipments[i].GetComponent<ItemSlot>().array[i]; //

            for (int index = 0; index < equipments[i].childCount; index++)
            {
                Destroy(equipments[i].GetChild(index).gameObject);
            }

            if (itemData != null && itemData.quantity > 0)
            {
                GameObject item = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.itemPrefab);

                var sprite = Sprite.Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[itemData.index].sprite);
                item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = itemData.quantity.ToString();

                item.transform.SetParent(equipments[i]);
                item.transform.localPosition = Vector2.zero;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = Vector2.one;
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            var itemData = slots[i].GetComponent<ItemSlot>().array[i]; //

            for (int index = 0; index < slots[i].childCount; index++)
            {
                Destroy(slots[i].GetChild(index).gameObject);
            }

            if (itemData != null && itemData.quantity > 0)
            {
                GameObject item = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.itemPrefab);

                var sprite = Sprite.Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[itemData.index].sprite);
                item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = sprite;
                item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = itemData.quantity.ToString();

                item.transform.SetParent(slots[i]);
                item.transform.localPosition = Vector2.zero;
                item.transform.localRotation = Quaternion.identity;
                item.transform.localScale = Vector2.one;
            }
        }

        Camera.main.GetComponent<PlayerController>().character.RefreshView();
    }
}
