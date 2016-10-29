using UnityEngine;
using System.Collections;

public class LightSwitch : MonoBehaviour, IGvrGazeResponder {

	Animator animator = null;

	public char LightSource { get; private set; }
	public bool IsLightActive {
		get {
			return animator.GetCurrentAnimatorStateInfo (0).shortNameHash == Animator.StringToHash ("LightSwitchOnAnimation");
		}
	}

	void Awake () {
		animator = GetComponent<Animator> ();
	}

	void Update () {
	
	}

	public void SetLightSource(char lightSource) {
		LightSource = lightSource;
	}

	public void SetGazedAt(bool gazedAt) {
		// For now, do nothing
	}

	public void ToggleLightSwitch() {
		if ((Camera.main.transform.position - transform.position).sqrMagnitude <= 4.0f) {
			if (IsLightActive) {
				animator.Play ("LightSwitchOffAnimation");
				LevelManager.LevelStructure.SetLightActive (LightSource, false);
			} else {
				animator.Play ("LightSwitchOnAnimation");
				LevelManager.LevelStructure.SetLightActive (LightSource, true);
			}
		}
	}

	#region IGvrGazeResponder implementation

	/// Called when the user is looking on a GameObject with this script,
	/// as long as it is set to an appropriate layer (see GvrGaze).
	public void OnGazeEnter() {
		SetGazedAt (true);
	}

	/// Called when the user stops looking on the GameObject, after OnGazeEnter
	/// was already called.
	public void OnGazeExit() {
		SetGazedAt (false);		
	}

	/// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
	public void OnGazeTrigger() {
		ToggleLightSwitch ();
	}

	#endregion
}
