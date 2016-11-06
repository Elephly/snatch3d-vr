using UnityEngine;

public class Door : Obstruction
{
	Animator DoorAnimator = null;
	public bool IsDoorOpen = false;
	public bool IsLocked { get; private set; }
	public bool IsTransitioning = false;

	public override bool IsObstructing()
	{
		return !IsDoorOpen;
	}

	void Awake()
	{
		DoorAnimator = GetComponent<Animator>();
		IsLocked = true;
	}

	public void SetLockedState(bool state)
	{
		IsLocked = state;
	}

	public void ToggleDoorState()
	{
		if (!IsLocked && !DoorAnimator.IsPlaying())
		{
			if (IsDoorOpen)
			{
				DoorAnimator.Play("DoorCloseAnimation");
			}
			else {
				DoorAnimator.Play("DoorOpenAnimation");
			}
		}
	}

	public void Interact()
	{
		ToggleDoorState();
	}
}
