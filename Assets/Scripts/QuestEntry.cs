using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestEntry : MonoBehaviour
{
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {
        Camera.main.GetComponent<PlayerController>().character.quests[index].show = false;
        Camera.main.GetComponent<PlayerController>().questViewer.Refresh();

        var questPanel = GameObject.FindObjectOfType<QuestsPanel>();
        if (questPanel != null)
        {
            questPanel.Refresh();
        }
    }
}
