using UnityEngine;
using System.Collections;

public class NodeRay : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		StepByStep.detectTile = false;
		Ray nRay = new Ray (this.transform.position, transform.up);
		RaycastHit hit;
		Physics.Raycast (nRay, out hit, 10000.0f);
		if (hit.collider != null && hit.collider.tag == "Tile") { 
			StepByStep.detectTile = true;
			Debug.DrawLine (nRay.origin, hit.point, Color.red);
		}
	}
}
