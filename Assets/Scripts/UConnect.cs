using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using UnityEngine;
using System.Text;

public class UConnect : MonoBehaviour
{
    public bool Connected { get { return client != null; } }

    [Header("Host Address")]
    [SerializeField]
    private string host = "127.0.0.1";
    [SerializeField]
    private int port = 8090;
    [SerializeField]
    private bool autoConnect = true;
    [SerializeField]
    private bool runInterScenes = false;
    [SerializeField]
    private bool debug = false;

    UdpClient client;
    IPEndPoint endpoint;
    //NetworkStream stream;
    //StreamReader reader;
    //StreamWriter writer;
    System.Threading.Thread thread;
    List<string> buffer = new List<string>();

    [Header("Network Event Filters")]
    public List<EventFilter> eventFilters = new List<EventFilter>();

    // Start is called before the first frame update
    void Start()
    {
        if (runInterScenes)
            DontDestroyOnLoad(this.gameObject);

        if (autoConnect)
            Connect();
    }

    // Update is called once per frame
    void Update()
    {
        while (true)
        {
            if (buffer.Count > 0)
            {
                try
                {
                    string data = buffer[0];
                    
                    foreach (EventFilter filter in eventFilters)
                    {
                        if (filter.Compare(data))
                        {
                            if (debug)
                                Debug.Log("INVOKED: " + filter.onMatched.GetPersistentMethodName(0));
                            filter.onMatched.Invoke(data.Split('\n'));
                            break;
                        }
                    }
                }
                catch { }

                buffer.RemoveAt(0);
            }
            else
            {
                break;
            }
        }
        
    }

    public void Connect()
    {
        client = new UdpClient();
        endpoint = new IPEndPoint(IPAddress.Parse(host), port);
        client.Connect(endpoint);

        thread = new System.Threading.Thread(() => Listen());
        thread.Start();
    }

    public void Disconnect()
    {
        client.Close();
        thread.Interrupt();
    }

    void Listen()
    {
        while (Connected)
        {
            var receivedData = client.Receive(ref endpoint);
            System.Threading.Thread th = new System.Threading.Thread(() => AddToBuffer(receivedData));
            th.Start();
        }
    }

    void AddToBuffer(byte[] receivedData)
    {
        string data = Encoding.UTF8.GetString(receivedData);
        buffer.Add(data);

        if (debug)
            Debug.Log("RESPONSE: " + data);
    }

    public void Send(string data)
    {
        if (Connected)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            client.Send(bytes, bytes.Length);

            if (debug)
                Debug.Log("REQUEST: " + data);
        }
    }

    private void OnDestroy()
    {
        GetComponent<UConnect>().Send("quit\n" + Camera.main.name);

        if (Connected)
            Disconnect();
    }
}

[System.Serializable]
public class EventFilter
{
    public string schema;
    public UnityEngine.Events.UnityEvent<string[]> onMatched;

    public EventFilter(string Schema, UnityEngine.Events.UnityEvent<string[]> OnMatchedEvent = null)
    {
        schema = Schema;
        onMatched = OnMatchedEvent;
    }

    public bool Compare(string input)
    {
        try
        {
            string[] filters = schema.Split('*');

            for (int i = 0; i < filters.Length; i++)
            {
                if (i == filters.Length - 1)
                {
                    if (filters[i] == "")
                    {
                        continue;
                    }
                    else
                    {
                        if (filters[i] == input)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else if (i == 0)
                {
                    if (filters[i] == "")
                    {
                        int start = input.IndexOf(filters[i + 1]);
                        if (start != -1)
                        {
                            input = input.Substring(start);
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        int start = input.IndexOf(filters[i + 1]);
                        if (start != -1 && filters[i] == input.Substring(0, filters[i].Length))
                        {
                            input = input.Substring(start);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    int start = input.IndexOf(filters[i + 1]);
                    if (start != -1 && filters[i] == input.Substring(0, filters[i].Length))
                    {
                        input = input.Substring(start);
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}

[System.Serializable]
public class NetworkData
{
    public string cmd;
    public string[] array;

    public NetworkData(params string[] _array)
    {
        array = _array;
    }
}

[System.Serializable]
public class DataCell
{
    public string id = "";
    public string value = "";
    public bool file = false;
    public Permission permission = new Permission();
    public List<DataCell> children = new List<DataCell>();
}

[System.Serializable]
public class Permission
{
    public Access read = Access.Private;
    public Access write = Access.Private;
}

[System.Serializable]
public enum Access
{
    Public,
    Private,
    Authority
}

[System.Serializable]
public class Request
{
    public string[] cmd;
    public bool await;
}