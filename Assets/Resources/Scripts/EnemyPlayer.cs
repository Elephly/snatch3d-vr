using UnityEngine;
using System.Collections.Generic;

public class EnemyPlayer : Player
{
	public List<Vector3> PatrolPath { get; private set; }
	public float RestTimeSeconds { get; set; }

	Animator EnemyAnimator = null;
	int CurrentPatrolPathDestinationIndex = -1;
	float remainingRestTimeSeconds = 0.0f;
	bool isResting = true;

	protected override void Awake()
	{
		base.Awake();
		EnemyAnimator = GetComponent<Animator>();
		PatrolPath = new List<Vector3>();
		RestTimeSeconds = 0.0f;
	}

	protected override void Update()
	{
		base.Update();

		if (remainingRestTimeSeconds > 0.0f || transform.position == Destination)
		{
			if (!isResting)
			{
				EnemyAnimator.Play("EasyAngryTuxedoEnemyIdleAnimation");
				isResting = true;
			}
		}
		else
		{
			if (isResting)
			{
				EnemyAnimator.Play("EasyAngryTuxedoEnemyWalkAnimation");
				isResting = false;
			}
		}

		if (remainingRestTimeSeconds > 0.0f)
		{
			transform.position -= Velocity;
			remainingRestTimeSeconds -= Time.deltaTime;
		}
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
			remainingRestTimeSeconds = RestTimeSeconds;
		}
	}
}
