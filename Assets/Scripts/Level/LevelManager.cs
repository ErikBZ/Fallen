using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public GameObject[] dontDestroyArray; 

	// Use this for initialization
	void Start () {
		foreach(GameObject g in dontDestroyArray){
			DontDestroyOnLoad(g);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void setEnabled(){
		foreach(GameObject go in dontDestroyArray){
			go.SetActive(true);
		}
	}

}
