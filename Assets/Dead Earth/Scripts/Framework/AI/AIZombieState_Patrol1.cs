using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombieState_Patrol1 : AIZombieState
{
	// Inspector Assigned
	//[SerializeField] private AIWaypointNetwork _waypointNetwork = null;
	//[SerializeField] private bool _randomPatrol = false;
	//[SerializeField] private int _currentWaypoint = 0;
	[SerializeField]private float _turnOnSpotThreshold;
	[SerializeField]private float _slerpSpeed;
	[SerializeField][Range(0.0f,3.0f)] private float _speed = 1.0f;
	[SerializeField][Range(0.0f,1.0f)] private float _ikLookAtWeight = 1.0f;

	public override AIStateType GetStateType()
	{
		return AIStateType.Patrol;
	}

	public override void OnEnterState()
	{
		//Debug.Log("[AIZombieState_Patrol1] Entering Partol State");
		base.OnEnterState();

		if(_zombieStateMachine == null)
			return;

		_zombieStateMachine.NavAgentControl(true,false);
		_zombieStateMachine.speed = _speed;
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.feeding = false;
		_zombieStateMachine.attackType = 0;


		_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.GetWaypointPosition(false));
		_zombieStateMachine.navAgent.isStopped = false;

		//if(_zombieStateMachine.targetType != AITargetType.Waypoint)
		//{
		//	_zombieStateMachine.ClearTarget();

		//	if(_waypointNetwork != null && _waypointNetwork.Waypoints.Count > 0)
		//	{
		//		if(_randomPatrol)
		//			_currentWaypoint = Random.Range(0,_waypointNetwork.Waypoints.Count);

		//		Transform waypoint = _waypointNetwork.Waypoints[_currentWaypoint];

		//		if(waypoint != null)
		//		{
		//			_zombieStateMachine.SetTarget(AITargetType.Waypoint,
		//				null,
		//				waypoint.position,
		//				Vector3.Distance(_zombieStateMachine.transform.position,waypoint.position));
					
		//			_zombieStateMachine.navAgent.SetDestination(waypoint.position);
		//			_zombieStateMachine.navAgent.isStopped = false;
		//		}
		//	}
		//}

	}

	public override AIStateType OnUpdate()
	{

		if(_zombieStateMachine == null) 
			return AIStateType.Idle;

		//Debug.Log("[AIZombieState_Patrol1] OnUpdate");
		
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Player)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			Debug.LogFormat("[AIZombieState_Patrol1] {0}",AIStateType.Pursuit);
			return AIStateType.Pursuit;
		}
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Light)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			Debug.LogFormat("[AIZombieState_Patrol1] {0}",AIStateType.Alerted);
			return AIStateType.Alerted;
		}
		if(_zombieStateMachine.audioThreat.type == AITargetType.Audio)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
			Debug.LogFormat("[AIZombieState_Patrol1] {0}",AIStateType.Alerted);
			return AIStateType.Alerted;
		}
	    if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Food)
		{
			if((1.0f - _zombieStateMachine.satisfaction) > (_zombieStateMachine.visualThreat.distance/_zombieStateMachine.sensorRadius))
			{ 
				Debug.LogFormat("[AIZombieState_Patrol1] {0}",AIStateType.Pursuit);
				_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
				return AIStateType.Pursuit;
			}
		}

		float angle = Vector3.Angle(_zombieStateMachine.transform.forward,
			(_zombieStateMachine.navAgent.steeringTarget - _zombieStateMachine.transform.position));

		if(Mathf.Abs(angle) > _turnOnSpotThreshold)
		{
			//Debug.LogFormat("[AIZombieState_Patrol1] Mathf.Abs(angle) > _turnOnSpotThreshold  Angle : {0}",angle);
			return AIStateType.Alerted;
		}

		if(!_zombieStateMachine.useRootRotation)
		{
			//Debug.LogFormat("[AIZombieState_Patrol1] useRootRotation");
			Quaternion targetRotation = Quaternion.LookRotation(_zombieStateMachine.navAgent.desiredVelocity);
			_zombieStateMachine.transform.rotation =  Quaternion.Slerp(_zombieStateMachine.transform.rotation,targetRotation,Time.deltaTime * _slerpSpeed);
		}

		//Debug.LogFormat("{0}  {1}  {2}",_zombieStateMachine.navAgent.isPathStale, 
		//	(!_zombieStateMachine.navAgent.hasPath),
		//	(_zombieStateMachine.navAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete));

		if(_zombieStateMachine.navAgent.isPathStale || 
			!_zombieStateMachine.navAgent.hasPath ||
			_zombieStateMachine.navAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
		{
			Debug.LogFormat("[AIZombieState_Patrol1] Calling NextWayPoint");
			_zombieStateMachine.GetWaypointPosition(true);
			//NextWayPoint();
		}


		return AIStateType.Patrol;
	}

	

	public override void OnDestinationReached(bool isReached)
	{
	   // Debug.LogFormat("[AIZombieState_Patrol1] OnDestinationReached {0}",isReached);
		base.OnDestinationReached(isReached);
		if(_zombieStateMachine == null || !isReached)
			return;

		if(_zombieStateMachine.targetType == AITargetType.Waypoint)
		{
			_zombieStateMachine.GetWaypointPosition(true);
			//NextWayPoint();
		}
	}

	/// <summary>
	/// Override IK Goals
	/// </summary>
	//public override void OnAnimatorIKUpdated()
	//{
	//	if(_zombieStateMachine == null)
	//		return;

	//	base.OnAnimatorIKUpdated();
	//	_zombieStateMachine.animator.SetLookAtPosition(_zombieStateMachine.navAgent.velo);
	//	_zombieStateMachine.animator.SetLookAtWeight(_ikLookAtWeight);
	//}
}
