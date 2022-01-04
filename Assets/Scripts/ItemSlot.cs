using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSlot : MonoBehaviour
{
    public Sprite unlockedSprite;
    public Sprite lockedSprite;
    public bool trash = false;
    public bool temporary = false;
    public bool locked = false;
    public List<ItemType> itemTypes;
    public List<EquipmentType> equipmentTypes;

    [HideInInspector]
    public string id;
    [HideInInspector]
    public int index;
    [HideInInspector]
    public InventoryItem[] array;

    [HideInInspector]
    public ItemSlot clone = null;
    [HideInInspector]
    public ItemSlot source = null;

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

    public void Init(int _index, string _id, InventoryItem[] _array, bool _locked = false)
    {
        index = _index;
        id = _id;
        array = _array;
        locked = _locked;
        GetComponent<UnityEngine.UI.Image>().sprite = locked ? lockedSprite : unlockedSprite;
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

            if (temporary)
            {
                source = null;
                Refresh();
                return;
            }
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
                if ((slot.array == array && slot.index == index) || slot.locked || trash || array[index].quantity == 0)
                    return;

                if (slot.trash)
                {
                    array[index] = new InventoryItem(0, 0);
                    slot.array[slot.index] = new InventoryItem(0, 0);
                    Refresh();
                    slot.Refresh();
                    return;
                }
                
                var itemData = Camera.main.GetComponent<PlayerController>().gameData.items[array[index].index];
                var otherItemData = Camera.main.GetComponent<PlayerController>().gameData.items[slot.array[slot.index].index];

                if ((array[index].quantity == 0 || (itemData.itemType != ItemType.Equipment && slot.itemTypes.Contains(itemData.itemType)) || (slot.itemTypes.Contains(ItemType.Equipment) && itemData.itemType == ItemType.Equipment && slot.equipmentTypes.Contains(itemData.equipmentType))) &&
                    (slot.array[slot.index].quantity == 0 || (otherItemData.itemType != ItemType.Equipment && itemTypes.Contains(otherItemData.itemType)) || (itemTypes.Contains(ItemType.Equipment) && otherItemData.itemType == ItemType.Equipment && equipmentTypes.Contains(otherItemData.equipmentType))))
                {
                    if (slot.temporary)
                    {
                        if (clone != null)
                        {
                            clone.source = null;
                            clone.Refresh();
                            clone = null;
                        }

                        clone = slot;
                        slot.source = this;
                        slot.Refresh();
                        Refresh();
                        return;
                    }

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

                        if (slot.clone != null)
                            slot.clone.Refresh();
                    }
                    else
                    {
                        otherArray[otherIndex] = array[index];
                        var item = new InventoryItem(otherItem, otherQuantity);
                        array[index] = item;

                        if (slot.clone != null)
                        {
                            slot.clone.source = null;
                            slot.clone.Refresh();
                            slot.clone = null;
                        }
                    }

                    if (clone != null)
                    {
                        clone.source = null;
                        clone.Refresh();
                        clone = null;
                    }

                    Refresh();
                    slot.Refresh();
                }

                break;
            }
        }
    }

    public void Refresh()
    {
        if (transform.childCount > 0)
            Destroy(transform.GetChild(0).gameObject);

        var itemData = temporary && source != null ? source.array[source.index] : array[index];

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

            item.GetComponent<UnityEngine.UI.Image>().color = new Color(item.GetComponent<UnityEngine.UI.Image>().color.r, item.GetComponent<UnityEngine.UI.Image>().color.g, item.GetComponent<UnityEngine.UI.Image>().color.b, clone == null ? 1 : 0.5f);
            item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color = new Color(item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color.r, item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color.g, item.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>().color.b, clone == null ? 1 : 0.75f);
            item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().color = new Color(item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().color.r, item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().color.g, item.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().color.b, clone == null ? 1 : 0.75f);
        }

        if (itemTypes.Count == 1 && itemTypes.Contains(ItemType.Equipment))
            Camera.main.GetComponent<PlayerController>().character.RefreshView();

        Camera.main.GetComponent<PlayerController>().character.CheckQuest();

        if (!temporary)
        {
            // Network
        }
    }

    public void ShowInfo()
    {
        if (array[index].quantity == 0)
            return;

        var info = Instantiate(Camera.main.GetComponent<PlayerController>().gameData.itemInfoPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        info.transform.GetChild(0).GetComponent<RectTransform>().position = transform.GetComponent<RectTransform>().position;
        info.GetComponent<ItemInfo>().Init(array[index].index);
    }
}
