using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sludge : MonoBehaviour {

	public BlackOil hive;
	Land land;
	public List<Sludge> neighbors = new List<Sludge>();


	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void setLand(Land l){
		land = l;
		l.terrain = 1;
		l.setInfected(true);
		l.setSludge(this);
		Vector3 pos = l.transform.position;
		Vector3 newPos = new Vector3(pos.x, -1, pos.z);
		gameObject.transform.position = newPos;
		gameObject.SetActive(true);

	}
	public Land getLand(){
		return land;
	}
	public void takeHit(){
		foreach(Sludge s in neighbors){
			s.blowUp(this);
		}
		blowUp(this);
	}

	public void blowUp(Sludge cameFrom){
		clearFromNeighbor(cameFrom);
		neighbors.Clear();
		land.terrain = 0;
		land.setSludge(null);
		land.setInfected(false);
		hive.openSludgeList.Add(this);
		hive.closedSludgeList.Remove(this);
		gameObject.SetActive(false);
	}
	public void clearFromNeighbor(Sludge cameFrom){
		foreach(Sludge s in neighbors){
			if(s != cameFrom){
				s.neighbors.Remove(this);
			}
		}
	}
	public void addNeighbor(Sludge s){
		neighbors.Add(s);
	}
}
