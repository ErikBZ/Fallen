using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlackOil : Player {

	public List<Sludge> openSludgeList = new List<Sludge>();
	public List<Sludge> closedSludgeList = new List<Sludge>();
	public List<Sludge> sludgesToAddToClose = new List<Sludge>();
	bool isTurnDone = false;
	Sludge currentSludge;
	int count = 0;
	public Land firstInfected;

	// Use this for initialization
	void Start () {
		foreach(Transform t in transform){
			openSludgeList.Add(t.GetComponent<Sludge>());
			t.GetComponent<Sludge>().hive = this;
		}
		Sludge s = openSludgeList[0];
		openSludgeList.Remove(s);
		closedSludgeList.Add(s);
		s.setLand(firstInfected);
	}

	// Update is called once per frame
	void Update () {
		if(myTurn){
			if(!isTurnDone){
				switch(phase){
				case 0:
					currentSludge = closedSludgeList[count];
					phase = 1;
					break;
				case 1:
					propogate(currentSludge);
					phase = 2;
					break;
				default:
					count++;
					if(count >= closedSludgeList.Count){
						foreach(Sludge s in sludgesToAddToClose){
							closedSludgeList.Add(s);
						}
						sludgesToAddToClose.Clear();
						isTurnDone = true;
						count = 0;
					}
					phase = 0;
					break;
				}
			}

			if(isTurnDone){
				otherPlayer.newTurn();
				myTurn = false;
			}
		}	
	}
	public void setNewSludgePatch(Land l){
		if(l == null || openSludgeList.Count == 0){
			return;
		}
		if(l.getInfected()){
			return;
		}

		Sludge nextS = openSludgeList[0];
		nextS.setLand(l);
		Land[] lArray = map.getNeighborsOfLand(l);
		for(int i=0; i<lArray.Length; i++){
			if(lArray[i] != null){
				Sludge possibleN = lArray[i].getSludge();
				if(possibleN != null){
					nextS.addNeighbor(possibleN);
					possibleN.addNeighbor(nextS);
				}
			}
		}
		openSludgeList.Remove(nextS);
		sludgesToAddToClose.Add(nextS);
	}
	public void propogate(Sludge s){
		if(s.neighbors.Count >3 || openSludgeList.Count == 0){
			return;
		}
		float num = Random.Range(0f, 1f);
		if(num < 5f){
			Land[] lArray = map.getNeighborsOfLand(s.getLand());
			for(int i =0; i<lArray.Length; i++){
				setNewSludgePatch(lArray[i]);
			}
		}
	}
	public override void setIsTurn (bool newBool){
		myTurn = newBool;
		isTurnDone = false;
		playerTurn.text = "Player " + team + " go";
	}
}
