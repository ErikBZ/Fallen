using UnityEngine;
using System.Collections;
using UnityEngine.UI;

// this class will give the a player that pops in all the GOs it needs to function
public class PlayerLevelLoader : MonoBehaviour {

	public Player otherPlayer;
	public BlackOil oil;
	public int team;
	public Map map;
	public Selector select;
	public Text t;
	public Text ut;
	public Text pt;
	public FightingManager fManager;
	public bool turn;
	public GameOver gameover;

	private bool lateStart = true;
	// Use this for initialization
	void Awake() {
	}
	void Start(){
	}

	// Update is called once per frame
	void Update () {
	}

	void OnLevelWasLoaded(int level){
	}
	public void setMeUp(Player playerScript){

		playerScript.otherPlayer = otherPlayer;

		AIPlayer thing = (AIPlayer)otherPlayer;
		thing.actualPlayer = playerScript;
		playerScript.map  = map;
		playerScript.thisSelector = select;
		playerScript.textObj = t;
		playerScript.unitInfo = ut;
		playerScript.playerTurn = pt;
		playerScript.fManager = fManager;
		oil.otherPlayer = playerScript;
		// make sure that the players have different team ints
		// since they're prefabs they'll occaisionally become the same
		// int
		playerScript.setIsTurn(turn);
		gameover.realPlayer = playerScript;

		playerScript.gameObject.SetActive(true);
	}
}
