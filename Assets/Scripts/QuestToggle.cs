using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestToggle : MonoBehaviour
{
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init(int _index)
    {
        index = _index;
        transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = Camera.main.GetComponent<PlayerController>().gameData.quests[index].name;
        transform.GetChild(2).GetComponent<UnityEngine.UI.Toggle>().SetIsOnWithoutNotify(Camera.main.GetComponent<PlayerController>().character.quests[index].show);
    }

    public void Select()
    {
        if (GetComponent<UnityEngine.UI.Toggle>().isOn)
        {
            GameObject.FindObjectOfType<QuestsPanel>().ShowQuestInfo(index);
        }
    }

    public void SetShow()
    {
        Camera.main.GetComponent<PlayerController>().character.quests[index].show = transform.GetChild(2).GetComponent<UnityEngine.UI.Toggle>().isOn;
        Camera.main.GetComponent<PlayerController>().character.CheckQuest();
    }
}
