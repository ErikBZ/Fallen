using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {

//	Dictionary<string, Land> map = new Dictionary<string, Land>();
	Dict<Land> map = new Dict<Land>();
	List<Land> landList = new List<Land>();
	List<int> intList = new List<int>();
	Dictionary<Land, int> f_Scores = new Dictionary<Land, int>();
	Dictionary<Land, int> g_Scores = new Dictionary<Land, int>();
	Dictionary<Land, Land> cameFrom = new Dictionary<Land, Land>();

	// Use this for initialization
	void Start () {
		setUpMap();

//		printContents();
	}
	
	// Update is called once per frame
	void Update (){	
	}

	public void check(){
		Debug.Log("There is an instance of this thing on another thing");
	}

	// getters and seters

	// i set these 2 getters to void and returned nothing but i do not know
	// why. so i guess this is why commenting is incredibly important

	// these will not work since the key is an object where the values
	// are technically not the same
	public Land getLandAt(Vector2 iarry){
		return map.get((int)iarry.x, (int)iarry.y);
	}
	public Land getLandAt(int x, int y){
//		Vector2 arry = new Vector2();
//		arry.x = x;
//		arry.y = y;
		string arry = x + "," + y;
		return map.get(arry);
	}
	public Land getLandAt(string pos){
		return map.get(pos);
	}


	// helper methods
	public void printContents(){
		string mapString = map.ToString();
		Debug.Log(mapString);
	}

	private void setUpMap(){
		foreach(Transform ch in transform){
			Land land = ch.gameObject.GetComponent<Land>();
			if(land != null){
				string pos = land.x + "," + land.y;
				map.Add(pos, land);
			}
			else
				Debug.Log(ch.position);
		}
	}

	public Land[] getNeighborsOfLand(Land current){
		Land[] lArray = new Land[4];
		lArray[0] = map.get(current.x-1, current.y);
		lArray[1] = map.get(current.x+1, current.y);
		lArray[2] = map.get(current.x, current.y+1);
		lArray[3] = map.get (current.x, current.y-1);
		return lArray;
	}


	// this recursive will hightlight all possible areas that a unit can go to
	// well here it goes
	// i think this is the A* algorithm

	// i might put this in a different script or class
	// most likely in a unit class since i can just make each unit have
	// a link to the map class
	public void findMovableLands(List<Land> arlLand, Land land, int m){
		arlLand.Add(land);
		recGetUnitMovableV(arlLand, land, m);
	}
	public void findMovableLands(Unit u, Land l, int m){
		u.getMoveableList().Add(l);
		recGetUnitLandList(u, l, m, -1);
	}

	// for attacking
	public void recGetUnitMovableV(List<Land> arlLand, Land land, int m){
		if(land == null)
			return;

		int x = land.x;
		int y = land.y;

		if(!arlLand.Contains(land))
				arlLand.Add(land);

		if(m > 0){
			m--;
			// check left
			recGetUnitMovableV(arlLand, map.get(x-1, y), m);
			// check right
			recGetUnitMovableV(arlLand, map.get(x+1, y), m);
			// check up
			recGetUnitMovableV(arlLand, map.get(x, y+1), m);
			// check down
			recGetUnitMovableV(arlLand, map.get(x, y-1), m);
		}
	}

	// for moving
	public void recGetUnitLandList(Unit u, Land l,int m, int indexPrior){
		if(l == null)
			return;
		
		m = m+l.terrain;
		int x = l.x;
		int y = l.y;
		bool walk = true;

		if(null != l.getUnit()){
			Unit other = l.getUnit();
			walk = other.getTeam() == u.getTeam();
		}
		else
			walk = l.getWalkable();
		if(m>=0 && (walk || u.getMoveableList().Contains(l))){
			if(!u.getMoveableList().Contains(l))
				u.getMoveableList().Add(l);
			
			if(indexPrior < u.getMoveableList().IndexOf(l)){
				m--;
				// check left
				recGetUnitLandList(u, map.get(x-1, y), m, u.getMoveableList().IndexOf(l));
				// check right
				recGetUnitLandList(u, map.get(x+1, y), m, u.getMoveableList().IndexOf(l));
				// check up
				recGetUnitLandList(u, map.get(x, y+1), m, u.getMoveableList().IndexOf(l));
				// check down
				recGetUnitLandList(u, map.get(x, y-1), m, u.getMoveableList().IndexOf(l));
			}
		}
	}

	// always clear the int list before hand
	public void recGetUnitLandList(Unit u, int m){
		List<Land> newList = new List<Land>();
		List<int> newIntList = new List<int>();

		bool areAllZero = true;

		foreach(Land current in landList){
			int thing = landList.IndexOf(current);
			int move = intList[thing];

			if(move > 0){
				areAllZero = false;
				Land[] lArray = new Land[4];
				lArray[0] = map.get(current.x-1, current.y);
				lArray[1] = map.get(current.x+1, current.y);
				lArray[2] = map.get(current.x, current.y+1);
				lArray[3] = map.get (current.x, current.y-1);

				for(int i =0; i<lArray.Length; i++){
					Land l = lArray[i];
					Debug.Log(l);
					if(l==null){
						Debug.Log(current);
					}
					else if(!u.getMoveableList().Contains(l) && !newList.Contains(l) && move-1+l.terrain >= 0 && l.walkable){
						u.getMoveableList().Add(l);
						newList.Add(l);
						newIntList.Add(move-1+l.terrain);
					}
				}
			}
		}
		// if all the outside ints are zero end the recursion
		if(areAllZero){
			return;
		}
		landList = newList;
		intList = newIntList;
		recGetUnitLandList(u, m);
	}
	// assume first l has been added already to the int list and landlist
	// best one
	public void getMoveRange(Unit u, Land current, int move){
		Land[] lArray = new Land[4];
		try{
			lArray[0] = map.get(current.x-1, current.y);
			lArray[1] = map.get(current.x+1, current.y);
			lArray[2] = map.get(current.x, current.y+1);
			lArray[3] = map.get (current.x, current.y-1);
		}
		catch{
			return;
		}
		for(int i =0; i<lArray.Length; i++){
			Land l = lArray[i];
			int newMove = 0;
			Unit uOnLand = null;
			bool goNext = true;

			if(l!=null){
				newMove = move-l.terrain-1;
				if(l.occupiedEh){
					uOnLand = l.getUnit();
					if(uOnLand.team != u.getTeam()){
						goNext = false;
					}
				}

			}
			if(l==null){
			}
			//ADD
			else if(!u.getMoveableList().Contains(l) && newMove >= 0 && (l.walkable||goNext)){
				u.getMoveableList().Add(l);
				intList.Add(move-1-l.terrain);
				getMoveRange(u, l, newMove);
			}
			//CHECK
			else if(u.getMoveableList().Contains(l) && (l.walkable||goNext)){
				int index = u.getMoveableList().IndexOf(l);
				int oldMove = intList[index];
				if(newMove > oldMove){
					intList[index] = newMove;
					getMoveRange(u, l, newMove);
				}
			}
		}
	}
	private void getFullAttackRange(Unit u, Land current, int move, List<Land> thing, List<Land> closeOccupiedLands){
		Land[] lArray = new Land[4];
		lArray[0] = map.get(current.x-1, current.y);
		lArray[1] = map.get(current.x+1, current.y);
		lArray[2] = map.get(current.x, current.y+1);
		lArray[3] = map.get (current.x, current.y-1);

		for(int i =0; i<lArray.Length; i++){
			Land l = lArray[i];
			int newMove = 0;
			if(l!=null)
				newMove = move-l.terrain-1;
			
			if(l==null){
			}
			//ADD
			else if(!thing.Contains(l) && newMove >= 0 && (l.walkable || (!l.walkable && l.occupiedEh))){
				thing.Add(l);
				intList.Add(move-1-l.terrain);
				if(l.occupiedEh){
					if(l.getUnit().team != u.team){
						closeOccupiedLands.Add(l);
					}
					else
						getFullAttackRange(u, l, newMove, thing, closeOccupiedLands);
				}
				else
					getFullAttackRange(u, l, newMove, thing, closeOccupiedLands);
			}
			//CHECK
			else if(thing.Contains(l) && (l.walkable || (!l.walkable && l.occupiedEh))){
				int index = thing.IndexOf(l);
				int oldMove = intList[index];
				if(newMove > oldMove){
					intList[index] = newMove;
					getFullAttackRange(u, l, newMove, thing, closeOccupiedLands);
				}
			}
		}
	}
	public Unit getUnitInRange(Unit u){
		List<Land> listTracker = new List<Land>();
		List<Land> actualList = new List<Land>();

		listTracker.Add(u.getLandInhabiting());
		landList.Clear();
		intList.Clear();
		landList.Add(u.getLandInhabiting());
		intList.Add(u.move);
		getFullAttackRange(u, u.getLandInhabiting(), u.move, listTracker, actualList);

		Unit target = null;
		int mdist = 100;

		foreach(Land l in actualList){
			int newDist = mDist(u.getLandInhabiting(), l);
			if(newDist < mdist){
				if(l.getUnit().getTeam() != u.getTeam()){
					target = l.getUnit();
				}
			}
		}
		return target;
	}

	public void getMoveList(Unit unit){
		int move = unit.getMove();
		unit.getMoveableList().Add(unit.getLandInhabiting());
		landList.Clear();
		intList.Clear();
		landList.Add(unit.getLandInhabiting());
		intList.Add(unit.move);

		getMoveRange(unit, unit.getLandInhabiting(), unit.move);
	}

	public Land landClostestToTarget(Unit u, Unit target, Land current, int m, List<Land> newLand){
		Land landTarget = target.getLandInhabiting();

		if(m<=0){
			newLand.Add(current);
			return current;
		}

		Land[] landArray = new Land[4];
		landArray[0] = map.get(current.x-1, current.y);
		landArray[1] = map.get(current.x+1, current.y);
		landArray[2] = map.get(current.x, current.y+1);
		landArray[3] = map.get (current.x, current.y-1);

		for(int i=0; i<landArray.Length; i++){
			Land l = landArray[i];
			int newM = m-1+l.terrain;
			float dist = distance(l, landTarget);
			float curr = distance(current, landTarget);

			if(!l.walkable && l.occupiedEh){
				Unit unitOnLand = l.getUnit();
				if(unitOnLand == target){
					newLand.Add(l);
					return current;
				}
			}
			else if(l.terrain < 0 && dist < curr){
				newLand.Add(l);

			}
			if(!(newM <= -1) && l.walkable && dist < curr){
				newLand.Add(l);
				return landClostestToTarget(u, target, l, newM, newLand);
			}
		}
		return null;
	}
//	public Land landClostestToTarget(Unit u, Unit target, Land current, int m){
//		Land landTarget = target.getLandInhabiting();
//		bool done = false;
//		Land nextLand = current;
//		// done is m == 0 or if distance is == 1
//		do{
//			float currDist = distance(current, landTarget);
//			Land[] landArray = new Land[4];
//			landArray[0] = map.get(nextLand.x-1, nextLand.y);
//			landArray[1] = map.get(nextLand.x+1, nextLand.y);
//			landArray[2] = map.get(nextLand.x, nextLand.y+1);
//			landArray[3] = map.get (nextLand.x, nextLand.y-1);
//
//			for(int i = 0; i<landArray.Length; i++){
//				Land l = landArray[i];
//				if(Mathf.Approximately(distance(l, landTarget), 0)){
//					done = true;
//				}
//				else if(distance(l, landTarget) < currDist
//				   && l.walkable){
//					nextLand = l;
//				}
//			}
//			if(!done)
//				m = m-1+nextLand.terrain;
//			if(m == 0)
//				done = true;
//
//		}while(!done);
//		return nextLand;
//	}
	private float distance(Land l1, Land l2){
		return distance2D(l1.transform.position, l2.transform.position);
	}
	private float distance2D(Vector3 vec1, Vector3 vec2){
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
	public List<Land> getNextMove(Unit u, Land target){
		Land first = u.getLandInhabiting();
		return aStarPathFinder(first, target, u);
	}

	private List<Land> aStarPathFinder(Land start, Land end, Unit u){
		f_Scores.Clear();
		g_Scores.Clear();
		cameFrom.Clear();
		List<Land> openList = new List<Land>();
		List<Land> closeList = new List<Land>();

		openList.Add(start);
		Land current = start;
		g_Scores.Add(start, 0);

		while(openList.Count > 0){
			// removing lowest rank
			current = getLowestFScore(end, openList);

			// checking if the g score is equal to the units move
			// or if current is the goal
			if(g_Scores[current] >= u.getMove() || current == end){
				if(current.occupiedEh){
					if(current.getUnit() != u){
						openList.Remove(current);
						closeList.Add(current);
						current = cameFrom[current];
					}
					else{
						end = current;
						break;
					}
				}
				else{
					if(g_Scores[current] == u.getMove())
						end = current;
					else if(g_Scores[current] > u.getMove()){
						end = cameFrom[current];
					}
					else
						end = current;
					break;
				}
			}
			else if(g_Scores[current] < u.getMove() && mDist(end, current) == 1 && end.occupiedEh){
				end = current;
				break;
			}

			// removing current from open
			openList.Remove(current);
			// add ing it to close
			closeList.Add(current);

			// for neighbors of current
			Land[] landArray = new Land[4];
			landArray[0] = map.get(current.x-1, current.y);
			landArray[1] = map.get(current.x+1, current.y);
			landArray[2] = map.get(current.x, current.y+1);
			landArray[3] = map.get (current.x, current.y-1);

			for(int i=0; i<landArray.Length; i++){
				Land l = landArray[i];
				bool goForward = true;
				if(l == null){
					goForward = false;
				}
				else if(l.occupiedEh){
					Unit unit = l.getUnit();
					if(unit.team != u.team)
						goForward = false;
//					if(!l.walkable){
//						goForward = false;
//					}
				}

				if(goForward){
					int cost = g_Scores[current] + l.terrain + 1;
					if(openList.Contains(l)){
						if(cost < g_Scores[l]){
							g_Scores[l] = cost;
						}
					}
					else if(closeList.Contains(l)){
						if(cost < g_Scores[l] + l.terrain+1){
							closeList.Remove(l);
							openList.Add(l);
							g_Scores[l] = cost;
							cameFrom[l] = current;
						}
					}
					else if(!openList.Contains(l)){
						openList.Add(l);
						g_Scores.Add(l, cost);
						cameFrom.Add(l, current);
					}
				}
			}
		}
		List<Land> thing = new List<Land>();
		try{
			thing = getPath(start, end);
		}
		catch{
			Debug.Log("something went wrong in getPath()");
		}
		return thing;
	}
	private List<Land> getPath(Land start, Land end){
		List<Land> newList = new List<Land>();
		Land l = end;

		if(start == end){
			newList.Add(end);
			return newList;
		}
		while(l != start){
			newList.Add(l);
			l = cameFrom[l];
		}
		newList.Reverse();
		return newList;
	}
	private int mDist(Land start, Land end){
		int sX = start.x;
		int sY = start.y;
		int eX = end.x;
		int eY = end.y;
		int deltaX = Mathf.Abs(sX - eX);
		int deltaY = Mathf.Abs(sY - eY);

		return deltaX + deltaY;
	}

	private Land getLowestFScore(Land end, List<Land> list){
		List<Land> lowest = new List<Land>();
		int numLow = 100;
		foreach(Land l in list){
			int fScore = mDist(l, end) + g_Scores[l];
			if(fScore < numLow){
				lowest.Add(l);
				numLow = fScore;
			}
		}
		if(lowest.Count == 1){
			return lowest[0];
		}
		else{
			return lowest[lowest.Count-1];
		}
	}
}