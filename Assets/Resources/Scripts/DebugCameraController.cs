using UnityEngine;
using System.Collections;

public class DebugCameraController : MonoBehaviour {

	private float speed = 2f;
	
	// Update is called once per frame
	void Update () {
		Vector3 velocity = Vector3.zero;
		if (Input.GetKey("w")) {
			velocity += Vector3.forward;
		}
		if (Input.GetKey("s")) {
			velocity += Vector3.back;
		}
		if (Input.GetKey("a")) {
			velocity += Vector3.left;
		}
		if (Input.GetKey("d")) {
			velocity += Vector3.right;
		}
		if (Input.GetKey("q")) {
			velocity += Vector3.up;
		}
		if (Input.GetKey("c")) {
			velocity += Vector3.down;
		}
		if (Input.GetKey("i")) {
			if (LevelManager.Level == 0) {
				LevelManager.LoadLevel (1);
			}
		}
		if (Input.GetKey("k")) {
			if (LevelManager.Level == 1) {
				LevelManager.LoadLevel (0);
			}
		}
		velocity = velocity.normalized * Time.deltaTime * speed;
		transform.Translate (velocity);
	}
}
