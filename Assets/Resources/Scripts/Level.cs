using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{

	public float LevelScale { get; private set; }
	public ArrayList LevelGrid { get; private set; }
	public ArrayList LevelEnvironmentObjects { get; set; }
	public Vector3 GoalLocation { get; set; }

	// Weak references
	public Dictionary<char, ArrayList> LightSourceMap { get; set; }
	public Dictionary<Vector3, Obstruction> ObstructionMap { get; set; }

	public Level(float levelScale)
	{
		LevelScale = levelScale;
		LevelGrid = new ArrayList();
		LevelEnvironmentObjects = new ArrayList();
		LightSourceMap = new Dictionary<char, ArrayList>();
		ObstructionMap = new Dictionary<Vector3, Obstruction>();
	}

	public void Destroy()
	{

		foreach (ArrayList row in LevelGrid)
		{
			foreach (Object obj in row)
			{
				MonoBehaviour.Destroy(obj);
			}
			row.Clear();
		}
		LevelGrid.Clear();

		foreach (Object obj in LevelEnvironmentObjects)
		{
			MonoBehaviour.Destroy(obj);
		}
		LevelEnvironmentObjects.Clear();

		LightSourceMap.Clear();

		ObstructionMap.Clear();
	}

	public int AddRow(ArrayList row)
	{

		return LevelGrid.Add(row);
	}

	public int AddToRow(int row, object value)
	{

		return (LevelGrid[row] as ArrayList).Add(value);
	}

	public bool HasObstruction(Vector3 position)
	{
		return (ObstructionMap.ContainsKey(position) && ObstructionMap[position].IsObstructing());
	}

	public void SetLightActive(char lightSource, bool state)
	{

		if (LightSourceMap.ContainsKey(lightSource))
		{
			foreach (GameObject light in LightSourceMap[lightSource])
			{
				light.SendMessage("SetLightActive", state);
			}
		}
	}

	public void ToggleLight(char lightSource)
	{

		if (LightSourceMap.ContainsKey(lightSource))
		{
			foreach (GameObject light in LightSourceMap[lightSource])
			{
				light.SendMessage("ToggleLight");
			}
		}
	}
}
