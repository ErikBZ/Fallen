using UnityEngine;
using System.Collections;

public class GameOver : MonoBehaviour {

	public Player realPlayer;
	public Player aiPlayer;
	float thing = 10f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		thing -= Time.deltaTime;
		if(thing <0)
			checkIfDead();
	}

	void checkIfDead(){
		Team realtTeam = realPlayer.teamManager;
		bool dead = isTeamDead(realtTeam);
		if(dead){
			Application.LoadLevel(2);
		}
		Team other = aiPlayer.teamManager;
		dead = isTeamDead(other);
		if(dead){
			Application.LoadLevel(3);
		}
	}
	bool isTeamDead(Team team){
		foreach(Unit u in team.usableTeam){
			if(!u.dead){
				return false;
			}
		}
		return true;
	}
}
