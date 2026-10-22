using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityEngine
{
	[AttributeUsage(AttributeTargets.Field)]
	public sealed class SerializeField : Attribute
	{
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class TooltipAttribute : Attribute
	{
		public TooltipAttribute(string text)
		{
		}
	}

	[AttributeUsage(AttributeTargets.Field)]
	public sealed class RangeAttribute : Attribute
	{
		public RangeAttribute(int minimum, int maximum)
		{
		}
	}

	public class Object
	{
		public static T FindObjectOfType<T>() where T : class
		{
			return null;
		}
	}

	public class MonoBehaviour : Object
	{
		public Coroutine LastStartedCoroutine { get; private set; }

		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			LastStartedCoroutine = new Coroutine(routine);
			return LastStartedCoroutine;
		}

		protected void StopCoroutine(Coroutine coroutine)
		{
		}
	}

	public sealed class Coroutine
	{
		public Coroutine(IEnumerator routine)
		{
			Routine = routine;
		}

		public IEnumerator Routine { get; }
	}

	public sealed class WaitForSecondsRealtime
	{
		public WaitForSecondsRealtime(float seconds)
		{
			waitTime = seconds;
		}

		public float waitTime { get; }
	}

	public static class Time
	{
		public static float realtimeSinceStartup;
	}

	public static class Mathf
	{
		public static float Max(float left, float right)
		{
			return Math.Max(left, right);
		}

		public static int Max(int left, int right)
		{
			return Math.Max(left, right);
		}

		public static int Clamp(int value, int minimum, int maximum)
		{
			return Math.Min(Math.Max(value, minimum), maximum);
		}

		public static int CeilToInt(float value)
		{
			return (int)Math.Ceiling(value);
		}
	}

	public static class Debug
	{
		public static readonly List<string> Messages = new List<string>();

		public static void Log(object message)
		{
			Messages.Add("INFO " + message);
		}

		public static void LogWarning(object message)
		{
			Messages.Add("WARN " + message);
		}

		public static void LogError(object message)
		{
			Messages.Add("ERROR " + message);
		}
	}
}

public enum Eventdefine
{
	None
}

public delegate void Callback<T1, T2>(T1 arg1, T2 arg2);

public delegate void Callback<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

public static class EventCenter
{
	public static void addlistener<T1, T2>(Eventdefine eventType, Callback<T1, T2> callback)
	{
	}

	public static void addlistener<T1, T2, T3>(Eventdefine eventType, Callback<T1, T2, T3> callback)
	{
	}

	public static void RemoveListenerTwo<T1, T2>(Eventdefine eventType, Callback<T1, T2> callback)
	{
	}

	public static void RemoveListenerThree<T1, T2, T3>(Eventdefine eventType, Callback<T1, T2, T3> callback)
	{
	}
}

public static class ControlDiagnostics
{
	private static int nextCommandId;

	public static string NewCommandId(string protocol)
	{
		nextCommandId++;
		return protocol + "-TEST-" + nextCommandId;
	}

	public static void Record(string category, string action, string target, string result, string detail)
	{
	}

	public static void RecordCommand(string commandId, string category, string action, string target, string result, string detail)
	{
	}
}
