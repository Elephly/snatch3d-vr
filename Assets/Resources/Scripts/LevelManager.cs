using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;

using SimpleJSON;

public static class LevelManager
{

	public static GameObject PlayerGameObject { get; private set; }
	public static int LevelNumber { get; private set; }
	public static float LevelScale { get; private set; }
	public static Level CurrentLevel { get; private set; }

	public static void Initialize(GameObject playerGameObject, float levelScale)
	{
		if (PlayerGameObject == null)
		{
			PlayerGameObject = playerGameObject;
			LevelScale = levelScale;
			LevelNumber = 0;
			CurrentLevel = new Level();
		}
	}

	public static void LoadLevel(int level)
	{
		LevelNumber = level;
		/* Path.Combine with string array parameter is unsupported for some reason on Mac OS X */
		//string levelDesignPath = Path.Combine ("Assets", "Snatch-VR", "LevelDesigns", "Level" + level + ".level");
		/* Using custom Path.Combine workaround */
		string levelDesignPath = Utils.Path.Combine("LevelDesigns", "Level" + level);
		TextAsset levelJSON = Resources.Load<TextAsset>(levelDesignPath);
		if (levelJSON == null)
		{
			if (level != 0)
			{
				LoadLevel(0);
			}
			return;
		}
		JSONNode levelDesign = JSON.Parse(levelJSON.text);
		bool playerStartSpace = false;

		CurrentLevel.Destroy();

		int i = 0;
		foreach (var row in levelDesign["Grid"].AsArray)
		{

			int rowIndex = (levelDesign["Grid"].Count - 1) - i;
			CurrentLevel.AddRow(new ArrayList());

			foreach (var column in row.ToString().Trim('"').ToCharArray().Select((value, index) => new { value, index }))
			{

				string assetPrefabPath = Utils.Path.Combine("Prefabs");
				string assetPath = null;
				Quaternion doorRotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
				Vector3 rowColumnPosition = new Vector3(column.index * LevelScale, 0.0f, rowIndex * LevelScale);

				switch (column.value)
				{
					case 'W':
						// Wall
						assetPath = Utils.Path.Combine(assetPrefabPath, "WallTiles", "BrickWallTile");
						GameObject wall = MonoBehaviour.Instantiate(Resources.Load(assetPath)) as GameObject;
						wall.transform.position = rowColumnPosition;
						wall.transform.localScale *= LevelScale;
						CurrentLevel.AddToRow(i, wall);
						break;
					case 'd':
						// Door left-right
						doorRotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
						goto case 'D';
					case 'D':
						// Door forward-backward
						assetPath = Utils.Path.Combine(assetPrefabPath, "Doors", "WoodenDoor");
						GameObject door = MonoBehaviour.Instantiate(Resources.Load(assetPath)) as GameObject;
						door.transform.position = rowColumnPosition;
						door.transform.rotation = doorRotation;
						door.transform.localScale *= LevelScale;
						CurrentLevel.LevelEnvironmentObjects.Add(door);
						CurrentLevel.ObstructionMap[rowColumnPosition] = door.GetComponent<Door>();
						goto case 'F';
					case '^':
						// Does not work with Google VR SDK
						// Start Facing Forward
						if (!playerStartSpace)
						{
							PlayerGameObject.transform.rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
						}
						goto case 'S';
					case 'v':
						// Does not work with Google VR SDK
						// Start Facing Backward
						if (!playerStartSpace)
						{
							PlayerGameObject.transform.rotation = Quaternion.Euler(0.0f, 180.0f, 0.0f);
						}
						goto case 'S';
					case '<':
						// Does not work with Google VR SDK
						// Start Facing Left
						if (!playerStartSpace)
						{
							PlayerGameObject.transform.rotation = Quaternion.Euler(0.0f, -90.0f, 0.0f);
						}
						goto case 'S';
					case '>':
						// Does not work with Google VR SDK
						// Start Facing Right
						if (!playerStartSpace)
						{
							PlayerGameObject.transform.rotation = Quaternion.Euler(0.0f, 90.0f, 0.0f);
						}
						goto case 'S';
					case 'S':
						// Start
						if (!playerStartSpace)
						{
							PlayerGameObject.transform.position = rowColumnPosition;
							PlayerGameObject.SendMessage("SetDestinationTarget", new DestinationTarget(rowColumnPosition));
							playerStartSpace = true;
						}
						goto case 'F';
					case 'G':
						// Goal
						assetPath = Utils.Path.Combine(assetPrefabPath, "Effects", "GoalSpaceTileEffect");
						GameObject goal = MonoBehaviour.Instantiate(Resources.Load(assetPath)) as GameObject;
						goal.transform.position = rowColumnPosition;
						goal.transform.localScale *= LevelScale;
						CurrentLevel.LevelEnvironmentObjects.Add(goal);
						CurrentLevel.GoalLocation = rowColumnPosition;
						goto case 'F';
					case 'F':
						// Floor
						assetPath = Utils.Path.Combine(assetPrefabPath, "SpaceTiles", "ConcreteFloorTiledCeilingSpaceTile");
						GameObject floor = MonoBehaviour.Instantiate(Resources.Load(assetPath)) as GameObject;
						floor.transform.position = rowColumnPosition;
						floor.transform.localScale *= LevelScale;
						CurrentLevel.AddToRow(i, floor);
						break;
					/*
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
		foreach (var row in levelDesign["LightSourceMap"].AsArray)
		{

			foreach (var column in row.ToString().Trim('"').ToCharArray().Select((value, index) => new { value, index }))
			{
				if (column.value == '-')
				{
					continue;
				}
				if (!CurrentLevel.LightSourceMap.ContainsKey(column.value))
				{
					CurrentLevel.LightSourceMap[column.value] = new ArrayList();
				}
				var spaceTile = (CurrentLevel.LevelGrid[i] as ArrayList)[column.index];
				(spaceTile as GameObject).SendMessage("SetLightSource", column.value);
				CurrentLevel.LightSourceMap[column.value].Add(spaceTile);
			}
			i++;
		}

		foreach (JSONNode lightSwitch in levelDesign["LightSwitches"].AsArray)
		{

			string assetPath = Utils.Path.Combine("Prefabs", "Switches", "LightSwitch");
			GameObject light = MonoBehaviour.Instantiate(Resources.Load(assetPath)) as GameObject;
			light.transform.position = new Vector3(lightSwitch["Position"]["X"].AsFloat * LevelScale, 0.0f, lightSwitch["Position"]["Y"].AsFloat * LevelScale);
			light.transform.rotation = Quaternion.Euler(0.0f, lightSwitch["Yaw"].AsFloat, 0.0f);
			light.transform.localScale *= LevelScale;
			light.SendMessage("SetLightSource", lightSwitch["LightSource"].ToString().Trim('"').ToCharArray()[0]);
			CurrentLevel.LevelEnvironmentObjects.Add(light);
		}

		if (!playerStartSpace)
		{
			Debug.LogException(new System.Exception("No player start position."));
			Application.Quit();
		}
	}

	public static void LoadNextLevel()
	{
		LoadLevel(LevelNumber + 1);
	}
}

