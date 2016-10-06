using UnityEngine;
using System.Collections;

public class NodeHandler : MonoBehaviour {
	private Vector3 initPos;
	private Vector3 velocity = Vector3.zero;

	void Start () {
		initPos = this.transform.position + new Vector3 (18.0f, 5.0f, 15.0f);
	}
	
	// Update is called once per frame
	void Update () {
		if (!StepByStep.gameStart) {
			this.transform.position = Vector3.SmoothDamp (this.transform.position, initPos, ref velocity, 0.5f);
		}
	}
}
