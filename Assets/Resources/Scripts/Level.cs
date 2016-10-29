using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level {

	public ArrayList LevelGrid { get; private set; }
	public ArrayList LevelEnvironmentObjects { get; set; }

	// Weak references
	public Dictionary<char, ArrayList> LightSourceMap { get; set; }

	public Level ()	{
		LevelGrid = new ArrayList ();
		LevelEnvironmentObjects = new ArrayList ();
		LightSourceMap = new Dictionary<char, ArrayList> ();
	}

	public void Destroy() {
		
		foreach (ArrayList row in LevelGrid) {
			foreach (Object obj in row) {
				MonoBehaviour.Destroy (obj);
			}
			row.Clear ();
		}
		LevelGrid.Clear ();

		foreach (Object obj in LevelEnvironmentObjects) {
			MonoBehaviour.Destroy (obj);
		}
		LevelEnvironmentObjects.Clear ();

		LightSourceMap.Clear ();
	}

	public int AddRow(ArrayList row) {

		return LevelGrid.Add (row);
	}

	public int AddToRow(int row, object value) {

		return (LevelGrid [row] as ArrayList).Add (value);
	}
}
