using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MP_Bot : Entity
{
    public bool turn = false;
    Coroutine coroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<NetworkController>().entities.Add(this);
        fixedPosition = transform.position;
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        if (coroutine == null)
        {
            coroutine = StartCoroutine(Turn());
        }
    }

    IEnumerator Turn()
    {
        yield return new WaitForSeconds(2);

        turn = !turn;
        speedChange = turn ? 1 : -1;
        coroutine = null;
    }

    void Step(AnimationEvent e)
    {
        //
    }
}
