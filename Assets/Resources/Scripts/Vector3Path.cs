using UnityEngine;

public class Vector3Path
{
	public Vector3 Origin { get; set; }
	public Vector3 Destination { get; set; }

	public Vector3Path(Vector3 origin, Vector3 destination)
	{
		Origin = origin;
		Destination = destination;
	}
}
