using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviour
{
	private static GameSceneManager _instance = null;

	public static GameSceneManager instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (GameSceneManager)GameObject.FindObjectOfType(typeof(GameSceneManager));
			}
			return _instance;
		}
	}


	private Dictionary<int,AIStateMachine> _stateMachines = new Dictionary<int, AIStateMachine>();
	
	/// <summary>
	/// Registers AI State Machine
	/// </summary>
	/// <param name="id"></param>
	/// <param name="machine"></param>
	public void RegisterAIStateMachine(int id, AIStateMachine machine)
	{
		if(!_stateMachines.ContainsKey(id))
		{
			_stateMachines.Add(id,machine);
		}
	}

	/// <summary>
	/// Gets AI State Machine for id
	/// </summary>
	/// <param name="id"></param>
	/// <returns></returns>
	public AIStateMachine GetAIStateMachine(int id)
	{
		AIStateMachine machine = null;
		if(_stateMachines.TryGetValue(id,out machine))
		{
			return machine;
		}
		return null;
	}
}
