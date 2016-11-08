using UnityEngine;
using System.Collections.Generic;

public class Player : MonoBehaviour
{

	Vector3 Destination = Vector3.zero;
	Queue<Vector3> NextDestinations = null;
	GameObject Target = null;
	float Speed = 3.0f;
	Vector3 Velocity = Vector3.zero;

	void Awake()
	{
		NextDestinations = new Queue<Vector3>();
	}

	void Start()
	{
		LevelManager.Initialize(gameObject, 2.0f);
		LevelManager.LoadLevel(0);
	}

	void Update()
	{
		Velocity = (Destination - transform.position).normalized * Speed * Time.deltaTime;
		if (transform.position != Destination)
		{
			if ((Destination - transform.position).sqrMagnitude <= Velocity.sqrMagnitude)
			{
				transform.position = Destination;
			}
			else {
				transform.position += Velocity;
			}
		}
		else
		{
			if (NextDestinations.Count > 0)
			{
				Destination = NextDestinations.Dequeue();
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
		Destination = transform.position;
		NextDestinations.Clear();

		int layerMask = ~(LayerMask.NameToLayer("WallTiles") | LayerMask.NameToLayer("Doors"));
		Vector3 forwardDirection = new Vector3(transform.forward.x, 0.0f, transform.forward.z).normalized;
		Vector3 rightDirection = new Vector3(transform.right.x, 0.0f, transform.right.z).normalized;
		Vector3 nextDestination = transform.position;
		Vector3 nextDirection = destinationTarget.Destination - nextDestination;
		Vector3 nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
		float nextMagnitude = (destinationTarget.Destination - nextDestination).magnitude;
		RaycastHit leftOffsetHit = new RaycastHit();
		RaycastHit rightOffsetHit = new RaycastHit();

		while (Physics.Raycast(nextDestination - (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out leftOffsetHit, nextMagnitude, layerMask) ||
		       Physics.Raycast(nextDestination + (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, out rightOffsetHit, nextMagnitude, layerMask))
		{
			RaycastHit hit = (leftOffsetHit.distance > 0 ? leftOffsetHit : rightOffsetHit);

			nextDestination = (hit.point / LevelManager.LevelScale) - Vector3.Project(hit.point - nextDestination, nextRight).normalized;
			nextDestination.x = Mathf.Round(nextDestination.x);
			nextDestination.y = Mathf.Round(nextDestination.y);
			nextDestination.z = Mathf.Round(nextDestination.z);
			nextDestination *= LevelManager.LevelScale;

			nextDirection = destinationTarget.Destination - nextDestination;
			nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
			nextMagnitude = (destinationTarget.Destination - nextDestination).magnitude;
			leftOffsetHit = new RaycastHit();
			rightOffsetHit = new RaycastHit();

			NextDestinations.Enqueue(nextDestination);
		}

		NextDestinations.Enqueue(destinationTarget.Destination);
		Target = destinationTarget.Target;
	}
}
