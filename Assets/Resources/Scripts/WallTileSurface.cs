using UnityEngine;

public class WallTileSurface : ObjectBase, IGvrGazeResponder
{
    Transform wallTransform = null;
    Material material = null;

    protected override void Awake()
    {
        base.Awake();
        wallTransform = TransformCached.parent;
        Renderer myRenderer = GetComponent<Renderer>();
        if (myRenderer != null)
            material = myRenderer.material;
    }

    public void SetGazedAt(bool gazedAt)
	{
        if (material != null)
        {
            if (!LevelManager.CurrentLevel.HasObstruction(TransformCached.position + (TransformCached.position - wallTransform.position)))
            {
                material.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
            }
            else
            {
                material.color = Color.white;
            }
        }
			
	}

	public void SetPlayerDestination()
	{
		if (!LevelManager.CurrentLevel.HasObstruction(TransformCached.position + (TransformCached.position - wallTransform.position)))
		{
			Camera.main.SendMessage("SetDestinationTarget", new DestinationTarget(TransformCached.position + (TransformCached.position - wallTransform.position)));
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
		SetPlayerDestination();
	}

	#endregion
}
