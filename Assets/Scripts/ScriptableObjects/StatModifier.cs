using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Modifier", menuName = "Game/Modifier")]
public class StatModifier : ScriptableObject
{
    public Sprite sprite;
    public string modifierName;
    public bool replacable;
    public Modifier exp;
    public Modifier sp;
    public Modifier maxHealth;
    public Modifier health;
    public Modifier maxStamina;
    public Modifier stamina;
    public Modifier attack;
    public Modifier defence;
    public Modifier skillSpeed;
    public Modifier movementSpeed;
    //public Modifier accuracy;
    //public Modifier evasion;
    public Modifier expMultiplier;
    public Modifier spMultiplier;
    public Modifier dropMultiplier;
    public Modifier chance;
    public Duration duration;
    public GameObject fx;
}


[System.Serializable]
public class Modifier : System.IDisposable
{
    public EModifierType type;
    public float amount;

    public void Dispose()
    {
        return;
    }
}

[System.Serializable]
public enum EModifierType
{
    Sum,
    Multiply
}

[System.Serializable]
public enum EDurationType
{
    Limited,
    Unlimited
}

[System.Serializable]
public class Duration
{
    public EDurationType type;
    public float amount;
}