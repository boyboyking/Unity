using UnityEngine;
using System.Collections;

public class TileMove : MonoBehaviour {
	private GameObject player;
	private Vector3 dir;
	private float speed;
	private float maxSpeed;
	private float minSpeed;

	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		dir = this.transform.position;
		maxSpeed = StepByStep.maxTileMoveSpeed;
		minSpeed = StepByStep.minTileMoveSpeed;
		this.transform.position += Vector3.up * 30.0f;
		speed = Random.Range (minSpeed, maxSpeed);
	}

	void Update () {
		if (!this.gameObject.GetComponent<Rigidbody> ()) {
			this.transform.position = Vector3.MoveTowards (this.transform.position, dir, speed * Time.deltaTime);
			fallenDown ();
		} 

		if ((player.transform.position.y - this.gameObject.transform.position.y) > 30.0f && !StepByStep.gameOver) {
			Destroy (this.gameObject);
		}
	}

	private void fallenDown () {
		Vector3 playerPos = player.transform.position;
		Vector3 tilePos = this.transform.position;
		playerPos.y = 0.0f;
		tilePos.y = 0.0f;

		Vector3 vec = tilePos - playerPos;
		Vector3 playerBack = new Vector3 (-1.0f, 0.0f, -1.0f);
		playerBack.Normalize ();
		float angleRad = Mathf.Acos (Vector3.Dot (playerBack, vec.normalized));
		if (angleRad * Mathf.Rad2Deg <= 90.0f && Vector3.Dot (vec, playerBack) > 20.0f) {
			this.gameObject.AddComponent<Rigidbody> ();
		}
	}
}
