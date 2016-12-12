using UnityEngine;
using System.Collections.Generic;

public abstract class Player : MonoBehaviour
{
	public static Player MainPlayer = null;
	public static int MainPlayerDetectionCount { get; private set; }

	public static void DetectingMainPlayer()
	{
		if (MainPlayerDetectionCount < 1 && ++MainPlayerDetectionCount > 0)
			MainPlayer.OnBecomeDetected();
	}

	public static void NotDetectingMainPlayer()
	{
		if (MainPlayerDetectionCount > 0 && (MainPlayerDetectionCount = Mathf.Max(0, MainPlayerDetectionCount - 1)) < 1)
			MainPlayer.OnBecomeUndetected();
	}

	protected Vector3 Destination = Vector3.zero;
	protected Stack<Vector3> NextDestinations = null;
	protected GameObject Target = null;
	protected Vector3 Velocity = Vector3.zero;

	float Speed = 3.0f;

	protected virtual void Awake()
	{
		NextDestinations = new Stack<Vector3>();
	}

	protected virtual void Update()
	{
		if (this == MainPlayer && MainPlayerDetectionCount > 0)
		{
			HandleDetection();
		}

		Velocity = (Destination - transform.position).normalized * Speed * Time.deltaTime;

		while (NextDestinations.Count > 0 && NextDestinations.Peek() == Destination)
		{
			NextDestinations.Pop();
		}

		if ((Destination - transform.position).sqrMagnitude <= Velocity.sqrMagnitude)
		{
			DestinationReached();
		}

		transform.position += Velocity;
	}

	public void SetPosition(Vector3 position)
	{
		transform.position = position;
		Destination = position;
		NextDestinations.Clear();
		Target = null;
	}

	public  virtual void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		List<Vector3> path = PathFinder.Dijkstra(LevelManager.CurrentLevel, transform.position, LevelManager.LevelGridCoords(destinationTarget.Destination));
		path.Add(transform.position);
		path = PathFinder.SmoothenPath(path);
		path[0] = destinationTarget.Destination;
		Destination = transform.position;
		NextDestinations.Clear();
		foreach (Vector3 point in path)
		{
			NextDestinations.Push(point);
		}
		Target = destinationTarget.Target;
	}

	protected virtual void DestinationReached()
	{
		if (NextDestinations.Count > 0)
		{
			Destination = NextDestinations.Pop();
			Velocity = (Destination - transform.position).normalized * Speed * Time.deltaTime;
		}
		else
		{
			TargetReached();
		}
	}

	protected virtual void TargetReached()
	{
		Velocity = Vector3.zero;
		transform.position = Destination;
		if (Target != null)
		{
			GameObject oldTarget = Target;
			Target = null;
			oldTarget.SendMessage("Interact", gameObject);
		}
	}

	protected abstract void OnBecomeDetected();

	protected abstract void OnBecomeUndetected();

	protected abstract void HandleDetection();
}
