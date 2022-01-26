using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour
{
    public Ship ship;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUpAsButton()
    {
        if (ship.GetComponent<Entity>().authority && Vector3.Distance(Camera.main.GetComponent<PlayerController>().character.transform.position, transform.position) < 5)
        {
            if (GameObject.FindObjectOfType<ShipControlPanel>() != null || Camera.main.GetComponent<PlayerController>().character == null || Camera.main.GetComponent<PlayerController>().busy)
                return;

            //var window = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().windowPrefab);
            var content = Instantiate<GameObject>(Camera.main.GetComponent<PlayerController>().shipControlPanelPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
            //window.GetComponent<Window>().Init("Ship Control", content.transform, 1, 1, false);
            content.GetComponent<ShipControlPanel>().Init(ship.GetComponent<Entity>());
        }
    }
}
