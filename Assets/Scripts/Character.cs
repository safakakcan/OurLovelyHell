using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Character : Entity
{
    [Header("Social")]
    public string partyId = "";
    public string clan = "";

    [Header("Slots")]
    public int inventorySlotCount = 10;
    public InventoryItem[] equipments = new InventoryItem[6];
    public InventoryItem[] inventory = new InventoryItem[24];

    [Header("Quests")]
    public List<QuestData> quests;

    [Header("Equipment Sockets")]
    public Transform handR_Socket;
    public Transform handL_Socket;

    [Header("Sounds")]
    public AudioClip stepSound;

    // Start is called before the first frame update
    void Start()
    {
        inventory[0] = new InventoryItem(0, 952);
        inventory[1] = new InventoryItem(0, 765);
        inventory[2] = new InventoryItem(10, 1);
        inventory[3] = new InventoryItem(9, 1);
        inventory[4] = new InventoryItem(9, 1);

        equipments[0] = new InventoryItem(7, 1);
        equipments[1] = new InventoryItem(8, 1);

        RefreshView();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        //transform.Rotate(Vector3.up * direction * 4);
    }

    public override Stats TotalStats()
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

        foreach (var e in equipments)
        {
            var modifier = Camera.main.GetComponent<PlayerController>().gameData.items[e.index].statModifier;
            if (e.quantity > 0 && modifier != null)
            {
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
        }

        return s;
    }

    void Step(AnimationEvent e)
    {
        if (e.animatorClipInfo.weight > 0.5f)
        {
            GetComponent<AudioSource>().pitch = Random.Range(1f, 1.2f);
            GetComponent<AudioSource>().PlayOneShot(stepSound);
        }
    }

    void Slash(AnimationEvent e)
    {
        // forward distance; animation speed
        string[] data = e.stringParameter.Split(';');
        var slash = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().gameData.slashFX[0]);
        Vector3 front = transform.forward * float.Parse(data[0]);
        slash.transform.position = transform.position + Vector3.up + front;
        slash.transform.rotation = Quaternion.Euler(new Vector3(0, transform.rotation.eulerAngles.y, e.floatParameter));
        slash.transform.GetChild(0).GetComponent<Animator>().speed = (data.Length > 1 ? float.Parse(data[1]) : 1) * GetComponent<Animator>().GetFloat("SkillSpeed");
        GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.25f);
        GetComponent<AudioSource>().PlayOneShot(Camera.main.GetComponent<PlayerController>().gameData.skillSounds[e.intParameter]);
    }

    void Hit(AnimationEvent e)
    {
        if (authority)
        {
            //var colliders = Physics.OverlapSphere(transform.TransformPoint(new Vector3(0, 1, 2f)), 2);
            var colliders = Physics.OverlapSphere(GetComponent<Entity>().bodyRenderer.bounds.center + transform.TransformDirection(Vector3.forward * 2), 2);
            Entity target = null;
            
            foreach (var collider in colliders)
            {
                if (collider.name == name)
                    continue;

                Entity entity;
                if (collider.gameObject.TryGetComponent<Entity>(out entity))
                {
                    if (!entity.dead)
                    {
                        int damage = (int)(e.floatParameter * (TotalStats().attack / entity.TotalStats().defence) * Random.Range(0.9f, 1.1f) * (1 + (stats.level * 0.1f)));
                        FindObjectOfType<UConnect>().CallEvent("send", "World", "ApplyDamage", name, entity.name, damage.ToString());

                        if (target == null && name == Camera.main.name)
                        {
                            target = entity;
                            Camera.main.GetComponent<PlayerController>().ShowEntityInfo(target);
                            Camera.main.GetComponent<Animator>().SetTrigger("Shake");
                        }
                    }
                }
            }
        }
    }

    public int AddItemToInventory(InventoryItem item)
    {
        int quantity = item.quantity;
        var gameData = Camera.main.GetComponent<PlayerController>().gameData;

        for (int i = 0; i < inventorySlotCount; i++)
        {
            if (inventory[i].index == item.index)
            {
                int count = inventory[i].quantity + quantity;
                if (count > gameData.items[item.index].stackSize)
                {
                    quantity = (inventory[i].quantity + quantity) - gameData.items[item.index].stackSize;
                    inventory[i].quantity = gameData.items[item.index].stackSize;
                }
                else
                {
                    inventory[i].quantity = count;
                    quantity = 0;
                    break;
                }
            }
            else if (inventory[i].quantity == 0)
            {
                inventory[i].index = item.index;
                inventory[i].durability = item.durability;
                inventory[i].socket = item.socket;

                if (quantity > gameData.items[item.index].stackSize)
                {
                    quantity = quantity - gameData.items[item.index].stackSize;
                    inventory[i].quantity = gameData.items[item.index].stackSize;
                }
                else
                {
                    inventory[i].quantity = quantity;
                    quantity = 0;
                    break;
                }
            }
        }

        CheckQuest();
        var inventoryPanel = GameObject.FindObjectOfType<Inventory>();
        if (inventoryPanel != null)
            inventoryPanel.Refresh();

        return quantity;
    }

    public bool ConsumeItemFromInventory(InventoryItem item, bool consume = true)
    {
        int quantity = item.quantity;
        bool available = false;
        int count = 0;

        for (int i = 0; i < inventorySlotCount; i++)
        {
            if (inventory[i].index == item.index && inventory[i].quantity > 0)
            {
                count += inventory[i].quantity;

                if (count >= quantity)
                {
                    available = true;
                    break;
                }
            }
        }

        if (available && consume)
        {
            for (int i = 0; i < inventorySlotCount; i++)
            {
                if (inventory[i].index == item.index && inventory[i].quantity > 0)
                {
                    if (inventory[i].quantity - quantity >= 0)
                    {
                        inventory[i].quantity -= quantity;
                        quantity = 0;
                    }
                    else
                    {
                        quantity -= inventory[i].quantity;
                        inventory[i].quantity = 0;
                    }

                    if (quantity == 0)
                    {
                        break;
                    }
                }
            }
        }

        CheckQuest();
        var inventoryPanel = GameObject.FindObjectOfType<Inventory>();
        if (inventoryPanel != null)
            inventoryPanel.Refresh();

        return available;
    }

    public void UpdateQuest(QuestData newQuest)
    {
        var qData = Camera.main.GetComponent<PlayerController>().gameData.quests[newQuest.index];
        var quest = (from q in quests where q.index == newQuest.index select q).FirstOrDefault();

        if (quest == null)
        {
            quests.Add(newQuest);
            newQuest.progress = new int[qData.conditions.Length];
            CheckQuest();
            Camera.main.GetComponent<PlayerController>().ShowPopup("Quest: " + newQuest.status.ToString(), "\"" + qData.questName + "\"");
        }
        else if (quest.status != newQuest.status)
        {
            quest.status = newQuest.status;
            Camera.main.GetComponent<PlayerController>().ShowPopup("Quest: " + newQuest.status.ToString(), "\"" + qData.questName + "\"");
        }
    }

    public void CheckQuest()
    {
        foreach (var quest in quests)
        {
            if (quest.status == EQuestStatus.Started || quest.status == EQuestStatus.Completed)
            {
                bool valid = true;

                for (var i = 0; i < Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index].conditions.Length; i++)
                {
                    var condition = Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index].conditions[i];
                    if (condition.condition == EQuestCondition.Bring)
                    {
                        int count = 0;

                        foreach (var item in inventory)
                        {
                            if (Camera.main.GetComponent<PlayerController>().gameData.items[item.index].itemName == condition.target)
                            {
                                count += item.quantity;
                            }
                        }

                        quest.progress[i] = count;
                    }

                    if (quest.progress[i] < condition.amount)
                    {
                        valid = false;
                    }
                }

                if (valid)
                {
                    UpdateQuest(new QuestData(quest.index, EQuestStatus.Completed));
                }
                else
                {
                    UpdateQuest(new QuestData(quest.index, EQuestStatus.Started));
                }
            }
        }

        Camera.main.GetComponent<PlayerController>().questViewer.Refresh();
        var questPanel = GameObject.FindObjectOfType<QuestsPanel>();
        if (questPanel != null)
            questPanel.Refresh();
    }

    public void OnTargetKilled(Entity target)
    {
        var gameData = Camera.main.GetComponent<PlayerController>().gameData;
        float exp = target.expReward * gameData.expByLevelDifference.Evaluate(target.stats.level - stats.level) * gameData.expByLevel.Evaluate(stats.level);
        AddExp(exp);

        foreach (var reward in target.rewards)
        {
            if (reward.type == ERewardType.Item)
            {
                AddItemToInventory(new InventoryItem(reward.index, reward.amount));
            }
            else if (reward.type == ERewardType.Quest)
            {
                UpdateQuest(new QuestData(reward.index, (EQuestStatus)reward.amount));

            }
            else if (reward.type == ERewardType.Modifier)
            {
                AddModifier(new ModifierData(reward.index));
            }
        }

        bool check = false;

        foreach (var quest in quests)
        {
            if (quest.status == EQuestStatus.Started)
            {
                for (var i = 0; i < Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index].conditions.Length; i++)
                {
                    var condition = Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index].conditions[i];
                    if (condition.condition == EQuestCondition.Kill && condition.target == target.displayName)
                    {
                        quest.progress[i]++;
                        check = true;
                    }
                }
            }
        }

        if (check)
            CheckQuest();
    }

    public void RefreshView()
    {
        Camera.main.GetComponent<PlayerController>().ClearContent(handR_Socket);
        Camera.main.GetComponent<PlayerController>().ClearContent(handL_Socket);

        if (equipments[0].quantity > 0)
        {
            Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[equipments[0].index].equipmentPrefab, handR_Socket);
        }

        if (equipments[1].quantity > 0)
        {
            Instantiate(Camera.main.GetComponent<PlayerController>().gameData.items[equipments[1].index].equipmentPrefab, handL_Socket);
        }
    }
}

public enum ECareerType
{
    Fisher,
    Farmer,
    Miner,
    Alchemist
}

public class Career
{
    public ECareerType careerType;
    public float progress;
}