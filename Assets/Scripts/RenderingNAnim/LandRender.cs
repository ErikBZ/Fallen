using UnityEngine;
using System.Collections;

public class LandRender : MonoBehaviour {

	public Material[] materials;
	public int deltaT;
	public Renderer rend;
	public Color c;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer>();
		rend.enabled = true;
		rend.material = materials[0];
	}
	
	// Update is called once per frame
	void Update () {
	}
	public void highlight(Color c){
		if(c.Equals(Color.white)){
			rend.material = materials[0];
		}
		else{
			rend.material = materials[1];
			rend.material.SetColor("_EmissionColor", c);
		}
	}
}
