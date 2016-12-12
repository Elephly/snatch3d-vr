using UnityEngine;

public class UserPlayer : Player
{
	enum FADE_TYPE
	{
		FADE_IN,
		FADE_OUT
	}
	
	const float footStepIntervalSeconds = 0.4f;
	const float footStepInitialOffset = footStepIntervalSeconds / 2.0f;

	public AudioClip UndetectedBackgroundMusicClip = null;
	public AudioClip DetectedBackgroundMusicClip = null;

	AudioSource backgroundMusicSource = null;
	GameObject redScreenOverlayObject = null;
	FADE_TYPE redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	float redScreenOverlayFadeSpeed = 1.5f;
	GvrAudioSource footStepsAudioSource = null;
	float lastFootStepElapsedTimeSeconds = footStepInitialOffset;

	protected override void Awake()
	{
		base.Awake();
		backgroundMusicSource = GetComponent<AudioSource>();
		Transform redScreenOverlay = transform.FindBreadthFirst("RedScreenOverlay");
		if (redScreenOverlay != null)
		{
			redScreenOverlayObject = redScreenOverlay.gameObject;
			Color col = redScreenOverlayObject.GetComponent<Renderer>().material.color;
			redScreenOverlayObject.GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, 0.0f);
		}
		footStepsAudioSource = transform.GetComponentInChildren<GvrAudioSource>();
		MainPlayer = this;
	}

	void Start()
	{
		LevelManager.Initialize(gameObject, 2.0f);
		LevelManager.LoadLevel(0);
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		ScreenOverlayFade(redScreenOverlayObject, redScreenOverlayFadeType, redScreenOverlayFadeSpeed);

		transform.position -= Velocity;

		Vector3 playerTileLocationUnrounded = transform.position / LevelManager.LevelScale;
		Vector3 playerTileLocation = new Vector3(Mathf.Round(playerTileLocationUnrounded.x), Mathf.Round(playerTileLocationUnrounded.y), Mathf.Round(playerTileLocationUnrounded.z));
		Vector3 xDirectionTileLocationUnrounded = playerTileLocation + Vector3.Project(Velocity, Vector3.right).normalized;
		Vector3 xDirectionTileLocation = new Vector3(xDirectionTileLocationUnrounded.x, xDirectionTileLocationUnrounded.y, xDirectionTileLocationUnrounded.z);
		object xDirectionTile = LevelManager.CurrentLevel.GetGameObjectAtRowColumnIndex((int)xDirectionTileLocation.z, (int)xDirectionTileLocation.x);
		Vector3 zDirectionTileLocationUnrounded = playerTileLocation + Vector3.Project(Velocity, Vector3.forward).normalized;
		Vector3 zDirectionTileLocation = new Vector3(zDirectionTileLocationUnrounded.x, zDirectionTileLocationUnrounded.y, zDirectionTileLocationUnrounded.z);
		object zDirectionTile = LevelManager.CurrentLevel.GetGameObjectAtRowColumnIndex((int)zDirectionTileLocation.z, (int)zDirectionTileLocation.x);

		if (xDirectionTile is GameObject && !(xDirectionTile as GameObject).CompareTag("SpaceTile") &&
			Vector3.Project((xDirectionTile as GameObject).transform.position - transform.position, Vector3.right).sqrMagnitude <=
			Mathf.Pow(LevelManager.LevelScale, 2))
		{
			GameObject xTileGO = xDirectionTile as GameObject;
			float repelX = (xTileGO.transform.position - (Vector3.Project(xTileGO.transform.position - transform.position, Vector3.right).normalized * LevelManager.LevelScale)).x;
			//transform.position = new Vector3(repelX, transform.position.y, transform.position.z);
			Velocity = Vector3.ProjectOnPlane(Velocity, Vector3.right);
		}

		if (zDirectionTile is GameObject && !(zDirectionTile as GameObject).CompareTag("SpaceTile") &&
			Vector3.Project((zDirectionTile as GameObject).transform.position - transform.position, Vector3.forward).sqrMagnitude <=
			Mathf.Pow(LevelManager.LevelScale, 2))
		{
			GameObject zTileGO = zDirectionTile as GameObject;
			float repelZ = (zTileGO.transform.position - (Vector3.Project(zTileGO.transform.position - transform.position, Vector3.forward).normalized * LevelManager.LevelScale)).z;
			//transform.position = new Vector3(transform.position.x, transform.position.y, repelZ);
			Velocity = Vector3.ProjectOnPlane(Velocity, Vector3.forward);
		}

		if ((Destination - transform.position).sqrMagnitude <= Mathf.Pow(0.725f * LevelManager.LevelScale, 2))
		{
			DestinationReached();
			lastFootStepElapsedTimeSeconds = footStepInitialOffset;
		}
		else
		{
			if (lastFootStepElapsedTimeSeconds >= footStepIntervalSeconds && !footStepsAudioSource.isPlaying)
			{
				footStepsAudioSource.Play();
				lastFootStepElapsedTimeSeconds = 0.0f;
			}
			lastFootStepElapsedTimeSeconds += Time.deltaTime;
		}

		transform.position += Velocity;

		if ((transform.position - LevelManager.CurrentLevel.GoalLocation).sqrMagnitude < 0.25f * LevelManager.LevelScale)
		{
			LevelManager.LoadNextLevel();
		}
	}

	public override void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		Destination = transform.position;
		NextDestinations.Clear();
		Vector3 dest = destinationTarget.Destination;
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(transform.position, transform.forward, out hit))
		{
			dest = new Vector3(hit.point.x, hit.transform.root.position.y, hit.point.z);
		}
	    NextDestinations.Push(dest);
		Target = destinationTarget.Target;
	}

	protected override void TargetReached()
	{
		Vector3 oldPosition = transform.position;
		base.TargetReached();
		transform.position = oldPosition;
	}

	void ScreenOverlayFade(GameObject screen, FADE_TYPE fadeType, float fadeSpeed)
	{
		Color col = screen.GetComponent<Renderer>().material.color;
		float newAlpha = Mathf.Clamp(col.a + Time.deltaTime * (fadeType == FADE_TYPE.FADE_IN ? 1.0f : -1.0f) * fadeSpeed, 0.0f, 1.0f);
		screen.GetComponent<Renderer>().material.color = new Color(col.r, col.g, col.b, newAlpha);
	}

	protected override void OnBecomeDetected()
	{
		if (backgroundMusicSource != null && DetectedBackgroundMusicClip != null)
		{
			backgroundMusicSource.clip = DetectedBackgroundMusicClip;
			backgroundMusicSource.time = 36.7f;
			backgroundMusicSource.Play();
		}
	}

	protected override void OnBecomeUndetected()
	{
		if (backgroundMusicSource != null && UndetectedBackgroundMusicClip != null)
		{
			backgroundMusicSource.clip = UndetectedBackgroundMusicClip;
			backgroundMusicSource.Play();
		}
		redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	}

	protected override void HandleDetection()
	{
		Color col = redScreenOverlayObject.GetComponent<Renderer>().material.color;
		if (col.a >= 1.0f)
			redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
		else if (col.a <= 0.25f)
			redScreenOverlayFadeType = FADE_TYPE.FADE_IN;
	}
}
