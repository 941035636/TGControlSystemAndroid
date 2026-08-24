using System;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T instance;

	public static T Instance => instance;

	public static T GetInstance()
	{
		if (instance == null)
		{
			return (T)Activator.CreateInstance(typeof(T));
		}
		return instance;
	}

	protected virtual void Awake()
	{
		instance = this as T;
	}

	protected virtual void OnDestroy()
	{
		instance = null;
	}
}
