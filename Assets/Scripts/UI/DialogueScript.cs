using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogueScript : MonoBehaviour {

	int index = 1;
	public GameObject dialogBox;
	public GameObject descBox;
	Image img;
	Text txt;
	bool firstFrame = true;

	// Use this for initialization
	void Start () {
		img = dialogBox.GetComponentInChildren<Image>();
		txt = dialogBox.GetComponentInChildren<Text>();
		speech1();
	}
	
	// Update is called once per frame
	// i'll have to create a couple of these with events and such
	void Update () {
		if(firstFrame){
			GameObject thing = GameObject.Find("Player1");
			Player ps = thing.GetComponent<Player>();
			ps.enabled = false;
			GameObject thing2 = GameObject.Find("Player2");
			Player ps2 = thing.GetComponent<Player>();
			ps2.enabled = false;
			firstFrame = false;
		}

		if(Input.GetMouseButtonDown(0) && !(index > 3)){
			clickToNext();
			if(index > 3){
				GameObject thing = GameObject.Find("Player1");
				Player ps = thing.GetComponent<Player>();
				ps.enabled = true;
				GameObject thing2 = GameObject.Find("Player2");
				Player ps2 = thing.GetComponent<Player>();
				ps2.enabled = true;
				dialogBox.SetActive(false);
				descBox.SetActive(true);
			}
			else
				SendMessage("speech"+index);
		}
	}
	
	public void clickToNext(){
		index++;
	}
	public void clickToNext(int x){
		index = x;
		SendMessage("speech"+index);
	}

	private void speech1(){
		txt.text = "Hello there!";
	}
	private void speech2(){
		txt.text = "uhhhhh";
	}
	private void speech3(){
		txt.text = "this is a really shitty way of doing things";
	}
}
