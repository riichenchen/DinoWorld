﻿using UnityEngine;
using System.Collections;

public class TerrainCell : MonoBehaviour{
	public static GameObject grass;
	public static float xLength;
	public static float zLength;
	public static int maxGrassPerCell;
	public int nGrass;
	public GameObject currentGrass = null;
	public GameObject[] grassPatches;
	public bool containsWater;
	// Use this for initialization
	//float size, GameObject grass, int grassAmount
	void Start(){
		nGrass = Random.Range (0, maxGrassPerCell); 
		System.Console.WriteLine("stugg");
		grassPatches = new GameObject[maxGrassPerCell];
		for (int i = 0; i < nGrass; i++) {
			float x = Random.Range (0, xLength);
			float z = Random.Range (0,  zLength);
			grassPatches[i] = (GameObject) Object.Instantiate (grass,
			                                  new Vector3 (0,0,0),
			                                  Quaternion.identity);
			grassPatches[i].transform.parent = gameObject.transform;

			grassPatches[i].transform.localPosition = new Vector3 (x, 
			                                                 (gameObject.transform.parent.gameObject.GetComponent<Terrain>()).SampleHeight(new Vector3(x,0,z) + gameObject.transform.position),
			                                                 z);
			grassPatches[i].AddComponent<Grass>();

		}
		for (int i = nGrass; i < maxGrassPerCell; i++) {
			grassPatches[i] = null;
		}
	}
	public void RemoveGrass(GameObject grass){
		if (grass == currentGrass) {
			currentGrass = QueryGrass(grass.transform.position);
		}
		for(int i = 0; i < maxGrassPerCell; i++){
			if(grassPatches[i] == grass){
				grassPatches[i] = null;
				break;
			}
		}
		nGrass--;
	}
	public GameObject QueryGrass(Vector3 position){
		if (currentGrass != null) {
			return currentGrass;
		}
		float nearestDistance = xLength;
		GameObject nearestTree = null;
		for(int i = 0; i < maxGrassPerCell; i++){
			if(grassPatches[i] != null){
				if(nearestTree == null){
					nearestTree = grassPatches[i];
				}
				else{
					if( Vector3.Distance(nearestTree.transform.position, grassPatches[i].transform.position) < nearestDistance){
						nearestDistance = Vector3.Distance(nearestTree.transform.position, grassPatches[i].transform.position);
						nearestTree = grassPatches[i];
					}
				}
			}
		}
		currentGrass = nearestTree;
		return nearestTree;
	}

	public GameObject ContainsWater(Vector3 position) {
		return grass;
	}
}
