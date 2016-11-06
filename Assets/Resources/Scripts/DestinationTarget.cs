using UnityEngine;

public class DestinationTarget
{
	public Vector3 Destination { get; private set; }
	public GameObject Target { get; private set; }

	public DestinationTarget(Vector3 destination, GameObject target = null)
	{
		Destination = destination;
		Target = target;
	}
}
