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
    public int stackSize = 1;

    [Header("Consumable")]
    public float healthIncrease;
    public float foodIncrease;
    public float waterIncrease;

    [Header("Equipment")]
    public GameObject equipmentPrefab;
    public EquipmentType equipmentType;
    public float maxHealth;
    public float health;
    public float damageIncrease;
    public float armorIncrease;
    public float speedIncrease;
}

[System.Serializable]
public enum ItemType
{
    Collection,
    Consumable,
    Equipment
}

[System.Serializable]
public enum EquipmentType
{
    Weapon,
    Head,
    Body,
    Legs,
    Feet
}

[System.Serializable]
public enum WeaponType
{
    None,
    OneHanded,
    TwoHanded,
    Bow
}