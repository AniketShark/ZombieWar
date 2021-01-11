using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombieState_Pursuit1 : AIZombieState
{
	[SerializeField] [Range(0, 10)] private float _speed = 1.0f;
	[SerializeField] private float _slerpSpeed = 5.0f;
	[SerializeField] private float _maxDuration = 40.0f;
	[SerializeField] private float _repathDistanceMultiplier = 0.035f;
	[SerializeField] private float _repathVisualMinDuration = 0.05f;
	[SerializeField] private float _repathVisualMaxDuration = 5.0f;
	[SerializeField] private float _repathAudioMinDuration = 0.25f;
	[SerializeField] private float _repathAudioMaxDuration = 5.0f;

	private float _timer = 0.0f;
	private float _repathTimer = 0.0f;



	public override AIStateType GetStateType()
	{
		return AIStateType.Pursuit;
	}

	public override void OnEnterState()
	{
		Debug.Log("[AIZombieState_Pursuit1] Entering Pursuit State");
		base.OnEnterState();

		if (_zombieStateMachine == null)
			return;

		_zombieStateMachine.NavAgentControl(true, false);
		_zombieStateMachine.speed = _speed;
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.feeding = false;
		_zombieStateMachine.attackType = 0;

		_timer = 0.0f;
		_repathTimer = 0.0f;

		_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.targetPosition);
		_zombieStateMachine.navAgent.isStopped = false;

	}

	public override AIStateType OnUpdate()
	{
		_timer += Time.deltaTime;
		_repathTimer += Time.deltaTime;
		if (_repathTimer > _maxDuration)
			return AIStateType.Patrol;

		if (_stateMachine.targetType == AITargetType.Visual_Player && _zombieStateMachine.inMeleeRange)
		{
			return AIStateType.Attack;
		}

		if (_zombieStateMachine.isTargetReached)
		{
			switch (_stateMachine.targetType)
			{
				case AITargetType.Audio:
				case AITargetType.Visual_Light:
					{ 
						Debug.LogError("Going Into Alerted State");
						_stateMachine.ClearTarget();
						return AIStateType.Alerted;
					}
				case AITargetType.Visual_Food:
					{ 
						Debug.LogError("Going Into Feeding State");
						return AIStateType.Feeding;
					}
			}
		}

		if (_zombieStateMachine.navAgent.isPathStale ||
			!_zombieStateMachine.navAgent.hasPath ||
			_zombieStateMachine.navAgent.pathStatus != UnityEngine.AI.NavMeshPathStatus.PathComplete)
		{
			return AIStateType.Alerted;
		}

		if (!_zombieStateMachine.useRootRotation &&
			_zombieStateMachine.targetType == AITargetType.Visual_Player &&
			_zombieStateMachine.visualThreat.type == AITargetType.Visual_Player &&
			_zombieStateMachine.isTargetReached)
		{
			Vector3 targetPos = _zombieStateMachine.targetPosition;
			targetPos.y = _zombieStateMachine.transform.position.y;
			Quaternion newRot = Quaternion.LookRotation(targetPos - _zombieStateMachine.transform.position);
			_zombieStateMachine.transform.rotation = newRot;
		}
		else if (!_stateMachine.useRootRotation && !_zombieStateMachine.isTargetReached)
		{
			Quaternion newRotation = Quaternion.LookRotation(_zombieStateMachine.navAgent.desiredVelocity);
			_zombieStateMachine.transform.rotation = Quaternion.Slerp(_zombieStateMachine.transform.rotation, newRotation, Time.deltaTime * _slerpSpeed);
		}
		else if (_zombieStateMachine.isTargetReached)
		{
			return AIStateType.Alerted;
		}

		if (_zombieStateMachine.visualThreat.type == AITargetType.Visual_Player)
		{
			if (_zombieStateMachine.targetPosition != _zombieStateMachine.visualThreat.position)
			{
				if (Mathf.Clamp(_zombieStateMachine.visualThreat.distance * _repathDistanceMultiplier,
					_repathVisualMinDuration,
					_repathVisualMaxDuration) < _repathTimer)
				{
					_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.visualThreat.position);
					_repathTimer = 0.0f;
				}
			}
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Pursuit;
		}
		//If we have a visual threat that is the player's light
		if (_zombieStateMachine.targetType == AITargetType.Visual_Player)
			return AIStateType.Pursuit;

		if (_zombieStateMachine.visualThreat.type == AITargetType.Visual_Light)
		{
			//and we currently have a lower priority target then drop into alerted 
			// mode and try to find source of light
			if (_zombieStateMachine.targetType == AITargetType.Audio || _zombieStateMachine.targetType == AITargetType.Visual_Food)
			{
				_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
				return AIStateType.Alerted;
			}
			else if (_zombieStateMachine.targetType == AITargetType.Visual_Light)
			{
				//get unique id of the collider 
				int currentID = _zombieStateMachine.targetColliderID;

				if (currentID == _zombieStateMachine.visualThreat.targetColliderID)
				{
					if (_zombieStateMachine.targetPosition != _zombieStateMachine.visualThreat.position)
					{
						if (Mathf.Clamp(_zombieStateMachine.visualThreat.distance * _repathDistanceMultiplier, _repathVisualMinDuration, _repathVisualMaxDuration)
							< _repathTimer)
						{
							_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.visualThreat.position);
							_repathTimer = 0.0f;
						}
					}
					_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
					return AIStateType.Pursuit;
				}
				else
				{
					_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
					return AIStateType.Alerted;
				}
			}
			else if (_zombieStateMachine.audioThreat.type == AITargetType.Audio)
			{
				if (_zombieStateMachine.targetType == AITargetType.Visual_Food)
				{
					_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
					return AIStateType.Alerted;
				}
				else if (_zombieStateMachine.targetType == AITargetType.Audio)
				{
					int currentID = _zombieStateMachine.targetColliderID;

					if (currentID == _zombieStateMachine.audioThreat.targetColliderID)
					{
						if (_zombieStateMachine.targetPosition != _zombieStateMachine.audioThreat.position)
						{
							if (Mathf.Clamp(_zombieStateMachine.audioThreat.distance * _repathDistanceMultiplier, _repathAudioMinDuration, _repathAudioMaxDuration)
							< _repathTimer)
							{
								_zombieStateMachine.navAgent.SetDestination(_zombieStateMachine.audioThreat.position);
								_repathTimer = 0.0f;
							}
						}
					}

					_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
					return AIStateType.Pursuit;
				}
				else
				{
					_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
					return AIStateType.Alerted;
				}
			}
		}

		//Default
		return AIStateType.Pursuit;
	}

}
