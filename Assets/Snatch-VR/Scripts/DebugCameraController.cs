using UnityEngine;
using System.Collections;

public class DebugCameraController : MonoBehaviour {

	private float speed = 2f;

	// Use this for initialization
	void Start () {
	
	}
	
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
		velocity = velocity.normalized * Time.deltaTime * speed;
		transform.Translate (velocity);
	}
}
