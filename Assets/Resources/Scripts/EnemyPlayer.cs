using UnityEngine;
using System.Collections.Generic;

public class EnemyPlayer : Player
{
	public char LightSource { get; private set; }
	public bool IsLightActive { get; private set; }
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
		LightSource = '-';
		IsLightActive = true;
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

	public void SetLightSource(char lightSource)
	{
		LightSource = lightSource;
	}

	public void SetLightActive(bool state)
	{
		IsLightActive = state;
		HandleLightStateChanged();
	}

	public void ToggleLight()
	{
		IsLightActive = !IsLightActive;
		HandleLightStateChanged();
	}

	public override void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		transform.forward = destinationTarget.Destination - transform.position;
		base.SetDestinationTarget(destinationTarget);
		remainingRestTimeSeconds = RestTimeSeconds;
	}

	protected override void TargetReached()
	{
		base.TargetReached();
		VisitNextPatrolPathDestination();
	}

	void HandleLightStateChanged()
	{
		GameObject lightSwitch = LevelManager.CurrentLevel.LightSourceMap[LightSource];
		if (!IsLightActive && LightSource != '-')
		{
			if (lightSwitch != null)
			{
				CurrentPatrolPathDestinationIndex--;
				lightSwitch.SendMessage("Interact", gameObject);
			}
		}
		else if (Target == lightSwitch)
		{
			VisitNextPatrolPathDestination();
		}
	}

	void VisitNextPatrolPathDestination()
	{
		if (PatrolPath.Count > 0)
		{
			DestinationTarget destinationTarget = new DestinationTarget(PatrolPath[++CurrentPatrolPathDestinationIndex % PatrolPath.Count], null);
			SetDestinationTarget(destinationTarget);
		}
	}
}
