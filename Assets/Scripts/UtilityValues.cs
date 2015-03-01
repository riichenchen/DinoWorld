using UnityEngine;
using System.Collections;

public class UtilityValues : MonoBehaviour {
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
	public int[] COEFFS = {721, 646, 57, 1435, 959, 1443, 893, 1106, 308, 9, 1243, 1169, 1485, 971};
	public const int N_ACTIONS = 2;
	public float[,] eqns = new float[N_ACTIONS,4];

	// Use this for initialization
	void Start () {
		for (int a = 0; a < N_ACTIONS; a++) {
			eqns[a,0] = COEFFS[Random.Range (0, COEFFS.Length - 1)];
			eqns[a,1] = COEFFS[Random.Range (0, COEFFS.Length - 1)];
			eqns[a,2] = 0 * COEFFS[Random.Range (0, COEFFS.Length - 1)];
			eqns[a,3] = 0 * COEFFS[Random.Range (0, COEFFS.Length - 1)];
		}
		/*
		eqns[1,0] = Random.Range (-3f, 3f);
		eqns[1,1] = Random.Range (-3f, 3f);
		eqns[1,2] = Random.Range (-3f, 3f);
		eqns[1,3] = Random.Range (-3f, 3f);
		*/
	}
	
	public float utility(int state, float need, float dist) {
		float n= 100 - eqns[state,0] * Mathf.Pow (need, 3) + eqns[state,1] * Mathf.Pow (dist,2);
		return n;
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
