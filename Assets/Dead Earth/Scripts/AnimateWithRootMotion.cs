using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnimateWithRootMotion : MonoBehaviour
{
	public Animator _animator;
	public AIWaypointNetwork network;
	public int currentIndex;
	public bool mixedMode;
	private NavMeshAgent _navmeshAgent;
	private bool hasPath;
	private bool pathPending;
	private bool pathStale;
	private NavMeshPathStatus pathStatus;

	private int _speedHash;
	private int _angleHash;
	private float _smoothAngle;
	void Start()
	{
		_speedHash = Animator.StringToHash("Speed");
		_angleHash = Animator.StringToHash("Angle");

		_navmeshAgent = GetComponent<NavMeshAgent>();
		Debug.Assert(network != null, "No waypoint network provided");
		_navmeshAgent.destination = network.Waypoints[currentIndex].position;
		//_navmeshAgent.updatePosition = false;
		//_navmeshAgent.updateRotation = false;
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

	private void OnAnimatorMove()
	{
		if(mixedMode && !_animator.GetCurrentAnimatorStateInfo(0).IsName("BaseLayer.Locomotion"))
			transform.rotation = _animator.rootRotation;
		_navmeshAgent.velocity = _animator.deltaPosition / Time.deltaTime;
	}

	public void Update()
	{
		pathPending = _navmeshAgent.pathPending;
		pathStale = _navmeshAgent.isPathStale;
		pathStatus = _navmeshAgent.pathStatus;

		Vector3 localDesiredVelocity = transform.InverseTransformVector(_navmeshAgent.desiredVelocity);
		float speed = localDesiredVelocity.magnitude;

		float angle = Mathf.Atan2(localDesiredVelocity.x, localDesiredVelocity.z) * Mathf.Rad2Deg;
		_smoothAngle = Mathf.MoveTowardsAngle(_smoothAngle, angle, 80f * Time.deltaTime);

		_animator.SetFloat(_speedHash, speed, 0.1f, Time.deltaTime);
		_animator.SetFloat(_angleHash, _smoothAngle, 1.0f, Time.deltaTime);

		if (_navmeshAgent.desiredVelocity.sqrMagnitude > Mathf.Epsilon)
		{
			if (!mixedMode || (mixedMode && Mathf.Abs(angle) < 80f && _animator.GetCurrentAnimatorStateInfo(0).IsName("BaseLayer.Locomotion")))
			{
				Quaternion lookRotation = Quaternion.LookRotation(_navmeshAgent.desiredVelocity, Vector3.up);
				transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 5.0f * Time.deltaTime);
			}
		}

		if ((_navmeshAgent.remainingDistance < _navmeshAgent.stoppingDistance && !pathPending) || (pathStatus == NavMeshPathStatus.PathInvalid))
		{
			SetDestination(true);
		}
		else if (pathStale)
			SetDestination(false);
	}
}
