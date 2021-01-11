 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombieState_Feeding1 : AIZombieState
{
	[SerializeField] float _slerpSpeed = 5.0f;
	private int _eatingStateHash = Animator.StringToHash("Feeding State");
	private int _eatingLayerIndex = -1;


	public override AIStateType GetStateType()
	{
		return AIStateType.Feeding;
	}

	public override void OnEnterState()
	{

		base.OnEnterState();

		if(_zombieStateMachine == null) return;

		if(_eatingLayerIndex == -1)
			_eatingLayerIndex = _zombieStateMachine.animator.GetLayerIndex("Cinematic");

		_zombieStateMachine.feeding = true;
		_zombieStateMachine.speed = 0;
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.attackType = 0;

		_zombieStateMachine.NavAgentControl(true,false);
	}

	public override AIStateType OnUpdate()
	{

		if(_zombieStateMachine.satisfaction > 0.9f)
		{
			_zombieStateMachine.GetWaypointPosition(false);
			return AIStateType.Alerted;
		}
		
		if(_zombieStateMachine.visualThreat.type != AITargetType.None && 
			_zombieStateMachine.visualThreat.type != AITargetType.Visual_Food)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Alerted;
		}
		if(_zombieStateMachine.audioThreat.type == AITargetType.Audio)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
			return AIStateType.Alerted;
		}

		if(_zombieStateMachine.animator.GetCurrentAnimatorStateInfo(_eatingLayerIndex).shortNameHash.Equals(_eatingStateHash))
		{
			_zombieStateMachine.satisfaction = Mathf.Min((_zombieStateMachine.satisfaction + (Time.deltaTime * _zombieStateMachine.replenishRate)), 1.0f);
		}

		if (!_zombieStateMachine.useRootRotation)
		{
			Vector3 targetPos = _zombieStateMachine.targetPosition;
			targetPos.y = _zombieStateMachine.transform.position.y;
			Quaternion newRot = Quaternion.LookRotation(targetPos - _zombieStateMachine.transform.position);
			_zombieStateMachine.transform.rotation = Quaternion.Slerp(_zombieStateMachine.transform.rotation,newRot,Time.deltaTime * _slerpSpeed);
		}

		return AIStateType.Feeding;
	}

	public override void OnExitState()
	{
		if(_zombieStateMachine != null)
			_zombieStateMachine.feeding = false;
	}

}
