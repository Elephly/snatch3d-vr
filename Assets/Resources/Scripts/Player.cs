using UnityEngine;
using System.Collections.Generic;

public abstract class Player : ObjectBase
{
	public static Player MainPlayer = null;
	public static int MainPlayerDetectionCount { get; private set; }
	protected static bool mainPlayerCaught = false;

	public static void Initialize()
	{
		MainPlayerDetectionCount = 0;
		MainPlayer.OnBecomeUndetected();
		mainPlayerCaught = false;
	}

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

	protected override void Awake()
	{
        base.Awake();
		NextDestinations = new Stack<Vector3>();
	}

	protected virtual void Update()
	{
		if (!mainPlayerCaught)
		{
			if (this == MainPlayer && MainPlayerDetectionCount > 0)
			{
				HandleDetection();
			}

			Velocity = (Destination - TransformCached.position).normalized * Speed * Time.deltaTime;

			while (NextDestinations.Count > 0 && NextDestinations.Peek() == Destination)
			{
				NextDestinations.Pop();
			}

			if ((Destination - TransformCached.position).sqrMagnitude <= Velocity.sqrMagnitude)
			{
				DestinationReached();
			}

			TransformCached.position += Velocity;
		}
	}

	public void SetPosition(Vector3 position)
	{
		TransformCached.position = position;
		Destination = position;
		NextDestinations.Clear();
		Target = null;
	}

	public  virtual void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		List<Vector3> path = PathFinder.Dijkstra(LevelManager.CurrentLevel, TransformCached.position, LevelManager.LevelGridCoords(destinationTarget.Destination));
		path.Add(TransformCached.position);
		path = PathFinder.SmoothenPath(path);
		path[0] = destinationTarget.Destination;
		Destination = TransformCached.position;
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
			Velocity = (Destination - TransformCached.position).normalized * Speed * Time.deltaTime;
		}
		else
		{
			TargetReached();
		}
	}

	protected virtual void TargetReached()
	{
		Velocity = Vector3.zero;
		TransformCached.position = Destination;
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

	public virtual void HandleGameOver()
	{
		mainPlayerCaught = true;
	}
}
