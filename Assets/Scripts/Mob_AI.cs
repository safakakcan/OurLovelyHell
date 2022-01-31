using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Entity))]
public class Mob_AI : MonoBehaviour
{
    public float behaviourDelayMin = 2;
    public float behaviourDelayMax = 4;
    public float aggroDistance = 10;
    public float attackDistance = 5;
    public float habitatRadius = 20;
    public float authorityDistance = 25;
    public float authorityCheckDelay = 5;

    Vector3 spawnPoint;
    Vector3 destination;
    Coroutine behaviour = null;
    Coroutine authorityCheck = null;

    // Start is called before the first frame update
    void Start()
    {
        spawnPoint = transform.position;
        destination = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Entity>().authority && !GetComponent<Entity>().dead)
        {
            if (behaviour == null)
            {
                behaviour = StartCoroutine(Behaviour());
            }

            //var t = GetComponent<UnityEngine.AI.NavMeshAgent>().steeringTarget;
            var lookPos = destination - transform.position;
            lookPos.y = 0;

            if (lookPos != Vector3.zero)
            {
                var rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 4);
            }
            
            if (Vector3.Distance(transform.position, destination) < 1)
            {
                destination = transform.forward;
                GetComponent<Entity>().speedChange = 0;
            }
        }

        if (authorityCheck == null)
        {
            authorityCheck = StartCoroutine(AuthorityCheck());
        }
    }

    IEnumerator AuthorityCheck()
    {
        yield return new WaitForSeconds(authorityCheckDelay);

        Character[] characters = GameObject.FindObjectsOfType<Character>();
        Character nearest = null;
        float distance = float.MaxValue;

        foreach (var ch in characters)
        {
            var dist = Vector3.Distance(transform.position, ch.transform.position);
            if (dist < distance)
            {
                nearest = ch;
                distance = dist;
            }
        }

        if (nearest != null && distance <= authorityDistance && nearest.name == Camera.main.name)
        {
            if (!GetComponent<Entity>().authority)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().Send(string.Format("{0}\n{1}\n{2}", "authority", name, nearest.name));
        }
        else
        {
            if (GetComponent<Entity>().authority)
                GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().Send(string.Format("{0}\n{1}\n{2}", "authority", name, ""));
        }

        authorityCheck = null;
    }

    IEnumerator Behaviour()
    {
        yield return new WaitForSeconds(Random.Range(behaviourDelayMin, behaviourDelayMax));

        Character[] allCharacters = GameObject.FindObjectsOfType<Character>();
        Character target = (from c in allCharacters where Vector3.Distance(transform.position, c.transform.position) < aggroDistance && !c.dead select c).FirstOrDefault();
        
        if (target == null)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                destination = spawnPoint + new Vector3(Random.Range(-habitatRadius, habitatRadius), Random.Range(-habitatRadius, habitatRadius), Random.Range(-habitatRadius, habitatRadius));
                //GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(destination);
                GetComponent<Entity>().speedChange = 1;
            }
            else
            {
                destination = transform.forward;
                GetComponent<Entity>().speedChange = 0;
            }
        }
        else
        {
            destination = target.transform.position;
            //GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(destination);

            if (Vector3.Distance(transform.position, destination) < attackDistance)
            {
                GetComponent<Entity>().speedChange = 0;
                GameObject.FindGameObjectWithTag("GameController").GetComponent<UConnect>().Send(string.Format("{0}\n{1}\n{2}", "skill", "Attack", name));
            }
            else
            {
                GetComponent<Entity>().speedChange = 1;
            }
        }

        behaviour = null;
    }

    void Hit(AnimationEvent e)
    {
        if (GetComponent<Entity>().authority)
        {
            var colliders = Physics.OverlapSphere(GetComponent<Entity>().bodyRenderer.bounds.center + transform.TransformDirection(Vector3.forward * (attackDistance / 2)), attackDistance / 2);
            Entity target = null;

            foreach (var collider in colliders)
            {
                if (collider.name == name)
                    continue;

                Character entity;
                if (collider.gameObject.TryGetComponent<Character>(out entity))
                {
                    if (!entity.dead)
                    {
                        int damage = (int)(e.floatParameter * (GetComponent<Entity>().TotalStats().attack / entity.TotalStats().defence) * Random.Range(0.9f, 1.1f) * (1 + (GetComponent<Entity>().stats.level * 0.1f)));
                        FindObjectOfType<UConnect>().Send(string.Format("{0}\n{1}\n{2}\n{3}", "damage", name, entity.name, damage.ToString()));

                        if (target == null && name == Camera.main.name)
                        {
                            target = entity;
                            Camera.main.GetComponent<PlayerController>().ShowEntityInfo(target);
                            Camera.main.GetComponent<Animator>().SetTrigger("Shake");
                        }
                    }
                }
            }
        }
    }

    void AttackFX(AnimationEvent e)
    {
        Instantiate<GameObject>((GameObject)e.objectReferenceParameter, transform);
    }
}
