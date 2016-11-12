public class UserPlayer : Player
{
	void Start()
	{
		LevelManager.Initialize(gameObject, 2.0f);
		LevelManager.LoadLevel(0);
	}

	// Update is called once per frame
	protected override void Update()
	{
		base.Update();

		if ((transform.position - LevelManager.CurrentLevel.GoalLocation).sqrMagnitude < 0.5f)
		{
			LevelManager.LoadNextLevel();
		}
	}
}
