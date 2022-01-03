using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    [Header("Effects")]
    public AudioClip[] skillSounds;
    public GameObject[] slashFX;

    [Header("Items")]
    public GameObject itemPrefab;
    public GameObject itemInfoPrefab;
    public List<Item> items;

    [Header("Quests")]
    public Quest[] quests;

    [Header("Modifiers")]
    public StatModifiers[] modifiers;
}
