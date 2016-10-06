using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHandler : MonoBehaviour {
	private Vector3 initPos;
	private GameObject playBtn;

	private void Start () {
		playBtn = GameObject.Find ("PlayButton");
		initPos = this.transform.position;
		this.transform.position = initPos + Vector3.up * 35.0f;
	}

	private void Update () {
		if (!StepByStep.gameStart) {
			this.transform.position = Vector3.MoveTowards (this.transform.position, initPos, 20.0f * Time.deltaTime);
			if ((this.transform.position - initPos).magnitude < 0.1f) {
				this.transform.position = initPos;
				playBtn.gameObject.GetComponent<Image> ().enabled = true;
				playBtn.gameObject.GetComponentInChildren<Text> ().enabled = true;
			}
		}
	}

	private void OnCollisionExit (Collision col) {
		col.gameObject.AddComponent<Rigidbody> ();
	}

	private void OnTriggerEnter (Collider col) {
		Destroy (col.gameObject);
		Debug.Log (col.tag);
		if (col.gameObject.tag.Equals ("Dimand")) {
			StepByStep.dimandCount++;
			Debug.Log ("Dimand :" + StepByStep.dimandCount);
		} else if (col.gameObject.tag.Equals ("Boost")) {
			StepByStep.isBoost = true;
		}
	}
}
