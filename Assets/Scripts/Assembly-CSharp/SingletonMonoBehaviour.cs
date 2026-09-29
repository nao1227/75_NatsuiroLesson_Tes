using UnityEngine;

public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance
	{
		get
		{
			if (instance != null)
			{
				return instance;
			}
			instance = (T)Object.FindObjectOfType(typeof(T));
			_ = instance == null;
			return instance;
		}
	}

	public static T InstanceNullable => instance;

	protected virtual void Awake()
	{
		if (instance != null && instance != this)
		{
			Debug.LogError(typeof(T)?.ToString() + " is multiple created", this);
		}
		else
		{
			instance = this as T;
		}
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
		{
			instance = null;
		}
	}
}
