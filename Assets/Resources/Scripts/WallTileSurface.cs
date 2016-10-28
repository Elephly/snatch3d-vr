using UnityEngine;
using System.Collections;

public class WallTileSurface : MonoBehaviour, IGvrGazeResponder {

	void Awake () {
		
	}

	public void SetGazedAt(bool gazedAt) {
		GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
	}

	public void SetPlayerDestination() {
		Camera.main.SendMessage("MoveTo",  transform.position + (transform.position - transform.parent.position));
	}

	#region IGvrGazeResponder implementation

	/// Called when the user is looking on a GameObject with this script,
	/// as long as it is set to an appropriate layer (see GvrGaze).
	public void OnGazeEnter() {
		SetGazedAt(true);
	}

	/// Called when the user stops looking on the GameObject, after OnGazeEnter
	/// was already called.
	public void OnGazeExit() {
		SetGazedAt(false);
	}

	/// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
	public void OnGazeTrigger() {
		SetPlayerDestination();
	}

	#endregion
}
