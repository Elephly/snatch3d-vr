using UnityEngine;
using System.Collections;

public class DoorSurface : MonoBehaviour, IGvrGazeResponder
{
	Transform rootParent = null;

	void Awake()
	{
		rootParent = transform.parent.parent;
	}

	public void SetGazedAt(bool gazedAt)
	{
		GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
	}

	public void Interact()
	{
		Vector3 positionWithRootY = new Vector3(transform.position.x, rootParent.position.y, transform.position.z);
		Vector3 positionBeforeDoor = rootParent.position + ((positionWithRootY - rootParent.position) * (2.0f / 0.75f));
		if ((Camera.main.transform.position - positionBeforeDoor).sqrMagnitude <= 4.0f)
		{
			rootParent.gameObject.SendMessage("Interact");
		}
		else
		{
			if (!LevelManager.CurrentLevel.HasObstruction(positionBeforeDoor))
			{
				Camera.main.SendMessage("SetDestinationTarget", new DestinationTarget(positionBeforeDoor, rootParent.gameObject));
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
		Interact();
	}

	#endregion
}
