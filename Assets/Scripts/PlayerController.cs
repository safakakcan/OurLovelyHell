using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameData gameData;
    public Character character;
    public GameObject gameUI;
    public GameObject windowPrefab;
    public GameObject inventoryPrefab;
    public StickController stickController;
    public Drag dragPad;
    public UnityEngine.UI.Text pingText;
    public UnityEngine.UI.Image hpbar;
    public float lookX = 15;
    float zoom = -2.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (character == null)
            return;

        if (Input.GetKeyDown(KeyCode.X))
            character.ApplyDamage(10);

        hpbar.fillAmount = character.health / character.maxHealth;
        pingText.text = "Ping: " + GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkController>().ping.ToString();

        int speedChange;
        int directionChange;

        if (Mathf.Abs(stickController.Value.x) > 0.25f)
        {
            directionChange = stickController.Value.x > 0 ? 1 : -1;
        }
        else
        {
            directionChange = 0;
        }

        if (Mathf.Abs(stickController.Value.y) > 0.25f)
        {
            speedChange = stickController.Value.y > 0 ? 1 : -1;
        }
        else
        {
            speedChange = 0;
        }

        character.speedChange = speedChange;
        character.directionChange = directionChange;
    }

    void FixedUpdate()
    {
        if (character == null)
            return;

        transform.localRotation = Quaternion.Euler(new Vector3(lookX, transform.localRotation.y, transform.localRotation.z));

        var targetPoint = character.transform.position + (character.transform.up * 2);
        var camPoint = character.transform.TransformPoint(new Vector3(0, 2, zoom));
        RaycastHit hit;

        if (Physics.Linecast(targetPoint, camPoint, out hit))
        {
            transform.localPosition = new Vector3(0, 2, -hit.distance + 0.1f);
        }
        else
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, zoom);
        }

        Debug.DrawLine(targetPoint, camPoint, Color.red);
    }

    void TraceCharacter()
    {
        transform.position = Vector3.Slerp(transform.position, character.transform.TransformPoint(new Vector3(-1, 2.5f, -2.75f)), Time.deltaTime * 4);
        transform.LookAt(character.transform.TransformPoint(new Vector3(-1, 1.75f, 0)));
    }

    public void MeleeAttackBegin()
    {
        if (character.equipments[2].quantity > 0 && gameData.items[character.equipments[2].index].itemType == ItemType.Equipment && gameData.items[character.equipments[2].index].equipmentType == EquipmentType.Weapon)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "MeleeAttack", character.name, "1");
    }

    public void MeleeAttackEnd()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "MeleeAttack", character.name, "0");
    }

    public void JumpAttackBegin()
    {
        if (character.equipments[2].quantity > 0 && gameData.items[character.equipments[2].index].itemType == ItemType.Equipment && gameData.items[character.equipments[2].index].equipmentType == EquipmentType.Weapon)
            GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "JumpAttack", character.name, "true");
    }

    public void JumpAttackEnd()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "JumpAttack", character.name, "false");
    }

    public void JumpBegin()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "Jump", character.name, "true");
    }

    public void ShowInventory()
    {
        if (GameObject.FindObjectOfType<Inventory>() != null)
            return;

        var window = Instantiate<GameObject>(windowPrefab);
        var inventory = Instantiate<GameObject>(inventoryPrefab);

        window.transform.SetParent(gameUI.transform);
        window.transform.localPosition = Vector2.zero;
        window.transform.localRotation = Quaternion.identity;
        window.transform.localScale = Vector2.one;
        window.GetComponent<RectTransform>().offsetMin = Vector2.one;
        window.GetComponent<RectTransform>().offsetMax = Vector2.one;
        window.GetComponent<RectTransform>().sizeDelta = new Vector2(900, window.GetComponent<RectTransform>().sizeDelta.y - 100);

        window.GetComponent<Window>().Init("Inventory", inventory.transform);
    }
}
