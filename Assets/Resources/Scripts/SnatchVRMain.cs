using UnityEngine;

public class SnatchVRMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		LevelManager.Initialize (gameObject, 2.0f);
		LevelManager.LoadLevel (0);
	}
}
