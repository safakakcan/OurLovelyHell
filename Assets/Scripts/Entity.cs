using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Entity : MonoBehaviour
{
    public string displayName = "Entity";
    public bool authority = false;
    public Stats stats = new Stats();
    public List<ModifierData> statModifiers;

    public float speed = 0;
    public float direction = 0;
    public int speedChange = 0;
    public int directionChange = 0;
    public Vector3 fixedPosition = Vector3.zero;
    public float positionFixingSpeed = 2;
    public float wrapDistance = 4;
    public bool dead = false;
    public AudioClip hitSound;
    public Reward[] rewards;
    public float expReward = 1;

    public int updateFrequency = 5;
    Coroutine update = null;
    public Renderer bodyRenderer;

    // Start is called before the first frame update
    void Start()
    {
        fixedPosition = transform.position;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Animator animator;
        if (TryGetComponent<Animator>(out animator))
        {
            var totalStats = TotalStats();
            animator.SetFloat("Velocity", speed);
            animator.SetFloat("Direction", direction);
            animator.SetFloat("SkillSpeed", totalStats.skillSpeed);
            animator.SetFloat("MovementSpeed", totalStats.movementSpeed);
        }


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
        else
        {
            var i = 0;
            while (i < statModifiers.Count)
            {
                var modifier = statModifiers[i];

                if (!modifier.CountDown())
                {
                    statModifiers.RemoveAt(i);
                    continue;
                }

                i++;
            }
        }
    }

    public virtual void FixedUpdate()
    {
        if (Vector3.Distance(transform.localPosition, fixedPosition) > 0.1f && !authority)
        {
            if (Vector3.Distance(transform.localPosition, fixedPosition) < wrapDistance)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, fixedPosition, Time.deltaTime * positionFixingSpeed);
            }
            else
            {
                transform.localPosition = fixedPosition;
            }
        }
    }

    private void OnGUI()
    {
        if (dead)
            return;

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
        GUI.Label(new Rect(pos + new Vector2(-200, bodyRenderer.bounds.max.y - 50), new Vector2(400, 50)), "<color=orange>" + displayName + "</color>", style);
    }

    public void Init()
    {
        fixedPosition = transform.position;
    }

    public virtual Stats TotalStats()
    {
        Stats s = new Stats();

        s.attack = stats.attack;
        s.defence = stats.defence;
        s.maxHealth = stats.maxHealth;
        s.health = stats.health;
        s.maxStamina = stats.stamina;
        s.stamina = stats.stamina;
        s.movementSpeed = stats.movementSpeed;
        s.skillSpeed = stats.skillSpeed;

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

    public void AddExp(float value)
    {
        stats.exp += value * stats.expMultiplier;

        bool levelUp = false;

        while (true)
        {
            if (stats.exp >= 1)
            {
                stats.exp -= 1;
                stats.level++;
                levelUp = true;
            }
            else
            {
                break;
            }
        }

        if (levelUp && name == Camera.main.name)
        {
            Camera.main.GetComponent<PlayerController>().ShowPopup("LEVEL UP !", string.Format("You have reached level {0}.", stats.level));
        }
    }

    public void ApplyDamage(int damage, Character causer = null)
    {
        if (dead)
            return;

        stats.health -= damage;

        if (stats.health < 0)
        {
            stats.health = 0;
            dead = true;

            Animator animator;
            if (TryGetComponent<Animator>(out animator))
                GetComponent<Animator>().SetBool("Death", true);
            
            if (causer != null)
                causer.OnTargetKilled(this);
        }

        GetComponent<AudioSource>().PlayOneShot(hitSound);

        var fx = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.hitFX);
        fx.transform.position = bodyRenderer == null ? transform.position : bodyRenderer.bounds.center;

        var text = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.damageText);
        text.transform.position = bodyRenderer == null ? transform.position : bodyRenderer.bounds.center;
        text.transform.GetChild(0).GetComponent<TextMesh>().text = damage.ToString();
        var source = new UnityEngine.Animations.ConstraintSource();
        source.sourceTransform = Camera.main.transform;
        source.weight = 1;
        text.transform.GetChild(0).GetComponent<UnityEngine.Animations.LookAtConstraint>().AddSource(source);
        Destroy(text, 1);
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

    IEnumerator UpdateNetwork()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("Move", name, speedChange.ToString(), directionChange.ToString(),
                transform.localPosition.x.ToString("n2"), transform.localPosition.y.ToString("n2"), transform.localPosition.z.ToString("n2"), transform.rotation.eulerAngles.y.ToString("n2"), transform.root.name == name ? "" : transform.root.name);
        
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
    public float attack = 1;
    public float defence = 1;
    public float skillSpeed = 1;
    public float movementSpeed = 1;
    //public float accuracy = 1;
    //public float evasion = 1;
    public float expMultiplier = 1;
    public float spMultiplier = 1;
    public float dropMultiplier = 1;
    public float chance = 1;
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