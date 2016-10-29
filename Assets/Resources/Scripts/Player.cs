using UnityEngine;

public class Player : MonoBehaviour {

	Vector3 Destination = Vector3.zero;
	float Speed = 2.0f;
	Vector3 Velocity = Vector3.zero;

	void Start () {
		LevelManager.Initialize (gameObject, 2.0f);
		LevelManager.LoadLevel (0);
	}

	void Update() {
		/*
		Velocity = (Destination - transform.position).normalized * Speed * Time.deltaTime;
		if (transform.position != Destination) {
			if ((Destination - transform.position).sqrMagnitude <= Velocity.sqrMagnitude) {
				transform.position = Destination;
			} else {
				transform.position += Velocity;
			}
		}
		*/
	}

	public void MoveTo(Vector3 destination) {
		Destination = destination;
	}
}
