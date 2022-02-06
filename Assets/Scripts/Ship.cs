using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Entity))]
public class Ship : MonoBehaviour
{
    public float waterLevel = 0;
    public Transform board;
    public Vector3 cameraPosition;
    public ParticleSystem speedFX;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Entity>().speedChange > 2)
        {
            if (!speedFX.isPlaying)
                speedFX.Play(true);
        }
        else
        {
            if (speedFX.isPlaying)
                speedFX.Stop(true);
        }
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
        bool move = true;
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, 20))
        {
            Entity e;
            move = hit.collider.TryGetComponent<Entity>(out e);
        }

        if (move)
            transform.Translate(Vector3.forward * GetComponent<Entity>().speedChange * Time.deltaTime);
        transform.Rotate(Vector3.up * GetComponent<Entity>().directionChange * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(0, transform.localRotation.eulerAngles.y, Mathf.Clamp(-GetComponent<Entity>().directionChange, -10, 10)), Time.deltaTime);
        transform.position = new Vector3(transform.position.x, waterLevel, transform.position.z);
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
