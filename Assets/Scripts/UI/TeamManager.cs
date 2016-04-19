using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TeamManager : MonoBehaviour {

	public GameObject unitButton;
	public GameObject[] panels;
	public Team[] teams;
	public List<Unit> unitsToBeUsed;
	public LevelManager lManager;
	public GameObject teamPanel;
	public int maxUnits;
	public int level;

	private Image[] imgs;
	private bool[] isUnitChosen;
	private int count = 0;
	private Team team;
	private GameObject aPanel;
	private int currTeam = 0;

	// Use this for initialization
	void Start () {
		imgs = new Image[maxUnits];
		for(int i = 0; i<maxUnits; i++){
			GameObject go = new GameObject();
			go.transform.SetParent(teamPanel.transform);
			go.AddComponent<Image>();
			imgs[i] = go.GetComponent<Image>();
		}

		updatePanelAndTeam();
		setUplPanel();
	}
	
	// Update is called once per frame
	void Update () {
	}

	private void updateTeamPanel(){
		int num = 0;
		for(int i=0; i<imgs.Length; i++){
			imgs[i].sprite = null;
		}
		foreach(Unit u in unitsToBeUsed){
			SpriteRenderer thing = u.gameObject.GetComponent<SpriteRenderer>();
			imgs[num].sprite = thing.sprite;
			num++;
		}
	}

	private void disableAllPanels(){
		for(int i=0; i<panels.Length; i++){
			panels[i].SetActive(false);
		}
	}
	private void updatePanelAndTeam(){
		disableAllPanels();
		team = teams[currTeam];
		aPanel = panels[currTeam];
		aPanel.SetActive(true);
	}
	private void setUplPanel(){
		isUnitChosen = new bool[team.team.Count];
		for(int i=0; i< team.team.Count; i++){
			makeButton(i);
			isUnitChosen[i] = false;
		}
	}
	private void makeButton(int num){
		GameObject newGO;
		newGO = (GameObject)Instantiate(unitButton);
		newGO.transform.SetParent(aPanel.transform);
		UnitSelectButton button = newGO.GetComponent<UnitSelectButton>();
		button.setButton(team.team[num], num);
		Button b = newGO.GetComponent<Button>();
		b.onClick.AddListener(() => putUnitInTeam(team.team[num], button));
	}
	public void putUnitInTeam(Unit u, UnitSelectButton b){
		if(!b.getClicked() && !unitsToBeUsed.Contains(u) && count < maxUnits){
			unitsToBeUsed.Add(u);
			b.setClicked(true);
			count++;
		}
		else if(count > 0 && b.getClicked() && unitsToBeUsed.Contains(u)){
			unitsToBeUsed.Remove(u);
			b.setClicked(false);
			count--;
		}

		updateTeamPanel();

		b.alternateButtonImage();
	}
	public void doneButton(){
		if(currTeam < teams.Length){
			teams[currTeam].setUsableTeam(unitsToBeUsed);
			unitsToBeUsed = new List<Unit>();
			count = 0;
			currTeam++;
		}
		if(currTeam >= teams.Length){
			lManager.setEnabled();
			Application.LoadLevel(level);
		}
		else{
			updatePanelAndTeam();
			setUplPanel();
		}
		updateTeamPanel();
	}
}
