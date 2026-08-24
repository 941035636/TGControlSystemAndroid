using System;
using UnityEngine;

public class UITools : MonoBehaviour
{
	public static Transform FindTheChildNode(Transform goParent, string chiildName)
	{
		try
		{
			Transform transform = null;
			transform = goParent.transform.Find(chiildName);
			if (transform == null)
			{
				foreach (Transform item in goParent)
				{
					transform = FindTheChildNode(item, chiildName);
					if (transform != null)
					{
						return transform;
					}
				}
			}
			return transform;
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.ToString());
			return null;
		}
	}

	public static T GetTheChildNodeComponetScripts<T>(Transform goParent, string childName) where T : Component
	{
		Transform transform = null;
		transform = FindTheChildNode(goParent, childName);
		if (transform != null)
		{
			return transform.gameObject.GetComponent<T>();
		}
		return null;
	}

	public static T AddChildNodeCompnent<T>(Transform goParent, string childName) where T : Component
	{
		Transform transform = null;
		transform = FindTheChildNode(goParent, childName);
		if (transform != null)
		{
			T[] components = transform.GetComponents<T>();
			for (int i = 0; i < components.Length; i++)
			{
				if (components[i] != null)
				{
					UnityEngine.Object.Destroy(components[i]);
				}
			}
			return transform.gameObject.AddComponent<T>();
		}
		return null;
	}

	public static void AddChildNodeToParentNode(Transform parents, Transform child)
	{
		child.SetParent(parents, worldPositionStays: false);
		child.localPosition = Vector3.zero;
		child.localScale = Vector3.one;
		child.localEulerAngles = Vector3.zero;
	}
}
