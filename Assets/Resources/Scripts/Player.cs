using UnityEngine;

public class Player : MonoBehaviour
{

	Vector3 Destination = Vector3.zero;
	GameObject Target = null;
	float Speed = 3.0f;
	Vector3 Velocity = Vector3.zero;

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
			if (Target != null)
			{
				Target.SendMessage("Interact");
				Target = null;
			}
		}

		if ((transform.position - LevelManager.CurrentLevel.GoalLocation).sqrMagnitude < 0.5f)
		{
			LevelManager.LoadNextLevel();
		}
	}

	public void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		Destination = destinationTarget.Destination;
		Target = destinationTarget.Target;
	}
}
