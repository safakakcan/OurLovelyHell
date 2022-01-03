using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NPC : MonoBehaviour
{
    public float interactDistance = 3;
    public Transform cameraSocket;
    public Dialog[] dialogs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        GUI.Label(new Rect(pos + new Vector2(-200, -50), new Vector2(400, 50)), "<color=yellow>" + name + "</color>", style);
    }

    public void OpenDialog()
    {
        if (GameObject.FindObjectOfType<DialogPanel>() != null || Camera.main.GetComponent<PlayerController>().character == null || 
            Camera.main.GetComponent<PlayerController>().busy || Vector3.Distance(transform.position, Camera.main.GetComponent<PlayerController>().character.transform.position) > interactDistance)
            return;

        var window = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().windowPrefab);
        var dialog = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().dialogPanelPrefab);
        dialog.GetComponent<DialogPanel>().Init(this);
        window.GetComponent<Window>().Init(name, dialog.transform, 1, 1, false);
    }

    private void OnMouseUpAsButton()
    {
        OpenDialog();
    }
}

[System.Serializable]
public class Quest
{
    public string name;
    public string description;
    public QuestCondition[] conditions;
    public EQuestStatus status;
    public string startNPC;
    public string completeNPC;
}

[System.Serializable]
public class QuestData
{
    public int index;
    public EQuestStatus status;
    public int[] progress;
    public bool show = true;

    public QuestData(int _index, EQuestStatus _status)
    {
        index = _index;
        status = _status;
    }
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

[System.Serializable]
public enum ERewardType
{
    Item,
    Modifier,
    Quest
}

[System.Serializable]
public class Reward
{
    public ERewardType type;
    public int index;
    public int amount;
}

[System.Serializable]
public class Dialog
{
    public string optionText;
    public string text;
    public DialogCondition[] conditions;
    public int[] options;
    public Reward[] rewards;
    public string function;
    public string parameter;

    public bool CheckConditions()
    {
        var quests = Camera.main.GetComponent<PlayerController>().character.quests;
        bool valid = true;

        foreach (var condition in conditions)
        {
            valid = (from q in quests where q.index == condition.questIndex && condition.questStatus.Contains(q.status) select q).Any();

            if (!valid)
                valid = !(from q in quests where q.index == condition.questIndex select q).Any() && condition.questStatus.Contains(EQuestStatus.Unavailable);

            if (valid)
                valid = Camera.main.GetComponent<PlayerController>().character.stats.level >= condition.requiredLevel;

            if (!valid)
                break;
        }

        return valid;
    }

    public void Execute()
    {
        Camera.main.gameObject.SendMessage(function, parameter);
    }
}

[System.Serializable]
public class DialogCondition
{
    public int questIndex;
    public EQuestStatus[] questStatus;
    public int requiredLevel;
}

public class NPCShop
{
    public NPCShopItem[] items;
}

public class NPCShopItem
{
    public int index;
    public InventoryItem price;
}