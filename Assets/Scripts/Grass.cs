using UnityEngine;
using System.Collections;

public class Grass : MonoBehaviour {
	
	public float nutrition = 50;
	
	
	// Use this for initialization
	void Start () {
		
	}
	
	public void EatNutrition(float amount) {
		nutrition = amount;
	}
	
	public void BeEaten(float amount) {
		if (nutrition > amount) {
			nutrition -= amount;
		} else {
			Die();
		}
	}
	
	public void Die() {
		transform.parent.GetComponent<TerrainCell>().RemoveGrass(gameObject);
		Destroy (gameObject);
	}
}
