using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New GameData", menuName = "GameData")]
public class GameData : ScriptableObject
{
    public GameObject itemPrefab;
    public List<Item> items;
}
