using UnityEngine;

public class LightSwitch : ObjectBase, IGvrGazeResponder
{

	Animator LightSwitchAnimator = null;
    Material LightSwitchBaseMaterial = null;
    Material LightSwitchSwitchMaterial = null;
	Color LightSwitchBaseColor = Color.white;
	Color LightSwitchSwitchColor = Color.white;
	GameObject LightSwitchBase = null;
	GameObject LightSwitchSwitch = null;

	public char LightSource { get; private set; }
	public bool IsLightActive
	{
		get
		{
			return !LightSwitchAnimator.GetCurrentAnimatorStateInfo(0).IsName("LightSwitchOffAnimation");
		}
	}

	protected override void Awake()
	{
        base.Awake();
		LightSwitchAnimator = GetComponent<Animator>();
		Transform lBase = TransformCached.FindBreadthFirst("Base");
		if (lBase != null)
		{
			LightSwitchBase = lBase.gameObject;
            Renderer LightSwitchBaseRenderer = LightSwitchBase.GetComponent<Renderer>();
            if (LightSwitchBaseRenderer != null)
            {
                LightSwitchBaseMaterial = LightSwitchBaseRenderer.material;
                LightSwitchBaseColor = LightSwitchBaseMaterial.color;
            }
		}
		Transform lSwitch = TransformCached.FindBreadthFirst("Switch");
		if (lSwitch != null)
		{
			LightSwitchSwitch = lSwitch.gameObject;
            Renderer LightSwitchSwitchRenderer = LightSwitchSwitch.GetComponent<Renderer>();
            if (LightSwitchSwitchRenderer != null)
            {
                LightSwitchSwitchMaterial = LightSwitchSwitchRenderer.material;
                LightSwitchSwitchColor = LightSwitchSwitchMaterial.color;
            }
		}
	}

	public void SetLightSource(char lightSource)
	{
		LightSource = lightSource;
	}

	public void SetGazedAt(bool gazedAt)
	{
        if (LightSwitchBaseMaterial != null)
            LightSwitchBaseMaterial.color = gazedAt ? Color.Lerp(Color.green, LightSwitchBaseColor, 0.5f) : LightSwitchBaseColor;
        if (LightSwitchSwitchMaterial != null)
            LightSwitchSwitchMaterial.color = gazedAt ? Color.Lerp(Color.green, LightSwitchSwitchColor, 0.5f) : LightSwitchSwitchColor;
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

	public void Interact(GameObject sender)
	{
		if ((sender.transform.position - TransformCached.position).sqrMagnitude <= 4.0f)
		{
			ToggleLightSwitch();
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
