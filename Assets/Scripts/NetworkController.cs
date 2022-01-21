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
    private Coroutine pingDetect = null;
    private System.DateTime pingTime = System.DateTime.Now;

    [Header("UI")]
    public UnityEngine.UI.Text pingText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (pingDetect == null)
        {
            pingDetect = StartCoroutine(Ping());
        }
        
    }

    IEnumerator Ping()
    {
        yield return new WaitForSeconds(1);
        pingTime = System.DateTime.Now;
        GetComponent<UConnect>().SendRequest("Ping", pingTime.Ticks.ToString());
    }

    public void CalculatePing(NetworkData data)
    {
        if (data.array[0] == pingTime.Ticks.ToString())
        {
            var time = System.DateTime.Now - pingTime;
            ping = Mathf.Clamp((int)time.TotalMilliseconds, 1, 999);
        }
        else
        {
            ping = 999;
        }

        pingDetect = null;
        pingText.text = "Ping: " + GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkController>().ping.ToString();
    }

    public void UpdateTransforms(NetworkData data)
    {
        string[] updatedTransforms = data.array[1].Split(';');

        foreach (string updatedTransform in updatedTransforms)
        {
            string[] values = updatedTransform.Split('|');
            Entity entity = (from e in entities where e.name == values[0] select e).FirstOrDefault();

            if (entity != null)
            {
                if (Camera.main.name != entity.name)
                {
                    entity.speedChange = int.Parse(values[1]);
                    entity.directionChange = int.Parse(values[2]);
                    entity.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(values[6]), 0));
                }
                
                var pos = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                entity.fixedPosition = pos;
            }
            else
            {
                Debug.Log("ENTITY NOT FOUND: " + values[0]);
            }
        }
    }

    public void Move(NetworkData data)
    {
        Entity entity = (from e in entities where e.name == data.array[1] select e).FirstOrDefault();

        if (entity != null)
        {
            if (Camera.main.name != entity.name)
            {
                entity.speedChange = int.Parse(data.array[2]);
                entity.directionChange = int.Parse(data.array[3]);
                entity.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(data.array[7]), 0));
                var pos = new Vector3(float.Parse(data.array[4]), float.Parse(data.array[5]), float.Parse(data.array[6]));
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
        if (data.array.Length > 0)
        {
            string[] characters = data.array[0].Split(';');
            foreach (string character in characters)
            {
                string[] values = character.Split('|');
                var c = Instantiate<GameObject>(characterPrefab);
                entities.Add(c.GetComponent<Entity>());

                c.name = values[0];
                c.GetComponent<Entity>().speedChange = int.Parse(values[1]);
                c.GetComponent<Entity>().directionChange = int.Parse(values[2]);
                c.transform.position = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
                c.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(values[6]), 0));
                
                c.GetComponent<Entity>().Init();
                Debug.Log("Spawn: " + values[0]);
            }
        }

        string charName = "Player_" + (Random.Range(100000, 999999).ToString());
        Camera.main.name = charName;
        Vector3 pos = GameObject.FindGameObjectWithTag("SpawnPoint").transform.position;
        GetComponent<UConnect>().CallEvent("SpawnCharacter", charName, pos.x.ToString(), pos.y.ToString(), pos.z.ToString());
    }

    public void SpawnCharacter(NetworkData data)
    {
        var character = Instantiate<GameObject>(characterPrefab);
        entities.Add(character.GetComponent<Entity>());
        character.transform.position = new Vector3(float.Parse(data.array[2]), float.Parse(data.array[3]), float.Parse(data.array[4]));
        character.transform.rotation = Quaternion.Euler(new Vector3(0, float.Parse(data.array[5]), 0));
        character.name = data.array[1];

        if (Camera.main.name == character.name)
        {
            Camera.main.transform.SetParent(character.transform);
            Camera.main.transform.localPosition = new Vector3(0, 2, -3);
            Camera.main.transform.localRotation = Quaternion.Euler(new Vector3(20, 0, 0));
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

    public void ApplyDamage()
    {

    }
}
