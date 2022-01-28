using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NetworkController : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject characterPrefab;

    [Header("Entities")]
    [SerializeField]
    public List<Entity> entities = new List<Entity>();

    [Header("Connection")]
    public int ping = 999;
    System.DateTime pingTime = System.DateTime.Now;
    float pingDelay = 1;

    [Header("UI")]
    public UnityEngine.UI.Text pingText;

    // Start is called before the first frame update
    void Start()
    {
        entities.AddRange(FindObjectsOfType<Entity>());
    }

    // Update is called once per frame
    void Update()
    {
        pingDelay -= Time.deltaTime;

        if (pingDelay <= 0)
        {
            SendPing();
            pingDelay = 1;
        }
    }

    void SendPing()
    {
        pingTime = System.DateTime.Now;
        GetComponent<UConnect>().SendRequest("Ping");
    }

    public void ShowPing()
    {
        ping = (int)Mathf.Clamp((float)((System.DateTime.Now - pingTime).TotalMilliseconds), 1f, 999f);
        pingText.text = "FPS: " + ((int)(1 / Time.deltaTime)).ToString() + " | Ping: " + ping.ToString();
    }

    public void Move(NetworkData data)
    {
        Entity entity = (from e in entities where e.name == data.array[1] select e).FirstOrDefault();

        if (entity != null)
        {
            if (!entity.authority)
            {
                entity.speedChange = int.Parse(data.array[2]);
                entity.directionChange = int.Parse(data.array[3]);
                var pos = new Vector3(float.Parse(data.array[4]), float.Parse(data.array[5]), float.Parse(data.array[6]));
                var rot = Quaternion.Euler(new Vector3(0, float.Parse(data.array[7]), 0));

                if (data.array[8] == "" && entity.transform.parent != null)
                {
                    entity.transform.parent = null;
                }
                else if (entity.transform.root.name != data.array[8])
                {
                    entity.transform.parent = (from ship in entities where ship.name == data.array[8] select ship.GetComponent<Ship>().board).FirstOrDefault();
                }

                entity.transform.rotation = rot;
                entity.fixedPosition = pos;
            }
        }
        else
        {
            Debug.Log("ENTITY NOT FOUND: " + data.array[1]);
        }
    }

    public void Login(string username, string password)
    {
        GetComponent<UConnect>().SendRequest("login", username, password);
    }

    public void JoinGame(NetworkData data)
    {
        GetComponent<UConnect>().SendRequest("JoinGame", "World");
        Camera.main.GetComponent<PlayerController>().mainMenu.SetActive(false);
    }

    public void LoadGame(NetworkData data)
    {
        if (data.array.Length > 0 && data.array[0] != "")
        {
            string[] characters = data.array[0].Split(';');
            foreach (string character in characters)
            {
                string[] values = character.Split('|');
                var c = Instantiate<GameObject>(characterPrefab);
                entities.Add(c.GetComponent<Entity>());

                c.name = values[0];
                c.GetComponent<Entity>().displayName = c.name;
                c.GetComponent<Entity>().speedChange = int.Parse(values[1]);
                c.GetComponent<Entity>().directionChange = int.Parse(values[2]);

                if (values[7] == "" && c.transform.parent != null)
                {
                    c.transform.parent = null;
                }
                else if (c.transform.root.name != values[7])
                {
                    c.transform.parent = (from ship in entities where ship.name == values[7] select ship.GetComponent<Ship>().board).FirstOrDefault();
                }

                c.transform.localPosition = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                c.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(values[6]), 0));

                c.GetComponent<Entity>().Init();
                Debug.Log("Spawn: " + values[0]);
            }
        }
        
        string charName = "Player_" + (Random.Range(100000, 999999).ToString());
        Camera.main.name = charName;
        Vector3 pos = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        GetComponent<UConnect>().CallEvent("SpawnCharacter", charName, pos.x.ToString(), pos.y.ToString(), pos.z.ToString(), "");
    }

    public void SpawnCharacter(NetworkData data)
    {
        var character = Instantiate<GameObject>(characterPrefab);
        entities.Add(character.GetComponent<Entity>());

        if (data.array[6] == "" && character.transform.parent != null)
        {
            character.transform.parent = null;
        }
        else if (character.transform.root.name != data.array[6])
        {
            character.transform.parent = (from ship in entities where ship.name == data.array[6] select ship.GetComponent<Ship>().board).FirstOrDefault();
        }

        character.transform.localPosition = new Vector3(float.Parse(data.array[2]), float.Parse(data.array[3]), float.Parse(data.array[4]));
        character.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(data.array[5]), 0));
        character.name = data.array[1];
        character.GetComponent<Entity>().displayName = data.array[1];

        if (Camera.main.name == character.name)
        {
            Camera.main.GetComponent<PlayerController>().Place(character.transform, new Vector3(0, 2, 0));
            Camera.main.GetComponent<PlayerController>().character = character.GetComponent<Character>();
            Camera.main.GetComponent<PlayerController>().gameUI.SetActive(true);
            character.GetComponent<Entity>().authority = true;
        }

        character.GetComponent<Entity>().Init();
        Debug.Log("Spawn: " + data.array[1]);
    }

    public void RemoveCharacter(NetworkData data)
    {
        Entity entity = (from e in entities where e.name == data.array[1] select e).FirstOrDefault();
        if (entity != null)
        {
            entities.Remove(entity);
            Destroy(entity.gameObject);
        }
    }

    public void UseSkill(NetworkData data)
    {
        Entity entity = (from e in entities where e.name == data.array[2] select e).FirstOrDefault();
        if (entity != null)
            entity.GetComponent<Animator>().SetTrigger(data.array[1]);
    }

    public void ApplyDamage(NetworkData data)
    {
        Entity causer = (from e in entities where e.name == data.array[1] select e).FirstOrDefault();
        Entity entity = (from e in entities where e.name == data.array[2] select e).FirstOrDefault();
        int damage = int.Parse(data.array[3]);
        
        Character character = null;
        if (causer.gameObject.TryGetComponent<Character>(out character))
        {
            entity.ApplyDamage(damage, character);
        }
        else
        {
            entity.ApplyDamage(damage);
        }
    }

    public void SetAuthority(NetworkData data)
    {
        Entity entity = (from e in entities where e.name == data.array[1] select e).FirstOrDefault();
        entity.authority = Camera.main.name == data.array[2];
    }
}
