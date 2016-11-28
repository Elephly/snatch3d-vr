using UnityEngine;
using System.Collections;

public class DoorUnlockSwitch : MonoBehaviour
{

	Animator DoorUnlockSwitchAnimator = null;
	Color DoorUnlockSwitchBaseColor = Color.white;
	Color DoorUnlockSwitchTopArmColor = Color.white;
	Color DoorUnlockSwitchLeftArmColor = Color.white;
	Color DoorUnlockSwitchRightArmColor = Color.white;
	GameObject DoorUnlockSwitchBase = null;
	GameObject DoorUnlockSwitchTopArm = null;
	GameObject DoorUnlockSwitchLeftArm = null;
	GameObject DoorUnlockSwitchRightArm = null;

	public bool IsLocked = true;

	void Awake()
	{
		DoorUnlockSwitchAnimator = GetComponent<Animator>();
		Transform switchBase = transform.FindBreadthFirst("Base");
		if (switchBase != null)
		{
			DoorUnlockSwitchBase = switchBase.gameObject;
			DoorUnlockSwitchBaseColor = DoorUnlockSwitchBase.GetComponent<Renderer>().material.color;
		}
		Transform switchTopArm = transform.FindBreadthFirst("TopArm");
		if (switchTopArm != null)
		{
			DoorUnlockSwitchTopArm = switchTopArm.gameObject;
			DoorUnlockSwitchTopArmColor = DoorUnlockSwitchTopArm.GetComponent<Renderer>().material.color;
		}
		Transform switchLeftArm = transform.FindBreadthFirst("LeftArm");
		if (switchLeftArm != null)
		{
			DoorUnlockSwitchLeftArm = switchLeftArm.gameObject;
			DoorUnlockSwitchLeftArmColor = DoorUnlockSwitchLeftArm.GetComponent<Renderer>().material.color;
		}
		Transform switchRightArm = transform.FindBreadthFirst("RightArm");
		if (switchRightArm != null)
		{
			DoorUnlockSwitchRightArm = switchRightArm.gameObject;
			DoorUnlockSwitchRightArmColor = DoorUnlockSwitchRightArm.GetComponent<Renderer>().material.color;
		}
	}

	public void SetGazedAt(bool gazedAt)
	{
		DoorUnlockSwitchBase.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchBaseColor, 0.5f) : DoorUnlockSwitchBaseColor;
		DoorUnlockSwitchTopArm.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchTopArmColor, 0.5f) : DoorUnlockSwitchTopArmColor;
		DoorUnlockSwitchLeftArm.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchLeftArmColor, 0.5f) : DoorUnlockSwitchLeftArmColor;
		DoorUnlockSwitchRightArm.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchRightArmColor, 0.5f) : DoorUnlockSwitchRightArmColor;
	}

	public void ToggleDoorUnlockSwitch()
	{
		if (IsLocked)
		{
			DoorUnlockSwitchAnimator.Play("DoorUnlockSwitchUnlockAnimation");
		}
		else {
			DoorUnlockSwitchAnimator.Play("DoorUnlockSwitchLockAnimation");
		}
	}

	public void Interact(GameObject sender)
	{
		if ((sender.transform.position - transform.position).sqrMagnitude <= 4.0f)
		{
			ToggleDoorUnlockSwitch();
		}
		else
		{
			if (!LevelManager.CurrentLevel.HasObstruction(transform.position))
			{
				sender.SendMessage("SetDestinationTarget", new DestinationTarget(transform.position, gameObject));
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
