using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

	public Map map;
	public Selector thisSelector;
	public Color playerColor;
	public Team teamManager;
	public int team;
	public Text textObj;
	public Text unitInfo;
	public Text playerTurn;
	public FightingManager fManager;
	public Player otherPlayer;


	private Unit unitOnLand; 
	// will show what the play can do and dictates the game state
	// turn will be a bool that allows more actions to be take

	private ClickableInterface landClicked;
	private ClickableInterface nextLand;
	private Land landToAttack;
	public bool myTurn = true;

	protected int phase = 0;

	// lerp things
	protected List<Land> pathList = new List<Land>();
	protected int phaseNum = 0;
	protected int index = 0;
	protected float unitSpeed = 1.0f;
	protected Vector3 endMarker;
	protected float startTime;
	protected float journeyLength;
	protected Land nextStep;

	private int turn = 0;
	// gettters and setters
	public bool getIsTurn(){
		return myTurn;
	}
	public virtual void setIsTurn(bool newBool){

		if(newBool){
			turn++;
			teamManager.newTurn();
			playerTurn.text = "Player " + team + " go";
		}
		myTurn = newBool;
	}
	public void newTurn(){
		turn++;
		teamManager.newTurn();
		playerTurn.text = "Player Turn";
		myTurn = true;
	}

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(teamManager.isTeamDone() && myTurn){
			myTurn = false;
			otherPlayer.setIsTurn(true);
		}

		if(myTurn){
			ClickableInterface hoverLand = thisSelector.select(false);
			if(hoverLand != null){
				textObj.text = hoverLand.getDescription();

				if(hoverLand.occupied()){
					Unit thing = hoverLand.getUnit();
					unitInfo.text = thing.ToString();
				}
			}

			switch(phase){
			case 0:
				phase0 ();
				break;
			case 1:
				// don't forget the breaks

				if(!unitOnLand.getIsMoveDone())
					showUnitRange();
			
				Land posNext = (Land)thisSelector.select(false);

				if(Input.GetMouseButtonDown(1)){
					highlightLandList(Color.white, unitOnLand.getMoveableList());
					thisSelector.deselect(landClicked);
					landClicked = null;
					phase = 0;
				}
				else if(Input.GetMouseButtonDown(0)){
					if(unitOnLand.getMoveableList().Contains(posNext)
					   && (posNext.getWalkable() || posNext == unitOnLand.getLandInhabiting())){
						nextLand = (Land) thisSelector.select(true);

						// we are now going to lerp
						pathList = map.getNextMove(unitOnLand, posNext);
						highlightLandList(Color.white, unitOnLand.getMoveableList());

						// will link to phase 2
						phase = 8;
					}
				}
				break;
			case 2:
				showUnitAttackRange();
				if(hoverLand != null){
					if(hoverLand.getUnit() != null){
						if(hoverLand.getUnit().getTeam() != unitOnLand.getTeam() &&
						   unitOnLand.getAttackRange().Contains((Land)hoverLand)){
							fManager.showFightStats(unitOnLand, hoverLand.getUnit(), unitOnLand.getLandInhabiting().getDistFrom((Land)hoverLand));
						}
					}
				}
				else{
					fManager.showNothing();
				}

				if(Input.GetMouseButtonDown(0)){
					if((unitOnLand.getAttackRange().Contains((Land)thisSelector.select (false)) ||
					   unitOnLand.getLandInhabiting() == (Land)thisSelector.select (false)) &&
					   (thisSelector.select(false).getUnit() != null ||
					    thisSelector.select(false).getInfected()))
					   landToAttack = (Land)thisSelector.select(true);
					else
						return;

					if(landToAttack == unitOnLand.getLandInhabiting()){
						highlightLandList(Color.white, unitOnLand.getAttackRange());
						phase = 4;
					}
					else if(landToAttack.getInfected()){
						Sludge sludge = landToAttack.getSludge();
						sludge.takeHit();
						highlightLandList(Color.white, unitOnLand.getAttackRange());
						phase = 4;
					}
					else if(landToAttack.getUnit() == null){
						// do nothing!
					}
					else if(landToAttack.getUnit().getTeam() != unitOnLand.getTeam()){
						phase = 6;
						highlightLandList(Color.white, unitOnLand.getAttackRange());
					}
				}
				else if(Input.GetMouseButtonDown(1)){
					thisSelector.deselect(unitOnLand.getAttackRange());
					fManager.showNothing();
					unitOnLand.moveTo(landClicked);
					nextLand = null;
					phase = 1;
				}
				break;
			case 4:
				unitOnLand.setIsMoveDone(true);
				unitOnLand.getNewMoveRange();
				reset(false);
				phase = 0;
				break;
			case 5:
				phase = 4;
				break;
			case 6:
				fManager.fight(unitOnLand, landToAttack.getUnit(),
				               unitOnLand.getLandInhabiting().getDistFrom(landToAttack));

				fManager.showNothing();
				phase = 4;
				break;
			case 7:
				showUnitRange();
			
				if(Input.GetMouseButtonDown(1)){
					highlightLandList(Color.white, unitOnLand.getMoveableList());
					thisSelector.deselect(unitOnLand);
					phase = 0;
				}
				break;
			case 8:
				lerpMove(unitOnLand);
				break;
			}
		}
	}

	private void lerpMove(Unit unit){
		switch(phaseNum){
		case(0):
			if(index >= pathList.Count)
				phaseNum = 2;
			else{
				Land l = pathList[index];
				endMarker = l.transform.position;
				endMarker = new Vector3(endMarker.x, 0.5f, endMarker.z);
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
			phase = 2;
			unit.moveTo(nextLand);
			break;
		}
	}

	private void phase0(){
		if(Input.GetMouseButtonDown(0)){
			landClicked = thisSelector.select(true);
			if(landClicked == null)
				return;

			if(landClicked.occupied()){
				Land whyDidIDoThis = (Land)landClicked;
				unitOnLand = whyDidIDoThis.getUnit();

				if(unitOnLand.getTeam() != team){
					unitOnLand.getNewMoveRange();
					phase = 7;
				}
				else if(!unitOnLand.getIsMoveDone()){
					unitOnLand.getNewMoveRange();
					phase = 1;
				}
			}
		}
		else if(Input.GetMouseButtonDown(1)){
			thisSelector.deselect(landClicked);
			if(unitOnLand != null)
				highlightLandList(Color.white, unitOnLand.getMoveableList());
			landClicked = null;
		}
	}

	private void reset(bool isSoftReset){
		landClicked = null;
		nextLand = null;
		thisSelector.deselect(unitOnLand);
		unitOnLand = null;
	}

	private void moveUnit(ClickableInterface next, Unit u){
		if(nextLand != null && u != null){
			u.moveTo(next);
		}
	}

	private void showUnitRange(){
		if(landClicked == null || !landClicked.occupied()){
			return;
		}
		if(unitOnLand.team != team)
			highlightLandList(Color.red, unitOnLand.getMoveableList());
		else
			highlightLandList(Color.blue,unitOnLand.getMoveableList());
	}

	private void showUnitAttackRange(){
		//setUnitAttack(unitOnLand.getLandInhabiting());
		setUnitAttack(unitOnLand);
		highlightLandList(Color.red, unitOnLand.getAttackRange());
	}

	protected void highlightLandList(Color c, List<Land> list){
		foreach(Land l in list){
			if(l == null){
				Debug.Log("there is no land in this list");
			}
			else{
				l.highlight(c);
			}
		}
	}

	private void setUnitAttack(Land l){
		List<Land> attackRange = new List<Land>();
		map.findMovableLands(attackRange, l, unitOnLand.getWeaponRange());
		unitOnLand.setAttackRange(attackRange);
	}

	private void setUnitMoveable(ClickableInterface l){
		map.findMovableLands(unitOnLand, (Land)l, unitOnLand.getMove());
	}

	private void setUnitAttack(Unit u){
		List<Land> atkRange = new List<Land>();
		if(u.getWeaponInclusive()){
			map.findMovableLands(atkRange, u.getLandInhabiting(), u.getWeaponRange());
		}
		else{
			List<Land> range1 = new List<Land>();
			map.findMovableLands(range1, u.getLandInhabiting(), 1);
			map.findMovableLands(atkRange, u.getLandInhabiting(), u.getWeaponRange());
			for(int i=0; i<range1.Count; i++){
				for(int j=0; j<atkRange.Count; j++){
					if(range1[i]==atkRange[j]){
						atkRange.Remove(range1[i]);
					}
				}
			}
		}

		u.setAttackRange(atkRange);
	}
	void OnLevelWasLoaded(int level){
		if(level == 1 && !(team == 2) && !(team == 3)){
			GameObject go = GameObject.Find("PlayerLevelLoader"+team);
			PlayerLevelLoader pll = go.GetComponent<PlayerLevelLoader>();
			pll.setMeUp(this);
			teamManager.gameObject.SetActive(true);
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
}
