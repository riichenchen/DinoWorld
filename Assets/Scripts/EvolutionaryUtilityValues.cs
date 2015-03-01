using UnityEngine;
using System.Collections;

public class EvolutionaryUtilityValues : MonoBehaviour {
	/* Evaluates the utility of an action as a third degree polynomial.
	 * - Importance: Constant, positive. Defined as the inherent 'importance' of
	 * an action. i.e. eating vs. idly walking
	 * - Need: 3rd degree, positive. Defined as how much an action is needed based
	 * on current status
	 * - Cost: 2nd degree, negative. Defined as distance travelled to get to a resource
	 * or risk taken.
	 * - Stupidity factor: Constant, negative. Inherent nature to do what is sensible.
	 */
	public int EAT = 0;
	public int DRINK = 1;
	public int SLEEP = 2;
	public const int N_ACTIONS = 3;
	public float[,] eqns = new float[N_ACTIONS,7];
	
	// Use this for initialization
	void Start () {
		for (int a = 0; a < N_ACTIONS; a++) {
			for (int i = 0; i < 7; i++) {
				eqns[a,i] = Random.Range(-5f, 5f);
			}
		}
	}
	
	public float utility(int state, float need, float dist) {
		float need_prod = 0;
		float dist_prod = 0;

		for (int i = 0; i < 2; i++) {
			need_prod += eqns[state, i];
			need_prod *= need;
		}

		need_prod += eqns [state, 2];
		need_prod += eqns [state, 3] / need;

		for (int i = 4; i < 7; i++) {
			dist_prod += eqns[state, i];
			dist_prod *= dist;
		}
		
		dist_prod += eqns [state, 2];

		return need_prod + dist_prod;
	}
	
	public void set (int n) {
		if (n == 0) {
			eqns[EAT,0] = 100;
			eqns[EAT,1] = 0;
			eqns[EAT,2] = 0;
			eqns[EAT,3] = 0;
		}
		if (n == 1) {
			eqns[EAT,0] = 100;
			eqns[EAT,1] = 0;
			eqns[EAT,2] = 0;
			eqns[EAT,3] = 0;
		}
	}
}
