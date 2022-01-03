using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Window : MonoBehaviour
{
    public bool draggable = true;
    public bool dragging = false;
    Vector2 delta = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (draggable && dragging)
            transform.GetComponent<RectTransform>().localPosition = (Vector2)Input.mousePosition - delta;
    }

    public void Init(string title, Transform content, float sizeX = 1, float sizeY = 1, bool _draggable = true)
    {
        transform.SetParent(GameObject.FindGameObjectWithTag("Canvas").transform);
        transform.localPosition = Vector2.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector2.one;
        GetComponent<RectTransform>().offsetMin = Vector2.one;
        GetComponent<RectTransform>().offsetMax = Vector2.one;
        GetComponent<RectTransform>().sizeDelta = new Vector2((Screen.width * sizeX) - Screen.width, (Screen.height * sizeY) - Screen.height);

        transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = title;

        content.SetParent(transform.GetChild(1));
        content.localPosition = Vector2.zero;
        content.localRotation = Quaternion.identity;
        content.localScale = Vector2.one;
        content.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        content.GetComponent<RectTransform>().offsetMax = Vector2.one;

        name = title;
        draggable = _draggable;
    }

    public void BeginDrag()
    {
        dragging = true;
        delta = Input.mousePosition - transform.GetComponent<RectTransform>().localPosition;
    }

    public void EndDrag()
    {
        dragging = false;
        delta = Vector2.zero;
    }

    public void Close()
    {
        Destroy(this.gameObject);
    }
}
