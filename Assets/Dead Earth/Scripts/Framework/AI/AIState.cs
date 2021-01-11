using UnityEngine;

public abstract class AIState : MonoBehaviour
{
	protected AIStateMachine _stateMachine;
	public virtual void SetStateMachine(AIStateMachine machine){ _stateMachine = machine; }

	// Default Handlers
	public virtual void OnEnterState(){ }
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

	public static void ConverSphereColliderToWorldSpace(SphereCollider collider, out Vector3 position, out float radius)
	{
		position = Vector3.zero;
		radius = 0.0f;

		if(collider == null)return;

		//Calculate world Space center of SphereCollider
		position = collider.transform.position;
		position.x += collider.center.x * collider.transform.lossyScale.x;
		position.y += collider.center.y * collider.transform.lossyScale.y;
		position.z += collider.center.z * collider.transform.lossyScale.z;


		//Calculate world Space radius of SphereCollider
		radius = Mathf.Max(collider.radius * collider.transform.lossyScale.x,
									collider.radius * collider.transform.lossyScale.y);
		radius = Mathf.Max(radius,collider.radius * collider.transform.lossyScale.z);
	}


	public static float FindSignedAngle(Vector3 fromVector,Vector3 toVector)
	{
		if(fromVector == toVector)
			return 0.0f;

		float angle = Vector3.Angle(fromVector,toVector);
		Vector3 cross = Vector3.Cross(fromVector,toVector);
		angle *= Mathf.Sign(cross.y);
		return angle;
	}

}
