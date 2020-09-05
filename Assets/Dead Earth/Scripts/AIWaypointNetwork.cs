using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum PathDisplayMode
{
	None,
	Connections,
	Path
}
public class AIWaypointNetwork : MonoBehaviour 
{
	[HideInInspector]
	public PathDisplayMode displayMode = PathDisplayMode.Connections;
	[HideInInspector]
	public int pathStart;
	[HideInInspector]
	public int pathEnd;

	public List<Transform> Waypoints = new List<Transform>();

}
