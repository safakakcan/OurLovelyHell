using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShipControlPanel : MonoBehaviour
{
    public Entity ship;
    public RectTransform rudder;
    public Slider sail;
    float angle = 0;
    float delta = 0;

    // Start is called before the first frame update
    void Start()
    {
        Camera.main.GetComponent<PlayerController>().gameUI.SetActive(false);
        Camera.main.GetComponent<PlayerController>().character.transform.localRotation = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        if (ship != null)
        {
            ship.directionChange = (int)angle / 5;
            ship.speedChange = (int)sail.value;
        }
    }

    public void Init(Entity _ship)
    {
        ship = _ship;
        angle = ship.directionChange * 5;
        rudder.localEulerAngles = new Vector3(0, 0, -angle);
        sail.value = ship.speedChange;
    }

    public void SetDelta()
    {
        Vector2 anchorPos = (Vector2)Input.mousePosition - new Vector2(rudder.position.x, rudder.position.y);
        anchorPos = new Vector2(anchorPos.x / rudder.lossyScale.x, anchorPos.y / rudder.lossyScale.y);
        delta = Mathf.Clamp(Vector3.SignedAngle(anchorPos - rudder.anchoredPosition, Vector3.up, Vector3.forward), -90, 90);
    }

    public void SetDirection()
    {
        Vector2 anchorPos = (Vector2)Input.mousePosition - new Vector2(rudder.position.x, rudder.position.y);
        anchorPos = new Vector2(anchorPos.x / rudder.lossyScale.x, anchorPos.y / rudder.lossyScale.y);
        angle = Mathf.Clamp(angle + (Vector3.SignedAngle(anchorPos - rudder.anchoredPosition, Vector3.up, Vector3.forward) - delta), -90, 90);
        
        rudder.localEulerAngles = new Vector3(0, 0, -angle);
        SetDelta();
    }

    public void Leave()
    {
        Destroy(this.gameObject);
    }

    private void OnDestroy()
    {
        Camera.main.GetComponent<PlayerController>().gameUI.SetActive(true);
    }
}
