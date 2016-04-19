using UnityEngine;
using System.Collections;

public class CameraBehavior : MonoBehaviour {

	
	public float scrollArea;
	public float speed;
	public Transform anchorLeftDown;
	public Transform anchorUpRight;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float mouseX = Input.mousePosition.x;
		float mouseY = Input.mousePosition.y;

		if(mouseX < scrollArea && anchorLeftDown.position.x < transform.position.x){
			transform.Translate(Vector3.left * Time.deltaTime * moveSpeed(mouseX, scrollArea)); 
		}
		if(mouseX >= Screen.width - scrollArea && anchorUpRight.position.x > transform.position.x){
			transform.Translate(Vector3.right * Time.deltaTime * moveSpeed(mouseX, Screen.width - scrollArea)); 
		}
		if(mouseY < scrollArea && anchorLeftDown.position.z < transform.position.z){
			transform.Translate(Vector3.down * Time.deltaTime * moveSpeed(mouseY, scrollArea)); 
		}
		if(mouseY >= Screen.height - scrollArea && anchorUpRight.position.z > transform.position.z){
			transform.Translate(Vector3.up * Time.deltaTime * moveSpeed(mouseY, Screen.height - scrollArea)); 
		}
	}

	private float moveSpeed(float mouseY, float thing){
		float actSpeed = speed * (1);
		return actSpeed;
	}
		                    
		                   
}
