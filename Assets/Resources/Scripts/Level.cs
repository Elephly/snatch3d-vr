using UnityEngine;
using System.Collections.Generic;

public class Level
{
	public float LevelScale { get; private set; }
	public Vector3 GoalLocation { get; set; }

	// Strong references
	public List<List<AbstractGameObject>> LevelGrid { get; private set; }
	public List<AbstractGameObject> LevelEnvironmentObjects { get; set; }
	public List<EnemyPlayer> LevelEnemies { get; set; }

	// Weak references
	public Dictionary<char, LightSwitch> LightSourceMap { get; set; }
	public Dictionary<char, List<ILightSourceListener>> LightSourceListenerMap { get; set; }
	public Dictionary<Vector3, Obstruction> ObstructionMap { get; set; }
    public Dictionary<Vector3, int> SpacePlayerOccupations { get; set; }

	public Level(float levelScale)
	{
		LevelScale = levelScale;
		GoalLocation = Vector3.zero;
		LevelGrid = new List<List<AbstractGameObject>>();
		LevelEnvironmentObjects = new List<AbstractGameObject>();
		LevelEnemies = new List<EnemyPlayer>();
		LightSourceMap = new Dictionary<char, LightSwitch>();
		LightSourceListenerMap = new Dictionary<char, List<ILightSourceListener>>();
		ObstructionMap = new Dictionary<Vector3, Obstruction>();
        SpacePlayerOccupations = new Dictionary<Vector3, int>();
	}

	public void Destroy()
	{
		foreach (var row in LevelGrid)
		{
			foreach (var obj in row)
			{
				MonoBehaviour.Destroy(obj.gameObject);
			}
			row.Clear();
		}
		LevelGrid.Clear();

		foreach (var obj in LevelEnvironmentObjects)
		{
			MonoBehaviour.Destroy(obj.gameObject);
		}
		LevelEnvironmentObjects.Clear();

		foreach (var obj in LevelEnemies)
		{
			MonoBehaviour.Destroy(obj.gameObject);
		}
		LevelEnemies.Clear();

		LightSourceMap.Clear();

		LightSourceListenerMap.Clear();

		ObstructionMap.Clear();

        SpacePlayerOccupations.Clear();
    }

	public void AddRow(List<AbstractGameObject> row)
	{
		LevelGrid.Add(row);
	}

	public void AddToRow(int row, AbstractGameObject value)
	{
		LevelGrid[row].Add(value);
	}

	public AbstractGameObject GetGameObjectAtRowColumnIndex(int row, int column)
	{
		AbstractGameObject go = null;
		int rowCount = LevelGrid.Count;

		if (row >= 0 && column >= 0 && rowCount > row && LevelGrid[rowCount - 1 - row] != null && LevelGrid[rowCount - 1 - row].Count > column)
		{
			go = LevelGrid[rowCount - 1 - row][column];
		}
		return go;
	}

	public bool HasObstruction(Vector3 position)
	{
		return (ObstructionMap.ContainsKey(position) && ObstructionMap[position].IsObstructing());
	}

	public void SetLightActive(char lightSource, bool state)
	{

		if (LightSourceListenerMap.ContainsKey(lightSource))
		{
			foreach (ILightSourceListener lightSourceListener in LightSourceListenerMap[lightSource])
			{
				lightSourceListener.SetLightActive(state);
			}
		}
	}

	public void ToggleLight(char lightSource)
	{
		if (LightSourceListenerMap.ContainsKey(lightSource))
		{
			foreach (ILightSourceListener lightSourceListener in LightSourceListenerMap[lightSource])
			{
				lightSourceListener.ToggleLight();
			}
		}
	}

    public bool SpaceOccupiedByPlayer(Vector3 location)
    {
        Vector2 loc = new Vector2(Mathf.Round(location.x / LevelScale), Mathf.Round(location.z / LevelScale));
        return SpacePlayerOccupations.ContainsKey(loc) && SpacePlayerOccupations[loc] > 0;
    }

    public void UpdatePlayerLocation(Player player, Vector3 oldPosition, Vector3 newPosition)
    {
        Vector2 oldPos = new Vector2(Mathf.Round(oldPosition.x / LevelScale), Mathf.Round(oldPosition.z / LevelScale));
        Vector2 newPos = new Vector2(Mathf.Round(newPosition.x / LevelScale), Mathf.Round(newPosition.z / LevelScale));
        if (SpacePlayerOccupations.ContainsKey(oldPos))
            SpacePlayerOccupations[oldPos] = Mathf.Max(SpacePlayerOccupations[oldPos] - 1, 0);
        if (!SpacePlayerOccupations.ContainsKey(newPos))
            SpacePlayerOccupations[newPos] = 0;
        SpacePlayerOccupations[newPos]++;
    }
}
