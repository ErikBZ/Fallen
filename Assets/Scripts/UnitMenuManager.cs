using UnityEngine;
using System.Collections;

public class UnitMenuManager : MonoBehaviour {

	public GameObject unitMenu;
	bool menuIsOn = false;

	// Use this for initialization
	void Start () {
//		unitMenu = GameObject.Find ("UnitMovementMenu");
	
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0) || menuIsOn){
			unitMenu.SetActive(true);
			snapToThisGameObject();
		}

		if(Input.GetMouseButton(1)){
			menuIsOn = false;
			unitMenu.SetActive(false);
		}
	}

	public void snapToThisGameObject(){
		float x = transform.position.x + 1f;
		float y = transform.position.y;
		float z = transform.position.z - 1f;
		Vector3 pos = new Vector3(x,y,z);
		Vector3 uiNextPos = Camera.main.WorldToViewportPoint(pos);
		RectTransform uiTrans = unitMenu.GetComponent<RectTransform>();

		uiTrans.anchorMin = uiNextPos;
		uiTrans.anchorMax = uiNextPos;
		menuIsOn = true;
	}
}
