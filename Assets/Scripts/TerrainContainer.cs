using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TerrainContainer : MonoBehaviour {
	public int nPartitions;
	public GameObject grass;
	public int maxGrassPerCell = 5;
	private GameObject[,] cells;
	private float xLength;
	private float zLength;
	public Vector3 center;
	public GameObject[] water_spots;
	// Use this for initialization
	void Start () {
		xLength = (gameObject.GetComponent<Terrain>()).terrainData.size.x;
		zLength = (gameObject.GetComponent<Terrain>()).terrainData.size.z;
		center = new Vector3 (xLength / 2, 0, zLength / 2) + transform.position;
		TerrainCell.grass = grass;
		TerrainCell.xLength = xLength / nPartitions;
		TerrainCell.zLength = zLength / nPartitions;
		TerrainCell.maxGrassPerCell = maxGrassPerCell;
		cells = new GameObject[nPartitions, nPartitions];
		for(int i = 0; i < nPartitions; i++){
			for(int j = 0; j < nPartitions; j++){
				cells[i,j] = new GameObject();
				cells[i,j].transform.parent = gameObject.transform;
				cells[i,j].transform.position = new Vector3(TerrainCell.xLength * i, 0, TerrainCell.zLength * j);
				
				
				System.Console.WriteLine("stugg");
				(cells[i,j]).AddComponent<TerrainCell>();
			}
		}

		water_spots = GameObject.FindGameObjectsWithTag ("Water_Spot");
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log (1 / Time.deltaTime);
	}
	/*public void RemoveGrass(Grass grass){
		Vector3 position = grass.transform.position;
		GameObject grassObject = grass.gameObject;
		((cells [(int)(position.x * nPartitions / xLength), (int)( position.z * nPartitions / zLength)].GetComponent<TerrainCell>())).RemoveGrass(grassObject);
	}*/
	public GameObject getFood(Vector3 position){
		return GetTerrainCell(position).QueryGrass(position);
	}
	public GameObject nearWater(Vector3 position){
		return GetTerrainCell(position).ContainsWater(position);
	}
	private TerrainCell GetTerrainCell(Vector3 position){
		return ((cells [(int)(position.x * nPartitions / xLength), (int)(position.z * nPartitions / zLength)].GetComponent<TerrainCell> ()));
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
