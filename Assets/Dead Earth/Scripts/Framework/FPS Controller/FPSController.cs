using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum PlayerMoveStatus { NotMoving,Walking,Running,NotGrounded,Landing}

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [SerializeField] private float _walkSpeed          = 1.0f;
    [SerializeField] private float _runSpeed           = 4.5f;
    [SerializeField] private float _jumpSpeed          = 7.5f;
    [SerializeField] private float _stickToGroundForce = 5.0f;
    [SerializeField] private float _gravityMultiplier  = 2.5f;
	[SerializeField] private MouseLook _mouseLook;

	private Camera _camera                = null;
	private bool _jumpButtonPressed       = false;
	private Vector2 _inputVector          = Vector2.zero;
	private Vector3 _moveDirection        = Vector3.zero;
	private bool _previousGrounded        = false;
	private bool _isWalking               = true;
	private bool _isJumping               = false;

	private float _fallingTimer           = 0.0f;
	private CharacterController _characterController = null;
	private PlayerMoveStatus _movementStatus = PlayerMoveStatus.NotMoving;
	private FPSInput _input;
	private Vector2 _moveInput = Vector2.zero;
	private bool _sprint = false;
	private bool _jump = false;

	public PlayerMoveStatus movementStatus { get { return _movementStatus;} }
	public float walkSpeed { get { return _walkSpeed;} }
	public float runSpeed { get { return _runSpeed;} }

	private void Start()
	{
		_characterController = GetComponent<CharacterController>();
		_camera = Camera.main;
		
		_movementStatus = PlayerMoveStatus.NotMoving;
		_fallingTimer = 0.0f;
		_input = new FPSInput();
		_input.KyeboardMouse.Move.performed += OnMove;
		_input.KyeboardMouse.Sprint.performed += OnSprint;
		_input.KyeboardMouse.Jump.performed += OnJump;
		_input.Enable();
	}

	private void OnJump(InputAction.CallbackContext callback)
	{
		_jump = callback.ReadValueAsButton();
		//Debug.LogErrorFormat("Jump : {0}",_jump);
	}

	private void OnSprint(InputAction.CallbackContext callback)
	{
		_sprint = callback.ReadValueAsButton();
		//Debug.LogErrorFormat("Sprint : {0}",_sprint);
	}

	private void OnMove(InputAction.CallbackContext callback)
	{
		_moveInput = callback.ReadValue<Vector2>();
	}

	private void OnDestroy()
	{
		_input.KyeboardMouse.Move.performed -= OnMove;
		_input.Dispose();
	}

	protected void Update()
	{
		if(_characterController.isGrounded) _fallingTimer = 0.0f;
		else _fallingTimer += Time.deltaTime;

		if (Time.timeScale > Mathf.Epsilon)
			_mouseLook.LookRotation(transform,_camera.transform);

		if(!_jumpButtonPressed)
			_jumpButtonPressed = _jump;

		if(!_previousGrounded && _characterController.isGrounded)
		{
			if(_fallingTimer > 0.5f)
			{
				//TODO : Play Landing Sound
			}
			_moveDirection.y = 0f;
			_isJumping = false;
			_movementStatus = PlayerMoveStatus.Landing;
		}
		else if(!_characterController.isGrounded)
		{ 
			_movementStatus = PlayerMoveStatus.NotGrounded;
		}
		else if(_characterController.velocity.sqrMagnitude < 0.01f)
		{ 
			_movementStatus = PlayerMoveStatus.NotMoving;
		}
		else if(_isWalking)
			_movementStatus = PlayerMoveStatus.Walking;
		else
			_movementStatus = PlayerMoveStatus.Running;

		_previousGrounded = _characterController.isGrounded;
	}

	protected void FixedUpdate()
	{
		//float horizontal = Input.GetAxis("Horizontal");
		//float vertical = Input.GetAxis("Vertical");

		bool wasWalking = _isWalking;
		_isWalking = !_sprint;//Input.GetKey(KeyCode.LeftShift);

		float speed = _isWalking ? _walkSpeed : _runSpeed;

		_inputVector = new Vector2(_moveInput.x ,_moveInput.y);

		if(_inputVector.sqrMagnitude > 1) _inputVector.Normalize();

		Vector3 desiredMove = transform.forward * _inputVector.y + (transform.right * _inputVector.x);

		RaycastHit hitInfo;
		if(Physics.SphereCast(transform.position,_characterController.radius,Vector3.down, out hitInfo,	_characterController.height / 2,1))
			desiredMove = Vector3.ProjectOnPlane(desiredMove,hitInfo.normal).normalized;

			_moveDirection.x = desiredMove.x * speed;
			_moveDirection.z = desiredMove.z * speed;

			if(_characterController.isGrounded)
			{
				_moveDirection.y = -_stickToGroundForce;

				if(_jumpButtonPressed)
				{
					_moveDirection.y = _jumpSpeed;
					_jumpButtonPressed = false;
					_jump = false;
					_isJumping = true;
					//TODO:Play Jump Sound
				}
			}
			else
			{
				_moveDirection += Physics.gravity * _gravityMultiplier * Time.fixedDeltaTime;
			}
			_characterController.Move(_moveDirection * Time.fixedDeltaTime);
		
	}
}
