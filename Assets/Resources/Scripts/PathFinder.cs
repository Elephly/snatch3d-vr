using UnityEngine;
using System.Collections.Generic;

public static class PathFinder
{
	public static List<Vector3> Dijkstra(Level level, Vector3 origin, Vector3 destination)
	{
		// Input: Graph G = (V, E), starting point s in V, positive length { le: e in E }
		// Output: For all vertices u reachable from s, return distance from s to u
		//
		// for all u in V
		//   dist(u) = inf
		//   prev(u) = nil
		// dist(s) = u
		//
		// H = makequeue(V) (using dist values as keys)
		// while H is not empty
		//   u = deletemin(H)
		//   for all edges(u, v) in E
		//     if dist(v) > dist(u) + l(u, v)
		//       dist(v) = dist(u) + l(u, v)
		//       prev(v) = u
		//       decreasekey(H, v)

		List<Vector3> path = new List<Vector3>();
		Vector3 orig = (origin / level.LevelScale);
		orig.x = Mathf.Round(orig.x);
		orig.y = Mathf.Round(orig.y);
		orig.z = Mathf.Round(orig.z);
		orig *= level.LevelScale;
		Dictionary<Vector3, AbstractGameObject> positionObjectMap = new Dictionary<Vector3, AbstractGameObject>();
		Dictionary<Vector3, float> distances = new Dictionary<Vector3, float>();
		Dictionary<Vector3, AbstractGameObject> previousPositions = new Dictionary<Vector3, AbstractGameObject>();
		Dictionary<Vector3, List<Vector3>> edges = new Dictionary<Vector3, List<Vector3>>();
		SortedList<float, List<Vector3>> queue = new SortedList<float, List<Vector3>>();

		foreach (var row in level.LevelGrid)
		{
			foreach (var tile in row)
			{
				if (tile.tag == "SpaceTile")
				{
					positionObjectMap[tile.TransformCached.position] = tile;
					distances[tile.TransformCached.position] = float.PositiveInfinity;
					previousPositions[tile.TransformCached.position] = null;
					queue[float.PositiveInfinity] = new List<Vector3>();
					queue[float.PositiveInfinity].Add(tile.TransformCached.position);
				}
			}
		}

		foreach (var row in level.LevelGrid)
		{
			foreach (var tile in row)
			{
				Vector3 tilePosition = tile.TransformCached.position;
				Vector3 tileForwardPosition = tilePosition + new Vector3(0.0f, 0.0f, level.LevelScale);
				Vector3 tileBackwardPosition = tilePosition + new Vector3(0.0f, 0.0f, -level.LevelScale);
				Vector3 tileLeftPosition = tilePosition + new Vector3(-level.LevelScale, 0.0f, 0.0f);
				Vector3 tileRightPosition = tilePosition + new Vector3(level.LevelScale, 0.0f, 0.0f);

				if (positionObjectMap.ContainsKey(tileForwardPosition))
				{
					if (!edges.ContainsKey(tilePosition))
						edges[tilePosition] = new List<Vector3>();
					edges[tilePosition].Add(tileForwardPosition);
				}
				if (positionObjectMap.ContainsKey(tileBackwardPosition))
				{
					if (!edges.ContainsKey(tilePosition))
						edges[tilePosition] = new List<Vector3>();
					edges[tilePosition].Add(tileBackwardPosition);
				}
				if (positionObjectMap.ContainsKey(tileLeftPosition))
				{
					if (!edges.ContainsKey(tilePosition))
						edges[tilePosition] = new List<Vector3>();
					edges[tilePosition].Add(tileLeftPosition);
				}
				if (positionObjectMap.ContainsKey(tileRightPosition))
				{
					if (!edges.ContainsKey(tilePosition))
						edges[tilePosition] = new List<Vector3>();
					edges[tilePosition].Add(tileRightPosition);
				}
			}
		}

		distances[orig] = 0.0f;
		queue[float.PositiveInfinity].Remove(orig);
		queue[0] = new List<Vector3>();
		queue[0].Add(orig);

		while (queue.Count > 0)
		{
			float firstKey = queue.Keys[0];
			Vector3 u = queue[firstKey][0];
			queue[firstKey].RemoveAt(0);
			if (queue[firstKey].Count < 1)
				queue.RemoveAt(0);
			if (edges.ContainsKey(u))
			{
				foreach (Vector3 v in edges[u])
				{
					if (distances[v] > distances[u] + level.LevelScale)
					{
						float oldDistance = distances[v];
						distances[v] = distances[u] + level.LevelScale;
						previousPositions[v] = positionObjectMap[u];
						if (queue.ContainsKey(oldDistance))
						{
							queue[oldDistance].Remove(v);
							if (queue[oldDistance].Count < 1)
								queue.Remove(oldDistance);
						}
						if (!queue.ContainsKey(distances[v]))
							queue[distances[v]] = new List<Vector3>();
						queue[distances[v]].Add(v);
					}
				}
			}
		}

		if (previousPositions.ContainsKey(destination))
		{
			for (Vector3 nextDest = destination; previousPositions[nextDest] != null; nextDest = previousPositions[nextDest].TransformCached.position)
			{
				path.Add(nextDest);
			}
		}

		return path;
	}

	public static List<Vector3> SmoothenPath(List<Vector3> unsmoothenedPath)
	{
		List<Vector3> path = unsmoothenedPath;
		int layerMask = ~(LayerMask.NameToLayer("WallTiles") | LayerMask.NameToLayer("Doors"));

		if (path.Count > 2)
		{
			int i = 0;
			while (i < path.Count - 2)
			{
				for (int j = path.Count - 1; j > i + 1; j--)
				{
					Vector3 nextDirection = path[i] - path[j];
					Vector3 nextRight = Vector3.Cross(Vector3.up, nextDirection).normalized;
					float nextMagnitude = (path[i] - path[j]).magnitude;
					if (!Physics.Raycast(path[j] - (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, nextMagnitude, layerMask) &&
			   			!Physics.Raycast(path[j] + (nextRight * 0.4f * LevelManager.LevelScale), nextDirection, nextMagnitude, layerMask))
					{
						path.RemoveRange(i + 1, j - i - 1);
						break;
					}
				}
				i++;
			}
		}

		return path;
	}
}
