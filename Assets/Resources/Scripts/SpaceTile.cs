using UnityEngine;
using System.Collections;

public class SpaceTile : ObjectBase, IGvrGazeResponder
{

	//GameObject CeilingTile = null;
	GameObject FloorTile = null;
	GameObject SpotLight = null;

    Material FloorTileMaterial = null;

	public char LightSource { get; private set; }
	public bool IsLightActive
	{
		get
		{
			return SpotLight.activeSelf;
		}
	}

	protected override void Awake()
	{
        base.Awake();
		//Transform ceiling = TransformCached.Find ("CeilingTile");
		//if (ceiling != null) {
		//	CeilingTile = ceiling.gameObject;
		//}
		Transform floor = TransformCached.Find("FloorTile");
		if (floor != null)
		{
			FloorTile = floor.gameObject;
            Renderer FloorTileRenderer = FloorTile.GetComponent<Renderer>();
            if (FloorTileRenderer != null)
                FloorTileMaterial = FloorTileRenderer.material;
		}
		Transform light = TransformCached.Find("SpotLight");
		if (light != null)
		{
			SpotLight = light.gameObject;
		}
		LightSource = '-';
	}

	public void SetLightSource(char lightSource)
	{
		LightSource = lightSource;
	}

	public void SetLightActive(bool state)
	{
		if (SpotLight != null)
		{
			SpotLight.SetActive(state);
		}
	}

	public void ToggleLight()
	{
		if (SpotLight != null)
		{
			SpotLight.SetActive(!SpotLight.activeSelf);
		}
	}

	public void SetGazedAt(bool gazedAt)
	{
        if (FloorTileMaterial != null)
        {
            if (!LevelManager.CurrentLevel.HasObstruction(TransformCached.position))
            {
                FloorTileMaterial.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
            }
            else
            {
                FloorTileMaterial.color = Color.white;
            }
        }
	}

	public void SetPlayerDestination()
	{
		if (!LevelManager.CurrentLevel.HasObstruction(TransformCached.position))
		{
			Camera.main.SendMessage("SetDestinationTarget", new DestinationTarget(TransformCached.position));
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
