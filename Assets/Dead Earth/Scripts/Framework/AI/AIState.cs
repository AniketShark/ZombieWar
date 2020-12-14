using UnityEngine;

public abstract class AIState : MonoBehaviour
{
	protected AIStateMachine _stateMachine;
	public virtual void SetStateMachine(AIStateMachine machine){ _stateMachine = machine; }

	// Default Handlers
	public virtual void OnEnterState()
	{ 
	}
	public virtual void OnExitState(){ }
	public virtual void OnAnimatorUpdated()
	{
		if(_stateMachine.useRootPosition)
			_stateMachine.navAgent.velocity  = _stateMachine.animator.deltaPosition / Time.deltaTime;
		if(_stateMachine.useRootRotation)
			_stateMachine.transform.rotation = _stateMachine.animator.rootRotation;
	}
	public virtual void OnAnimatorIKUpdated() { }
	public virtual void OnTriggerEvent(AITriggerEventType eventType,Collider other){ }
	public virtual void OnDestinationReached(bool isReached){ }
	public abstract AIStateType GetStateType();
	public abstract AIStateType OnUpdate();

}
