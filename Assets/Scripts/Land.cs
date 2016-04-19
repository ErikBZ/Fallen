using UnityEngine;
using System.Collections;

public class Land : MonoBehaviour, ClickableInterface{
	
	private Vector2 posOnMap = new Vector2();		// position on the map for this land
	public bool clickedOn = false;				// checks if the mouse is over this land]
	public bool walkable = true;
	public bool occupiedEh = false;
	public LandRender landRend;
	public Unit unitOnLand;

	private bool isInfected = false;
	private Sludge sludge;

	bool thing = true;
	

	public int terrain = 0; // -1 if it is a hazards terrain, 0 if it's neutral
							 // 1 if it helps a unit move
	public int x = 0;
	public int y = 0;


    // i'm writing something random
	void Awake(){
		posOnMap.x = (int)transform.position.x;
		posOnMap.y = (int)transform.position.z;

		x = (int)transform.position.x;
		y = (int)transform.position.z;

		GetComponent<Collider>().isTrigger = true;
		unitOnLand = null;
		landRend = gameObject.GetComponent<LandRender>();
	}
	void Start(){
	}
	// Use this for initialization
//	void Start () {
//		posOnMap[0] = (int)transform.position.x;
//		posOnMap[1] = (int)transform.position.y;
//	}
	
	// Update is called once per frame
	void Update () {
//		if(thing){
//			if(!walkable)
//				highlight(Color.yellow);
//			thing = false;	
//		}
	}
	public Sludge getSludge(){
		return sludge;
	}
	public void setSludge(Sludge sl){
		sludge = sl;
	}
	public bool getInfected(){
		return isInfected;
	}
	public void setInfected(bool infect){
		isInfected = infect;
	}
	public int getTerrain(){
		return terrain;
	}
	public void setTerrain(int newT){
		terrain = newT;
	}
	public bool getWalkable(){
		return walkable;
	}
	public void setWalkable(bool newBool){
		walkable = newBool;
		if(walkable){
			unitOnLand = null;
		}
	}
	// Clickable Interface
	public void clicked(){
		clickedOn = true;
		highlight();
	}
	public void unClicked(){
		clickedOn = false;
		highlight();
	}
	public void highlight(){
		if(clickedOn)
			landRend.highlight(Color.blue);
		else
			landRend.highlight(Color.white);
	}
	public void highlight(bool boolean){
		if(boolean)
			landRend.highlight(Color.yellow);
		else
			highlight();
	}
	public void highlight(Color colour){
		landRend.highlight(colour);
	}
	public GameObject getGO(){
		return gameObject;
	}
	public int getX(){
		return x;
	}
	public int getY(){
		return y;
	}
	public string getDescription(){
		return ToString();
	}
	public Unit getUnit(){
		return unitOnLand;
	}
	public bool occupied(){
		return occupiedEh;
	}
	public int getDistFrom(Land other){
		int x = this.getX() - other.getX();
		int y = this.getY() - other.getY();
		return Mathf.Abs (x) + Mathf.Abs(y);
	}
	public void startOnThisLand(Unit unit){
		unitOnLand = unit;
		unitOnLand.setLandInhabiting(this);
		walkable = false;
		occupiedEh = true;
	}

	// methods
	public override string ToString(){
		return this.x + "," + this.y;
	}
	// i may have some trouble with this later so i'm just
	// noting this here. Land might become fasley unwalkable beacuse
	// of this section of code
	void OnTriggerEnter(Collider col){
//		try{
//			unitOnLand = col.GetComponent<Unit>();
//			unitOnLand.setLandInhabiting(this);
//			walkable = false;
//			occupiedEh = true;
//		}
//		catch{
//			Debug.Log("no unit on land");
//		}
	}
	void OnTriggerLeave(){
		walkable = true;
		occupiedEh = false;
	}
}