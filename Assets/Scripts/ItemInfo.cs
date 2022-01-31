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
        string desc = string.Empty;

        if (item.itemType == ItemType.Equipment)
        {
            desc = string.Format("{0}\n\nItem Type: {1} ({2})\nDurability: {3} / {4}\nRequired Level: {5}\n\nPrice: {6}\n",
            item.description, item.itemType, item.equipmentType, slot.array[slot.index].durability, item.maxDurability, item.requiredLevel, System.String.Format("{0:n0}", item.price.quantity * slot.array[slot.index].quantity));
        }
        else
        {
            desc = string.Format("{0}\n\nItem Type: {1}\nDurability: {2} / {3}\nRequired Level: {4}\n\nPrice: {5}\n",
            item.description, item.itemType, slot.array[slot.index].durability, item.maxDurability, item.requiredLevel, System.String.Format("{0:n0}", item.price.quantity * slot.array[slot.index].quantity));
        }

        if (item.statModifier != null)
        {
            using (var modifier = item.statModifier.attack)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nAttack: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.defence)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nDefence: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.maxHealth)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nMax Health: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.health)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nHealth: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.maxStamina)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nMax Stamina: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.stamina)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nStamina: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.expMultiplier)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nEXP Multiplier: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.spMultiplier)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nSP Multiplier: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.dropMultiplier)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nDrop Multiplier: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.movementSpeed)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nMovement Speed: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }

            using (var modifier = item.statModifier.skillSpeed)
            {
                if ((modifier.type == EModifierType.Sum && modifier.amount != 0) || (modifier.type == EModifierType.Multiply && modifier.amount != 1))
                    desc += "\nSkill Speed: " + (modifier.type == EModifierType.Sum ? (modifier.amount > 0 ? "+" : "") + modifier.amount.ToString() : (modifier.amount > 0 ? "+" : "") + ((modifier.amount - 1) * 100).ToString() + "%");
            }
        }

        transform.GetChild(1).GetChild(2).GetComponent<UnityEngine.UI.Text>().text = desc;
        transform.GetChild(1).GetChild(3).gameObject.SetActive(Camera.main.GetComponent<PlayerController>().gameData.items[slot.array[slot.index].index].function != "");
    }

    public void Use()
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
