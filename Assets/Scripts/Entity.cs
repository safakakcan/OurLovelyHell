using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Entity : MonoBehaviour
{
    public bool authority = false;
    public Stats stats = new Stats();
    public List<ModifierData> statModifiers;

    public float speed = 0;
    public float direction = 0;
    public int speedChange = 0;
    public int directionChange = 0;
    public Vector3 fixedPosition = Vector3.zero;
    public bool dead = false;
    public Reward[] rewards;

    public int updateFrequency = 5;
    Coroutine update = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        GetComponent<Animator>().SetFloat("Velocity", speed);
        GetComponent<Animator>().SetFloat("Direction", direction);
        GetComponent<Animator>().SetFloat("SkillSpeed", TotalStats().skillSpeed);
        GetComponent<Animator>().SetFloat("MovementSpeed", TotalStats().movementSpeed);


        if (Mathf.Abs(directionChange) != 0)
        {
            direction = Mathf.Clamp(direction + (Time.deltaTime * 4 * directionChange), -1, 1);
        }
        else
        {
            if (direction > 0)
            {
                direction = Mathf.Clamp(direction - (Time.deltaTime * 2), 0, 1);
            }
            else
            {
                direction = Mathf.Clamp(direction + (Time.deltaTime * 2), -1, 0);
            }
        }

        if (Mathf.Abs(speedChange) != 0)
        {
            speed = Mathf.Clamp(speed + (Time.deltaTime * 4 * speedChange), -1, 1);
        }
        else
        {
            if (speed > 0)
            {
                speed = Mathf.Clamp(speed - (Time.deltaTime * 2), 0, 1);
            }
            else
            {
                speed = Mathf.Clamp(speed + (Time.deltaTime * 2), -1, 0);
            }
        }

        if (authority)
        {
            if (update == null)
            {
                update = StartCoroutine(UpdateNetwork());
            }

            var i = 0;
            while (i < statModifiers.Count)
            {
                var modifier = statModifiers[i];

                if (!modifier.CountDown())
                {
                    if (name == Camera.main.name)
                        Destroy(statModifiers[i].icon);

                    statModifiers.RemoveAt(i);
                    continue;
                }
                else
                {
                    int time = (int)modifier.duration;
                    string formatted = time.ToString() + " sec";

                    if (time >= 86400)
                    {
                        time /= 86400;
                        formatted = time.ToString() + " day";
                    }
                    else if (time >= 3600)
                    {
                        time /= 3600;
                        formatted = time.ToString() + " hour";
                    }
                    else if (time >= 60)
                    {
                        time /= 60;
                        formatted = time.ToString() + " min";
                    }

                    modifier.icon.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = formatted;
                }

                i++;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, fixedPosition) > 0.1f && Camera.main.name != name)
        {
            if (Vector3.Distance(transform.position, fixedPosition) < 4)
            {
                transform.position = Vector3.MoveTowards(transform.position, fixedPosition, Time.deltaTime * 2);
            }
            else
            {
                transform.position = fixedPosition;
            }
        }
    }

    private void OnGUI()
    {
        var eventData = new UnityEngine.EventSystems.PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = (Vector2)Camera.main.WorldToScreenPoint(transform.position + (transform.up * 2));
        List<UnityEngine.EventSystems.RaycastResult> results = new List<UnityEngine.EventSystems.RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
        foreach (var result in results)
        {
            Window window;
            if (result.gameObject.TryGetComponent<Window>(out window))
                return;
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (Camera.main.name == name || Vector3.Distance(Camera.main.transform.position, transform.position) > 20 || !GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
            return;

        var style = new GUIStyle();
        style.fontSize = 40 - (int)Vector3.Distance(Camera.main.transform.position, transform.position);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.richText = true;
        
        var pos = (Vector2)Camera.main.WorldToScreenPoint(transform.position + (transform.up * 2));
        pos.y = Screen.height - pos.y;
        GUI.Label(new Rect(pos + new Vector2(-200, -50), new Vector2(400, 50)), "<color=white>" + name + "</color>", style);
    }

    public void Init()
    {
        fixedPosition = transform.position;
    }

    public virtual Stats TotalStats()
    {
        Stats s = new Stats();

        s.attack += stats.attack;
        s.defence += stats.defence;
        s.maxHealth += stats.maxHealth;
        s.health += stats.health;
        s.maxStamina += stats.stamina;
        s.stamina += stats.stamina;
        s.movementSpeed += stats.movementSpeed;
        s.skillSpeed += stats.skillSpeed;

        foreach (var modifierData in statModifiers)
        {
            var modifier = Camera.main.GetComponent<PlayerController>().gameData.modifiers[modifierData.index];
            s.attack = modifier.attack.type == EModifierType.Multiply ? s.attack * modifier.attack.amount : s.attack + modifier.attack.amount;
            s.defence = modifier.defence.type == EModifierType.Multiply ? s.defence * modifier.defence.amount : s.defence + modifier.defence.amount;
            s.maxHealth = modifier.maxHealth.type == EModifierType.Multiply ? s.maxHealth * modifier.maxHealth.amount : s.maxHealth + modifier.maxHealth.amount;
            s.maxStamina = modifier.maxStamina.type == EModifierType.Multiply ? s.maxStamina * modifier.maxStamina.amount : s.maxStamina + modifier.maxStamina.amount;
            s.movementSpeed = modifier.movementSpeed.type == EModifierType.Multiply ? s.movementSpeed * modifier.movementSpeed.amount : s.movementSpeed + modifier.movementSpeed.amount;
            s.skillSpeed = modifier.skillSpeed.type == EModifierType.Multiply ? s.skillSpeed * modifier.skillSpeed.amount : s.skillSpeed + modifier.skillSpeed.amount;
            s.expMultiplier = modifier.expMultiplier.type == EModifierType.Multiply ? s.expMultiplier * modifier.expMultiplier.amount : s.expMultiplier + modifier.expMultiplier.amount;
            s.spMultiplier = modifier.spMultiplier.type == EModifierType.Multiply ? s.spMultiplier * modifier.spMultiplier.amount : s.spMultiplier + modifier.spMultiplier.amount;
            s.chance = modifier.chance.type == EModifierType.Multiply ? s.chance * modifier.chance.amount : s.chance + modifier.chance.amount;
        }
        
        return s;
    }

    public bool AddModifier(ModifierData modifierData, bool force = false)
    {
        var old = (from m in statModifiers where m.index == modifierData.index select m).FirstOrDefault();
        var modifier = Camera.main.GetComponent<PlayerController>().gameData.modifiers[modifierData.index];

        if (old == null)
        {
            statModifiers.Add(modifierData);
            var icon = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().modifierIconPrefab, Camera.main.GetComponent<PlayerController>().modifierPanel);
            modifierData.icon = icon;
            icon.GetComponent<UnityEngine.UI.Image>().sprite = modifier.sprite;
            stats.exp = modifier.exp.type == EModifierType.Multiply ? stats.exp * modifier.exp.amount : stats.exp + modifier.exp.amount;
            stats.sp = modifier.sp.type == EModifierType.Multiply ? stats.sp * modifier.sp.amount : stats.sp + modifier.sp.amount;
            stats.health = modifier.health.type == EModifierType.Multiply ? stats.health * modifier.health.amount : stats.health + modifier.health.amount;
            stats.stamina = modifier.stamina.type == EModifierType.Multiply ? stats.stamina * modifier.stamina.amount : stats.stamina + modifier.stamina.amount;

            return true;
        }
        else if (modifier.replacable || force)
        {
            old.duration = modifierData.duration;

            stats.exp = modifier.exp.type == EModifierType.Multiply ? stats.exp * modifier.exp.amount : stats.exp + modifier.exp.amount;
            stats.sp = modifier.sp.type == EModifierType.Multiply ? stats.sp * modifier.sp.amount : stats.sp + modifier.sp.amount;
            stats.health = modifier.health.type == EModifierType.Multiply ? stats.health * modifier.health.amount : stats.health + modifier.health.amount;
            stats.stamina = modifier.stamina.type == EModifierType.Multiply ? stats.stamina * modifier.stamina.amount : stats.stamina + modifier.stamina.amount;

            return true;
        }
        else
        {
            return false;
        }
    }

    public void ApplyDamage(float damage)
    {
        stats.health -= damage / (stats.defence + 1);

        if (stats.health < 0)
        {
            stats.health = 0;
            Die();
        }
    }

    public void Restore(float value)
    {
        if (stats.health > 0)
        {
            stats.health += value;
            if (stats.health > stats.maxHealth)
                stats.health = stats.maxHealth;
        }
    }

    public void Die()
    {
        dead = true;
    }

    IEnumerator UpdateNetwork()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("Move", name, speedChange.ToString(), directionChange.ToString(),
                transform.position.x.ToString("n2"), transform.position.y.ToString("n2"), transform.position.z.ToString("n2"), transform.rotation.eulerAngles.y.ToString("n2"));
        
        yield return new WaitForSeconds((1 / updateFrequency));
        update = null;
    }
}

[System.Serializable]
public class Stats
{
    public int level = 1;
    public float exp = 0;
    public float sp = 0;
    public float maxHealth = 100;
    public float health = 100;
    public float maxStamina = 100;
    public float stamina = 100;
    public float attack = 0;
    public float defence = 0;
    public float skillSpeed = 0;
    public float movementSpeed = 0;
    //public float accuracy = 0;
    //public float evasion = 0;
    public float expMultiplier = 1;
    public float spMultiplier = 1;
    public float dropMultiplier = 1;
    public float chance = 1;
}

[System.Serializable]
public class StatModifiers
{
    public Sprite sprite;
    public string name;
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

    public bool CountDown()
    {
        if (duration.type == EDurationType.Limited)
        {
            duration.amount -= Time.deltaTime;
            return duration.amount > 0;
        }
        else
        {
            return true;
        }
    }
}

public interface IInteractable
{
    public string Interact();
}

[System.Serializable]
public class Modifier
{
    public EModifierType type;
    public float amount;
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

[System.Serializable]
public class ModifierData
{
    public int index;
    public float duration;
    public GameObject icon;

    public ModifierData(int _index)
    {
        index = _index;
        duration = Camera.main.GetComponent<PlayerController>().gameData.modifiers[index].duration.amount;
    }

    public bool CountDown()
    {
        var m = Camera.main.GetComponent<PlayerController>().gameData.modifiers[index];
        if (m.duration.type == EDurationType.Limited)
        {
            duration -= Time.deltaTime;
            return duration > 0;
        }
        else
        {
            return true;
        }
    }
}