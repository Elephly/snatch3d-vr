using UnityEngine;

public class Door : Obstruction
{
	Animator DoorAnimator = null;
	GvrAudioSource doorAudioSource = null;
	bool wasTransitioning = false;

	public AudioClip doorOpenCloseAudioClip;
	public AudioClip doorFinishOpenCloseAudioClip;
	public AudioClip doorLockedAudioClip;
	public bool IsDoorOpen = false;
	public bool IsLocked
	{
		get
		{
			return (UnlockSwitch != null && UnlockSwitch.IsLocked);
		}
	}
	public DoorUnlockSwitch UnlockSwitch { get; private set; }

	public override bool IsObstructing()
	{
		return !IsDoorOpen;
	}

	void Awake()
	{
		DoorAnimator = GetComponent<Animator>();
		UnlockSwitch = null;
		doorAudioSource = transform.GetComponentInChildren<GvrAudioSource>();
	}

	void Update()
	{
		if (!DoorAnimator.IsPlaying())
		{
			if (wasTransitioning)
			{
				doorAudioSource.Stop();
				doorAudioSource.gainDb = 20.0f;
				doorAudioSource.PlayOneShot(doorFinishOpenCloseAudioClip);
			}
		}
		if (DoorAnimator.GetCurrentAnimatorStateInfo(0).IsName("DoorClosedStateAnimation"))
		{
			wasTransitioning = false;
		}
		else
		{
			wasTransitioning = DoorAnimator.IsPlaying();
		}
	}

	public void SetUnlockSwitch(DoorUnlockSwitch unlockSwitch)
	{
		UnlockSwitch = unlockSwitch;
	}

	public void ToggleDoorState()
	{
		if (!DoorAnimator.IsPlaying())
		{
			if (IsDoorOpen)
			{
				DoorAnimator.Play("DoorCloseAnimation");
				doorAudioSource.gainDb = 8.0f;
				doorAudioSource.PlayOneShot(doorOpenCloseAudioClip);
			}
			else {
				if (!IsLocked)
				{
					DoorAnimator.Play("DoorOpenAnimation");
					doorAudioSource.gainDb = 8.0f;
					doorAudioSource.PlayOneShot(doorOpenCloseAudioClip);
				}
				else
				{
					doorAudioSource.gainDb = 8.0f;
					doorAudioSource.PlayOneShot(doorLockedAudioClip);
				}
			}
		}
	}

	public void Interact(GameObject sender)
	{
		ToggleDoorState();
	}
}
