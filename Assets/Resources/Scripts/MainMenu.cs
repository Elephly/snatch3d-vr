using UnityEngine;

public class MainMenu : MonoBehaviour, IGvrGazeResponder
{
	public Camera mainCamera;
	
	// Update is called once per frame
	void Update ()
	{
		transform.position = mainCamera.transform.position;
		transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, mainCamera.transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
	}

	public void ToggleVRMode()
	{
		GvrViewer.Instance.VRModeEnabled = !GvrViewer.Instance.VRModeEnabled;
	}

	#region IGvrGazeResponder implementation

	/// Called when the user is looking on a GameObject with this script,
	/// as long as it is set to an appropriate layer (see GvrGaze).
	public void OnGazeEnter() {}

	/// Called when the user stops looking on the GameObject, after OnGazeEnter
	/// was already called.
	public void OnGazeExit() {}

	/// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
	public void OnGazeTrigger() {}

	#endregion
}
