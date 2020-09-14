using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimateWithoutRootMotion : MonoBehaviour
{

	public Animator _animator;
	public AIWaypointNetwork network;
	public int currentIndex;
	private NavMeshAgent _navmeshAgent;
	private bool hasPath;
	private bool pathPending;
	private bool pathStale;
	private NavMeshPathStatus pathStatus;

	private int _horizontalHash;
	private int _verticalHash;
	private int _attackHash;
	private float _originalSpeed;
	private int turnOnSpot;
	void Start()
	{
		_horizontalHash = Animator.StringToHash("Horizontal");
		_verticalHash = Animator.StringToHash("Vertical");
		_attackHash = Animator.StringToHash("Attack");

		_navmeshAgent = GetComponent<NavMeshAgent>();
		Debug.Assert(network != null, "No waypoint network provided");
		_navmeshAgent.destination = network.Waypoints[currentIndex].position;
		_originalSpeed = _navmeshAgent.speed;
	}
	
	public void SetDestination(bool increament)
	{
		Debug.Assert(network != null, "No waypoint network provided");

		if (increament)
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

		Vector3 cross = Vector3.Cross(transform.forward, _navmeshAgent.desiredVelocity.normalized);
		float horizontal = cross.y < 0 ? -cross.magnitude : cross.magnitude;
		float vertical = Mathf.Clamp(_navmeshAgent.desiredVelocity.magnitude, -5.6f, 5.6f);
		horizontal = Mathf.Clamp(horizontal, -2.32f, 2.32f);

		_animator.SetFloat(_horizontalHash, horizontal, 1.0f, Time.deltaTime);
		_animator.SetFloat(_verticalHash, vertical, 0.1f, Time.deltaTime);

		if (_navmeshAgent.desiredVelocity.magnitude < 1.0f && Vector3.Angle(transform.forward, _navmeshAgent.desiredVelocity) > 10)
		{
			_navmeshAgent.speed = 0.1f;
			turnOnSpot = (int)Mathf.Sign(horizontal);
		}
		else
		{
			_navmeshAgent.speed = _originalSpeed;
			turnOnSpot = 0;
		}


		if ((_navmeshAgent.remainingDistance < _navmeshAgent.stoppingDistance && !pathPending) || (pathStatus == NavMeshPathStatus.PathInvalid))
		{
			SetDestination(true);
		}
		else if (pathStale)
			SetDestination(false);
	}
}
