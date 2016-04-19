using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Selector : MonoBehaviour {

	public Map theMap;

	private ClickableInterface lastClicked;
	private Unit lastUnit;

	private List<Land> moveableLand = new List<Land>();

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
//		if(Input.GetKeyDown(KeyCode.A)){
//			testRec();
//		}
//		if(Input.GetMouseButtonDown(0)){
//			select();
//		}
//		if(Input.GetMouseButtonDown(1))
//			deselect();
	}

	// testing the recursive out
	public void findLand(Unit u){
		int landX = lastClicked.getX();
		int landY = lastClicked.getY();
		List<Land> moveableLand = new List<Land>();
		theMap.findMovableLands(moveableLand, theMap.getLandAt(landX, landY), u.getMove());
		u.setMoveableList(moveableLand);
	}
	public ClickableInterface getLastClicked(){
		return lastClicked;
	}
	private void setLastClicked(ClickableInterface newClicked){
		lastClicked = newClicked;
	}

	public ClickableInterface select(bool toClick){
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		ClickableInterface clickable;

		if(Physics.Raycast(ray, out hit)){
			try{
				// unhightlighting and clearing the moveable land list
				deselect(moveableLand);

				clickable = hit.transform.gameObject.GetComponent<Land>() as ClickableInterface;

				// if you do not want to actually click ie, just a hover
				if(!toClick){
					return clickable;
				}
				else if(clickable != null && lastClicked != null){
					// this forgets the last one clicked and creates
					// sets it to a new one
					lastClicked.unClicked();
					lastClicked = clickable;
					lastClicked.clicked();
					return lastClicked;
				}
				else if(clickable != null && lastClicked == null){
					lastClicked = clickable;
					lastClicked.clicked();
					return lastClicked;
				}

				return null;
			}
			catch{
				Debug.Log("Something went wrong");
			}
		}
		return null;
	}

	// moved this over to player
//	private void setUpUnit(){
//		if(lastClicked.occupied()){
//			lastUnit = lastClicked.getUnit();
//			findLand(lastUnit);
//
//			foreach(Land l in moveableLand){
//				l.highlight(true);
//			}
//		}
//	}


	// i'm an idiot i can't use deselect outside of Selector
	public bool deselect(){
		lastClicked.clicked();
		lastClicked.unClicked();
		return true;
	}
	public bool deselect(ClickableInterface obj){
		if(obj != null){
			obj.unClicked();
		}
		// i shouldn't need this here anymore
//		if(lastUnit.getMoveableList() != null){
//			deselect(lastUnit.getMoveableList());;
//		}

		// nulling it in a seperate class does not work
		obj = null;
		return true;
	}
	public bool deselect(List<Land> l){
		if(l != null){
			foreach(Land x in l){
				x.highlight(false);
//				x.clicked();
				x.unClicked();
			}
		}
		// setting the list to null so that i can create a new one for a different unit
		return true;
	}
	public bool deselect(Unit thing){
		return true;
	}
}
