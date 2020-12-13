using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIStateMachineLink : StateMachineBehaviour
{
    protected AIStateMachine _stateMachine;

	public AIStateMachine stateMachine{ set { _stateMachine = value;} }
}
