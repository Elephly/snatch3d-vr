using UnityEngine;

public class DestinationTarget
{
	public Vector3 Destination { get; private set; }
	public IInteractive Target { get; private set; }

	public DestinationTarget(Vector3 destination, IInteractive target = null)
	{
		Destination = destination;
		Target = target;
	}
}
