using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class FightingManager : MonoBehaviour {

	public Text text1;	// attacker

	public Player playerOne;
	public Player enemyPlayer;

	// units fighting
	Unit u1;
	Unit u2;
	public Text u1Info;
	public Text u2Info;

	public GameObject mainPanel;
	public Image attackerImg;
	public Image defenderImg;
	public Text attackerName;
	public Text defenderName;
	public Text attackerStats;
	public Text defenderStats;
	public Text atackerDamTaken;
	public Text defenderDamTaken;
	private Dictionary<Unit, Text> unitToText = new Dictionary<Unit, Text>();
	private Dictionary<Unit, Text> unitToStats = new Dictionary<Unit, Text>();
	private Dictionary<Unit, Unit> uToOther = new Dictionary<Unit, Unit>();

	private int u1Avoid;
	private int u1HitRate;
	private int u2Avoid;
	private int u2HitRate;
	bool areUnitFighting = false;

	List<int> damageQueue = new List<int>();
	List<Unit> unitQueue = new List<Unit>();
	float trigTime = 0;
	float interval = 1f;
	int count = 0;
	int expGained = 0;

	Unit u;
	int dam;

	// Use this for initialization
	void Start () {
		trigTime = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if(areUnitFighting){
			if(trigTime <= 0){
				if(count > unitQueue.Count - 1){
					areUnitFighting = false;
					count = 0;
					damageQueue.Clear();
					unitQueue.Clear();
					playerOne.enabled = true;
					enemyPlayer.enabled = true;
					mainPanel.SetActive(false);
					defenderDamTaken.text = "";
					atackerDamTaken.text = "";
					u = null;
					dam = 0;
				}
				else{
					u = unitQueue[count];
					dam = damageQueue[count];
					u.takeHit(dam);
					Unit ou = uToOther[u];

					unitToStats[u].text = "Health: " + u.actualHP;
					unitToStats[u].text += "\nHit: " + hitRate(u, ou);
					unitToStats[u].text += "\nDamage: " + ou.getDamageTaken(u.getDamage(), u.getIsWeaponEnergy());
					unitToStats[ou].text = "Health: " + ou.actualHP;
					unitToStats[ou].text += "\nHit: " + hitRate(ou, u);
					unitToStats[ou].text += "\nDamage: " + u.getDamageTaken(ou.getDamage(), ou.getIsWeaponEnergy());

					trigTime = interval;
					count++;
				}
			}
			if(trigTime > 0){
				trigTime -= Time.deltaTime;

				if(!(trigTime < interval/4)){
					// add the panel here
					if(dam > 0)
						unitToText[u].text = dam+"!";
					else if(dam == 0)
						unitToText[u].text = "No Damage!";
					else if(dam < 0)
						unitToText[u].text = "Dodge!";
				}
				else
					unitToText[u].text = "";
			}
		}
	}

	// advantage set up
	// assume u1 is the attacking unit
	private void setUpUnits(){
		u1Avoid = u1.getAvoid () + rpsEngines (u1, u2);
		u2Avoid = u2.getAvoid () + rpsEngines (u2, u1);

		u1HitRate = u1.getHitRate () + rpsChassis () - u2Avoid;
		if(u1HitRate > 100){
			u1HitRate = 100;
		}
		u2HitRate = u2.getHitRate () + rpsChassis () - u1Avoid;
		if(u2HitRate > 100){
			u2HitRate = 100;
		}
	}

	private int hitRate(Unit atk, Unit def){
		int x = atk.getHitRate() - def.getAvoid();
		if(x > 100){
			return 100;
		}
		return x;
	}

	private int rpsEngines(Unit unit1, Unit unit2){
		return 0;
	}
	private int rpsChassis(){
		return 0;
	}

	public void setUnit1(Unit un){
		u1 = un;
	}
	public void setUnit2(Unit un){
		u2 = un;
	}

	// assume u1 is attacker
	public void fight(Unit unit1, Unit unit2, int dist){
		setUnit1(unit1);
		setUnit2(unit2);

		unitToText.Clear();
		unitToStats.Clear();
		uToOther.Clear();

		unit1.resetDamageTaken();
		unit2.resetDamageTaken();

		expGained = 0;
		if((dist > unit2.getWeaponRange()) ||					// atk range is past u2's atk range
		   (dist == 1 && !unit2.getWeaponInclusive())){	// atk is at close comabt and u2 can't fight CC
			fightTypeOne(unit1, unit2);
		}
		else{
			fightTypeTwo(unit1, unit2);
		}
		if(unit1.speed > unit2.speed+4 && unit1.getDamageSum() < unit1.actualHP){
			fightTypeOne(unit1, unit2);
		}
		else if(unit2.speed > unit1.speed+4 && unit2.getDamageSum() < unit2.actualHP){
			fightTypeOne(unit2, unit1);
		}
		if(unit2.dead && unit2.team != 1){
			expGained += getExp(unit1, unit2);
		}

		if(unit1.team == 1)
			unit1.addExp(expGained);
		else if (unit2.team ==1)
			unit2.addExp(expGained);

		areUnitFighting = true;
		mainPanel.SetActive(true);
		playerOne.enabled = false;
		enemyPlayer.enabled = false;

		attackerImg.sprite = u1.GetComponent<SpriteRenderer>().sprite;
		defenderImg.sprite = u2.GetComponent<SpriteRenderer>().sprite;
		attackerName.text = u1.getName();
		defenderName.text = u2.getName();
		attackerStats.text = "Health: " + u1.actualHP;
		attackerStats.text += "\nHit: " + hitRate(u1, u2);
		attackerStats.text += "\nDamage: " + u2.getDamageTaken(u1.getDamage(), u1.getIsWeaponEnergy());
		defenderStats.text = "Health: " + u2.actualHP;
		defenderStats.text += "\nHit: " + hitRate(u2, u1);
		defenderStats.text += "\nDamage: " + u1.getDamageTaken(u2.getDamage(), u2.getIsWeaponEnergy());
		unitToText.Add(u1, atackerDamTaken);
		unitToText.Add(u2, defenderDamTaken);
		unitToStats.Add(u1, attackerStats);
		unitToStats.Add(u2, defenderStats);
		uToOther.Add(unit1, unit2);
		uToOther.Add(unit2, unit1);
	}

	private int getExp(Unit u1, Unit u2){
		int x = u1.level;
		int y = u2.level;
		int e = 30 + (y - x)*10;
		if(e < 20){
			e = 20;
		}
		return e;
	}

	// this occurs when the range on u1 is greater
	// or lesser than the attack range on u2
	private void fightTypeOne(Unit attacker, Unit defender){
		int atkHit = hitRate(attacker, defender);

		int hit = Random.Range (1, 101);
		int damage = 0;
		if (hit <= atkHit) {
			damage = defender.getDamageTaken(attacker.getDamage(), attacker.getIsWeaponEnergy());
			damageQueue.Add(damage);
			unitQueue.Add(defender);
			if(attacker.team == 1){
				expGained += getExp(attacker, defender);
			}
		}
		else{
			damageQueue.Add(-1);
			unitQueue.Add(defender);
		}
	}

	// standerd fight where u1 and u2 are in each others ranges
	private void fightTypeTwo(Unit attacker, Unit defender){
//		int atkHit = hitRate(attacker, defender);
//		int defHit = hitRate(defender, attacker);
//
//		int u1Hit = Random.Range (1, 101);
//		int u2Hit = Random.Range (1, 101);
//
//		if (u1Hit <= atkHit) {
//			defender.takeHit(attacker.getDamage(), attacker.getIsWeaponEnergy());
//		}
//		if (u2Hit <= defHit && !defender.dead) {
//			attacker.takeHit(defender.getDamage(), defender.getIsWeaponEnergy());
//		}
		fightTypeOne(attacker, defender);

		if(!(defender.getDamageSum() >= defender.actualHP))
			fightTypeOne(defender, attacker);
	}

	public void showFightStats(Unit newU1, Unit newU2, int dist){
		if(dist > 3){
			showNothing();
		}
		if(newU1 == null || newU2 == null){
			showNothing();
			return;
		}

		string u1Text = "";
		string u2Text = "";

		u1Text += newU1.getName();
		u1Text += "\nHit: " + hitRate(newU1, newU2);
		u1Text += "\nDmg: " + newU2.getDamageTaken(newU1.getDamage(), newU1.getIsWeaponEnergy());
		if(newU1.speed > newU2.speed + 4)
			u1Text += " X2";

		if(!((dist > newU2.getWeaponRange()) ||					// atk range is past u2's atk range
		   (dist == 1 && !newU2.getWeaponInclusive()))){
			u2Text += "Enemy";
			u2Text += "\nHit: " + hitRate(newU2, newU1);
			u2Text += "\nDmg: " + newU1.getDamageTaken(newU2.getDamage(), newU2.getIsWeaponEnergy());
			if(newU2.speed > newU1.speed + 4)
				u2Text += " X2";
		}

		u1Info.text = u1Text;
		u2Info.text = u2Text;
	}
	public void showNothing(){
		if(u1 == null || u2 == null){
			u1Info.text = "";
			u2Info.text = "";
			return;
		}

		string t = "";
		u1Info.text = t;
		u2Info.text = t;

		u1 = null;
		u2 = null;
	}
	private int mDist(Land l1, Land l2){
		int deltaX = Mathf.Abs (l1.x - l2.x);
		int deltaY = Mathf.Abs(l1.y - l2.y);
		return	deltaX + deltaY;
	}
}
	