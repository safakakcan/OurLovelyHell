using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Ship : MonoBehaviour
{
    public Transform board;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Character character;
        if (other.TryGetComponent<Character>(out character))
        {
            other.transform.parent = board;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character character;
        if (other.TryGetComponent<Character>(out character))
        {
            if (other.transform.root.name == name)
                other.transform.parent = null;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.forward * GetComponent<Entity>().speedChange * Time.deltaTime);
        transform.Rotate(Vector3.up * GetComponent<Entity>().directionChange * Time.deltaTime);
    }

    private void OnMouseUpAsButton()
    {
        if (Camera.main.GetComponent<PlayerController>().character.transform.root.name == Camera.main.GetComponent<PlayerController>().character.name && 
            Vector3.Distance(Camera.main.GetComponent<PlayerController>().character.transform.position, transform.position) < 10)
        {
            Camera.main.GetComponent<PlayerController>().character.transform.parent = board;
            Camera.main.GetComponent<PlayerController>().character.transform.localPosition = Vector3.zero;
        }
    }
}
