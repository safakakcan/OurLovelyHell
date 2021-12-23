using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickController : MonoBehaviour
{
    public Vector2 Value { get { return transform.GetChild(0).localPosition / transform.GetComponent<RectTransform>().rect.width; } }
    public UnityEngine.Events.UnityEvent onHoldBegin;
    public UnityEngine.Events.UnityEvent onHoldEnd;

    bool trace = false;
    Vector2 beginPos = Vector2.zero;
    int fingerId = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (trace && Input.GetMouseButtonUp(0))
            {
                trace = false;
                beginPos = Vector2.zero;
                transform.GetChild(0).localPosition = Vector2.zero;
                onHoldEnd.Invoke();
            }
            else if (trace)
            {
                transform.GetChild(0).localPosition = Vector2.MoveTowards(Vector2.zero, (Vector2)Input.mousePosition - beginPos, transform.GetComponent<RectTransform>().rect.width / 2);
            }
            return;
        }

        Touch touch = new Touch();
        bool valid = false;
        foreach (Touch t in Input.touches)
        {
            if (t.fingerId == fingerId)
            {
                touch = t;
                valid = true;
            }
        }

        if (trace && !valid)
        {
            trace = false;
            beginPos = Vector2.zero;
            transform.GetChild(0).localPosition = Vector2.zero;
            onHoldEnd.Invoke();
        }
        else if (trace && valid)
        {
            transform.GetChild(0).localPosition = Vector2.MoveTowards(Vector2.zero, touch.position - beginPos, transform.GetComponent<RectTransform>().rect.width / 2);
        }
    }

    public void Hold()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            trace = true;
            beginPos = Input.mousePosition;
            onHoldBegin.Invoke();
            return;
        }

        if (trace || Input.touchCount == 0)
            return;

        Touch touch = new Touch();
        float distance = Mathf.Infinity;
        foreach (Touch t in Input.touches)
        {
            float dist = Vector2.Distance(transform.GetComponent<Rect>().center, t.position);
            if (dist < distance)
            {
                distance = dist;
                touch = t;
            }
        }

        fingerId = touch.fingerId;
        trace = true;
        beginPos = touch.position;
        onHoldBegin.Invoke();
    }
}
