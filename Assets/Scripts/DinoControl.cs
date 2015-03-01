using UnityEngine;
using System.Collections;

/* Dinosaur
 * params: hunger
 * to do: idle, move (random & to), look for food, eat, die
 */

public class DinoControl : MonoBehaviour {
	public const float MAX_HUNGER = 100.0f;
	public const float HUNGRY = 40.0f;
	public const float STARVE_RATE = 0.2f;
	public const float EAT_RATE = 0.4f;
	public const float TREE_RADIUS = 5f;

	public enum State{DEAD, IDLE, FIND_FOOD, EATING};
	public const int N_ACTIONS = 1;
	public enum Actions{EAT=0};
	
	public Transform terrainManager;
	public UtilityValues utilityFunction;
	public State state;
	public float changeFactor;
	public float hunger;
	public float speed; 
	public bool diseased;
	public float disease_progress;
	public float disease_resistance;
	public GameObject nextFood;

	void Start () {
		state = State.IDLE;
		diseased = true;
		nextFood = null;
		disease_progress = 0;
		disease_resistance = Random.Range (.1f, .95f);
		terrainManager = GameObject.FindGameObjectWithTag ("Terrain").transform;
		SetBehaviour (1);

	}

	void SetBehaviour(int n) {
		if (n == 0) {
			changeFactor = 40;
			utilityFunction.set(0);
		}
		if (n == 1) {
			changeFactor = 150;
			utilityFunction.set(1);
		}
	}

	void UpdateState() {

		int eatingUtility = -1;
		int drinkingUtility = -1;

		if (hunger < 0 || disease_progress >= 200f)
			state = State.DEAD;

		else if (hunger < HUNGRY && state != State.EATING) {
			nextFood =terrainManager.transform.GetComponent<ResourceContainer>().getFood(transform.position);
			if (nextFood != null) {
				float dist = (transform.position - nextFood.transform.position).magnitude;
				if (utilityFunction.utility(utilityFunction.EAT,1-hunger/100,dist/100000000) 
				    > changeFactor){
					state = State.FIND_FOOD;
				}
				if ( dist <= TREE_RADIUS)
					state = State.EATING;
			}
		}
		else if (state == State.EATING) {
			if (hunger >= MAX_HUNGER || nextFood==null) state = State.IDLE;
		}
		else
			state = State.IDLE;
	}
	
	// Update is called once per frame
	void Update () {
		hunger -= STARVE_RATE  * Time.deltaTime;

		if (diseased) {
			if (Random.Range (0f,1f) > .99998){
				diseased = false;
				disease_progress = 0;
			} else{
				disease_progress += 0.01f;
			}
		}

		UpdateState();

		if (state == State.DEAD)
			Destroy (gameObject);

		if (state == State.EATING) {
			if (nextFood == null) state = State.IDLE;
			else {
				transform.GetComponent<DinoMove>().Move(new Vector3(0,0,0));
				hunger += EAT_RATE;
				nextFood.GetComponent<Grass>().BeEaten(EAT_RATE);
			//state = State.FIND_FOOD;
			}
		}
			/* Do something to tell grass how much it has been eaten? */
		if (state == State.FIND_FOOD) {
			/* Get location of food */
			nextFood =terrainManager.transform.GetComponent<ResourceContainer>().getFood(transform.position);
			if (nextFood==null) state = State.IDLE;
			else {
				Vector3 foodPos = nextFood.transform.position;
				Vector3 vel = (foodPos - transform.position).normalized;
				transform.GetComponent<DinoMove>().Move(vel * speed);
			}
		}
		if (state == State.IDLE) {
			//Vector3 acc = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f));
			Vector3 toCenter = terrainManager.GetComponent<WorldContainer>().center - transform.position;
			toCenter.y = 0;
			toCenter.Normalize ();
			Vector3 acc = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f)) + toCenter/2;

			acc = acc * speed/10;
			transform.GetComponent<DinoMove>().Accelerate(acc, speed);
		}
	}
}
