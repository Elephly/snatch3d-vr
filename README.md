# snatch3d-vr
Computer Science Undergraduate Honours Project - A virtual reality puzzle platformer game!

### Known Issues
- Sometimes enemy, when chasing player, will approach player very close then turn in a different direction at the last minute as if no longer detecting the player
- Possible to create a stack overflow when touching the light switch within the same relative time span as an enemy's attempt, resulting in an exception being thrown (not game crashing) and the enemy to behave strangely, resuming its patrol path
