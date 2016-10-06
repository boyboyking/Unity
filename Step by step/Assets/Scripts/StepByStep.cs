using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StepByStep : MonoBehaviour {
	public GameObject player;
	public GameObject plane;
	public GameObject[] node = new GameObject[7];
	public Text scoreText;
	public Text highScoreText;
	public Button playBtn;
	public Transform boostTar;

	public static float maxTileMoveSpeed = 50.0f;
	public static float minTileMoveSpeed = 30.0f;
	public static bool detectTile = false;
	public static bool gameStart = false;
	public static bool isBoost = false;
	public static int dimandCount = 0;

	private const float oneStep = 3.0f;
	private const float BOUND_SIZE = 3.0f;

	private Camera mainCamera;

	private List<Vector2> oriList = new List<Vector2> ();
	private List<Vector2> dirList = new List<Vector2> ();
	private List<Vector2> lineOddOriList = new List<Vector2> ();
	private List<Vector2> lineOddDirList = new List<Vector2> ();
	private List<Vector2> lineEvenOriList = new List<Vector2> ();
	private List<Vector2> lineEvenDirList = new List<Vector2> ();
	private List<Vector3> playerPrePos = new List<Vector3> ();

	private Vector2 lastLineOri = new Vector2 (15.0f, 0.0f);
	private Vector2 lastLineDir = new Vector2 (0.0f, 15.0f);
//	private Vector3 camMoveVec = new Vector3 (3.0f, 2.0f, 3.0f);
	private Vector3 cameraVelocity = Vector3.zero;

	private int scoreCount = 0;
	private int maxNumOddRaw = 7;
	private int minNumOddRaw = 5;
	private int maxNumEvenRaw = 6;
	private int minNumEvenRaw = 4;
	private int numPlayerPrePos = 10;

	private float lastStep = 4.0f;
	private float rayDistance = 10000.0f;
//	private float camMoveSpeed = 5.0f;
	private float camSmoothTime = 0.5f;
	private float tileFallSec = 1.0f;
	private float preTime = 0.0f;

	private bool rightMove;
	private bool oddRaw = true;
	public static bool gameOver = false;

	private void Start () {
		mainCamera = Camera.main;
		init ();

	}

	private void Update () {
		if (!gameOver && gameStart) {
			if (!isBoost) {
				// move the camera
				cameraMovement ();
				if (Input.GetKeyDown (KeyCode.RightArrow)) {
					rightMove = true;
					playerMove ();
				} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {
					rightMove = false;
					playerMove ();
				}
				detectOnTile ();
			} else {
				boosting ();
			}

		}else if (gameOver){
			if (Input.anyKeyDown) {
				SceneManager.LoadScene ("StepByStep");
			}
		}
	}

	private void FixedUpdate () {
//			mainCamera.transform.localPosition += camMoveVec.normalized * camMoveSpeed * Time.deltaTime;
		// ray of node
		foreach (GameObject go in node) {
			Ray nodeRay = new Ray (go.transform.position, Vector3.up);
			RaycastHit hit;
			Physics.Raycast (nodeRay, out hit, rayDistance);
			if (hit.collider != null && hit.collider.tag == "Tile") {
				detectTile = true;
			}
		}
		if (!detectTile) {
			// random create a tile 
			randCreateTile ();
		}
		detectTile = false;
		if (!gameOver && !isBoost) {
			float t = Time.time;
			if ((t - preTime) >= tileFallSec && playerPrePos.Count != 0 && scoreCount > 5) {
				tileFall ();
			}
		}
	}

	private void init () {
		gameStart = false;
		gameOver = false;
		// origin
		for (int i = 18; i > 0; i -= 3) {
			oriList.Add (new Vector2 (0.0f, i));
		}
		for (int i = 0; i <= 18; i += 3) {
			oriList.Add (new Vector2 (i, 0.0f));
		}
		// direction
		for (int i = 21; i > 3; i -= 3) {
			dirList.Add (new Vector2 (3.0f, i));
		}
		for (int i = 3; i <= 21; i += 3) {
			dirList.Add (new Vector2 (i, 3.0f));
		}
		addLine ();
	}

	private void cameraMovement () {	// camera follow player
		Vector3 ori = new Vector3 (1.0f, 0.0f, 1.0f);
		ori.Normalize ();
		Vector3 vec = player.transform.position;
		vec.y = 0.0f;
		vec.Normalize ();
		float angleRad = Mathf.Acos (Vector3.Dot (vec, ori));

		vec = player.transform.position;
		vec.y = 0.0f;
		float desiredPos = vec.magnitude * Mathf.Cos (angleRad) * Mathf.Cos (45.0f * Mathf.Deg2Rad) - 15.0f;
//		Debug.DrawLine (Vector3.zero, new Vector3 (desiredPos + 15.0f, player.transform.position.y, desiredPos + 9.5f), Color.red);
		mainCamera.transform.position = Vector3.SmoothDamp(mainCamera.transform.position, new Vector3 (desiredPos, player.transform.position.y + 24.0f, desiredPos), ref cameraVelocity, camSmoothTime);
	}

	private void detectOnTile () {
		// ray of player
		Ray playerRay = new Ray (player.transform.position, Vector3.down);
		RaycastHit hit;
		Physics.Raycast (playerRay, out hit, rayDistance);

		if (hit.collider.tag != "Tile" || hit.collider.GetComponent<Rigidbody> () != null) {
			GameOver ();
		}
	}

	private void playerMove () {
		if (scoreCount > (lastStep - 3)) {
			return;
		}

		Transform playerTr = player.transform;
		Transform planeTr = plane.transform;
		scoreCount++;
		scoreText.text = scoreCount.ToString ();

		if (rightMove) {
			playerTr.position = new Vector3 (playerTr.position.x + oneStep, scoreCount, playerTr.position.z);
			playerTr.rotation = new Quaternion (0.0f, 1.0f, 0.0f, 1.0f);
//			planeTr.position = new Vector3 (planeTr.position.x + oneStep, planeTr.position.y + 1, planeTr.position.z);
		} else {
			playerTr.position = new Vector3 (playerTr.position.x, scoreCount, playerTr.position.z + oneStep);
			playerTr.rotation = new Quaternion (0.0f, 0.0f, 0.0f, 1.0f);
//			planeTr.position = new Vector3 (planeTr.position.x, planeTr.position.y + 1, planeTr.position.z + oneStep);
		}

		if (playerPrePos.Count < numPlayerPrePos) {
			playerPrePos.Add (playerTr.position);
		} else {
			for (int i = 1; i < playerPrePos.Count; i++) {
				playerPrePos [i - 1] = playerPrePos [i];
			}
			playerPrePos.RemoveAt (playerPrePos.Count-1);
			playerPrePos.Add (playerTr.position);
		}
		// speed up
		if (scoreCount % 10 == 0) {
			tileFallSec -= 0.01f;
			Debug.Log ("tileFallSec: " + tileFallSec);
		}
	}

	private void boosting () {
		// player boosting
		player.GetComponent<Rigidbody> ().useGravity = false;

		if ((player.transform.position)
		player.transform.position = Vector3.Lerp (player.transform.position, boostTar.position, 3.0f * Time.deltaTime);

	}

	private void tileFall () {
		if (playerPrePos.Count == 1) {
			Ray playerRay = new Ray (player.transform.position, Vector3.down);
			RaycastHit hit;
			Physics.Raycast (playerRay, out hit, rayDistance);
			if (!hit.collider.GetComponent<Rigidbody> ()) {
				hit.collider.gameObject.AddComponent<Rigidbody> ();
				hit.collider.gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * 10.0f, ForceMode.Impulse);
				player.GetComponent<Rigidbody> ().AddForce (transform.up * 12.0f, ForceMode.Impulse);
			}
		}

		Vector3 ori = new Vector3 (playerPrePos [0].x - 0.5f, playerPrePos [0].y - 1.0f, playerPrePos [0].z - 0.5f);

		Ray leftR = new Ray (ori, new Vector3 (-1.0f, 0.0f, 1.0f));
		Ray rightR = new Ray (ori, new Vector3 (1.0f, 0.0f, -1.0f));
		RaycastHit leftHit, rightHit;
		Physics.Raycast (leftR, out leftHit, 100.0f);
		Physics.Raycast (rightR, out rightHit, 100.0f);

		if (leftHit.collider != null && leftHit.collider.tag == "Tile") {
			if (!leftHit.collider.gameObject.GetComponent<Rigidbody> ()) {
				leftHit.collider.gameObject.AddComponent<Rigidbody> ();
				leftHit.collider.gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * Random.Range (5.0f, 10.0f), ForceMode.Impulse);
				leftHit.collider.gameObject.GetComponent<BoxCollider> ().enabled = false;
				float randAng = Random.Range (-30.0f, 30.0f);
				leftHit.collider.gameObject.transform.Rotate (new Vector3 (randAng, randAng, randAng));
			}
		}
		if (rightHit.collider != null && rightHit.collider.tag == "Tile") {
			if (!rightHit.collider.gameObject.GetComponent<Rigidbody> ()) {
				rightHit.collider.gameObject.AddComponent<Rigidbody> ();
				rightHit.collider.gameObject.GetComponent<Rigidbody> ().AddForce (transform.up * Random.Range (5.0f, 10.0f), ForceMode.Impulse);
				rightHit.collider.gameObject.GetComponent<BoxCollider> ().enabled = false;
				float randAng = Random.Range (-30.0f, 30.0f);
				rightHit.collider.gameObject.transform.Rotate (new Vector3 (randAng, randAng, randAng));
			}
		}
		if (leftHit.collider == null && rightHit.collider == null) {
			playerPrePos.RemoveAt (0);
			preTime = Time.time;
		}
	}

	private void randCreateTile () {
		lastStep++;
		lastLineOri += Vector2.right * 3.0f;
		lastLineDir += Vector2.up * 3.0f;
		int num;
		int colum;
		if (oddRaw) {
			num = Random.Range (minNumOddRaw, maxNumOddRaw);
			for (int i = 0; i < num; i++) {
				colum = Random.Range (0, lineOddOriList.Count);
				Vector2 vec = LineIntersect (lineOddOriList [colum], lineOddDirList [colum], lastLineOri, lastLineDir);
				Object obj = Resources.Load ("Tile");
				GameObject tile = (GameObject)Instantiate (obj);
				tile.transform.position = new Vector3 (vec.x, lastStep, vec.y);
				lineOddOriList.RemoveAt (colum);
				lineOddDirList.RemoveAt (colum);
			}
		} else {
			num = Random.Range (minNumEvenRaw, maxNumEvenRaw);
			for (int i = 0; i < num; i++) {
				colum = Random.Range (0, lineEvenOriList.Count);
				Vector2 vec = LineIntersect (lineEvenOriList [colum], lineEvenDirList [colum], lastLineOri, lastLineDir);
				Object obj = Resources.Load ("Tile");
				GameObject tile = (GameObject)Instantiate (obj);
				tile.transform.position = new Vector3 (vec.x, lastStep, vec.y);
				lineEvenOriList.RemoveAt (colum);
				lineEvenDirList.RemoveAt (colum);
			}
		}
		oddRaw = !oddRaw;

		addLine ();
	}

	private void addLine () {
		lineOddOriList.Clear ();
		lineOddDirList.Clear ();
		lineEvenOriList.Clear ();
		lineEvenDirList.Clear ();

		for (int i = 0; i < oriList.Count; i += 2) {
			lineOddOriList.Add (oriList [i]);
			lineOddDirList.Add (dirList [i]);
		}
		for (int i = 1; i < oriList.Count; i += 2) {
			lineEvenOriList.Add (oriList [i]);
			lineEvenDirList.Add (dirList [i]);
		}
	}

	private Vector2 LineIntersect(Vector2 origin1,Vector2 direction1,Vector2 origin2,Vector2 direction2){
		Vector2 intersect = Vector2.zero;

		Vector2 slopeV1 = origin1 - direction1;
		float slopeF1 = slopeV1.y / slopeV1.x;

		Vector2 slopeV2 = origin2 - direction2;
		float slopeF2 = slopeV2.y / slopeV2.x;

		intersect.x = (slopeF1 * origin1.x - slopeF2 * origin2.x + origin2.y - origin1.y) / (slopeF1 - slopeF2);
		intersect.y = slopeF1 * (intersect.x - origin1.x) + origin1.y;

		return intersect;
	}

	private void GameOver () {
		gameOver = true;
		Debug.Log ("Game Over!");
	}

	public void OnButtonClick () {
		gameStart = true;
		playBtn.gameObject.SetActive (false);
	}
}
