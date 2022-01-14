using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest", menuName = "Game/Quest")]
public class Quest : ScriptableObject
{
    public string questName;
    public string description;
    public QuestCondition[] conditions;
    public string startNPC;
    public string completeNPC;
}

[System.Serializable]
public enum EQuestStatus
{
    Unavailable,
    Available,
    Started,
    Completed,
    Given
}

[System.Serializable]
public enum EQuestCondition
{
    Dialog,
    Bring,
    Kill,
    Explore,
}

[System.Serializable]
public class QuestCondition
{
    public EQuestCondition condition;
    public string target;
    public int amount;
}
