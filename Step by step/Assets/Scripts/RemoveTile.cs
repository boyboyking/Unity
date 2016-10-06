using UnityEngine;
using System.Collections;

public class RemoveTile : MonoBehaviour {

	private void OnCollisionEnter (Collision col) {
		Destroy (col.gameObject);
	}
}
