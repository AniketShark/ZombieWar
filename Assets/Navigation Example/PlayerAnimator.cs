using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimator : MonoBehaviour
{

	public Animator _animator;
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

	private int _horizontalHash;
	private int _verticalHash;
	private int _attackHash;
	private float _originalSpeed;
	private int turnOnSpot;
	void Start()
	{
		_horizontalHash = Animator.StringToHash("horizontal");
		_verticalHash = Animator.StringToHash("vertical");
		_attackHash = Animator.StringToHash("attack");

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
		//NoRootMotion();
	}

	public void WithRootMotion()
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

		//if (_navmeshAgent.isOnOffMeshLink)
		//{
		//	StartCoroutine(Jump(1.0f));
		//	return;
		//}

		if ((_navmeshAgent.remainingDistance.Equals(_navmeshAgent.stoppingDistance) && !pathPending) ||
		(pathStatus == NavMeshPathStatus.PathInvalid))
		{
			SetDestination(true);
		}
		else if (pathStale)
			SetDestination(false);
	}

	public void NoRootMotion()
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

		//if (_navmeshAgent.isOnOffMeshLink)
		//{
		//	StartCoroutine(Jump(1.0f));
		//	return;
		//}

		if ((_navmeshAgent.remainingDistance.Equals(_navmeshAgent.stoppingDistance) && !pathPending) ||
		(pathStatus == NavMeshPathStatus.PathInvalid))
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
		while (Vector3.Distance(endPosition, _navmeshAgent.transform.position) > 0.1f)
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
