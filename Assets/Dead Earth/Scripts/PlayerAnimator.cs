using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	public Animator _animator;

	public int _horizontalHash;
	public int _verticalHash;
	public int _attackHash;

	void Start()
	{
		_horizontalHash = Animator.StringToHash("horizontal");
		_verticalHash = Animator.StringToHash("vertical");
		_attackHash = Animator.StringToHash("attack");
	}
	void Update()
	{
		float horizontal = Input.GetAxis("Horizontal") * 2.32f;
		float vertical = Input.GetAxis("Vertical") * 5.67f;
		bool attack = Input.GetMouseButtonDown(0);
		//Debug.LogError(string.Format("Horizontal {0} Vertical {1}  ", horizontal, vertical));
		_animator.SetFloat(_horizontalHash, horizontal,0.1f,Time.deltaTime);
		_animator.SetFloat(_verticalHash, vertical,1.0f, Time.deltaTime);
		if(attack)
			_animator.SetTrigger(_attackHash);
	}
}
