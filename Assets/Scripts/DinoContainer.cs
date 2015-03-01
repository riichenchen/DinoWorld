using UnityEngine;

using System.Collections;
using System.Collections.Generic;
public class DinoContainer : MonoBehaviour {
	public int nDinoTypes = 1;
	public GameObject[] DinoTypes;
	public int nDinos;
	private int nPartitions;
	public List<GameObject> Dinosaurs;
	private GameObject[,] cells;
	private float xLength;
	private float zLength;
	bool updated = false;
	float lastDeath = 0;
	// Use this for initialization
	void Awake () {
		gameObject.transform.position = new Vector3 (0, 0, 0);
		xLength = (gameObject.GetComponent<Terrain>()).terrainData.size.x;
		zLength = (gameObject.GetComponent<Terrain>()).terrainData.size.z;
		Dinosaurs = new List<GameObject>();
		nDinoTypes = DinoTypes.Length;
		cells = (gameObject.GetComponent<WorldContainer>()).cells;
		nPartitions = gameObject.GetComponent<WorldContainer> ().nPartitions;

		for(int i = 0; i < nDinos; i++){
			float x = Random.Range (4f * xLength / nPartitions, xLength - 4f * xLength / nPartitions);
			float z = Random.Range (4f * zLength / nPartitions,  zLength - 4f * zLength / nPartitions);
			int DinoType = Random.Range (0, nDinoTypes);
			Dinosaurs.Add((GameObject) Object.Instantiate (DinoTypes[DinoType],
			                                               new Vector3 (x,
												             (gameObject.GetComponent<Terrain>()).SampleHeight(new Vector3(x,0,z)) + 10,
												             z),
			                                               Quaternion.identity));
			Debug.Log (new Vector3 (x, 
			                        (gameObject.GetComponent<Terrain>()).SampleHeight(new Vector3(x,0,z)),
			                        z));
			GetDinoCell(new Vector3 (x, 0, z)).AddDinosaur(Dinosaurs [i]);
		}
	}
	public void initialize(){{;
	}
	}
	void Update(){
		if(updated == false){
			initialize();
			updated = true;
		}
	}
	public void RelocateDino (GameObject relocatedDinosaur) {
		DinoCell dinocell = GetDinoCell (relocatedDinosaur.transform.position);
		if (dinocell != null)
			dinocell.AddDinosaur (relocatedDinosaur);
		else {
			Dinosaurs.Remove (relocatedDinosaur);
			Object.Destroy (relocatedDinosaur);
		}
		relocatedDinosaur = null;
	}

	public void Remove(GameObject dino) {
		DinoCell dinocell = GetDinoCell (dino.transform.position);
		dinocell.Dinosaurs.Remove (dino);
		Dinosaurs.Remove (dino);
		Object.Destroy (dino);
		Debug.Log (Time.time - lastDeath);
		lastDeath = Time.time;
		float x = Random.Range (4f * xLength / nPartitions, xLength - 4f * xLength / nPartitions);
		float z = Random.Range (4f * zLength / nPartitions,  zLength - 4f * zLength / nPartitions);
		int DinoType = Random.Range (0, nDinoTypes);
		GameObject newDino = ((GameObject) Object.Instantiate (DinoTypes[DinoType],
		                                                       new Vector3 (x,
		             (gameObject.GetComponent<Terrain>()).SampleHeight(new Vector3(x,0,z))+ 7,
		             z),
		                                                       Quaternion.identity));
		DinoControlMulti dcm = newDino.GetComponent<DinoControlMulti>();
		EvolutionaryUtilityValues evu = newDino.GetComponent<EvolutionaryUtilityValues>();
		dcm.changeFactor = 0;
		GameObject mostFit = Dinosaurs[0];
		GameObject secondFit = Dinosaurs[1];
		float highestFitness = mostFit.GetComponent<DinoControlMulti>().hunger + mostFit.GetComponent<DinoControlMulti>().thirst;
		float secondFitness = secondFit.GetComponent<DinoControlMulti>().hunger + secondFit.GetComponent<DinoControlMulti>().thirst;
		if (secondFitness > highestFitness) {
			mostFit = Dinosaurs[1];
			secondFit = Dinosaurs[0];
		}
		foreach(GameObject dinosaur in Dinosaurs){
			if(dinosaur.GetComponent<DinoControlMulti>().hunger + dinosaur.GetComponent<DinoControlMulti>().thirst > highestFitness){
				highestFitness = dinosaur.GetComponent<DinoControlMulti>().hunger + dinosaur.GetComponent<DinoControlMulti>().thirst;
				mostFit = dinosaur;
			}
			else if(dinosaur.GetComponent<DinoControlMulti>().hunger + dinosaur.GetComponent<DinoControlMulti>().thirst > secondFitness){
				secondFitness = dinosaur.GetComponent<DinoControlMulti>().hunger + dinosaur.GetComponent<DinoControlMulti>().thirst;
				secondFit = dinosaur;
			}
			dcm.changeFactor += dinosaur.GetComponent<DinoControlMulti>().changeFactor;
		}
		
		for(int j = 0; j < EvolutionaryUtilityValues.N_ACTIONS; j++){
			for(int i = 0; i < 7; i++){
				evu.eqns[j,i] = (mostFit.GetComponent<EvolutionaryUtilityValues>().eqns[j,i] + 
				                 secondFit.GetComponent<EvolutionaryUtilityValues>().eqns[j,i])/2;
			}
		}
		dcm.changeFactor /= Dinosaurs.Count;
		Dinosaurs.Add (newDino);
		RelocateDino(newDino);
	}

	public DinoCell GetDinoCell(Vector3 position){
		if (position.x < xLength && position.z < zLength && position.x > 0 && position.z > 0)
			return ((cells [(int)(position.x * nPartitions / xLength), (int)(position.z * nPartitions / zLength)].GetComponent<DinoCell> ()));
		else
			return null;

	}

	public List<GameObject> GetDinosaurs(Vector3 position){
		if (GetDinoCell (position) != null)
			return GetDinoCell (position).GetDinosaurs ();
		else
			return null;
	}
	public List<GameObject> GetDinosaursInRange(GameObject Dinosaur, float Range){
		if (GetDinoCell (Dinosaur.transform.position) != null)
			return GetDinoCell (Dinosaur.transform.position).GetComponent<DinoCell> ().GetDinosaursInRange (Dinosaur, Range);
		else
			return null;
	}
}
