using UnityEngine;

public class MainMenu : ObjectBase, IGvrGazeResponder
{
	public Camera mainCamera;
    public Transform mainCameraTransform;

    protected override void Awake()
    {
        base.Awake();
        if (mainCamera != null)
            mainCameraTransform = mainCamera.transform;
    }

    void Update ()
	{
		TransformCached.position = mainCameraTransform.position;
		TransformCached.rotation = Quaternion.Euler(TransformCached.rotation.eulerAngles.x, mainCameraTransform.rotation.eulerAngles.y, TransformCached.rotation.eulerAngles.z);
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
