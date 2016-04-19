using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Team : MonoBehaviour {

	public List<Unit> team = new List<Unit>();
	public List<Unit> usableTeam = new List<Unit>();
	public int teamNum;
	public Map map;

	public int[] startPosX;
	public int[] startPosY;

	// Use this for initialization
	void Start () {
		map = GameObject.Find("MAP").GetComponent<Map>();

		disableAll();
		enableUsableTeam();
		positionTeam();
	}
	
	// Update is called once per frame
	void Update () {
	}

	// getters and setters
	public void setUsableTeam(List<Unit> teamSelected){
		usableTeam = teamSelected;
	}
	public List<Unit> getUsableTeam(){
		return usableTeam;
	}

	// privs
	protected void setUpTeam(List<Unit> tList){
		foreach(Transform t in transform){
			Unit u = t.GetComponent<Unit>();
			if(u != null){
				tList.Add(u);
			}
		}
	}
	protected void positionTeam(){
		int count = 0;
		foreach(Unit u in usableTeam){
			Vector3 pos = new Vector3(startPosX[count],0,startPosY[count]);
			Land l = map.getLandAt(startPosX[count], startPosY[count]);
			u.transform.position = pos;
			l.startOnThisLand(u);
			count++;
		}
	}
	protected void disableAll(){
		foreach(Unit u in team){
			u.gameObject.SetActive (false);
		}
	}
	protected void enableUsableTeam(){
		foreach(Unit u in usableTeam){
			u.gameObject.SetActive(true);
		}
	}

	// pubs
	public bool isTeamDone(){
		foreach(Unit u in usableTeam){
			if(!u.getIsMoveDone()){
				return false;
			}
		}
		if(usableTeam.Capacity == 0){
			return false;
		}
		return true;
	}

	public void newTurn(){
		foreach(Unit u in usableTeam){
			if(!u.dead)
				u.setIsMoveDone(false);
		}
	}

	public void addNewTeammate(Unit noobie){
		team.Add(noobie);
	}
}
