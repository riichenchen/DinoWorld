using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DinoCell : Cell{
	public List<GameObject> Dinosaurs;
	public DinoContainer dinoContainer;
	// Use this for initialization
	void Awake(){
		Debug.Log ("this is reached in Dinocell");
		Dinosaurs = new List<GameObject>();
		dinoContainer = gameObject.transform.parent.gameObject.GetComponent<DinoContainer> ();
	}
	public void AddDinosaur(GameObject Dinosaur){
		Dinosaur.transform.parent = gameObject.transform;
		Dinosaurs.Add (Dinosaur);
	}
	public void Update(){
		List<GameObject> toRemove = new List<GameObject>();
		foreach(GameObject dinosaur in Dinosaurs){
			Vector3 position = dinosaur.transform.position;
			if(dinoContainer.GetDinoCell(position) != this){
				dinoContainer.RelocateDino(dinosaur);
				toRemove.Add(dinosaur);
			}
		}
		foreach(GameObject dinosaur in toRemove){
			Dinosaurs.Remove (dinosaur);
		}
	}
	public List<GameObject> GetDinosaurs(){
		return Dinosaurs;
	}
	public List<GameObject> GetDinosaursInRange(GameObject Dinosaur, float Range){
		List<GameObject> DinosaursInRange = new List<GameObject> ();
		Vector3 position1 = Dinosaur.transform.position;
		foreach(GameObject dinosaur in Dinosaurs){
			Vector3 position2 = dinosaur.transform.position;
			if(Vector3.Distance(position1, position2) < Range){
				DinosaursInRange.Add (dinosaur);
			}
		}
		return DinosaursInRange;
	}
}
