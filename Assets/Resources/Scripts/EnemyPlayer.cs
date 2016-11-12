using UnityEngine;
using System.Collections.Generic;

public class EnemyPlayer : Player
{
	public List<Vector3> PatrolPath { get; private set; }
	public float RestTimeSeconds { get; set; }

	int CurrentPatrolPathDestinationIndex = -1;

	protected override void Awake()
	{
		base.Awake();
		PatrolPath = new List<Vector3>();
		RestTimeSeconds = 0.0f;
	}

	protected override void Update()
	{
		base.Update();

		// Add rest wait functionality
	}

	public void SetPatrolPath(List<Vector3> patrolPath)
	{
		PatrolPath = patrolPath;
	}

	public void SetRestTimeSeconds(float restTimeSeconds)
	{
		RestTimeSeconds = restTimeSeconds;
	}

	protected override void TargetReached()
	{
		base.TargetReached();
		if (PatrolPath.Count > 0)
		{
			DestinationTarget destinationTarget = new DestinationTarget(PatrolPath[++CurrentPatrolPathDestinationIndex % PatrolPath.Count], null);
			transform.forward = destinationTarget.Destination - transform.position;
			SetDestinationTarget(destinationTarget);
		}
	}
}
