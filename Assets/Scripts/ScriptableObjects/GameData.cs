using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "Game/Data")]
public class GameData : ScriptableObject
{
    [Header("Curves")]
    public AnimationCurve expByLevel;
    public AnimationCurve expByLevelDifference;

    [Header("Effects")]
    public AudioClip[] skillSounds;
    public GameObject[] slashFX;
    public GameObject damageText;
    public GameObject hitFX;

    [Header("Items")]
    public GameObject itemPrefab;
    public GameObject itemInfoPrefab;
    public Item[] items;

    [Header("Quests")]
    public Quest[] quests;

    [Header("Modifiers")]
    public StatModifier[] modifiers;
}
