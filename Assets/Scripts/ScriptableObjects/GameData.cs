using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "Game/Data")]
public class GameData : ScriptableObject
{
    [Header("Effects")]
    public AudioClip[] skillSounds;
    public GameObject[] slashFX;

    [Header("Items")]
    public GameObject itemPrefab;
    public GameObject itemInfoPrefab;
    public Item[] items;

    [Header("Quests")]
    public Quest[] quests;

    [Header("Modifiers")]
    public StatModifier[] modifiers;
}
