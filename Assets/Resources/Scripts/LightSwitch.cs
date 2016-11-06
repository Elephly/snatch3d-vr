using UnityEngine;
using System.Collections;

public class LightSwitch : MonoBehaviour, IGvrGazeResponder
{

	Animator LightSwitchAnimator = null;
	Color LightSwitchBaseColor = Color.white;
	Color LightSwitchSwitchColor = Color.white;
	GameObject LightSwitchBase = null;
	GameObject LightSwitchSwitch = null;

	public char LightSource { get; private set; }
	public bool IsLightActive
	{
		get
		{
			return LightSwitchAnimator.GetCurrentAnimatorStateInfo(0).shortNameHash == Animator.StringToHash("LightSwitchOnAnimation");
		}
	}

	void Awake()
	{
		LightSwitchAnimator = GetComponent<Animator>();
		Transform lBase = transform.FindBreadthFirst("Base");
		if (lBase != null)
		{
			LightSwitchBase = lBase.gameObject;
			LightSwitchBaseColor = LightSwitchBase.GetComponent<Renderer>().material.color;
		}
		Transform lSwitch = transform.FindBreadthFirst("Switch");
		if (lSwitch != null)
		{
			LightSwitchSwitch = lSwitch.gameObject;
			LightSwitchSwitchColor = LightSwitchSwitch.GetComponent<Renderer>().material.color;
		}
	}

	public void SetLightSource(char lightSource)
	{
		LightSource = lightSource;
	}

	public void SetGazedAt(bool gazedAt)
	{
		LightSwitchBase.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, LightSwitchBaseColor, 0.5f) : LightSwitchBaseColor;
		LightSwitchSwitch.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, LightSwitchSwitchColor, 0.5f) : LightSwitchSwitchColor;
	}

	public void ToggleLightSwitch()
	{
		if (IsLightActive)
		{
			LightSwitchAnimator.Play("LightSwitchOffAnimation");
			LevelManager.CurrentLevel.SetLightActive(LightSource, false);
		}
		else {
			LightSwitchAnimator.Play("LightSwitchOnAnimation");
			LevelManager.CurrentLevel.SetLightActive(LightSource, true);
		}
	}

	public void Interact()
	{
		if ((Camera.main.transform.position - transform.position).sqrMagnitude <= 4.0f)
		{
			ToggleLightSwitch();
		}
		else
		{
			if (!LevelManager.CurrentLevel.HasObstruction(transform.position))
			{
				Camera.main.SendMessage("SetDestinationTarget", new DestinationTarget(transform.position, gameObject));
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
