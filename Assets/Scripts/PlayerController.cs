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
    public GameObject dialogPanelPrefab;
    public GameObject upgradePanelPrefab;
    public GameObject shipControlPanelPrefab;
    public GameObject popupPrefab;
    public GameObject modifierIconPrefab;
    public Transform modifierPanel;
    public QuestViewer questViewer;
    public GameObject questsPanelPrefab;
    public GameObject mainMenu;
    public UnityEngine.UI.InputField username;
    public UnityEngine.UI.InputField password;
    public StickController stickController;
    public Drag dragPad;
    public UnityEngine.UI.Text playerName;
    public UnityEngine.UI.Text level;
    public UnityEngine.UI.Image exp;
    public UnityEngine.UI.Text sp;
    public UnityEngine.UI.Image hpbar;
    public UnityEngine.UI.Image staminabar;
    public float lookX = 2;
    public float zoom = 0;
    public Transform target;
    public Vector3 offset;
    public bool localOffset = true;
    public bool busy = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (character == null || busy)
            return;

        if (Input.GetKeyUp(KeyCode.C))
            character.ConsumeItemFromInventory(new InventoryItem(0,2));

        playerName.text = character.displayName;
        level.text = "Lv " + character.stats.level.ToString();
        exp.fillAmount = character.stats.exp;
        sp.text = character.stats.sp.ToString();
        hpbar.fillAmount = character.stats.health / character.stats.maxHealth;
        staminabar.fillAmount = character.stats.stamina / character.stats.maxStamina;

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

        if (Input.GetKey(KeyCode.D))
        {
            directionChange = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            directionChange = -1;
        }

        if (Input.GetKey(KeyCode.W))
        {
            speedChange = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            speedChange = -1;
        }

        character.speedChange = speedChange;
        character.directionChange = directionChange;
    }

    void FixedUpdate()
    {
        /*
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
        */
    }

    private void LateUpdate()
    {
        TraceCharacter();
    }

    public void Place(Transform _target, Vector3 _offset, bool _localOffset = true)
    {
        //Camera.main.transform.SetParent(target);
        offset = _offset;
        target = _target;
        localOffset = _localOffset;
        lookX = 1;
        //zoom = offset.z;
    }

    void TraceCharacter()
    {
        if (target == null)
            return;

        RaycastHit hit;

        if (Physics.Linecast(target.position + (Vector3.up * 2), character.transform.TransformPoint(offset + (Vector3.forward * zoom)), out hit))
        {
            transform.position = Vector3.Slerp(transform.position, character.transform.TransformPoint(offset + (Vector3.forward * -hit.distance)), 1);
        }
        else
        {
            transform.position = Vector3.Slerp(transform.position, character.transform.TransformPoint(offset + (Vector3.forward * zoom)), 1);
        }

        //transform.position = Vector3.Slerp(transform.position, character.transform.TransformPoint(offset + (Vector3.forward * zoom)), Time.deltaTime * 8);
        transform.LookAt(character.transform.TransformPoint(Vector3.up * lookX));
    }

    public void UseSkill(string skill)
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("send", "World", "Skill", skill, character.name);
    }

    public void ShowInventory()
    {
        if (GameObject.FindObjectOfType<Inventory>() != null || character == null || busy)
            return;

        var window = Instantiate<GameObject>(windowPrefab);
        var inventory = Instantiate<GameObject>(inventoryPrefab);
        window.GetComponent<Window>().Init("Inventory", inventory.transform, 0.5f, 0.9f);
    }

    public void ShowUpgrade()
    {
        if (GameObject.FindObjectOfType<UpgradePanel>() != null || character == null || busy)
            return;

        var window = Instantiate<GameObject>(windowPrefab);
        var content = Instantiate<GameObject>(upgradePanelPrefab);
        window.GetComponent<Window>().Init("Upgrade", content.transform, 0.5f, 0.9f);
    }

    public void ShowQuests()
    {
        if (GameObject.FindObjectOfType<QuestsPanel>() != null || character == null || busy)
            return;

        var window = Instantiate<GameObject>(windowPrefab);
        var content = Instantiate<GameObject>(questsPanelPrefab);
        window.GetComponent<Window>().Init("Quests", content.transform, 0.8f, 0.9f);
    }

    public void ShowPopup(string title, string text)
    {
        var popup = Instantiate<GameObject>(popupPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        popup.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = title;
        popup.transform.GetChild(1).GetComponent<UnityEngine.UI.Text>().text = text;
        Destroy(popup, 5);
    }

    public void ShowEntityInfo(Entity entity)
    {

    }

    public void Login()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkController>().Login(username.text, password.text);
    }

    public void ClearContent(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}

public static class GameFormat
{
    public static string Quantity(int quantity)
    {
        if (quantity >= 1000000000)
        {
            return (quantity / 1000000000f).ToString("n1") + "B";
        }
        else if (quantity >= 1000000)
        {
            return (quantity / 1000000f).ToString("n1") + "M";
        }
        else if (quantity >= 1000)
        {
            return (quantity / 1000f).ToString("n1") + "K";
        }
        else
        {
            return quantity.ToString();
        }
    }
}