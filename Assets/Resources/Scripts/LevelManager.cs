using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

using SimpleJSON;

public static class LevelManager {

	public static GameObject PlayerGameObject { get; private set; }
	public static int LevelNumber { get; private set; }
	public static float LevelScale { get; private set; }
	public static Level LevelStructure { get; private set; }

	public static void Initialize(GameObject playerGameObject, float levelScale) {
		if (PlayerGameObject == null) {
			PlayerGameObject = playerGameObject;
			LevelScale = levelScale;
			LevelNumber = 0;
			LevelStructure = new Level ();
		}
	}


	public static void LoadLevel(int level) {
		LevelNumber = level;
		/* Path.Combine with string array parameter is unsupported for some reason on Mac OS X */
		//string levelDesignPath = Path.Combine ("Assets", "Snatch-VR", "LevelDesigns", "Level" + level + ".level");
		/* Using custom Path.Combine workaround */
		string levelDesignPath = Utils.Path.Combine ("Assets", "Resources", "LevelDesigns", "Level" + level + ".json");
		JSONNode levelDesign = JSON.Parse (File.ReadAllText (levelDesignPath));
		bool playerStartSpace = false;

		LevelStructure.Destroy ();

		int i = 0;
		foreach (var row in levelDesign["Grid"].AsArray) {

			int rowIndex = (levelDesign["Grid"].Count - 1) - i;
			LevelStructure.AddRow (new ArrayList ());

			foreach (var column in row.ToString().Trim('"').ToCharArray().Select((value, index) => new {value, index})) {

				string assetPrefabPath = Utils.Path.Combine ("Prefabs");
				string assetPath = null;
				Quaternion doorRotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);

				switch (column.value) {
				case 'W':
				// Wall
					assetPath = Utils.Path.Combine (assetPrefabPath, "WallTiles", "BrickWallTile");
					GameObject wall = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					wall.transform.position = new Vector3 (column.index * LevelScale, 0.0f, rowIndex * LevelScale);
					wall.transform.localScale *= LevelScale;
					LevelStructure.AddToRow (i, wall);
					break;
				case 'd':
				// Door left-right
					doorRotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
					goto case 'D';
				case 'D':
				// Door forward-backward
					assetPath = Utils.Path.Combine (assetPrefabPath, "DoorTiles", "WoodenDoorTile");
					GameObject door = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					door.transform.position = new Vector3 (column.index * LevelScale, 0.0f, rowIndex * LevelScale);
					door.transform.rotation = doorRotation;
					door.transform.localScale *= LevelScale;
					LevelStructure.LevelEnvironmentObjects.Add (door);
					goto case 'F';
				case '^':
				// Does not work with Google VR SDK
				// Start Facing Forward
					if (!playerStartSpace) {
						PlayerGameObject.transform.rotation = Quaternion.Euler (0.0f, 0.0f, 0.0f);
					}
					goto case 'S';
				case 'v':
				// Does not work with Google VR SDK
				// Start Facing Backward
					if (!playerStartSpace) {
						PlayerGameObject.transform.rotation = Quaternion.Euler (0.0f, 180.0f, 0.0f);
					}
					goto case 'S';
				case '<':
				// Does not work with Google VR SDK
				// Start Facing Left
					if (!playerStartSpace) {
						PlayerGameObject.transform.rotation = Quaternion.Euler (0.0f, -90.0f, 0.0f);
					}
					goto case 'S';
				case '>':
				// Does not work with Google VR SDK
				// Start Facing Right
					if (!playerStartSpace) {
						PlayerGameObject.transform.rotation = Quaternion.Euler (0.0f, 90.0f, 0.0f);
					}
					goto case 'S';
				case 'S':
				// Start
					if (!playerStartSpace) {
						PlayerGameObject.transform.position = new Vector3 (column.index * LevelScale, 0.0f, rowIndex * LevelScale);
						playerStartSpace = true;
					}
					goto case 'F';
				case 'G':
					// Goal
					assetPath = Utils.Path.Combine (assetPrefabPath, "Effects", "GoalSpaceTileEffect");
					GameObject goal = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					goal.transform.position = new Vector3 (column.index * LevelScale, 0.0f, rowIndex * LevelScale);
					goal.transform.localScale *= LevelScale;
					LevelStructure.LevelEnvironmentObjects.Add (goal);
					goto case 'F';
				case 'F':
				// Floor
					assetPath = Utils.Path.Combine (assetPrefabPath, "SpaceTiles", "ConcreteFloorTiledCeilingSpaceTile");
					GameObject floor = MonoBehaviour.Instantiate (Resources.Load (assetPath)) as GameObject;
					floor.transform.position = new Vector3 (column.index * LevelScale, 0.0f, rowIndex * LevelScale);
					floor.transform.localScale *= LevelScale;
					LevelStructure.AddToRow (i, floor);
					break;
				/*
				case 'L':
				// Light Switch
					break;
				case 'U':
				// Door Unlock Switch
					break;
				case 'E':
				// Enemy
					break;
				*/
				default:
					break;
				}
			}
			i++;
		}

		i = 0;
		foreach (var row in levelDesign["LightMap"].AsArray) {

			foreach (var column in row.ToString().Trim('"').ToCharArray().Select((value, index) => new {value, index})) {
				if (column.value == '-') {
					continue;
				}
				if (!LevelStructure.LightMap.ContainsKey (column.value)) {
					LevelStructure.LightMap [column.value] = new ArrayList ();
				}
				var spaceTile = (LevelStructure.LevelGrid [i] as ArrayList) [column.index];
				(spaceTile as GameObject).SendMessage ("SetMapKey", column.value);
				LevelStructure.LightMap [column.value].Add (spaceTile);
			}
			i++;
		}
		if (!playerStartSpace) {
			Debug.LogException (new System.Exception ("No player start position."));
			Application.Quit ();
		}
	}
}

