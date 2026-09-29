using UnityEngine;

public abstract class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	public static T Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = (T)Object.FindObjectOfType(typeof(T));
				_ = _instance == null;
			}
			return _instance;
		}
	}

	public virtual void Awake()
	{
		if (this != Instance)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	private void OnDestroy()
	{
		if (this == Instance)
		{
			_instance = null;
		}
	}
}
