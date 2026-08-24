using System;
using System.Collections.Generic;
using UnityEngine;

public class EventCenter
{
	public static Dictionary<Eventdefine, Delegate> Table = new Dictionary<Eventdefine, Delegate>();

	public static void addlistener(Eventdefine eventtype, Callback callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (Callback)Delegate.Combine((Callback)Delegate.Remove((Callback)Table[eventtype], callback), callback);
	}

	public static void addlistener<T>(Eventdefine eventtype, Callback<T> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (Callback<T>)Delegate.Combine((Callback<T>)Delegate.Remove((Callback<T>)Table[eventtype], callback), callback);
	}

	public static void addlistener<T1, T2>(Eventdefine eventtype, Callback<T1, T2> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (Callback<T1, T2>)Delegate.Combine((Callback<T1, T2>)Delegate.Remove((Callback<T1, T2>)Table[eventtype], callback), callback);
	}

	public static void addlistener<T1, T2, T3>(Eventdefine eventtype, Callback<T1, T2, T3> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (Callback<T1, T2, T3>)Delegate.Combine((Callback<T1, T2, T3>)Delegate.Remove((Callback<T1, T2, T3>)Table[eventtype], callback), callback);
	}

	public static void AddlistenerString<T3>(Eventdefine eventtype, CallbackString<T3> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (CallbackString<T3>)Delegate.Combine((CallbackString<T3>)Table[eventtype], callback);
	}

	public static void AddlistenerBool<T>(Eventdefine eventtype, CallbackBool<T> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Table.Add(eventtype, null);
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			throw new Exception($"事件类型{eventtype},对应的委托类型{Table[eventtype].GetType()},要添加的委托类型{callback.GetType()}");
		}
		Table[eventtype] = (CallbackBool<T>)Delegate.Combine((CallbackBool<T>)Table[eventtype], callback);
	}

	public static void RemoveListener(Eventdefine eventtype, Callback callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogError("要移除的监听不存在");
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (Callback)Delegate.Remove((Callback)Table[eventtype], callback);
	}

	public static void RemoveListenerBool<T>(Eventdefine eventtype, CallbackBool<T> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogWarning("要移除的监听不存在: " + eventtype);
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (CallbackBool<T>)Delegate.Remove((CallbackBool<T>)Table[eventtype], callback);
		Debug.Log("移除监听");
	}

	public static void RemoveListener<T>(Eventdefine eventtype, Callback<T> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogWarning("要移除的监听不存在: " + eventtype);
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (Callback<T>)Delegate.Remove((Callback<T>)Table[eventtype], callback);
	}

	public static void RemoveListenerTwo<T1, T2>(Eventdefine eventtype, Callback<T1, T2> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogWarning("要移除的监听不存在: " + eventtype);
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (Callback<T1, T2>)Delegate.Remove((Callback<T1, T2>)Table[eventtype], callback);
	}

	public static void RemoveListenerThree<T1, T2, T3>(Eventdefine eventtype, Callback<T1, T2, T3> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogWarning("要移除的监听不存在: " + eventtype);
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (Callback<T1, T2, T3>)Delegate.Remove((Callback<T1, T2, T3>)Table[eventtype], callback);
	}

	public static void RemoveListenerString<T3>(Eventdefine eventtype, CallbackString<T3> callback)
	{
		if (!Table.ContainsKey(eventtype))
		{
			Debug.LogWarning("要移除的监听不存在: " + eventtype);
			return;
		}
		if ((object)Table[eventtype] != null && Table[eventtype].GetType() != callback.GetType())
		{
			Debug.LogError("要移除的监听类型与字典中存放的监听类型不一致");
			return;
		}
		Table[eventtype] = (CallbackString<T3>)Delegate.Remove((CallbackString<T3>)Table[eventtype], callback);
	}

	public static void BroadCast(Eventdefine eventtype)
	{
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				return;
			}
			foreach (Delegate subscriber in value.GetInvocationList())
			{
				try
				{
					((Callback)subscriber)();
				}
				catch (Exception ex)
				{
					Debug.LogError("事件执行失败 " + eventtype + ": " + ex);
				}
			}
		}
	}

	public static void BroadCast<T>(Eventdefine eventtype, T arg)
	{
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				return;
			}
			foreach (Delegate subscriber in value.GetInvocationList())
			{
				try
				{
					((Callback<T>)subscriber)(arg);
				}
				catch (Exception ex)
				{
					Debug.LogError("事件执行失败 " + eventtype + ": " + ex);
				}
			}
		}
	}

	public static void BroadCast<T1, T2>(Eventdefine eventtype, T1 arg, T2 arg2)
	{
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				return;
			}
			foreach (Delegate subscriber in value.GetInvocationList())
			{
				try
				{
					((Callback<T1, T2>)subscriber)(arg, arg2);
				}
				catch (Exception ex)
				{
					Debug.LogError("事件执行失败 " + eventtype + ": " + ex);
				}
			}
		}
	}

	public static void BroadCast<T1, T2, T3>(Eventdefine eventtype, T1 arg, T2 arg2, T3 arg3)
	{
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				return;
			}
			foreach (Delegate subscriber in value.GetInvocationList())
			{
				try
				{
					((Callback<T1, T2, T3>)subscriber)(arg, arg2, arg3);
				}
				catch (Exception ex)
				{
					Debug.LogError("事件执行失败 " + eventtype + ": " + ex);
				}
			}
		}
	}

	public static string BroadCastString<T3>(Eventdefine eventtype, T3 arg)
	{
		string result = string.Empty;
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				throw new Exception("字典中委托为空，广播失败");
			}
			result = ((CallbackString<T3>)value)(arg);
		}
		return result;
	}

	public static void BroadCastBool<T>(Eventdefine eventtype, T arg)
	{
		if (Table.TryGetValue(eventtype, out var value))
		{
			if ((object)value == null)
			{
				throw new Exception("字典中委托为空，广播失败");
			}
			((CallbackBool<T>)value)(arg);
		}
	}
}
