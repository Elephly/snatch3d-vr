﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level
{
	public float LevelScale { get; private set; }
	public Vector3 GoalLocation { get; set; }

	// Strong references
	public ArrayList LevelGrid { get; private set; }
	public ArrayList LevelEnvironmentObjects { get; set; }
	public ArrayList LevelEnemies { get; set; }

	// Weak references
	public Dictionary<char, GameObject> LightSourceMap { get; set; }
	public Dictionary<char, ArrayList> LightSourceListenerMap { get; set; }
	public Dictionary<Vector3, Obstruction> ObstructionMap { get; set; }

	public Level(float levelScale)
	{
		LevelScale = levelScale;
		GoalLocation = Vector3.zero;
		LevelGrid = new ArrayList();
		LevelEnvironmentObjects = new ArrayList();
		LevelEnemies = new ArrayList();
		LightSourceMap = new Dictionary<char, GameObject>();
		LightSourceListenerMap = new Dictionary<char, ArrayList>();
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

		foreach (Object obj in LevelEnemies)
		{
			MonoBehaviour.Destroy(obj);
		}
		LevelEnemies.Clear();

		LightSourceMap.Clear();

		LightSourceListenerMap.Clear();

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

	public object GetGameObjectAtRowColumnIndex(int row, int column)
	{
		object go = null;
		int rowCount = LevelGrid.Count;

		if (row >= 0 && column >= 0 && rowCount > row && LevelGrid[rowCount - 1 - row] is ArrayList && (LevelGrid[rowCount - 1 - row] as ArrayList).Count > column)
		{
			go = (LevelGrid[rowCount - 1 - row] as ArrayList)[column];
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
			foreach (GameObject lightSourceListener in LightSourceListenerMap[lightSource])
			{
				lightSourceListener.SendMessage("SetLightActive", state);
			}
		}
	}

	public void ToggleLight(char lightSource)
	{

		if (LightSourceListenerMap.ContainsKey(lightSource))
		{
			foreach (GameObject lightSourceListener in LightSourceListenerMap[lightSource])
			{
				lightSourceListener.SendMessage("ToggleLight");
			}
		}
	}
}
