using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, UnityEngine.EventSystems.IDragHandler, UnityEngine.EventSystems.IBeginDragHandler, UnityEngine.EventSystems.IEndDragHandler
{
    public bool dragging = false;
    Vector2 beginPos;
    int fingerId = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDrag(UnityEngine.EventSystems.PointerEventData data)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            Camera.main.GetComponent<PlayerController>().lookX = Mathf.Clamp(Camera.main.GetComponent<PlayerController>().lookX - ((Input.mousePosition.y - beginPos.y) * -0.005f), -3, 6);
            if (Camera.main.GetComponent<PlayerController>().localOffset)
            {
                Camera.main.GetComponent<PlayerController>().target.transform.rotation = 
                    Quaternion.Euler(new Vector3(0, Camera.main.GetComponent<PlayerController>().character.transform.rotation.eulerAngles.y + ((Input.mousePosition.x - beginPos.x) * 0.25f), 0));
            }
            else
            {
                Camera.main.GetComponent<PlayerController>().offset = Quaternion.AngleAxis(((Input.mousePosition.x - beginPos.x) * 0.25f), Vector3.up) * Camera.main.GetComponent<PlayerController>().offset;
            }

            //Camera.main.GetComponent<PlayerController>().character.directionChange = (int)Mathf.Round(Mathf.Clamp((Input.mousePosition.x - beginPos.x) / 100, -1, 1));
            
            beginPos = Input.mousePosition;
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

        if (valid)
        {
            Camera.main.GetComponent<PlayerController>().lookX = Mathf.Clamp(Camera.main.GetComponent<PlayerController>().lookX - (touch.deltaPosition.y * -0.01f), -3, 6);
            if (Camera.main.GetComponent<PlayerController>().localOffset)
            {
                Camera.main.GetComponent<PlayerController>().target.transform.rotation = 
                    Quaternion.Euler(new Vector3(0, Camera.main.GetComponent<PlayerController>().character.transform.rotation.eulerAngles.y + (touch.deltaPosition.x * 0.25f), 0));
            }
            else
            {
                Camera.main.GetComponent<PlayerController>().offset = Quaternion.AngleAxis((touch.deltaPosition.x * 0.25f), Vector3.up) * Camera.main.GetComponent<PlayerController>().offset;
            }

            //Camera.main.GetComponent<PlayerController>().character.directionChange = (int)Mathf.Round(Mathf.Clamp(touch.deltaPosition.x / 100, -1, 1));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragging = true;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            beginPos = Input.mousePosition;
            return;
        }

        if (Input.touchCount == 0)
            return;

        Touch touch = new Touch();
        float distance = Mathf.Infinity;
        foreach (Touch t in Input.touches)
        {
            float dist = Vector2.Distance(t.position, new Vector2(Screen.width, 0));
            if (dist < distance)
            {
                touch = t;
                distance = dist;
            }
        }

        fingerId = touch.fingerId;
        beginPos = touch.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragging = false;
        Camera.main.GetComponent<PlayerController>().character.directionChange = 0;
    }
}
