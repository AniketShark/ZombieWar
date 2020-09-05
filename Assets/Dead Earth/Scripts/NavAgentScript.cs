using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentScript : MonoBehaviour
{
	public AIWaypointNetwork network;
	public int currentIndex;
	private NavMeshAgent _navmeshAgent;
	private bool hasPath;
	private bool pathPending;
	private bool pathStale;
	
	private NavMeshPathStatus pathStatus;
	[SerializeField]
	private AnimationCurve animationCurve;
	[SerializeField]
	private float jumpHeight;

	private void Start()
	{
		_navmeshAgent = GetComponent<NavMeshAgent>();
		Debug.Assert(network != null, "No waypoint network provided");
		_navmeshAgent.destination = network.Waypoints[currentIndex].position;
	}

	public void SetDestination(bool increament)
	{
		Debug.Assert(network != null, "No waypoint network provided");
	
		if(increament)
		{
			currentIndex = ++currentIndex % network.Waypoints.Count;
			_navmeshAgent.destination = network.Waypoints[currentIndex].position;
		}
		else
		{
			_navmeshAgent.destination = network.Waypoints[currentIndex].position;
		}
	}

	public void Update()
	{
		pathPending = _navmeshAgent.pathPending;
		pathStale = _navmeshAgent.isPathStale;
		pathStatus = _navmeshAgent.pathStatus;

		if (_navmeshAgent.isOnOffMeshLink)
		{
			StartCoroutine(Jump(1.0f));
			return;
		}

		if ((_navmeshAgent.remainingDistance.Equals(_navmeshAgent.stoppingDistance) && !pathPending) || (pathStatus == NavMeshPathStatus.PathInvalid))// || pathStatus == NavMeshPathStatus.PathPartial))
		{
			SetDestination(true);
		}
		else if (pathStale)
			SetDestination(false);

		

	}

	IEnumerator Jump(float duration)
	{
		OffMeshLinkData linkData = _navmeshAgent.currentOffMeshLinkData;
		Vector3 startPosition = _navmeshAgent.transform.position;
		Vector3 endPosition = linkData.endPos + (_navmeshAgent.baseOffset * Vector3.up);
		float time = 0.0f;
		while(Vector3.Distance(endPosition, _navmeshAgent.transform.position) > 0.1f)
		{
			float t = time / duration;
			float height = animationCurve.Evaluate(t);
			_navmeshAgent.transform.position = Vector3.Lerp(startPosition, endPosition, t) + (jumpHeight * height * Vector3.up);
			time += Time.deltaTime;
			yield return null;
		}
		_navmeshAgent.CompleteOffMeshLink();
	}

}
