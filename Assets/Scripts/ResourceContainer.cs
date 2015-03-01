using UnityEngine;
using System.Collections;
[RequireComponent(typeof(WorldContainer))]

public class ResourceContainer : MonoBehaviour {

	public GameObject[] water_spots;
	public GameObject grass;
	public int maxGrassPerCell = 5;

	void Start(){

		water_spots = GameObject.FindGameObjectsWithTag ("Water_Spot");

	}
	public GameObject getFood(Vector3 position){
		ResourceCell cell = GetResourceCell (position);
		if (cell != null)
			return cell.QueryGrass(position);
		else
			return null;
	}
	public GameObject nearWater(Vector3 position){
		return GetResourceCell(position).ContainsWater(position);
	}
	private ResourceCell GetResourceCell(Vector3 position){
		GameObject cell = gameObject.GetComponent<WorldContainer> ().GetCell (position);
		if (cell != null)
			return cell.GetComponent<ResourceCell> ();
		else
			return null;
	}

	public GameObject getWater(Vector3 pos) {
		int minIndex = 0;
		float minDist = 100;
		
		for (int i = 0; i < water_spots.Length; i++) {
			float dist = (pos - water_spots[i].transform.position).magnitude;
			if (dist < minDist) {
				minDist = dist;
				minIndex = i;
			}
		}
		
		return water_spots[minIndex];
	}
}
