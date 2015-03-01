using UnityEngine;
using System.Collections;
[RequireComponent (typeof (CharacterController))]
public class DinoMovementController : MonoBehaviour {
	
	
	public Animator animator;
	
	enum CharacterState {
		Idle,
		Walking,
		Trotting,
		Running,
		Jumping,
	}
	
	private CharacterState _characterState;
	
	float inAirControlAcceleration = 3.0f;
	
	// How high do we jump when pressing jump and letting go immediately
	float jumpHeight = 0.5f;
	
	// The gravity for the character
	float gravity = 20.0f;
	// The gravity in controlled descent mode
	float speedSmoothing = 10.0f;
	float rotateSpeed = 500.0f;
	
	bool canJump = false;
	
	private float jumpRepeatTime = 0.05f;
	private float jumpTimeout = 0.15f;
	private float groundedTimeout = 0.25f;
	
	// The current move direction in x-z
	private Vector3 moveDirection = Vector3.zero;
	// The current vertical speed
	private float verticalSpeed = 0.0f;
	// The current x-z move speed
	private float moveSpeed = 0.0f;
	
	// The last collision flags returned from controller.Move
	private CollisionFlags collisionFlags; 
	
	// Are we jumping? (Initiated with jump button and not grounded yet)
	private bool jumping = false;
	private bool jumpingReachedApex = false;
	
	// Are we moving backwards (This locks the camera to not do a 180 degree spin)
	private bool movingBack = false;
	// Is the user pressing any keys?
	private bool isMoving = false;
	// When did the user start walking (Used for going into trot after a while)
	private float walkTimeStart = 0.0f;
	// Last time the jump button was clicked down
	private float lastJumpButtonTime = -10.0f;
	// Last time we performed a jump
	private float lastJumpTime = -1.0f;
	
	
	// the height we jumped from (Used to determine for how long to apply extra jump power after jumping.)
	private float lastJumpStartHeight = 0.0f;
	
	
	private Vector3 inAirVelocity = Vector3.zero;
	
	private float lastGroundedTime = 0.0f;
	
	
	private bool isControllable = true;
	
	private Vector3 velocity = Vector3.zero;
	
	void Awake ()
	{
		moveDirection = transform.TransformDirection(Vector3.forward);
		
		animator = GetComponent<Animator>();
		
	}
	
	
	void UpdateSmoothedMovementDirection ()
	{
		bool grounded = IsGrounded();
		
		bool wasMoving = isMoving;
		isMoving = velocity.magnitude > 0.1;
		
		// Grounded controls
		if (grounded)
		{
			if (velocity.magnitude > 0.1) {
				moveDirection = velocity;
				moveDirection = moveDirection.normalized;
			} else if (moveDirection.magnitude == 0) {
				moveDirection = Vector3.forward;
			}
			_characterState = CharacterState.Idle;
			
			
		}
		// In air controls
		else
		{
			if (isMoving)
				inAirVelocity += velocity.normalized * Time.deltaTime * inAirControlAcceleration;
		}
		
		
		
	}
	
	
	void ApplyJumping ()
	{
		// Prevent jumping too fast after each other
		if (lastJumpTime + jumpRepeatTime > Time.time)
			return;
		
		if (IsGrounded()) {
			// Jump
			// - Only when pressing the button down
			// - With a timeout so you can press the button slightly before landing		
			if (canJump && Time.time < lastJumpButtonTime + jumpTimeout) {
				verticalSpeed = CalculateJumpVerticalSpeed (jumpHeight);
				SendMessage("DidJump", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	
	void ApplyGravity ()
	{
		if (isControllable)	// don't move player at all if not controllable.
		{
			// Apply gravity
			bool jumpButton = Input.GetButton("Jump");
			
			
			// When we reach the apex of the jump we send out a message
			if (jumping && !jumpingReachedApex && verticalSpeed <= 0.0)
			{
				jumpingReachedApex = true;
				SendMessage("DidJumpReachApex", SendMessageOptions.DontRequireReceiver);
			}
			
			if (IsGrounded ())
				verticalSpeed = 0.0f;
			else
				verticalSpeed -= gravity * Time.deltaTime;
		}
	}
	
	float CalculateJumpVerticalSpeed (float targetJumpHeight)
	{
		// From the jump height and gravity we deduce the upwards speed 
		// for the character to reach at the apex.
		return Mathf.Sqrt(2 * targetJumpHeight * gravity);
	}
	
	void DidJump ()
	{
		jumping = true;
		jumpingReachedApex = false;
		lastJumpTime = Time.time;
		lastJumpStartHeight = transform.position.y;
		lastJumpButtonTime = -10;
		
		_characterState = CharacterState.Jumping;
	}
	
	void Update() {
		
		if (!isControllable)
		{
			// kill all inputs if not controllable.
			Input.ResetInputAxes();
		}
		
		if (Input.GetButtonDown ("Jump"))
		{
			lastJumpButtonTime = Time.time;
		}
		
		UpdateSmoothedMovementDirection();
		
		// Apply gravity
		// - extra power jump modifies gravity
		// - controlledDescent mode modifies gravity
		ApplyGravity ();
		
		// Apply jumping logic
		ApplyJumping ();
		
		// Calculate actual motion
		Vector3 movement = velocity + new Vector3 (0, verticalSpeed, 0) + inAirVelocity;
		movement *= Time.deltaTime;
		
		// Move the controller
		CharacterController controller = GetComponent<CharacterController>();
		collisionFlags = controller.Move(movement);
		
		// ANIMATION sector
		/** if(_characterState == CharacterState.Jumping) 
		{
			if(!jumpingReachedApex) {
				_animation[jumpPoseAnimation.name].speed = jumpAnimationSpeed;
				_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade(jumpPoseAnimation.name);
			} else {
				_animation[jumpPoseAnimation.name].speed = -landAnimationSpeed;
				_animation[jumpPoseAnimation.name].wrapMode = WrapMode.ClampForever;
				_animation.CrossFade(jumpPoseAnimation.name);				
			}
		} 
		else  */
		//{
		animator.SetFloat("Speed", (controller.velocity.sqrMagnitude));
		//}
		
		// ANIMATION sector
		
		// Set rotation to the move direction
		if (IsGrounded())
		{
			
			transform.rotation = Quaternion.LookRotation(moveDirection);
			
		}	
		else
		{
			Vector3 xzMove = movement;
			xzMove.y = 0;
			if (xzMove.sqrMagnitude > 0.001)
			{
				transform.rotation = Quaternion.LookRotation(xzMove);
			}
		}	
		
		// We are in jump mode but just became grounded
		if (IsGrounded())
		{
			lastGroundedTime = Time.time;
			inAirVelocity = Vector3.zero;
			if (jumping)
			{
				jumping = false;
				SendMessage("DidLand", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		//	Debug.DrawRay(hit.point, hit.normal);
		if (hit.moveDirection.y > 0.01) 
			return;
	}
	
	public void SetVelocity(Vector3 velocity) {
		this.velocity = velocity;
	}
	
	public float GetSpeed () {
		return moveSpeed;
	}
	
	bool IsJumping () {
		return jumping;
	}
	
	bool IsGrounded () {
		return (collisionFlags & CollisionFlags.CollidedBelow) != 0;
	}
	
	Vector3 GetDirection () {
		return moveDirection;
	}
	
	bool IsMovingBackwards () {
		return movingBack;
	}
	
	bool IsMoving () 
	{
		return Mathf.Abs(Input.GetAxisRaw("Vertical")) + Mathf.Abs(Input.GetAxisRaw("Horizontal")) > 0.5;
	}
	
	bool HasJumpReachedApex ()
	{
		return jumpingReachedApex;
	}
	
	bool IsGroundedWithTimeout ()
	{
		return lastGroundedTime + groundedTimeout > Time.time;
	}
	
	void Reset ()
	{
		gameObject.tag = "Player";
	}
	
	
}
