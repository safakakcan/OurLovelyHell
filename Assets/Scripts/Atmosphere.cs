using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    float frequency = 500;
    Coroutine change = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<AudioLowPassFilter>().cutoffFrequency = Mathf.Lerp(GetComponent<AudioLowPassFilter>().cutoffFrequency, frequency, Time.deltaTime);

        if (change == null)
            change = StartCoroutine(Change());
    }

    IEnumerator Change()
    {
        yield return new WaitForSeconds(Random.Range(2, 4));
        frequency = Random.Range(250, 900);
        change = null;
    }
}
