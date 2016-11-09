using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

	Vector3 Destination = Vector3.zero;
	Stack<Vector3> NextDestinations = null;
	GameObject Target = null;
	float Speed = 3.0f;
	Vector3 Velocity = Vector3.zero;

	void Awake()
	{
		NextDestinations = new Stack<Vector3>();
	}

	void Start()
	{
		LevelManager.Initialize(gameObject, 2.0f);
		LevelManager.LoadLevel(0);
	}

	void Update()
	{
		Velocity = (Destination - transform.position).normalized * Speed * Time.deltaTime;

		while (NextDestinations.Count > 0 && NextDestinations.Peek() == Destination)
		{
			NextDestinations.Pop();
		}

		if (transform.position != Destination)
		{
			if ((Destination - transform.position).sqrMagnitude <= Velocity.sqrMagnitude)
			{
				if (NextDestinations.Count > 0)
				{
					Destination = NextDestinations.Pop();
					transform.position += Velocity;
				}
				else
				{
					transform.position = Destination;
				}
			}
			else {
				transform.position += Velocity;
			}
		}
		else
		{
			if (NextDestinations.Count > 0)
			{
				Destination = NextDestinations.Pop();
			}
			else
			{
				if (Target != null)
				{
					Target.SendMessage("Interact");
					Target = null;
				}
			}
		}

		if ((transform.position - LevelManager.CurrentLevel.GoalLocation).sqrMagnitude < 0.5f)
		{
			LevelManager.LoadNextLevel();
		}
	}

	public void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		var path = PathFinder.Dijkstra(LevelManager.CurrentLevel, transform.position, destinationTarget.Destination);
		path.Add(transform.position);
		path = PathFinder.SmoothenPath(path);
		Destination = transform.position;
		NextDestinations.Clear();
		foreach (Vector3 point in path)
		{
			NextDestinations.Push(point);
		}
		Target = destinationTarget.Target;
	}

	// Recursive
	/*
	public void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		Destination = transform.position;
		NextDestinations.Clear();
		Target = destinationTarget.Target;
		Stack<Vector3> pathStack = new Stack<Vector3>();
		PushPathToDestination(transform.position, destinationTarget.Destination, ref pathStack);
		while (pathStack.Count > 0) NextDestinations.Push(pathStack.Pop());
	}

	void PushPathToDestination(Vector3 origin, Vector3 destination, ref Stack<Vector3> pathStack)
	{
		int layerMask = ~(LayerMask.NameToLayer("WallTiles") | LayerMask.NameToLayer("Doors"));
		Vector3 nextDestination = origin;
		Vector3 nextDirection = destination - nextDestination;
		Vector3 nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
		float nextMagnitude = (destination - nextDestination).magnitude;
		RaycastHit leftOffsetHit = new RaycastHit();
		RaycastHit rightOffsetHit = new RaycastHit();

		while (Physics.Raycast(nextDestination - (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out leftOffsetHit, nextMagnitude, layerMask) ||
			   Physics.Raycast(nextDestination + (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out rightOffsetHit, nextMagnitude, layerMask))
		{
			RaycastHit hit = (leftOffsetHit.distance > 0 ? leftOffsetHit : rightOffsetHit);
			Vector3 prevDestination = nextDestination;

			nextDestination = (hit.point / LevelManager.LevelScale) - Vector3.Project(hit.point - prevDestination, nextRight).normalized;
			nextDestination.x = Mathf.Round(nextDestination.x);
			nextDestination.y = Mathf.Round(nextDestination.y);
			nextDestination.z = Mathf.Round(nextDestination.z);
			nextDestination *= LevelManager.LevelScale;

			nextDirection = destination - nextDestination;
			nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
			nextMagnitude = (destination - nextDestination).magnitude;
			leftOffsetHit = new RaycastHit();
			rightOffsetHit = new RaycastHit();

			PushPathToDestination(prevDestination, nextDestination, ref pathStack);
		}

		pathStack.Push(destination);
	}
	*/

	// ITERATIVE
	/*
		Destination = transform.position;
		NextDestinations.Clear();
		Target = destinationTarget.Target;

		int layerMask = ~(LayerMask.NameToLayer("WallTiles") | LayerMask.NameToLayer("Doors"));
		Stack<Vector3Path> pathStack = new Stack<Vector3Path>();
		pathStack.Push(new Vector3Path(transform.position, destinationTarget.Destination));

		int DEBUG_COUNTER = 0;
		while (pathStack.Count > 0)
		{
			Vector3Path path = pathStack.Pop();
			Debug.Log(path.Origin + " -> " + path.Destination);
			Vector3 nextDestination = path.Origin;
			Vector3 nextDirection = destinationTarget.Destination - nextDestination;
			Vector3 nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
			float nextMagnitude = (destinationTarget.Destination - nextDestination).magnitude;
			RaycastHit leftOffsetHit = new RaycastHit();
			RaycastHit rightOffsetHit = new RaycastHit();

			while (Physics.Raycast(nextDestination - (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out leftOffsetHit, nextMagnitude, layerMask) ||
				   Physics.Raycast(nextDestination + (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out rightOffsetHit, nextMagnitude, layerMask))
			{
				RaycastHit hit = (leftOffsetHit.distance > 0 ? leftOffsetHit : rightOffsetHit);
				Vector3 prevDestination = nextDestination;

				nextDestination = (hit.point / LevelManager.LevelScale) - Vector3.Project(hit.point - prevDestination, nextRight).normalized;
				nextDestination.x = Mathf.Round(nextDestination.x);
				nextDestination.y = Mathf.Round(nextDestination.y);
				nextDestination.z = Mathf.Round(nextDestination.z);
				nextDestination *= LevelManager.LevelScale;

				nextDirection = destinationTarget.Destination - nextDestination;
				nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
				nextMagnitude = (destinationTarget.Destination - nextDestination).magnitude;
				leftOffsetHit = new RaycastHit();
				rightOffsetHit = new RaycastHit();

				pathStack.Push(new Vector3Path(prevDestination, nextDestination));
			}

			NextDestinations.Push(path.Destination);

			if (DEBUG_COUNTER++ > 100) { Debug.Log("FUCK"); break; }
		}
	*/
}
