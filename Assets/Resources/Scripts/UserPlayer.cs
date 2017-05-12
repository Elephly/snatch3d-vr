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
	public AudioClip NextLevelAudioClip = null;
	public AudioClip GameOverAudioClip = null;

	AudioSource backgroundMusicSource = null;
	AudioSource soundEffectAudioSource = null;
    GameObject whiteScreenOverlayObject = null;
    GameObject blackScreenOverlayObject = null;
    GameObject redScreenOverlayObject = null;
    Material whiteScreenOverlayMaterial = null;
    Material blackScreenOverlayMaterial = null;
    Material redScreenOverlayMaterial = null;
    FADE_TYPE whiteScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	FADE_TYPE blackScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	FADE_TYPE redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	float whiteScreenOverlayFadeSpeed = 0.5f;
	float blackScreenOverlayFadeSpeed = 2.5f;
	float redScreenOverlayFadeSpeed = 1.5f;
	GvrAudioSource footStepsAudioSource = null;
	float lastFootStepElapsedTimeSeconds = footStepInitialOffset;

	protected override void Awake()
	{
		base.Awake();
        Cursor.lockState = CursorLockMode.Locked;
		backgroundMusicSource = GetComponent<AudioSource>();
		soundEffectAudioSource = TransformCached.FindBreadthFirst("SoundEffects").GetComponent<AudioSource>();
		Transform whiteScreenOverlay = TransformCached.FindBreadthFirst("WhiteScreenOverlay");
		if (whiteScreenOverlay != null)
		{
			whiteScreenOverlayObject = whiteScreenOverlay.gameObject;
            Renderer whiteScreenOverlayRenderer = whiteScreenOverlayObject.GetComponent<Renderer>();
            if (whiteScreenOverlayRenderer != null)
            {
                whiteScreenOverlayMaterial = whiteScreenOverlayRenderer.material;
                Color col = whiteScreenOverlayMaterial.color;
                whiteScreenOverlayMaterial.color = new Color(col.r, col.g, col.b, 0.0f);
            }
		}
		Transform blackScreenOverlay = TransformCached.FindBreadthFirst("BlackScreenOverlay");
		if (blackScreenOverlay != null)
		{
			blackScreenOverlayObject = blackScreenOverlay.gameObject;
            Renderer blackScreenOverlayRenderer = blackScreenOverlayObject.GetComponent<Renderer>();
            if (blackScreenOverlayRenderer != null)
            {
                blackScreenOverlayMaterial = blackScreenOverlayRenderer.material;
                Color col = blackScreenOverlayMaterial.color;
                blackScreenOverlayMaterial.color = new Color(col.r, col.g, col.b, 0.0f);
            }
		}
		Transform redScreenOverlay = TransformCached.FindBreadthFirst("RedScreenOverlay");
		if (redScreenOverlay != null)
		{
			redScreenOverlayObject = redScreenOverlay.gameObject;
            Renderer redScreenOverlayRenderer = redScreenOverlayObject.GetComponent<Renderer>();
            if (redScreenOverlayRenderer != null)
            {
                redScreenOverlayMaterial = redScreenOverlayRenderer.material;
                Color col = redScreenOverlayMaterial.color;
                redScreenOverlayMaterial.color = new Color(col.r, col.g, col.b, 0.0f);
            }
		}
		footStepsAudioSource = TransformCached.GetComponentInChildren<GvrAudioSource>();
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

        if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();

		ScreenOverlayFade(whiteScreenOverlayMaterial, whiteScreenOverlayFadeType, whiteScreenOverlayFadeSpeed);
		ScreenOverlayFade(blackScreenOverlayMaterial, blackScreenOverlayFadeType, blackScreenOverlayFadeSpeed);
		ScreenOverlayFade(redScreenOverlayMaterial, redScreenOverlayFadeType, redScreenOverlayFadeSpeed);

		if (!mainPlayerCaught)
		{
			TransformCached.position -= Velocity;

			Vector3 playerTileLocationUnrounded = TransformCached.position / LevelManager.LevelScale;
			Vector3 playerTileLocation = new Vector3(Mathf.Round(playerTileLocationUnrounded.x), Mathf.Round(playerTileLocationUnrounded.y), Mathf.Round(playerTileLocationUnrounded.z));
			Vector3 xDirectionTileLocationUnrounded = playerTileLocation + Vector3.Project(Velocity, Vector3.right).normalized;
			Vector3 xDirectionTileLocation = new Vector3(xDirectionTileLocationUnrounded.x, xDirectionTileLocationUnrounded.y, xDirectionTileLocationUnrounded.z);
			AbstractGameObject xDirectionTile = LevelManager.CurrentLevel.GetGameObjectAtRowColumnIndex((int)xDirectionTileLocation.z, (int)xDirectionTileLocation.x);
			Vector3 zDirectionTileLocationUnrounded = playerTileLocation + Vector3.Project(Velocity, Vector3.forward).normalized;
			Vector3 zDirectionTileLocation = new Vector3(zDirectionTileLocationUnrounded.x, zDirectionTileLocationUnrounded.y, zDirectionTileLocationUnrounded.z);
            AbstractGameObject zDirectionTile = LevelManager.CurrentLevel.GetGameObjectAtRowColumnIndex((int)zDirectionTileLocation.z, (int)zDirectionTileLocation.x);

			if (xDirectionTile != null && !xDirectionTile.CompareTag("SpaceTile") &&
				Vector3.Project(xDirectionTile.TransformCached.position - TransformCached.position, Vector3.right).sqrMagnitude <=
				Mathf.Pow(LevelManager.LevelScale, 2))
			{
                Velocity = Vector3.ProjectOnPlane(Velocity, Vector3.right).normalized * Velocity.magnitude;
			}

			if (zDirectionTile != null && !zDirectionTile.CompareTag("SpaceTile") &&
				Vector3.Project(zDirectionTile.TransformCached.position - TransformCached.position, Vector3.forward).sqrMagnitude <=
				Mathf.Pow(LevelManager.LevelScale, 2))
			{
                Velocity = Vector3.ProjectOnPlane(Velocity, Vector3.forward).normalized * Velocity.magnitude;
			}

            Vector3 destDiff = Destination - TransformCached.position;
			if ((Destination - TransformCached.position).sqrMagnitude <= Velocity.sqrMagnitude ||
                ((Destination - TransformCached.position).sqrMagnitude <= 0.75f * LevelManager.LevelScale &&
                Vector2.Dot(new Vector2(destDiff.x, destDiff.z).normalized, new Vector2(Velocity.x, Velocity.z).normalized) < 0.8f))
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

            TransformCached.position += Velocity;

			if ((TransformCached.position - LevelManager.CurrentLevel.GoalLocation).sqrMagnitude < Mathf.Pow(0.5f * LevelManager.LevelScale, 2.0f))
			{
				whiteScreenOverlayFadeType = FADE_TYPE.FADE_IN;
				Color col = whiteScreenOverlayMaterial.color;
				if (col.a >= 1.0f)
				{
					soundEffectAudioSource.PlayOneShot(NextLevelAudioClip);
					LevelManager.LoadNextLevel();
				}
			}
			else
			{
				whiteScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
		}
		}

		if (blackScreenOverlayFadeType == FADE_TYPE.FADE_IN)
		{
			Color col = blackScreenOverlayMaterial.color;
			if (col.a >= 1.0f)
			{
				LevelManager.LoadLevel(LevelManager.LevelNumber);
				blackScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
			}
		}
	}

	public override void SetDestinationTarget(DestinationTarget destinationTarget)
	{
		Destination = TransformCached.position;
		NextDestinations.Clear();
		Vector3 dest = destinationTarget.Destination;
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(TransformCached.position, TransformCached.forward, out hit))
		{
			dest = new Vector3(hit.point.x, hit.transform.root.position.y, hit.point.z);
		}
	    NextDestinations.Push(dest);
		Target = destinationTarget.Target;
	}

	protected override void TargetReached()
	{
		Vector3 oldPosition = TransformCached.position;
		base.TargetReached();
        TransformCached.position = oldPosition;
	}

	void ScreenOverlayFade(Material screenMaterial, FADE_TYPE fadeType, float fadeSpeed)
	{
		Color col = screenMaterial.color;
		float newAlpha = Mathf.Clamp(col.a + Time.deltaTime * (fadeType == FADE_TYPE.FADE_IN ? 1.0f : -1.0f) * fadeSpeed, 0.0f, 1.0f);
		screenMaterial.color = new Color(col.r, col.g, col.b, newAlpha);
	}

	protected override void OnBecomeDetected()
	{
		if (backgroundMusicSource != null && DetectedBackgroundMusicClip != null)
		{
			if (backgroundMusicSource.clip != DetectedBackgroundMusicClip)
				backgroundMusicSource.clip = DetectedBackgroundMusicClip;
			if (!backgroundMusicSource.isPlaying)
			{
				backgroundMusicSource.time = 36.7f;
				backgroundMusicSource.Play();
			}
		}
	}

	protected override void OnBecomeUndetected()
	{
		if (backgroundMusicSource != null && UndetectedBackgroundMusicClip != null)
		{
			if (backgroundMusicSource.clip != UndetectedBackgroundMusicClip)
				backgroundMusicSource.clip = UndetectedBackgroundMusicClip;
			if (!backgroundMusicSource.isPlaying)
				backgroundMusicSource.Play();
		}
		redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
	}

	protected override void HandleDetection()
	{
		Color col = redScreenOverlayMaterial.color;
		if (col.a >= 1.0f)
			redScreenOverlayFadeType = FADE_TYPE.FADE_OUT;
		else if (col.a <= 0.25f)
			redScreenOverlayFadeType = FADE_TYPE.FADE_IN;
	}

	public override void HandleGameOver()
	{
		base.HandleGameOver();
		blackScreenOverlayFadeType = FADE_TYPE.FADE_IN;
		soundEffectAudioSource.PlayOneShot(GameOverAudioClip);
	}
}
