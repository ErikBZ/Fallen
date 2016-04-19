using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {

	public int team = 0;
	public int move = 5;
	private List<Land> moveableLand = new List<Land>();
	private List<Land> attackRange = new List<Land>();
	public Land landInhabiting;
	public bool isMoveDone = false;
	public bool dead = false;
	public Map map;

	public string name;
	public Sprite sprite;
	public string history;

	// stats
	public int level;
	public int HP;
	public int actualHP;
	public int power;
	public int energy;
	public int skill;
	public int speed;
	public int shield;
	public int armor;
	public bool inclusive;
	public int weaponRange;
	public bool isWeaponEnergy;
	public int weaponHit;
	public int weaponDam;

	// growth rates must be less than 100
	public int HPG;
	public int powerG;
	public int energyG;
	public int skillG;
	public int speedG;
	public int shieldG;
	public int armorG;

	private int exp;
	// stats that change often
	private int avoid;
	private int hitrate;
	private int sludgePain = 1;

	private int damageSum = 0;

	private bool thing = true;
	// Use this for initialization
	void Start () {
		actualHP = HP;
		map = GameObject.Find("MAP").GetComponent<Map>();
	}
	
	// Update is called once per frame
	void Update () {
//		if(thing){
//			getNewMoveRange();
//		}
	}

	public override string ToString ()
	{
		string toPrint = "";
		toPrint += "Name: " + name;
		toPrint += "\nLevel: " + level;
		toPrint += "\nHealth: " + actualHP;
		toPrint += "\nPower: " + power;
		toPrint += "\nEnergy: " + energy;
		toPrint += "\nSkill: " + skill;
		toPrint += "\nSpeed: " + speed;
		toPrint += "\nArmor: " + armor;
		toPrint += "\nShield: " + shield;
		return toPrint;
	}


	// getters and setters
	public void setTeam(int num){
		team = num;
	}
	public int getTeam(){
		return team;
	}
	public int getDef(bool isMagi){
		if(isMagi)
			return shield;
		else
			return armor;
	}
	public void setMove(int newMove){
		move = newMove;
	}
	public int getMove(){
		return move;
	}
	public void setMoveableList(List<Land> newList){
		moveableLand = newList;
	}
	public List<Land> getMoveableList(){
		return moveableLand;
	}
	public void setAttackRange(List<Land> newList){
		attackRange = newList;
	}
	public List<Land> getAttackRange(){
		return attackRange;
	}
	public void setLandInhabiting(Land newLand){
		landInhabiting = newLand;
	}
	public Land getLandInhabiting(){
		return landInhabiting;
	}
	public void setIsMoveDone(bool newBool){
		if(newBool){
			if(landInhabiting.getInfected() && team == 1){
				takeHit(sludgePain);
				sludgePain++;
			}
			else{
				int healthGain = level/2 + 3;
				if((actualHP + healthGain > HP)){
					actualHP = HP;
				}
				else{
					actualHP += healthGain;
				}
			}
		}

		isMoveDone = newBool;
	}
	public bool getIsMoveDone(){
		return isMoveDone;
	}
	public int getAvoid(){
		return speed;
	}
	public void updateAvoid(){
		// land hazard will be added when i test it in testing
		// engine advantage adds in when you choose a target and will be calculated in FightingManager
		avoid = speed;
	}
	public int getHitRate(){
		return skill * 2 + weaponHit;
	}
	public void updateHitRate(){
		hitrate = skill * 2 + 70;
	}
	public int getWeaponRange(){
		return weaponRange;
	}
	public bool getWeaponInclusive(){
		return inclusive;
	}
	public int getDamage(){
		if(isWeaponEnergy)
			return energy + weaponDam;		
		else
			return power + weaponDam;
	}
	public bool getIsWeaponEnergy(){
		return isWeaponEnergy;
	}
	public string getName(){
		return name;
	}
	public void addExp(int gained){
		exp += gained;
		if(exp > 100){
			exp -= 100;
			levelUp();
		}
	}

	public void resetDamageTaken(){
		damageSum = 0;
	}
	public int getDamageSum(){
		return damageSum;
	}

	// methods
	public void moveTo(ClickableInterface next){
		if(next == null){
			return;
		}
		Land nextLand = (Land) next;

		if(nextLand == landInhabiting){
			return;
		}

		int x = next.getX();
		int y = next.getY();
		Vector3 nextVec = new Vector3(x, 0.5f, y);

		transform.position = nextVec;

		landInhabiting.setWalkable(true);
		landInhabiting.occupiedEh = false;
		landInhabiting = nextLand;
		landInhabiting.setWalkable(false);
		landInhabiting.occupiedEh = true;
		landInhabiting.unitOnLand = this;

		getNewMoveRange();

		updateAvoid();
	}

	public void levelUp(){
		level++;
		HP += proc (HPG);
		power += proc (powerG);
		energy += proc (energyG);
		skill += proc (skillG);
		speed += proc (skillG);
		shield += proc (shieldG);
		armor += proc (shieldG);
	}

	// returns 1 or 0
	private int proc(float chance){
		int nextNum = Random.Range (1, 101);
		if (chance >= nextNum) {
			return 1;
		}	
		return 0;
	}

	public void takeHit(int damage){
		actualHP -= damage;
		if(actualHP < 0){
			die();
		}
	}
	public int getDamageTaken(int damage, bool isMagic){
		int damageTaken = 0;
		if(isMagic){
			if(damage - shield <= 0)
				damageTaken = 0;
			else{
				damageTaken = damage - shield;
			}
		}
		else{
			if(damage - armor <= 0)
				damageTaken = 0;
			else{
				damageTaken = damage - armor;
			}
		}
		damageSum += damageTaken;
		return damageTaken;
	}

	private void die(){
		landInhabiting.setWalkable(true);
		landInhabiting.occupiedEh = false;
		transform.position = new Vector3(-10, 0, -10);
		dead = true;
	}
	public void getNewMoveRange(){
		moveableLand.Clear();
		map.getMoveList(this);
		//map.findMovableLands(this, landInhabiting, move);
	}

}
