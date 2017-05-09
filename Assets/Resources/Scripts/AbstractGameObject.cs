using UnityEngine;

public abstract class AbstractGameObject : MonoBehaviour
{
    public Transform TransformCached = null;

	protected virtual void Awake()
    {
        TransformCached = transform;
    }
}
