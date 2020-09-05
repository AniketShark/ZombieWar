using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.AI;

[CustomEditor(typeof(AIWaypointNetwork))]
public class AIWaypointNetworkEditor : Editor
{
	public override void OnInspectorGUI()
	{
		AIWaypointNetwork network = (AIWaypointNetwork)target;

		EditorGUILayout.BeginVertical();
		network.displayMode =(PathDisplayMode)EditorGUILayout.EnumPopup("Display Type",network.displayMode);
		if (network.displayMode == PathDisplayMode.Path)
		{
			network.pathStart = EditorGUILayout.IntSlider("Path Start",network.pathStart, 0, network.Waypoints.Count);
			network.pathEnd   = EditorGUILayout.IntSlider("Path End",network.pathEnd, 0, network.Waypoints.Count);
		}
		EditorGUILayout.EndVertical();
		DrawDefaultInspector();
	}
	void OnSceneGUI()
	{
		AIWaypointNetwork aiWaypointNetwork = (AIWaypointNetwork)target;

		Vector3[] linePoints = new Vector3[aiWaypointNetwork.Waypoints.Count + 1];

		Handles.color = Color.magenta;
		for (int i = 0; i < aiWaypointNetwork.Waypoints.Count; i++)
		{
			if (aiWaypointNetwork.Waypoints[i] != null)
			{
				Handles.Label(aiWaypointNetwork.Waypoints[i].position, ("Waypoint " + i.ToString()));
			}
		}

		if (aiWaypointNetwork.displayMode == PathDisplayMode.Connections)
		{
			for (int i = 0; i < aiWaypointNetwork.Waypoints.Count; i++)
			{
				int index = i != aiWaypointNetwork.Waypoints.Count ? 1 : 0;

				if (aiWaypointNetwork.Waypoints[i] != null)
				{
					linePoints[i] = aiWaypointNetwork.Waypoints[i].position;
				}
				else
				{
					linePoints[i] = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
				}

				Handles.color = Color.cyan;
				Handles.DrawPolyLine(linePoints);
			}
		}
		else if(aiWaypointNetwork.displayMode == PathDisplayMode.Path)
		{
			NavMeshPath path = new NavMeshPath();
			
			Vector3 pathStart = aiWaypointNetwork.Waypoints[aiWaypointNetwork.pathStart].position;
			Vector3 pathEnd = aiWaypointNetwork.Waypoints[aiWaypointNetwork.pathEnd].position;

			if(NavMesh.CalculatePath(pathStart, pathEnd, NavMesh.AllAreas, path))
			{
				Handles.color = Color.yellow;
				Handles.DrawPolyLine(path.corners);
			}
		}
	}
}
