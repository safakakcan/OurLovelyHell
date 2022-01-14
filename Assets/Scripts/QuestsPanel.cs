using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestsPanel : MonoBehaviour
{
    public int selectedQuestIndex = 0;
    public GameObject questTogglePrefab;
    public GameObject questInfoConditionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int select = 0)
    {
        selectedQuestIndex = select;
        var rect = transform.GetChild(0).GetComponent<ScrollRect>();
        Camera.main.GetComponent<PlayerController>().ClearContent(rect.content);

        for (int i = 0; i < Camera.main.GetComponent<PlayerController>().character.quests.Count; i++)
        {
            var q = Instantiate<GameObject>(questTogglePrefab, rect.content);
            q.GetComponent<QuestToggle>().Init(i);
        }

        if (rect.content.childCount > selectedQuestIndex)
            rect.content.GetChild(selectedQuestIndex).GetComponent<Toggle>().isOn = true;
    }

    public void Refresh()
    {
        var rect = transform.GetChild(0).GetComponent<ScrollRect>();

        for (int i = 0; i < rect.content.childCount; i++)
        {
            rect.content.GetChild(i).GetChild(2).GetComponent<Toggle>().SetIsOnWithoutNotify(Camera.main.GetComponent<PlayerController>().character.quests[i].show);
        }

        ShowQuestInfo(selectedQuestIndex);
    }

    public void ShowQuestInfo(int index)
    {
        selectedQuestIndex = index;
        var quest = Camera.main.GetComponent<PlayerController>().character.quests[index];
        var questData = Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index];

        transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<Text>().text = questData.questName;
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<Text>().text = questData.description;
        transform.GetChild(1).GetChild(0).GetChild(3).GetComponent<Text>().text = string.Format("Status: {0}", quest.status.ToString());
        transform.GetChild(1).GetChild(0).GetChild(4).GetComponent<Text>().text = string.Format("Start NPC: {0}", questData.startNPC);
        Camera.main.GetComponent<PlayerController>().ClearContent(transform.GetChild(1).GetChild(0).GetChild(7));
        for (int i = 0; i < questData.conditions.Length; i++)
        {
            var c = Instantiate<GameObject>(questInfoConditionPrefab, transform.GetChild(1).GetChild(0).GetChild(7));
            c.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}: {1} ({2}/{3})", 
                questData.conditions[i].condition.ToString(), questData.conditions[i].target, quest.progress[i], questData.conditions[i].amount);
        }
        transform.GetChild(1).GetChild(0).GetChild(9).GetComponent<Text>().text = string.Format("Complete NPC: {0}", questData.completeNPC);
        transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
    }
}
