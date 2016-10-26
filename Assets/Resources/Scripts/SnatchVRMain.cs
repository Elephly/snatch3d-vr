using UnityEngine;

public class SnatchVRMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (LevelManager.PlayerGameObject == null) {
			LevelManager.PlayerGameObject = gameObject;
			LevelManager.LoadLevel (0);
		}
	}
}
