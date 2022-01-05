using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("General")]
    public string itemName;
    public string description;
    public ItemType itemType;
    public Sprite sprite;
    public int requiredLevel = 1;
    public int stackSize = 1;
    public InventoryItem price = new InventoryItem(0, 1);

    [Header("Consumable")]
    public string function;

    [Header("Equipment")]
    public GameObject equipmentPrefab;
    public EquipmentType equipmentType;
    public EquipmentGrade grade;
    public float maxDurability;
    public StatModifiers statModifiers;
}

[System.Serializable]
public enum ItemType
{
    Collection,
    Consumable,
    Alchemy,
    Equipment,
    Crystal,
    Quest
}

[System.Serializable]
public enum EquipmentType
{
    PrimaryWeapon,
    SecondaryWeapon,
    Head,
    Body,
    Legs,
    Feet
}

public enum Grade
{
    Dark,
    Normal,
    Polished,
    Illuminated,
    Reinforced,
    Enchanted,
    Resurrected,
    Blessed,
    Deified
}

public class EquipmentGrade
{
    public Grade grade = Grade.Normal;
    public int lowerGradeIndex = -1;
    public int higherGradeIndex = -1;
    public InventoryItem[] recipe;
}