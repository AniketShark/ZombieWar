using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIZombieState : AIState
{
	protected int _playerLayerMask = -1;
	protected int _bodyPartLayer;
	protected int _visualRaycastMask;
	protected AIZombieStateMachine _zombieStateMachine;
	private void Awake()
	{
		_playerLayerMask = LayerMask.GetMask("Player", "AI Body Part") + 1;
		_visualRaycastMask = LayerMask.GetMask("Player", "AI Body Part", "Visual Aggravator") + 1;
		_bodyPartLayer = LayerMask.NameToLayer("AI Body Part");
	}

	public override void SetStateMachine(AIStateMachine machine)
	{
		if(machine.GetType() == typeof(AIZombieStateMachine))
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
			else if(other.CompareTag("Flash Light") && curType != AITargetType.Visual_Player)
			{
				BoxCollider flashLightTrigger = (BoxCollider)other;
				float distanceToThreat = Vector3.Distance(_zombieStateMachine.sensorPostion,flashLightTrigger.transform.position);
				float zSize = flashLightTrigger.size.z * flashLightTrigger.transform.lossyScale.z;
				float aggrFactor = distanceToThreat / zSize;
				if(aggrFactor < _zombieStateMachine.sight && aggrFactor <= _zombieStateMachine.intelligence)
				{
					_zombieStateMachine.visualThreat.Set(AITargetType.Visual_Light,other,other.transform.position,distanceToThreat);
				}
			}
			else if(other.CompareTag("AI Sound Emitter"))
			{
				SphereCollider soundTrigger = (SphereCollider)other;

				if(soundTrigger == null)
					return;

				Vector3 agentSensorPosition = _zombieStateMachine.sensorPostion;
				Vector3 soundPos;
				float soundRadius;

				AIState.ConverSphereColliderToWorldSpace(soundTrigger,out soundPos,out soundRadius);
				float distanceToThreat = (soundPos - agentSensorPosition).magnitude;
				float distanceFactor = (distanceToThreat / soundRadius);

				distanceFactor += distanceFactor * (1.0f - _zombieStateMachine.hearing);

				//Debug.LogErrorFormat("[AIZombieStateMachine] DistanceFactor {0} DistanceToThreat {1}",distanceFactor,distanceToThreat);
				if(distanceFactor > 1.0f)
				{
					return;
				}

				//Debug.LogErrorFormat("[AIZombieStateMachine] distanceToThreat < _zombieStateMachine.audioThreat.distance {0} ",(distanceToThreat < _zombieStateMachine.audioThreat.distance));
				if(distanceToThreat < _zombieStateMachine.audioThreat.distance)
				{ 
					//Debug.LogErrorFormat("[AIZombieStateMachine] Set Audio Distance {0} ",distanceFactor);
					_zombieStateMachine.audioThreat.Set(AITargetType.Audio,other,soundPos,distanceToThreat);
				}
			}
			else if(other.CompareTag("AI Food") &&
				curType != AITargetType.Visual_Light && 
				curType != AITargetType.Visual_Player && 
				_zombieStateMachine.audioThreat.type == AITargetType.None && 
				_zombieStateMachine.satisfaction <= 0.9f)
			{
				float distanceToThreat = Vector3.Distance(other.transform.position,_zombieStateMachine.sensorPostion);

				if(distanceToThreat < _zombieStateMachine.visualThreat.distance)
				{
					RaycastHit hitInfo;
					if(ColliderIsVisible(other,out hitInfo,_visualRaycastMask))
					{
						_zombieStateMachine.visualThreat.Set(AITargetType.Visual_Food,other,other.transform.position,distanceToThreat);
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
					if (_stateMachine  != GameSceneManager.instance.GetAIStateMachine(hit.rigidbody.GetInstanceID()))
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
