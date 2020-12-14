using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIZombieState : AIState
{
	protected int _playerLayerMask = -1;
	protected int _bodyPartLayer;
	protected AIZombieStateMachine _zombieStateMachine;
	private void Awake()
	{
		_playerLayerMask = LayerMask.GetMask("Player", "AI Body Part") + 1;
		_bodyPartLayer = LayerMask.NameToLayer("AI Body Part");
	}

	public override void SetStateMachine(AIStateMachine machine)
	{
		if(_stateMachine.GetType() == typeof(AIZombieStateMachine))
		{
			base.SetStateMachine(machine);
			_zombieStateMachine = (AIZombieStateMachine)machine;
		}
	}

	public override void OnTriggerEvent(AITriggerEventType eventType, Collider other)
	{
		base.OnTriggerEvent(eventType, other);

		if (_stateMachine == null)
			return;

		if (eventType != AITriggerEventType.Exit)
		{
			AITargetType curType = _stateMachine.visualThreat.type;

			if (other.CompareTag("Player"))
			{
				float distance = Vector3.Distance(_stateMachine.sensorPostion, other.transform.position);
				if (curType != AITargetType.Visual_Player ||
					(curType == AITargetType.Visual_Player && distance < _stateMachine.visualThreat.distance))
				{
					RaycastHit hitOut;
					if (ColliderIsVisible(other, out hitOut, _playerLayerMask))
					{
						//Yep its close and its in our FOV so store the current most dangerous threat
						_stateMachine.visualThreat.Set(AITargetType.Visual_Player,other,other.transform.position,distance);
					}
				}
			}
		}

	}

	protected virtual bool ColliderIsVisible(Collider other, out RaycastHit hitInfo, int playerLayerMask = -1)
	{
		hitInfo = new RaycastHit();

		if (this._zombieStateMachine == null )
			return false;

		Vector3 head = _zombieStateMachine.sensorPostion;
		Vector3 direction = other.transform.position - head;
		float angle = Vector3.Angle(direction, transform.forward);

		if (angle > _zombieStateMachine.fov * 0.5f)
			return false;

		RaycastHit[] hits = Physics.RaycastAll(head, direction.normalized, _zombieStateMachine.sensorRadius * _zombieStateMachine.sight, playerLayerMask);

		float closestColliderDistance = float.MaxValue;
		Collider closestCollider = null;

		for (int i = 0; i < hits.Length; i++)
		{
			RaycastHit hit = hits[i];
			if (hit.distance < closestColliderDistance)
			{
				if (hit.transform.gameObject.layer == _bodyPartLayer)
				{
					if (_zombieStateMachine != GameSceneManager.instance.GetAIStateMachine(hit.rigidbody.GetInstanceID()))
					{
						closestColliderDistance = hit.distance;
						closestCollider = hit.collider;
						hitInfo = hit;
					}
				}
				else
				{
					closestColliderDistance = hit.distance;
					closestCollider = hit.collider;
					hitInfo = hit;
				}
			}
		}

		if (closestCollider && closestCollider.gameObject == other.gameObject)
			return true;

		return false;
	}
}
