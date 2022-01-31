using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Route : MonoBehaviour
{
    public List<Route> routes = new List<Route>();
    public List<string> tags = new List<string>();

    void LateUpdate()
    {
        foreach (var route in routes)
        {
            if (route != null)
            {
                Debug.DrawLine(transform.position, route.transform.position, Color.blue);
                Debug.DrawRay(transform.position, (route.transform.position - transform.position).normalized + (Vector3.up * 0.1f), Color.cyan);
            }
        }
    }
}
