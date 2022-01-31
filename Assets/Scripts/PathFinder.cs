using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[ExecuteAlways]
public class PathFinder : MonoBehaviour
{
	public List<Route> routes;

	Coroutine pathFinding = null;
	public LineRenderer line = null;
	public Route targetRoute = null;
	public List<Route> path = new List<Route>();

    private void Update()
    {
		if (pathFinding == null && targetRoute != null && Camera.main.GetComponent<PlayerController>().character != null)
		{
			pathFinding = StartCoroutine(PathFinding());
		}
	}

	IEnumerator PathFinding()
	{
		path = GetShortestPath(FindNearestRoute(Camera.main.GetComponent<PlayerController>().character.transform.position), targetRoute);

		yield return new WaitForSeconds(2);
		pathFinding = null;
	}

	public Route FindNearestRoute(Vector3 position)
    {
		Route route = null;
		float distance = float.MaxValue;

		foreach (var r in routes)
        {
			float dist = Vector3.Distance(position, r.transform.position);
			if (dist < distance && !Physics.Linecast(position, r.transform.position))
            {
				route = r;
				distance = dist;
            }
        }

		return route;
    }

	public Route FindRouteByTag(string tag)
    {
		foreach (var r in routes)
        {
			if (r.tags.Contains(tag))
            {
				return r;
            }
        }

		return null;
	}

	public List<Route> GetShortestPath(Route start, Route end)
	{
		if (start == null || end == null)
		{
			throw new ArgumentNullException();
		}
		
		List<Route> path = new List<Route>();

		if (start == end)
		{
			path.Add(start);
			return path;
		}
		// The list of unvisited routes
		List<Route> unvisited = new List<Route>();

		// Previous routes in optimal path from source
		Dictionary<Route, Route> previous = new Dictionary<Route, Route>();

		// The calculated distances, set all to Infinity at start, except the start Route
		Dictionary<Route, float> distances = new Dictionary<Route, float>();

		for (int i = 0; i < routes.Count; i++)
		{
			Route route = routes[i];
			unvisited.Add(route);

			// Setting the route distance to Infinity
			distances.Add(route, float.MaxValue);
		}

		// Set the starting Route distance to zero
		distances[start] = 0f;
		while (unvisited.Count != 0)
		{

			// Ordering the unvisited list by distance, smallest distance at start and largest at end
			unvisited = unvisited.OrderBy(route => distances[route]).ToList();

			// Getting the Route with smallest distance
			Route current = unvisited[0];

			// Remove the current route from unvisisted list
			unvisited.Remove(current);

			// When the current route is equal to the end route, then we can break and return the path
			if (current == end)
			{

				// Construct the shortest path
				while (previous.ContainsKey(current))
				{

					// Insert the route onto the final result
					path.Insert(0, current);

					// Traverse from start to end
					current = previous[current];
				}

				// Insert the source onto the final result
				path.Insert(0, current);
				break;
			}

			// Looping through the Route connections (neighbors) and where the connection (neighbor) is available at unvisited list
			for (int i = 0; i < current.routes.Count; i++)
			{
				Route neighbor = current.routes[i];

				// Getting the distance between the current route and the connection (neighbor)
				float length = Vector3.Distance(current.transform.position, neighbor.transform.position);

				// The distance from start route to this connection (neighbor) of current route
				float alt = distances[current] + length;

				// A shorter path to the connection (neighbor) has been found
				if (alt < distances[neighbor])
				{
					distances[neighbor] = alt;
					previous[neighbor] = current;
				}
			}
		}
		
		return path;
	}

	[UnityEditor.MenuItem("Routes/Refresh")]
	public static void RefreshRoutes()
	{
		Route[] routeArray = GameObject.FindObjectsOfType<Route>();
		GameObject.FindObjectOfType<PathFinder>().routes = routeArray.ToList();

		foreach (Route route in routeArray)
		{
			List<Route> newRoutes = new List<Route>();
			route.routes = new List<Route>();

			foreach (Route r in routeArray)
			{
				if (route != r && Vector3.Distance(route.transform.position, r.transform.position) < 20)
				{
					if (!Physics.Linecast(route.transform.position, r.transform.position))
						newRoutes.Add(r);
				}
			}

			route.routes = newRoutes;
		}
	}
}
