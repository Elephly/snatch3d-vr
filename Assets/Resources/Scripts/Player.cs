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
}
