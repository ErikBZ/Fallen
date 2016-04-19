using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UnitSelectButton : MonoBehaviour {

	private Unit unit;
	public Sprite normal;
	public Sprite alternate;
	public bool clickedOn;
	public int unitInt;
	public Image image;
	public Button button;
	public Text t;

	void Awake(){
		t = gameObject.GetComponentInChildren<Text>();
		button = gameObject.GetComponent<Button>();

	}

	// Use this for initialization
	void Start () {
		button.image.sprite = normal;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// getters and setters
	public void setButton(Unit newU, int newInt){
		unitInt = newInt;
		setUnit(newU);
		setImage();
		setText();
	}
	private void setImage(){
		image.sprite = unit.sprite;
	}
	private void setText(){
		t.text = unit.name;
	}
	private void setUnit(Unit newUnit){
		unit = newUnit;
	}
	public Unit getUnit(){
		return unit;
	}
	public bool getClicked(){
		return clickedOn;
	}
	public void setClicked(bool click){
		clickedOn = click;
	}
	public void alternateButtonImage(){
		if(clickedOn){
			button.image.sprite = alternate;
		}
		else{
			button.image.sprite = normal;
		}
	}
	public void sayHello(){
		GameObject go = GameObject.Find("InfoPanel");
		Image img = go.GetComponentInChildren<Image>();
		Text txt = go.GetComponentInChildren<Text>();
		InfoPanel ip = go.GetComponent<InfoPanel>();
		txt = ip.txt;
		Text history = ip.history;


		img.sprite = unit.sprite;
		txt.text = unit.ToString();
		history.text = unit.history;
	}
}
