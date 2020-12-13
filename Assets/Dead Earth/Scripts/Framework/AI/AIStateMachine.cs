using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIStateType
{
	None,
	Idle,
	Alerted,
	Patrol,
	Attack,
	Feeding,
	Pursuit,
	Dead
}

public enum AITargetType
{
	None,
	Waypoint,
	Visual_Player,
	Visual_Light,
	Visual_Food,
	Audio
}

public enum AITriggerEventType
{
	Enter,Stay,Exit
}

public struct AITarget
{
	private AITargetType _type;
	private Vector3 _position;
	private Collider _collider;
	private float _distance;
	private float _time;

	public AITargetType type { get { return _type; } }
	public Collider collider { get { return _collider; } }
	public Vector3 position { get { return position; } }
	public float distance { get { return _distance; } set{ _distance = value;} }
	public float time { get { return _time; } }


	public void Set(AITargetType t, Collider c, Vector3 p, float d)
	{
		_type = t;
		_collider = c;
		_position = p;
		_distance = d;
		_time = Time.time;
	}

	public void Clear()
	{
		_type = AITargetType.None;
		_collider = null;
		_position = Vector3.zero;
		_time = 0;
		_distance = Mathf.Infinity;
	}
}

public abstract class AIStateMachine : MonoBehaviour
{
	public AITarget visualThreat = new AITarget();
	public AITarget audioThreat = new AITarget();
	
	protected Dictionary<AIStateType, AIState> _states = new Dictionary<AIStateType, AIState>();
	protected AITarget _target          = new AITarget();
	protected int _rootPositionRefCount = 0;
	protected int _rootRotationRefCount = 0;

	[SerializeField] protected AIState _currentState = null;
	[SerializeField] protected AIStateType _currentStateType = AIStateType.Idle;
	[SerializeField] protected SphereCollider  _targetTrigger = null;
	[SerializeField] protected SphereCollider _sensorTrigger  = null;
	[SerializeField] [Range(0,15)]  protected float _stoppingDistance = 1.0f;

	protected Animator _animator = null;
	protected NavMeshAgent _navAgent = null;
	protected Collider _collider = null;
	protected Transform _transform = null;

	public Animator animator {get{return _animator;}}

	public NavMeshAgent navAgent { get{return _navAgent;} }

	public Vector3 sensorPostion
	{
		get
		{
			if (_sensorTrigger != null) return Vector3.zero;

			Vector3 point = _sensorTrigger.transform.position;
			point.x += _sensorTrigger.center.x * _sensorTrigger.transform.lossyScale.x;
			point.y += _sensorTrigger.center.y * _sensorTrigger.transform.lossyScale.y;
			point.z += _sensorTrigger.center.z * _sensorTrigger.transform.lossyScale.z;

			return point;
		}
	}

	public float sensorRadius
	{
		get
		{
			if(_sensorTrigger == null) return 0.0f;
			float radius = Mathf.Max(_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.x,
									_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.y);
			return Mathf.Max(radius,_sensorTrigger.radius * _sensorTrigger.transform.lossyScale.z);
		}
	}

	public bool useRootPosition{get{ return _rootPositionRefCount > 0;} }
	public bool useRootRotation{get{ return _rootRotationRefCount > 0;} }

	protected virtual void Awake()
	{
		_animator = GetComponent<Animator>();
		_navAgent = GetComponent<NavMeshAgent>();
		_collider = GetComponent<Collider>();
		_transform = GetComponent<Transform>();


		if(GameSceneManager.instance != null)
		{
			if(_collider) GameSceneManager.instance.RegisterAIStateMachine(_collider.GetInstanceID(),this);
			if(_sensorTrigger) GameSceneManager.instance.RegisterAIStateMachine(_sensorTrigger.GetInstanceID(),this);
		}
	}

	protected virtual void Start()
	{

		if(_sensorTrigger != null)
		{
			AISensor sensor = _sensorTrigger.GetComponent<AISensor>();
			if(sensor != null)
				sensor.parentStateMachine = this;
		}

		AIState[] states = GetComponents<AIState>();

		foreach (AIState aiState in states)
		{
			if (aiState != null && !_states.ContainsKey(aiState.GetStateType()))
			{
				_states.Add(aiState.GetStateType(), aiState);
				aiState.SetStateMachine(this);
			}
		}

		if(_states.ContainsKey(_currentStateType))
		{
			_currentState = _states[_currentStateType];
			_currentState.OnEnterState();
		}
		else
		{
			_currentState = null;
			Debug.LogErrorFormat("Wrong State Type : " + _currentStateType);
		}

		if(_animator != null)
		{
			AIStateMachineLink[] scripts = _animator.GetBehaviours<AIStateMachineLink>();
			foreach(AIStateMachineLink link in scripts)
			{
				link.stateMachine = this;
			}
		}
	}

	public void SetTarget(AITargetType t, Collider c, Vector3 v, float d)
	{
		_target.Set(t,c,v,d);

		if(_targetTrigger != null)
		{
			_targetTrigger.radius = _stoppingDistance;
			_targetTrigger.transform.position = _target.position;
			_targetTrigger.enabled = true;
		}
	}

	public void SetTarget(AITargetType t, Collider c, Vector3 v, float d,float s)
	{
		_target.Set(t,c,v,d);

		if(_targetTrigger != null)
		{
			_targetTrigger.radius = s;
			_targetTrigger.transform.position = _target.position;
			_targetTrigger.enabled = true;
		}
	}

	public void SetTarget(AITarget t)
	{
		_target = t;

		if(_targetTrigger != null)
		{
			_targetTrigger.radius = _stoppingDistance;
			_targetTrigger.transform.position = t.position;
			_targetTrigger.enabled = true;
		}
	}

	public void ClearTarget()
	{
		_target.Clear();

		if (_targetTrigger != null)
		{
			_targetTrigger.enabled = false;
		}
	}

	protected virtual void FixedUpdate()
	{
		visualThreat.Clear();
		audioThreat.Clear();

		if(_target.type != AITargetType.None)
		{
			_target.distance = Vector3.Distance(_transform.position,_target.position);
		}
	}

	protected virtual void Update()
	{
		if(_currentState == null)
			return;

		AIStateType newType = _currentState.OnUpdate();

		if(!_currentStateType.Equals(newType))
		{
			AIState newState = null;

			if(_states.TryGetValue(newType,out newState))
			{
				_currentState.OnExitState();
				newState.OnEnterState();
				_currentState = newState;
			}
			else if(_states.TryGetValue(AIStateType.Idle,out newState))
			{
				_currentState.OnExitState();
				newState.OnEnterState();
				_currentState = newState;
			}
			_currentStateType = newType;
		}
	}

	protected virtual void OnTriggerEnter(Collider other)
	{
		if(_targetTrigger == null || other != _targetTrigger) return;

		if(_currentState != null)
			_currentState.OnDestinationReached(true); 
	}

	protected virtual void OnTriggerExit(Collider other)
	{
		if(_targetTrigger == null || other != _targetTrigger) return;

		if(_currentState != null)
			_currentState.OnDestinationReached(false); 
	}

	public virtual void OnTriggerEvent(AITriggerEventType type, Collider other)
	{
		if(_currentState != null)
			_currentState.OnTriggerEvent(type,other);
	}

	protected virtual void OnAnimatorMove()
	{
		if(_currentState != null)
			_currentState.OnAnimatorUpdated(); 
	}

	protected virtual void OnAnimatorIK(int layerIndex)
	{
		if(_currentState != null)
			_currentState.OnAnimatorIKUpdated();
	}

	public void NavAgentControl(bool positionUpdate,bool rotationUpdate)
	{
		if(_navAgent != null)
		{ 
			_navAgent.updatePosition = positionUpdate;
			_navAgent.updateRotation = rotationUpdate;
		}
	}

	public void AddRootMotionRequest(int rootPosition, int rootRotation)
	{
		_rootPositionRefCount += rootPosition;
		_rootRotationRefCount += rootRotation;
	}
}
