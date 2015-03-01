using UnityEngine;
using System.Collections;
[RequireComponent(typeof(ResourceContainer))]
[RequireComponent(typeof(DinoContainer))]

public class WorldContainer : MonoBehaviour {
	public int nPartitions;
	public GameObject[,] cells;
	public Vector3 center;
	
	private float xLength;
	private float zLength;

		// Use this for initialization
	void Awake () {
		xLength = (gameObject.GetComponent<Terrain>()).terrainData.size.x;
		zLength = (gameObject.GetComponent<Terrain>()).terrainData.size.z;
		center = new Vector3 (xLength/2, 0, zLength/2) + transform.position;

		cells = new GameObject[nPartitions, nPartitions];
		Cell.xLength = xLength / nPartitions;
		Cell.zLength = zLength / nPartitions;

		for(int i = 0; i < nPartitions; i++){
			for(int j = 0; j < nPartitions; j++){
				cells[i,j] = new GameObject();
				cells[i,j].transform.parent = gameObject.transform;
				cells[i,j].transform.position = new Vector3(Cell.xLength * i, 0, Cell.zLength * j);
				cells[i,j].AddComponent<ResourceCell>();
			  	cells[i,j].AddComponent<DinoCell>();
			}
		}
	}

	public GameObject GetCell(Vector3 position){
		if (position.x < xLength && position.z < zLength && position.x > 0 && position.z > 0) {
			return ((cells [(int)(position.x * nPartitions / xLength), (int)(position.z * nPartitions / zLength)]));
		}
		return null;
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (1 / Time.deltaTime);
	}
}
