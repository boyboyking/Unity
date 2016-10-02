using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Main : MonoBehaviour {

	void Start () {
		Debug.Log("Number of scenes int build settings" + SceneManager.sceneCountInBuildSettings);
	}

	void Update () {
		// movement of cube
		Vector3 moveVector = Vector3.zero;
		moveVector.x = Input.GetAxis ("Horizontal") * 10.0f;
		moveVector.z = Input.GetAxis ("Vertical") * 10.0f;
		this.transform.Translate (moveVector * Time.deltaTime);
	}
	// load scene
	void OnTriggerEnter (Collider col) {
		switch (col.name) {
		case "Collider 1":
			LoadSubScene (1, 2, 3);
			break;
		case "Collider 2":
			LoadSubScene (1, 4, 6);
			break;
		case "Collider 3":
			LoadSubScene (3, 5, 8);
			break;
		case "Collider 4":
			LoadSubScene (6, 7, 8);
			break;
		default:
			break;
		}
		Debug.Log ("Total number of scenes" + SceneManager.sceneCount);
	}
	// unload scene
	void OnTriggerExit (Collider col) {
		switch (col.name) {
		case "Collider 1":
			StartCoroutine (UnloadSubScene (1, 2, 3));
			break;
		case "Collider 2":
			StartCoroutine (UnloadSubScene (1, 4, 6));
			break;
		case "Collider 3":
			StartCoroutine (UnloadSubScene (3, 5, 8));
			break;
		case "Collider 4":
			StartCoroutine (UnloadSubScene (6, 7, 8));
			break;
		default:
			break;
		}
		Debug.Log ("Total number of scenes" + SceneManager.sceneCount);
	}

	void LoadSubScene(int iScene1, int iScene2, int iScene3) {
			SceneManager.LoadScene (iScene1, LoadSceneMode.Additive);
			SceneManager.LoadScene (iScene2, LoadSceneMode.Additive);
			SceneManager.LoadScene (iScene3, LoadSceneMode.Additive);
	}
	IEnumerator UnloadSubScene(int iScene1, int iScene2, int iScene3) {
		yield return 0;
		SceneManager.UnloadScene(iScene1);
		SceneManager.UnloadScene(iScene2);
		SceneManager.UnloadScene(iScene3);
	}
}
