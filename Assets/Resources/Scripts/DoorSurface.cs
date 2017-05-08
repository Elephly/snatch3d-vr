using UnityEngine;

public class DoorSurface : ObjectBase, IGvrGazeResponder
{
	Transform doorTransform = null;
    Door door = null;
    Renderer myRenderer = null;

	protected override void Awake()
	{
        base.Awake();
        doorTransform = TransformCached.parent.parent;
        door = doorTransform.GetComponent<Door>();
        myRenderer = GetComponent<Renderer>();
	}

	public void SetGazedAt(bool gazedAt)
	{
        if (myRenderer != null)
		    myRenderer.material.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
	}

	public void Interact(GameObject sender)
	{
		Vector3 positionWithRootY = new Vector3(TransformCached.position.x, doorTransform.position.y, TransformCached.position.z);
		Vector3 positionBeforeDoor = doorTransform.position + ((positionWithRootY - doorTransform.position) * (2.0f / 0.75f));
		if ((sender.transform.position - positionBeforeDoor).sqrMagnitude <= 4.0f)
		{
			door.Interact(sender);
		}
		else
		{
			if (!LevelManager.CurrentLevel.HasObstruction(positionBeforeDoor))
			{
				sender.SendMessage("SetDestinationTarget", new DestinationTarget(positionBeforeDoor, doorTransform.gameObject));
			}
		}
	}

	#region IGvrGazeResponder implementation

	/// Called when the user is looking on a GameObject with this script,
	/// as long as it is set to an appropriate layer (see GvrGaze).
	public void OnGazeEnter()
	{
		SetGazedAt(true);
	}

	/// Called when the user stops looking on the GameObject, after OnGazeEnter
	/// was already called.
	public void OnGazeExit()
	{
		SetGazedAt(false);
	}

	/// Called when the viewer's trigger is used, between OnGazeEnter and OnGazeExit.
	public void OnGazeTrigger()
	{
		Interact(Camera.main.gameObject);
	}

	#endregion
}
