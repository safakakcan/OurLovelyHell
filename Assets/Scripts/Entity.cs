using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public bool controlled = false;
    public float maxHealth = 100;
    public float health = 100;
    public float armor = 0;
    public float speed = 0;
    
    public float direction = 0;
    public int speedChange = 0;
    public int directionChange = 0;
    public Vector3 fixedPosition = Vector3.zero;
    public bool dead = false;

    public int updateFrequency = 5;
    public Coroutine update = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        GetComponent<Animator>().SetFloat("Velocity", speed);
        GetComponent<Animator>().SetFloat("Direction", direction);
        
        if (Mathf.Abs(directionChange) != 0)
        {
            direction = Mathf.Clamp(direction + (Time.deltaTime * 4 * directionChange), -1, 1);
        }
        else
        {
            if (direction > 0)
            {
                direction = Mathf.Clamp(direction - (Time.deltaTime * 4), 0, 1);
            }
            else
            {
                direction = Mathf.Clamp(direction + (Time.deltaTime * 4), -1, 0);
            }
        }

        if (Mathf.Abs(speedChange) != 0)
        {
            speed = Mathf.Clamp(speed + (Time.deltaTime * 4 * speedChange), -1, 1);
        }
        else
        {
            if (speed > 0)
            {
                speed = Mathf.Clamp(speed - (Time.deltaTime * 2), 0, 1);
            }
            else
            {
                speed = Mathf.Clamp(speed + (Time.deltaTime * 2), -1, 0);
            }
        }

        if (update == null && Camera.main.name == name)
        {
            update = StartCoroutine(UpdateNetwork());
        }
    }

    public virtual void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, fixedPosition) > 0.1f && Camera.main.name != name)
        {
            if (Vector3.Distance(transform.position, fixedPosition) < 4)
            {
                transform.position = Vector3.MoveTowards(transform.position, fixedPosition, Time.deltaTime * 2);
            }
            else
            {
                transform.position = fixedPosition;
            }
        }
    }

    private void OnGUI()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
        if (Camera.main.name == name || Vector3.Distance(Camera.main.transform.position, transform.position) > 10 || !GeometryUtility.TestPlanesAABB(planes, GetComponent<Collider>().bounds))
            return;

        var style = new GUIStyle();
        style.fontSize = 40 - (int)Vector3.Distance(Camera.main.transform.position, transform.position);
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.richText = true;
        
        var pos = (Vector2)Camera.main.WorldToScreenPoint(transform.position + (transform.up * 2));
        pos.y = Screen.height - pos.y;
        GUI.Label(new Rect(pos + new Vector2(-200, -50), new Vector2(400, 50)), "<color=white>" + name + "</color>", style);
    }

    public void Init()
    {
        fixedPosition = transform.position;
    }

    public void ApplyDamage(float damage)
    {
        health -= damage / (armor + 1);

        if (health < 0)
        {
            health = 0;
            Die();
        }
    }

    public void Restore(float value)
    {
        if (health > 0)
        {
            health += value;
            if (health > maxHealth)
                health = maxHealth;
        }
    }

    public void Die()
    {
        dead = true;
    }

    IEnumerator UpdateNetwork()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().CallEvent("Move", name, speedChange.ToString(), directionChange.ToString(),
                transform.position.x.ToString("n2"), transform.position.y.ToString("n2"), transform.position.z.ToString("n2"), transform.rotation.eulerAngles.y.ToString("n2"));
        
        yield return new WaitForSeconds((1 / updateFrequency));
        update = null;
    }
}
