using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIZombieState_Idle1 : AIZombieState
{
	[SerializeField] Vector2 _idleTimeRange = new Vector2(10.0f,60.0f);

	float _idleTime = 0.0f;
	float _timer = 0.0f;

	public override AIStateType GetStateType()
	{
		//Debug.Log("State Type bieng fetched by state machine");
		return AIStateType.Idle;
	}

	public override void OnEnterState()
	{
		Debug.Log("[AIZombieState_Idle1] Entering Idle State");
		base.OnEnterState();

		if(_zombieStateMachine == null)
			return;

		_idleTime = Random.Range(_idleTimeRange.x,_idleTimeRange.y);
		_timer = 0.0f;

		_zombieStateMachine.NavAgentControl(true,false);
		_zombieStateMachine.speed = 0;
		_zombieStateMachine.seeking = 0;
		_zombieStateMachine.feeding = false;
		_zombieStateMachine.ClearTarget();
	}

	public override AIStateType OnUpdate()
	{
		if(_zombieStateMachine == null) return AIStateType.Idle;
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Player)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Pursuit;
		}
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Light)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Alerted;
		}
		if(_zombieStateMachine.audioThreat.type == AITargetType.Audio)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.audioThreat);
			return AIStateType.Alerted;
		}
		if(_zombieStateMachine.visualThreat.type == AITargetType.Visual_Food)
		{
			_zombieStateMachine.SetTarget(_zombieStateMachine.visualThreat);
			return AIStateType.Pursuit;
		}
		_timer += Time.deltaTime;

		if(_timer > _idleTime)
		{
			//Debug.Log("[AIZombieState_Idle1] Going to Patrol");
			return AIStateType.Patrol;
		}
		return AIStateType.Idle;
	}

	public override void OnExitState()
	{
		//Debug.Log("AIZombieState_Idle1] Exit Idle State");
		base.OnExitState();
	}

	
}
