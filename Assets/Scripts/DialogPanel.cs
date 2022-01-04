using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogPanel : MonoBehaviour
{
    public GameObject renderCameraPrefab;
    private GameObject renderCamera;
    public NPC npc;
    public int currentDialogIndex;

    public UnityEngine.UI.Text text;
    public UnityEngine.UI.ScrollRect options;
    public GameObject optionPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(NPC _npc)
    {
        npc = _npc;
        renderCamera = Instantiate<GameObject>(renderCameraPrefab, npc.cameraSocket);
        
        for (int i = 0; i < npc.dialogs.Length; i++)
        {
            if (npc.dialogs[i].CheckConditions())
            {
                ShowDialog(i);
                break;
            }
        }
    }

    public void ShowDialog(int index)
    {
        Dialog dialog = npc.dialogs[index];

        currentDialogIndex = index;
        text.text = dialog.text;

        foreach (var reward in dialog.rewards)
        {
            if (reward.type == ERewardType.Item)
            {
                Camera.main.GetComponent<PlayerController>().character.AddItemToInventory(new InventoryItem(reward.index, reward.amount));
            }
            else if (reward.type == ERewardType.Modifier)
            {
                var modifier = Camera.main.GetComponent<PlayerController>().gameData.modifiers[reward.index];
                Camera.main.GetComponent<PlayerController>().character.AddModifier(new ModifierData(reward.index));
            }
            else if (reward.type == ERewardType.Quest)
            {
                Camera.main.GetComponent<PlayerController>().character.UpdateQuest(new QuestData(reward.index, (EQuestStatus)reward.amount));
            }
        }

        Camera.main.GetComponent<PlayerController>().ClearContent(options.content);

        foreach (int option in dialog.options)
        {
            if (npc.dialogs[option].CheckConditions())
            {
                var o = Instantiate(optionPrefab, options.content);
                o.GetComponent<DialogOption>().dialog = this;
                o.GetComponent<DialogOption>().index = option;
                o.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = npc.dialogs[option].optionText;
            }
        }

        if (!string.IsNullOrEmpty(dialog.function))
        {
            dialog.Execute();
        }

        bool check = false;
        var questData = Camera.main.GetComponent<PlayerController>().gameData.quests;
        foreach (var q in Camera.main.GetComponent<PlayerController>().character.quests)
        {
            for (var i = 0; i < questData[q.index].conditions.Length; i++)
            {
                var c = questData[q.index].conditions[i];
                if (questData[q.index].status == EQuestStatus.Started && c.condition == EQuestCondition.Dialog && c.target == name && c.amount == currentDialogIndex)
                {
                    q.progress[i]++;
                    check = true;
                }
            }
        }

        if (check)
            Camera.main.GetComponent<PlayerController>().character.CheckQuest();
    }

    private void OnDestroy()
    {
        Destroy(renderCamera);
    }
}
