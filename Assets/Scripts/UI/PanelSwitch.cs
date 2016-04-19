using UnityEngine;
using System.Collections;

public class PanelSwitch : MonoBehaviour {

	static public PanelSwitch currentPanel;
	static bool alreadyStarted = false;

	// Use this for initialization
	void Start () {
		if(!alreadyStarted){
			currentPanel = this;
			alreadyStarted = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void switchPanels(){
		if(currentPanel == this){
			return;
		}
		// disabling old script
		GameObject currentGO = currentPanel.gameObject;
		currentGO.SetActive(false);

		// enabling new one
		currentPanel = this;
		gameObject.SetActive(true);
	}
}
