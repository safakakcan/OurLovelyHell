using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public bool equipment = false;
    public EquipmentType equipmentType = EquipmentType.Weapon;
    public GameObject itemInfoPrefab;

    [HideInInspector]
    public int index;
    [HideInInspector]
    public InventoryItem[] array;
    Transform draggingItem = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (draggingItem != null)
            draggingItem.GetComponent<RectTransform>().anchoredPosition = GameObject.FindGameObjectWithTag("Canvas").GetComponent<RectTransform>().InverseTransformPoint((Vector2)Input.mousePosition);
    }

    public void BeginDrag()
    {
        if (transform.childCount == 0)
            return;
        
        draggingItem = transform.GetChild(0);
        transform.GetChild(0).SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
    }

    public void EndDrag()
    {
        if (draggingItem != null)
        {
            draggingItem.SetParent(transform);
            draggingItem.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            draggingItem = null;
        }

        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Input.mousePosition;
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            ItemSlot slot;
            if (result.gameObject.TryGetComponent<ItemSlot>(out slot))
            {
                if (slot.index == index)
                    return;

                var itemData = array[index].index > -1 ? Camera.main.GetComponent<PlayerController>().gameData.items[array[index].index] : null;
                var otherItemData = slot.array[slot.index].index > -1 ? Camera.main.GetComponent<PlayerController>().gameData.items[slot.array[slot.index].index] : null;

                if ((array[index].quantity == 0 || (!slot.equipment) || (slot.equipment && itemData.itemType == ItemType.Equipment && slot.equipmentType == itemData.equipmentType)) &&
                    (slot.array[slot.index].quantity == 0 || (!equipment) || (equipment && otherItemData.itemType == ItemType.Equipment && equipmentType == otherItemData.equipmentType)))
                {
                    int otherIndex = slot.index;
                    var otherArray = slot.array;
                    int otherItem = slot.array[otherIndex].index;
                    int otherQuantity = slot.array[otherIndex].quantity;

                    if (otherArray[otherIndex].index == array[index].index && otherQuantity < otherItemData.stackSize && array[index].quantity < itemData.stackSize)
                    {
                        int otherTotal = otherArray[otherIndex].quantity + array[index].quantity;

                        if (otherTotal <= otherItemData.stackSize)
                        {
                            array[index].quantity = 0;
                            otherArray[otherIndex].quantity = otherTotal;
                        }
                        else
                        {
                            array[index].quantity = otherTotal - otherItemData.stackSize;
                            otherArray[otherIndex].quantity = otherItemData.stackSize;
                        }
                    }
                    else
                    {
                        otherArray[otherIndex] = array[index];
                        var item = new InventoryItem(otherItem, otherQuantity);
                        array[index] = item;
                    }

                    Refresh();
                    slot.Refresh();
                    break;
                }
            }
        }
    }

    public void Refresh()
    {
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);

        var itemData = array[index];

        if (itemData != null && itemData.quantity > 0)
        {
            GameObject item = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.itemPrefab);

            var sprite = Sprite.Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[itemData.index].sprite);
            item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().sprite = sprite;
            item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = itemData.quantity.ToString();

            item.transform.SetParent(transform);
            item.transform.localPosition = Vector2.zero;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localScale = Vector2.one;
        }

        if (equipment)
            Camera.main.GetComponent<PlayerController>().character.RefreshView();
    }

    public void ShowInfo()
    {
        if (array[index].quantity == 0)
            return;

        var info = Instantiate(itemInfoPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        info.transform.GetChild(0).GetComponent<RectTransform>().position = transform.GetComponent<RectTransform>().position;
        info.GetComponent<ItemInfo>().Init(array[index].index);
    }
}
