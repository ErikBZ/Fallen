using UnityEngine;
using System.Collections;

public class UnitMenu : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// each time a new unit is selected, we will remove all prior listenrs
	// on the button from the menu and then re add listeners from this script
	// that way we do not have to keep making new gameobjects and only have to use one
	// EXP:
	/*
	 * get the buttons
	 * buttons.removeListners
	 * buttons.addListeners(things from this script)
	 * done
	 */


}
