using UnityEngine;
using System.Collections;

public class DinoMove : MonoBehaviour {
	
	public DinoMovementController controller;
	public Vector3 current_velocity;
	public Vector3 target_velocity;
	
	// Use this for initialization
	void Start () {
		controller = GetComponent<DinoMovementController> ();
	}
	
	public void Move(Vector3 velocity) {
		target_velocity = velocity;

	}
	public void Accelerate(Vector3 acc, float speedCap) {
		target_velocity = current_velocity + acc;
		current_velocity = target_velocity + current_velocity;
		current_velocity.Normalize ();
		current_velocity = speedCap * current_velocity;
		Move (current_velocity);
	}
	void Update() {
		current_velocity = current_velocity + target_velocity;
		current_velocity = 0.5f * current_velocity;
		controller.SetVelocity(current_velocity);

	}
}
