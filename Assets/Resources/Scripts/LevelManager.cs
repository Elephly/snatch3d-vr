using UnityEngine;
using System.IO;
using System.Linq;

public static class LevelManager {

	public static GameObject PlayerGameObject { get; set; }
	public static int Level { get; private set; }

	public static void LoadLevel(int level) {
		Level = level;
		/* Path.Combine with string array parameter is unsupported for some reason on Mac OS X */
		//string levelDesignPath = Path.Combine ("Assets", "Snatch-VR", "LevelDesigns", "Level" + level + ".level");
		/* Using custom Path.Combine workaround */
		string levelDesignPath = Utils.Path.Combine ("Assets", "Resources", "LevelDesigns", "Level" + level + ".level");
		string[] levelDesign = File.ReadAllLines (levelDesignPath);
		bool playerStartSpace = false;
		foreach (var row in levelDesign.Select((value, index) => new {value, index})) {
			foreach (var column in row.value.ToCharArray().Select((value, index) => new {value, index})) {
				string assetPath = Utils.Path.Combine ("Prefabs");
				switch (column.value) {
				case 'W':
					// Wall
					assetPath = Utils.Path.Combine(assetPath, "WallTiles", "BrickWallTile");
					GameObject wall = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					wall.transform.position = new Vector3 (column.index * wall.transform.localScale.x, 0.0f, row.index * wall.transform.localScale.z);
					break;
				case 'S':
					// Start
					playerStartSpace = true;
					goto case 'F';
				case 'F':
					// Floor
					assetPath = Utils.Path.Combine (assetPath, "SpaceTiles", "ConcreteFloorTiledCeilingSpaceTile");
					GameObject floor = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					floor.transform.position = new Vector3 (column.index * floor.transform.localScale.x, 0.0f, row.index * floor.transform.localScale.z);
					if (playerStartSpace) {
						PlayerGameObject.transform.position = new Vector3 (column.index * floor.transform.localScale.x, 0.0f, row.index * floor.transform.localScale.z);
						playerStartSpace = false;
					}
					break;
				case 'L':
					// Light Switch
					break;
				case 'U':
					// Door Unlock Switch
					break;
				case 'D':
					// Door
					break;
				case 'E':
					// Enemy
					break;
				case 'G':
					// Goal
					break;
				default:
					break;
				}
			}
		}
		if (!playerStartSpace) {
			Debug.LogException (new System.Exception ("No player start position."));
			Application.Quit ();
		}
	}
}

