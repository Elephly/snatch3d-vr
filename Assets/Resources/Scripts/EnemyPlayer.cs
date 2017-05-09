using UnityEngine;
using System.Collections.Generic;

public class EnemyPlayer : Player, ILightSourceListener
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
			Vector3 playerDirection = MainPlayer.TransformCached.position - TransformCached.position;
			float angleBetweenPlayerAndForward = Vector3.Angle(forward, playerDirection);
			float fov = SearchingForMainPlayer ? FieldOfView * 2.0f : FieldOfView;
			RaycastHit hitInfo = new RaycastHit();
			if (IsLightActive)
				Physics.Raycast(TransformCached.position, playerDirection, out hitInfo);
			else
				Physics.Raycast(TransformCached.position, playerDirection, out hitInfo, LevelManager.LevelScale);
			return (((angleBetweenPlayerAndForward < fov * 1.5f) && (playerDirection.sqrMagnitude < Mathf.Pow(LevelManager.LevelScale * 1.4f, 2.0f))) ||
			        (angleBetweenPlayerAndForward < fov / 2.0f)) &&
				(hitInfo.transform == MainPlayer.TransformCached);
		}
	}

	public bool SearchingForMainPlayer = false;

	Animator EnemyAnimator = null;
	int CurrentPatrolPathDestinationIndex = -1;
	Transform headNode = null;
	float remainingRestTimeSeconds = 0.0f;
	bool isResting = true;
	bool detectingMainPlayer = false;
	bool wasSearchingForMainPlayer = false;

	Vector3 forward
	{
		get
		{
			return headNode != null ? headNode.forward : TransformCached.forward;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		EnemyAnimator = GetComponent<Animator>();
		LightSource = '-';
		IsLightActive = true;
		PatrolPath = new List<Vector3>();
		headNode = TransformCached.FindBreadthFirst("HeadNode");
		RestTimeSeconds = 0.0f;
		FieldOfView = 45.0f;
	}

	protected override void Update()
	{
		if (CanSeeMainPlayer)
		{
			if (!detectingMainPlayer)
			{
				Player.DetectingMainPlayer();
			}
			ChaseMainPlayer();
			detectingMainPlayer = true;
			SearchingForMainPlayer = false;
			wasSearchingForMainPlayer = false;
		}
		else if (detectingMainPlayer && !IsLightActive)
		{
			Player.NotDetectingMainPlayer();
			HandleLightStateChanged();
			detectingMainPlayer = false;
		}

		if (!SearchingForMainPlayer && !mainPlayerCaught)
		{
			if (detectingMainPlayer && wasSearchingForMainPlayer)
			{
				Player.NotDetectingMainPlayer();
				VisitNextPatrolPathDestination();
				detectingMainPlayer = false;
			}

			base.Update();

			if (remainingRestTimeSeconds > 0.0f || TransformCached.position == Destination)
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
				TransformCached.position -= Velocity;
				remainingRestTimeSeconds -= Time.deltaTime;
			}
			else
			{
				if (Destination != TransformCached.position)
				{
					TransformCached.forward = Destination - TransformCached.position;
				}
			}


			if (this != MainPlayer && (TransformCached.position - MainPlayer.TransformCached.position).sqrMagnitude < Mathf.Pow(0.6f * LevelManager.LevelScale, 2.0f))
			{
				MainPlayer.HandleGameOver();
			}
		}

		wasSearchingForMainPlayer = SearchingForMainPlayer;
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
		if (detectingMainPlayer)
		{
			EnemyAnimator.Play("EasyAngryTuxedoEnemySearchingAnimation");
		}
		else
		{
			VisitNextPatrolPathDestination();
		}
	}

	void HandleLightStateChanged()
	{
		LightSwitch lightSwitch = LevelManager.CurrentLevel.LightSourceMap[LightSource];
        if (lightSwitch != null)
        {
            if (!IsLightActive && LightSource != '-')
            {
                if (!detectingMainPlayer)
                {
                    CurrentPatrolPathDestinationIndex--;
                }
                lightSwitch.Interact(this);
            }
            else if (Target as AbstractGameObject == lightSwitch)
            {
                VisitNextPatrolPathDestination();
            }
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
		if (!detectingMainPlayer && IsLightActive)
		{
			CurrentPatrolPathDestinationIndex--;
		}
		SetDestinationTarget(new DestinationTarget(MainPlayer.TransformCached.position));
		remainingRestTimeSeconds = 0.0f;
	}

	protected override void OnBecomeDetected() { }

	protected override void OnBecomeUndetected() { }

	protected override void HandleDetection() { }
}
