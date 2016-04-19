using UnityEngine;
using System.Collections;

public class AIUnit : MonoBehaviour {

	public AIStance stance;
	public Unit target;
	public Land[] wanderPath;
	public Unit thisUnit;
	public bool looping;

	private int index = 0;

	// Use this for initialization
	void Start () {
		thisUnit = transform.GetComponent<Unit>();
	}
	// Update is called once per frame
	void Update () {
	
	}
	public Land getNextTarget(){
		if(stance == AIStance.Aggressive){
			return target.getLandInhabiting();
		}
		else if(stance == AIStance.Wandering){
			return next();
		}
		return null;
	}
	private Land next(){
		Land nextL = null;
		if(index < wanderPath.Length && thisUnit.getLandInhabiting() == wanderPath[index]){
			index++;
			if(index<wanderPath.Length)
				nextL = wanderPath[index];
			else{
				index = 0;
				nextL = wanderPath[index];
			}
		}
		else 
			nextL = wanderPath[index];
		return nextL;
	}
}
