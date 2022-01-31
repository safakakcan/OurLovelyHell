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
        GetComponent<UConnect>().Send("ping");
    }

    public void ShowPing()
    {
        ping = (int)Mathf.Clamp((float)((System.DateTime.Now - pingTime).TotalMilliseconds), 1f, 999f);
        pingText.text = "FPS: " + ((int)(1 / Time.deltaTime)).ToString() + " | Ping: " + ping.ToString();
    }

    public void Connected(string[] data)
    {
        GetComponent<UConnect>().Connected = true;
    }

    public void Move(string[] data)
    {
        Entity entity = (from e in entities where e.name == data[1] select e).FirstOrDefault();
        
        if (entity != null)
        {
            if (!entity.authority)
            {
                entity.speedChange = int.Parse(data[2]);
                entity.directionChange = int.Parse(data[3]);
                Vector3 pos = new Vector3(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
                Quaternion rot = Quaternion.Euler(new Vector3(0, float.Parse(data[7]), 0));
                
                if (data[8] == "")
                {
                    if (entity.transform.parent != null)
                        entity.transform.parent = null;

                    pos = new Vector3(float.Parse(data[4]), float.Parse(data[5]), float.Parse(data[6]));
                }
                else
                {
                    if (entity.transform.root.name != data[8])
                        entity.transform.parent = (from ship in entities where ship.name == data[8] select ship.GetComponent<Ship>().board).FirstOrDefault();

                    pos = new Vector3(float.Parse(data[9]), float.Parse(data[10]), float.Parse(data[11]));
                }
                
                entity.fixedPosition = pos;
                entity.transform.rotation = rot;

                //entity.gameObject.SetActive(Vector3.Distance(Camera.main.transform.position, entity.transform.position) < 25);
            }
        }
        else
        {
            Debug.Log("ENTITY NOT FOUND: " + data[1]);
        }
    }

    public void Login(string username, string password)
    {
        GetComponent<UConnect>().Send(string.Format("{0}\n{1}\n{2}", "login", username, password));
    }

    public void JoinGame()
    {
        Camera.main.name = Camera.main.GetComponent<PlayerController>().username.text;
        GetComponent<UConnect>().Send("load");
        Camera.main.GetComponent<PlayerController>().mainMenu.SetActive(false);
    }

    public void LoadGame(string[] data)
    {
        if (data.Length > 1)
        {
            for (int i = 1; i < data.Length; i++)
            {
                string[] values = data[i].Split('\0');
                var c = Instantiate<GameObject>(characterPrefab);
                entities.Add(c.GetComponent<Entity>());
                
                c.name = values[0];
                c.GetComponent<Entity>().displayName = c.name;
                c.GetComponent<Entity>().speedChange = int.Parse(values[1]);
                c.GetComponent<Entity>().directionChange = int.Parse(values[2]);

                if (values[7] == "")
                {
                    c.transform.parent = null;
                    c.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                    c.GetComponent<Entity>().fixedPosition = c.transform.position;
                }
                else
                {
                    c.transform.parent = (from ship in entities where ship.name == values[7] select ship.GetComponent<Ship>().board).FirstOrDefault();
                    c.transform.localPosition = new Vector3(float.Parse(values[8]), float.Parse(values[9]), float.Parse(values[10]));
                    c.GetComponent<Entity>().fixedPosition = c.transform.localPosition;
                }

                c.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(values[6]), 0));

                c.GetComponent<Entity>().Init();
                Debug.Log("Spawn: " + values[0]);
            }
        }
        
        //string charName = "Player_" + (Random.Range(100000, 999999).ToString());
        //Camera.main.name = charName;
        Vector3 pos = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        GetComponent<UConnect>().Send(string.Format("{0}\n{1}\n{2}\n{3}\n{4}", "spawn", Camera.main.name, pos.x.ToString(), pos.y.ToString(), pos.z.ToString()));
    }

    public void SpawnCharacter(string[] data)
    {
        var character = Instantiate<GameObject>(characterPrefab);
        entities.Add(character.GetComponent<Entity>());
        
        character.transform.position = new Vector3(float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]));
        character.name = data[1];
        character.GetComponent<Entity>().displayName = data[1];

        if (Camera.main.name == character.name)
        {
            Camera.main.GetComponent<PlayerController>().Place(character.transform, new Vector3(0, 2, 0));
            Camera.main.GetComponent<PlayerController>().character = character.GetComponent<Character>();
            Camera.main.GetComponent<PlayerController>().gameUI.SetActive(true);
            character.GetComponent<Entity>().authority = true;
        }

        character.GetComponent<Entity>().Init();
        Debug.Log("Spawn: " + data[1]);
    }

    public void RemoveCharacter(string[] data)
    {
        Entity entity = (from e in entities where e.name == data[1] select e).FirstOrDefault();
        if (entity != null)
        {
            entities.Remove(entity);
            Destroy(entity.gameObject);
        }
    }

    public void UseSkill(string[] data)
    {
        Entity entity = (from e in entities where e.name == data[2] select e).FirstOrDefault();
        if (entity != null)
            entity.GetComponent<Animator>().SetTrigger(data[1]);
    }

    public void ApplyDamage(string[] data)
    {
        Entity causer = (from e in entities where e.name == data[1] select e).FirstOrDefault();
        Entity entity = (from e in entities where e.name == data[2] select e).FirstOrDefault();
        int damage = int.Parse(data[3]);
        
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

    public void SetAuthority(string[] data)
    {
        Entity entity = (from e in entities where e.name == data[1] select e).FirstOrDefault();
        entity.authority = Camera.main.name == data[2];
    }
}
