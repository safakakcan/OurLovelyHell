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
        var slot = transform.GetChild(0).GetChild(0).GetComponent<ItemSlot>();
        if (slot.source == null)
            return;

        var item = slot.source.array[slot.source.index];
        var itemData = Camera.main.GetComponent<PlayerController>().gameData.items[item.index];

        if (itemData.grade.higherGradeIndex == -1 || itemData.grade.grade == Grade.Dark || item.quantity == 0)
            return;

        bool valid = true;
        for (int i = 0; i < itemData.grade.recipe.Length; i++)
        {
            var recipeSlot = transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>();
            if (recipeSlot.array[recipeSlot.index].index != itemData.grade.recipe[i].index || recipeSlot.array[recipeSlot.index].quantity < itemData.grade.recipe[i].quantity)
            {
                valid = false;
                break;
            }
        }

        if (valid)
        {
            for (int i = 0; i < itemData.grade.recipe.Length; i++)
            {
                var recipeSlot = transform.GetChild(1).GetChild(i).GetComponent<ItemSlot>();
                recipeSlot.array[recipeSlot.index].quantity -= itemData.grade.recipe[i].quantity;
                recipeSlot.Refresh();
            }

            item.quantity--;
            var newItem = new InventoryItem(itemData.grade.higherGradeIndex, 1);
            newItem.durability = Camera.main.GetComponent<PlayerController>().gameData.items[itemData.grade.higherGradeIndex].maxDurability;
            newItem.socket = item.socket;

            Camera.main.GetComponent<PlayerController>().character.AddItemToInventory(newItem);
        }
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
