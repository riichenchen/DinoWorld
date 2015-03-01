using UnityEngine;
using System.Collections;

/* Dinosaur
 * params: hunger
 * to do: idle, move (random & to), look for food, eat, die
 */

public class DinoControlMulti : MonoBehaviour {
	public const float MAX_HUNGER = 100.0f;
	public const float HUNGRY = 40.0f;
	public const float STARVE_RATE = 0.6f;
	public const float EAT_RATE = 1f;
	public const float TREE_RADIUS = 5f;
	
	public const float MAX_THIRST = 100f;
	public const float THIRSY = 60f;
	public const float DEHYDRATE_RATE = 0.5f;
	public const float DRINK_RATE = 8f;
	public float WATER_RADIUS = 7f;

	public const float MAX_ENERGY = 100f;
	public const float TIRE_RATE = 0.5f;
	public const float ENERGY_RECOVERY_RATE = 2f;
	public const float SLEEP_TIME = 30;

	public const float DISEASE_MAX = 200f;
	public const float DISEASE_INCREMENT = 0.01f;

	
	public enum State{DEAD, IDLE, FIND_FOOD, EATING, DRINKING, FIND_WATER, SLEEPING};
	public const int N_ACTIONS = 1;
	public enum Actions{EAT=0};
	
	public Transform terrainManager;
	public EvolutionaryUtilityValues utilityFunction;
	public State state;
	public float changeFactor;
	public float hunger;
	public float thirst;
	public float energy;
	public float speed; 
	public bool diseased;
	public float disease_progress;
	public float disease_resistance;
	public float frames_dead;
	public GameObject goal;

	public float sleepStart;
	public float time;

	public Animator anim;

	public Material sickMaterial;
	public Material healthyMaterial;

	public float contagiousness = 0.1f;
	
	void Start () {
		state = State.IDLE;
		diseased = false;
		goal = null;
		disease_progress = 0;
		disease_resistance = Random.Range (.1f, .95f);
		terrainManager = GameObject.FindGameObjectWithTag ("Terrain").transform;
		utilityFunction = GetComponent<EvolutionaryUtilityValues> ();
		anim = GetComponent<Animator> ();
		frames_dead = 0;
		
	}
	
	void UpdateState() {
		
		float eatingUtility = -1;
		float drinkingUtility = -1;
		float sleepingUtility = -1;
		
		if (hunger < 0 || disease_progress >= DISEASE_MAX || thirst < 0 || energy < 0) {
			state = State.DEAD;
			return;
		}

		if (state == State.EATING) {
			if (hunger >= MAX_HUNGER || goal==null) {
				state = State.IDLE;
				anim.SetBool("Eating", false);
			}
			return;
		}

		if (hunger < HUNGRY && state != State.EATING) {
			goal = terrainManager.transform.GetComponent<ResourceContainer>().getFood(transform.position);
			if (goal != null) {
				float dist = (transform.position - goal.transform.position).magnitude;
				if ( dist <= TREE_RADIUS) {
					state = State.EATING;
					anim.SetBool("Eating", true);
				} else {
					eatingUtility = utilityFunction.utility(utilityFunction.EAT, hunger/100, dist/50);
				}
			}
		}

		if (state != State.DRINKING && thirst < THIRSY) {
			goal = terrainManager.transform.GetComponent<ResourceContainer>().getWater(transform.position);

			if (goal != null) {
				float dist = (transform.position - goal.transform.position).magnitude;
				if (dist < WATER_RADIUS) {
					state = State.DRINKING;
					anim.SetBool("Drink", true);
				} else {
					drinkingUtility = utilityFunction.utility(utilityFunction.DRINK, thirst, dist);
				}
			}

		}

		if (state != State.SLEEPING) {
			sleepingUtility = utilityFunction.utility(utilityFunction.SLEEP, energy, 0);
		}

		float highest_util = Mathf.Max (sleepingUtility, Mathf.Max (eatingUtility, drinkingUtility));

		if (highest_util > changeFactor) {
			if (eatingUtility == highest_util){
				state = State.FIND_FOOD;
			} else if (drinkingUtility == highest_util) {
				state = State.FIND_WATER;
			} else if (sleepingUtility == highest_util) {
				state = State.SLEEPING;
				sleepStart = Time.time;
				anim.SetBool("Sleep", true);
			}
		}


	}
	
	// Update is called once per frame
	void Update () {
		hunger -= STARVE_RATE  * Time.deltaTime;
		thirst -= DEHYDRATE_RATE * Time.deltaTime;

		if (state != State.SLEEPING) {
			energy -= TIRE_RATE * Time.deltaTime; 
		}
		
		if (diseased) {
			if (Random.Range (0f,1f) > .9998){
				diseased = false;
				disease_progress = 0;
				GetComponentInChildren<Renderer>().material = healthyMaterial;
			} else{
				disease_progress += DISEASE_INCREMENT;
			}
		}
		
		UpdateState();
		if (state == State.DEAD && frames_dead == 0) {
			anim.SetTrigger ("Death");
			anim.SetBool("Sleep", false);
			transform.GetComponent<DinoMove> ().Move (new Vector3 (0, 0, 0));
			frames_dead++;
		} else if ((state == State.DEAD && frames_dead <= 2000) || diseased) {
			if (state == State.DEAD) {
				Debug.Log ("infect");
				frames_dead++;
			}
			if (Random.Range (0f, 1f) < contagiousness) {
				GameObject[] dinos_in_range = transform.parent.GetComponent<DinoCell>().GetDinosaursInRange(gameObject, 50).ToArray();
				if (dinos_in_range.Length != 0) {
					dinos_in_range[Random.Range (0, dinos_in_range.Length)].transform.GetComponent<DinoControlMulti>().Expose();
				}
			}
		} else if (state == State.DEAD) {
			Die ();
		}
		
		if (state == State.EATING) {
			if (goal == null) {
				state = State.IDLE;
				goal = null;
		    } else {
				transform.GetComponent<DinoMove> ().Move (new Vector3 (0, 0, 0));
				hunger += EAT_RATE * Time.deltaTime;
				goal.GetComponent<Grass> ().BeEaten (EAT_RATE);
			}
		} else if (state == State.DRINKING) {
			if (thirst >= MAX_THIRST) {
				state = State.IDLE;
				goal = null;
				anim.SetBool("Drink", false);
			} else {
				transform.GetComponent<DinoMove> ().Move (new Vector3 (0, 0, 0));
				thirst += DRINK_RATE * Time.deltaTime;
			}
		} else if (state == State.SLEEPING) {
			transform.GetComponent<DinoMove> ().Move (new Vector3 (0, 0, 0));
			time = Time.time;
			if (Time.time - sleepStart > SLEEP_TIME) {
				energy += 50;
				if (energy > MAX_ENERGY) {
					energy = MAX_ENERGY;
				}
				state = State.IDLE;
				anim.SetBool("Sleep", false);
			}
		}
		/* Do something to tell grass how much it has been eaten? */
		if (state == State.FIND_FOOD) {
			/* Get location of food */
			goal = terrainManager.transform.GetComponent<ResourceContainer> ().getFood (transform.position);
			if (goal == null)
				state = State.IDLE;
			else {
				Vector3 foodPos = goal.transform.position;
				Vector3 vel = (foodPos - transform.position).normalized;
				transform.GetComponent<DinoMove> ().Move (vel * speed);
			}
		} else if (state == State.FIND_WATER) {
			Vector3 waterPos = goal.transform.position;
			Vector3 vel = (waterPos - transform.position).normalized;
			transform.GetComponent<DinoMove> ().Move (vel * speed);
		} else if (state == State.IDLE) {
			Vector3 toCenter = terrainManager.GetComponent<WorldContainer>().center - transform.position;
			toCenter.y = 0;
			toCenter.Normalize ();
			Vector3 acc = new Vector3(Random.Range(-1f,1f),0,Random.Range(-1f,1f)) + toCenter/9;
			
			acc = acc * speed/10;
			transform.GetComponent<DinoMove>().Accelerate(acc, speed);
		}
	}

	void Expose() {
		if (Random.Range (0f, 1f) > disease_resistance) {
			diseased = true;
			GetComponentInChildren<Renderer>().material = sickMaterial;
		}
	}

	private void Die() {
		terrainManager.GetComponent<DinoContainer> ().Remove (gameObject);
	}
}
