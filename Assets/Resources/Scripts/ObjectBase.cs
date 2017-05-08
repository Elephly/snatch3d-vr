using UnityEngine;

public abstract class ObjectBase : MonoBehaviour
{
    public Transform TransformCached = null;

	protected virtual void Awake()
    {
        TransformCached = transform;
    }
}
