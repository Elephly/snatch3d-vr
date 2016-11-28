using UnityEngine;
using System.Collections;

public class SpaceTile : MonoBehaviour, IGvrGazeResponder
{

	//GameObject CeilingTile = null;
	GameObject FloorTile = null;
	GameObject SpotLight = null;

	public char LightSource { get; private set; }
	public bool IsLightActive
	{
		get
		{
			return SpotLight.activeSelf;
		}
	}

	void Awake()
	{
		//Transform ceiling = transform.Find ("CeilingTile");
		//if (ceiling != null) {
		//	CeilingTile = ceiling.gameObject;
		//}
		Transform floor = transform.Find("FloorTile");
		if (floor != null)
		{
			FloorTile = floor.gameObject;
		}
		Transform light = transform.Find("SpotLight");
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
		if (!LevelManager.CurrentLevel.HasObstruction(transform.position))
		{
			FloorTile.GetComponent<Renderer>().material.color = gazedAt ? Color.Lerp(Color.green, Color.white, 0.5f) : Color.white;
		}
		else
		{
			FloorTile.GetComponent<Renderer>().material.color = Color.white;
		}
	}

	public void SetPlayerDestination()
	{
		if (!LevelManager.CurrentLevel.HasObstruction(transform.position))
		{
			Camera.main.SendMessage("SetDestinationTarget", new DestinationTarget(transform.position));
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
