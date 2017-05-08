using UnityEngine;

public class DoorUnlockSwitch : ObjectBase
{

	Animator DoorUnlockSwitchAnimator = null;
    Renderer DoorUnlockSwitchBaseRenderer = null;
    Renderer DoorUnlockSwitchTopArmRenderer = null;
    Renderer DoorUnlockSwitchLeftArmRenderer = null;
    Renderer DoorUnlockSwitchRightArmRenderer = null;
    Color DoorUnlockSwitchBaseColor = Color.white;
	Color DoorUnlockSwitchTopArmColor = Color.white;
	Color DoorUnlockSwitchLeftArmColor = Color.white;
	Color DoorUnlockSwitchRightArmColor = Color.white;
	GameObject DoorUnlockSwitchBase = null;
	GameObject DoorUnlockSwitchTopArm = null;
	GameObject DoorUnlockSwitchLeftArm = null;
	GameObject DoorUnlockSwitchRightArm = null;

	public bool IsLocked = true;

	protected override void Awake()
	{
        base.Awake();
		DoorUnlockSwitchAnimator = GetComponent<Animator>();
		Transform switchBase = TransformCached.FindBreadthFirst("Base");
		if (switchBase != null)
		{
			DoorUnlockSwitchBase = switchBase.gameObject;
            DoorUnlockSwitchBaseRenderer = DoorUnlockSwitchBase.GetComponent<Renderer>();
            if (DoorUnlockSwitchBaseRenderer != null)
                DoorUnlockSwitchBaseColor = DoorUnlockSwitchBaseRenderer.material.color;
		}
		Transform switchTopArm = TransformCached.FindBreadthFirst("TopArm");
		if (switchTopArm != null)
		{
			DoorUnlockSwitchTopArm = switchTopArm.gameObject;
            DoorUnlockSwitchTopArmRenderer = DoorUnlockSwitchTopArm.GetComponent<Renderer>();
            if (DoorUnlockSwitchTopArmRenderer != null)
                DoorUnlockSwitchTopArmColor = DoorUnlockSwitchTopArmRenderer.material.color;
		}
		Transform switchLeftArm = TransformCached.FindBreadthFirst("LeftArm");
		if (switchLeftArm != null)
		{
			DoorUnlockSwitchLeftArm = switchLeftArm.gameObject;
            DoorUnlockSwitchLeftArmRenderer = DoorUnlockSwitchLeftArm.GetComponent<Renderer>();
            if (DoorUnlockSwitchLeftArmRenderer != null)
                DoorUnlockSwitchLeftArmColor = DoorUnlockSwitchLeftArmRenderer.material.color;
		}
		Transform switchRightArm = TransformCached.FindBreadthFirst("RightArm");
		if (switchRightArm != null)
		{
			DoorUnlockSwitchRightArm = switchRightArm.gameObject;
            DoorUnlockSwitchRightArmRenderer = DoorUnlockSwitchRightArm.GetComponent<Renderer>();
            if (DoorUnlockSwitchRightArmRenderer != null)
                DoorUnlockSwitchRightArmColor = DoorUnlockSwitchRightArmRenderer.material.color;
		}
    }

	public void SetGazedAt(bool gazedAt)
    {
        if (DoorUnlockSwitchBaseRenderer != null)
            DoorUnlockSwitchBaseRenderer.material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchBaseColor, 0.5f) : DoorUnlockSwitchBaseColor;
        if (DoorUnlockSwitchTopArmRenderer != null)
            DoorUnlockSwitchTopArmRenderer.material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchTopArmColor, 0.5f) : DoorUnlockSwitchTopArmColor;
        if (DoorUnlockSwitchLeftArmRenderer != null)
            DoorUnlockSwitchLeftArmRenderer.material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchLeftArmColor, 0.5f) : DoorUnlockSwitchLeftArmColor;
        if (DoorUnlockSwitchRightArmRenderer != null)
            DoorUnlockSwitchRightArmRenderer.material.color = gazedAt ? Color.Lerp(Color.green, DoorUnlockSwitchRightArmColor, 0.5f) : DoorUnlockSwitchRightArmColor;
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
		if ((sender.transform.position - TransformCached.position).sqrMagnitude <= 4.0f)
		{
			ToggleDoorUnlockSwitch();
		}
		else
		{
			if (!LevelManager.CurrentLevel.HasObstruction(TransformCached.position))
			{
				sender.SendMessage("SetDestinationTarget", new DestinationTarget(TransformCached.position, gameObject));
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
