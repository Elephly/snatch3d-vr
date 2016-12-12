using UnityEngine;
using System.Collections.Generic;

public class EnemyPlayer : Player
{
	public char LightSource { get; private set; }
	public bool IsLightActive { get; private set; }
	public List<Vector3> PatrolPath { get; private set; }
	public float RestTimeSeconds { get; set; }
	public float FieldOfView { get; private set; }

	public bool CanSeeMainPlayer
	{
		get
		{
			Vector3 playerDirection = MainPlayer.transform.position - transform.position;
			RaycastHit hitInfo = new RaycastHit();
			if (IsLightActive)
				Physics.Raycast(transform.position, playerDirection, out hitInfo);
			else
				Physics.Raycast(transform.position, playerDirection, out hitInfo, LevelManager.LevelScale);
			return (Vector3.Angle(transform.forward, playerDirection) < FieldOfView / 2.0f) && hitInfo.transform == MainPlayer.transform;
		}
	}

	Animator EnemyAnimator = null;
	int CurrentPatrolPathDestinationIndex = -1;
	float remainingRestTimeSeconds = 0.0f;
	bool isResting = true;
	bool detectingMainPlayer = false;

	protected override void Awake()
	{
		base.Awake();
		EnemyAnimator = GetComponent<Animator>();
		LightSource = '-';
		IsLightActive = true;
		PatrolPath = new List<Vector3>();
		RestTimeSeconds = 0.0f;
		FieldOfView = 45.0f;
	}

	protected override void Update()
	{
		if (CanSeeMainPlayer)
		{
			if (!detectingMainPlayer)
				Player.DetectingMainPlayer();
			ChaseMainPlayer();
			detectingMainPlayer = true;
		}
		else
		{
			if (detectingMainPlayer)
				Player.NotDetectingMainPlayer();
			detectingMainPlayer = false;
		}
		
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
		else
		{
			if (Destination != transform.position)
			{
				transform.forward = Destination - transform.position;
			}
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

	void ChaseMainPlayer()
	{
	}
}
