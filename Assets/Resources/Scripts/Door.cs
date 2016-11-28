using UnityEngine;

public class Door : Obstruction
{
	Animator DoorAnimator = null;
	public bool IsDoorOpen = false;
	public bool IsLocked
	{
		get
		{
			return (UnlockSwitch != null && UnlockSwitch.IsLocked);
		}
	}
	public bool IsTransitioning = false;
	public DoorUnlockSwitch UnlockSwitch { get; private set; }

	public override bool IsObstructing()
	{
		return !IsDoorOpen;
	}

	void Awake()
	{
		DoorAnimator = GetComponent<Animator>();
		UnlockSwitch = null;
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
			}
			else {
				if (!IsLocked)
				{
					DoorAnimator.Play("DoorOpenAnimation");
				}
			}
		}
	}

	public void Interact(GameObject sender)
	{
		ToggleDoorState();
	}
}
