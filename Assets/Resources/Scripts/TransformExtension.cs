using UnityEngine;

public static class TransformExtension
{

	public static Transform FindBreadthFirst(this Transform transform, string name)
	{
		Transform result = transform.Find(name);
		if (result != null)
		{
			return result;
		}
		foreach (Transform child in transform)
		{
			result = child.FindBreadthFirst(name);
			if (result != null)
			{
				return result;
			}
		}
		return null;
	}

	public static Transform FindDepthFirst(this Transform transform, string name)
	{
		foreach (Transform child in transform)
		{
			if (child.name == name)
			{
				return child;
			}
			Transform result = child.FindDepthFirst(name);
			if (result != null)
			{
				return result;
			}
				
		}
		return null;
	}
}
