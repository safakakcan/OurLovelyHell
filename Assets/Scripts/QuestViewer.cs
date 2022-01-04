using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestViewer : MonoBehaviour
{
    [HideInInspector]
    public bool show = true;
    public GameObject questEntryPrefab;
    public GameObject conditionEntryPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Refresh()
    {
        var rect = transform.GetChild(0).GetComponent<ScrollRect>();
        Camera.main.GetComponent<PlayerController>().ClearContent(rect.content);

        for (int q = 0; q < Camera.main.GetComponent<PlayerController>().character.quests.Count; q++)
        {
            var quest = Camera.main.GetComponent<PlayerController>().character.quests[q];
            
            if (quest.show)
            {
                var questData = Camera.main.GetComponent<PlayerController>().gameData.quests[quest.index];

                if (quest.status == EQuestStatus.Started)
                {
                    var questEntry = Instantiate<GameObject>(questEntryPrefab, rect.content);
                    questEntry.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = questData.name;
                    questEntry.GetComponent<QuestEntry>().index = q;

                    for (int i = 0; i < quest.progress.Length; i++)
                    {
                        var conditionEntry = Instantiate<GameObject>(conditionEntryPrefab, questEntry.transform.GetChild(1));

                        conditionEntry.transform.GetChild(0).GetComponent<Text>().text = string.Format("{0}: {1} ({2}/{3})",
                            questData.conditions[i].condition.ToString(), questData.conditions[i].target, quest.progress[i], questData.conditions[i].amount);

                        conditionEntry.transform.GetChild(0).GetChild(0).gameObject.SetActive(quest.progress[i] >= questData.conditions[i].amount);
                    }
                }
                else if (quest.status == EQuestStatus.Completed)
                {
                    var questEntry = Instantiate<GameObject>(questEntryPrefab, rect.content);
                    questEntry.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = questData.name;

                    var conditionEntry = Instantiate<GameObject>(conditionEntryPrefab, questEntry.transform.GetChild(1));
                    conditionEntry.transform.GetChild(0).GetComponent<Text>().text = string.Format("Talk to {0}.", questData.completeNPC);
                }
            }
        }

        StartCoroutine(RefreshAsync());

        transform.GetChild(2).GetChild(0).gameObject.SetActive(transform.GetChild(0).GetComponent<ScrollRect>().content.childCount > 0);
    }

    IEnumerator RefreshAsync()
    {
        yield return new WaitForFixedUpdate();
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SetShow (bool toggle)
    {
        show = toggle;
        transform.GetChild(1).gameObject.SetActive(toggle);
        transform.GetChild(2).gameObject.SetActive(!toggle);
        GetComponent<RectTransform>().anchoredPosition = new Vector3(toggle ? -250 : 250, 200);
        transform.GetChild(2).GetChild(0).gameObject.SetActive(false);
    }
}
