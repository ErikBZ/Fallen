using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIPlayer : Player {

	public Transform camera;
	
	int unitListIndex = 0;
	float timeBuffer = 2f;
	float ticker = 0f;

	Unit unit;
	AIUnit unitAI;
	Land nextLand;
	Land targetLand;
	public Player actualPlayer;

	// variables for lerp
	public float speed = 1.0f;
//	Vector3 endMarker;
//	float startTime;
//	float journeyLength;
//	List<Land> pathList = new List<Land>();

	// lerp move
//	int phaseNum = 0;
//	int index = 0;
//	public float unitSpeed = 1.0f;
	Land tileToMoveTo;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(teamManager.isTeamDone() && myTurn){
			unitListIndex = 0;
			phase = 0;
			myTurn = false;
			otherPlayer.setIsTurn(true);
			if(pathList.Count > 0)
				highlightLandList(Color.white, pathList);
		}
		if(myTurn){
			switch(phase){
			case(0):
				unit = teamManager.usableTeam[unitListIndex];
				while(unit.dead){
					teamManager.usableTeam.Remove(unit);
					unitListIndex++;
					if(unitListIndex >= teamManager.usableTeam.Count)
						unitListIndex = 0;

					unit = teamManager.usableTeam[unitListIndex];
				}
				phase = 3;
				break;
			// sets up the lerp
			case(1):
				endMarker = unit.transform.position;
				endMarker = new Vector3(endMarker.x, 5, endMarker.z);
				startTime = Time.time;
				journeyLength = Vector3.Distance(camera.position, endMarker);
				phase = 2;
				break;
			// actual lerp
			case(2):
				float distCovered = (Time.time - startTime) * speed;
				float fracJourney = distCovered/journeyLength;
				camera.position = Vector3.Lerp(camera.position, endMarker, fracJourney);

				if(distance(camera.position, endMarker) <= 0.5){
					phase = 4;
				}
				break;
			case(3):
				// check to see if enemy is in range first then do this
				Unit nextTarget = isThereUnitInRange(unit);
				unitAI = unit.GetComponent<AIUnit>();

				if(nextTarget == null){
					// move to target location
					if(unitAI.stance == AIStance.Aggressive || unitAI.stance == AIStance.Wandering){
						targetLand = unitAI.getNextTarget();
						phase = 1;
					}
					// do nothing
					else{
						unit.setIsMoveDone(true);
						phase = 10;
					}
				}
				else{
					targetLand = nextTarget.getLandInhabiting();
					phase = 1;
				}
				break;
			case(4):
				if(targetLand == null){
					// do nothing
					unit.setIsMoveDone(true);
					phase = 10;
					break;
				}

				nextLand = getMove(unit, targetLand);
				Vector3 pos = unit.transform.position;
				unit.transform.position = new Vector3(pos.x, 0.5f, pos.z);
				phase = 5;
				break;
			case(5):
				lerpMoveUnit();
				break;
			case(6):
				int dist = mDist(unit, targetLand.getUnit());
				if(dist == 1){
					fManager.fight(unit,targetLand.getUnit(), dist);
				}
				unit.setIsMoveDone(true);
				phase = 10;
				break;
			default:
				if(unit.dead){
					teamManager.getUsableTeam().Remove(unit);
					unit.gameObject.SetActive(false);
				}
				unitListIndex++;
				if(unitListIndex >= teamManager.getUsableTeam().Count)
					unitListIndex = 0;

				tileToMoveTo = null;
				nextLand = null;
				highlightLandList(Color.white, pathList);
				phase = 0;
				break;
			}
		}
	}
	void lerpMoveUnit(){
		switch(phaseNum){
		case(0):
			if(index >= pathList.Count)
				phaseNum = 2;
			else{
				Land l = pathList[index];
				endMarker = l.transform.position;
				endMarker = new Vector3(endMarker.x, 2f, endMarker.z);
				startTime = Time.time;
				journeyLength = Vector3.Distance(unit.transform.position, endMarker);
				phaseNum = 1;
			}
			break;
		case(1):
			float distCovered = (Time.time - startTime) * unitSpeed;
			float fracJourney = distCovered/journeyLength;
			unit.transform.position = Vector3.Lerp(unit.transform.position, endMarker, fracJourney);

			if(distance(unit.transform.position, endMarker) <= 0.01f){
				unit.transform.position = endMarker;
				index++;
				phaseNum = 0;
			}
			break;
		default:
			phaseNum = 0;
			index = 0;
			phase = 6;
			unit.moveTo(nextLand);
			unit.setIsMoveDone(true);
			break;
		}
	}

	private float distance(Vector3 vec1, Vector3 vec2){
		float x1 = vec1.x;
		float z1 = vec1.z;
		float x2 = vec2.x;
		float z2 = vec2.z;
		float deltaX = x1 - x2;
		float deltaZ = z1 - z2;
		float beforeSquare = deltaX * deltaX + deltaZ * deltaZ;
		float dist = Mathf.Sqrt(beforeSquare);
		return dist;
	}
	private int mDist(Unit u1, Unit u2){
		return mDist(u1.getLandInhabiting(), u2.getLandInhabiting());
	}
	private int mDist(Land l1, Land l2){
		int deltaX = Mathf.Abs (l1.x - l2.x);
		int deltaY = Mathf.Abs(l1.y - l2.y);
		return	deltaX + deltaY;
	}
	private Land getLandClosest(){
		return null;
	}
	public Land getMove(Unit u, Land target){
		pathList.Clear();
		pathList = map.getNextMove(u, target);
		if(pathList.Count == 0){
			return null;
		}
		return pathList[pathList.Count - 1];
	}
	public Unit isThereUnitInRange(Unit u){
		return map.getUnitInRange(u);
	}
	void OnLeveWasLoaded(int level){
		// do nothing!
	}
}
