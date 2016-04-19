using UnityEngine;
using System.Collections;

public class EnemyTeam : Team {

	bool trigger = true;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(trigger){
			foreach(Unit u in team){
				usableTeam.Add(u);
			}
			enableUsableTeam();
			positionTeam();
			trigger = false;
		}
	}

	void positionTeam(){
		int count = 0;
		foreach(Unit u in usableTeam){
			int x = Mathf.RoundToInt(u.transform.position.x);
			int y = Mathf.RoundToInt(u.transform.position.z);
			Vector3 pos = new Vector3(x, 0.5f, y);
			u.transform.position = pos;
			Land l = map.getLandAt(x, y);
			l.startOnThisLand(u);
			u.getNewMoveRange();
			count++;
		}
	}
}
