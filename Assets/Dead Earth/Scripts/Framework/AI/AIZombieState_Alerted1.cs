using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombieState_Alerted1 : AIZombieState
{
	[SerializeField][Range(1.0f,60.0f)] private float _maxDuration = 10.0f;
	[SerializeField] private float _waypointAngleThreshold = 90.0f;
	[SerializeField] private float _threatAngleThreshold;
	[SerializeField] private float _directionChangeTime = 0;

	private float _timer = 0.0f;
	private float _directionChangedTimer = 0;
	public override AIStateType GetStateType()
	{
		return AIStateType.Alerted;
	}

	public override void OnEnterState()
	{

		Debug.Log("[AIZombieState_Alerted1] Entering Idle State");
		base.OnEnterState();

		if(_zombieStateMachine == null)
			return;
		
		_zombieStateMachine.NavAgentControl(true,false);
		_zombieStateMachine.speed = 0;
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.feeding = false;
		_zombieStateMachine.attackType = 0;

		_timer = _maxDuration;
	}

	public override AIStateType OnUpdate()
	{
		_timer -= Time.deltaTime;
		_directionChangedTimer += Time.deltaTime;
		if(_timer < 0.0f)
		{
			//Debug.LogError("[AIZombieState_Alerted1] Leaving Alerted State");
			_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.GetWaypointPosition(false));
			_zombieStateMachine.navAgent.isStopped = false;
			_timer = _maxDuration;
		}

		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Player)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			Debug.LogFormat("[AIZombieState_Alerted1] {0}",AIStateType.Pursuit);
			return AIStateType.Pursuit;
		}
		
		if(_zombieStateMachine.audioThreat.type == AITargetType.Audio)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
			_timer = _maxDuration;
		}
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Light)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			_timer = _maxDuration;
		}

		if (_zombieStateMachine.audioThreat.type == AITargetType.None &&
			_zombieStateMachine.visualThreat.type == AITargetType.Visual_Food)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Pursuit;
		}

		float angle = 0.0f;

		if((_zombieStateMachine.targetType == AITargetType.Audio || 
			_zombieStateMachine.targetType == AITargetType.Visual_Light) && 
			!_zombieStateMachine.isTargetReached)
		{
			angle = AIState.FindSignedAngle(_zombieStateMachine.transform.forward,
				(_zombieStateMachine.targetPosition - _zombieStateMachine.transform.position));

			if(_zombieStateMachine.targetType == AITargetType.Audio && Mathf.Abs(angle) < _threatAngleThreshold)
			{
				return AIStateType.Pursuit;
			}

			if(_directionChangedTimer > _directionChangeTime)
			{ 
				if(Random.value < _zombieStateMachine.intelligence)
				{
					_zombieStateMachine.seeking = (int)Mathf.Sign(angle);
				}
				else
				{
					_zombieStateMachine.seeking = (int)Mathf.Sign(Random.Range(-1.0f,1.0f));
				}
				_directionChangedTimer = 0.0f;
			}
		}
		else if(_zombieStateMachine.targetType == AITargetType.Waypoint && !_zombieStateMachine.navAgent.pathPending)
		{
			angle = AIState.FindSignedAngle(_zombieStateMachine.transform.forward,
				(_zombieStateMachine.navAgent.steeringTarget - _zombieStateMachine.transform.position));

			if(Mathf.Abs(angle) < _waypointAngleThreshold) return AIStateType.Patrol;
			_zombieStateMachine.seeking = (int)Mathf.Sign(angle);
		}

		return AIStateType.Alerted;
	}
}
