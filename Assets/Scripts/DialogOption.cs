using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogOption : MonoBehaviour
{
    [HideInInspector]
    public DialogPanel dialog;
    public int index;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Select()
    {
        dialog.ShowDialog(index);
    }
}
