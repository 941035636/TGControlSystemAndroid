using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public static class ControlDiagnostics
{
	private static readonly object QueueLock = new object();

	private static readonly Queue<string> PendingLines = new Queue<string>();

	private static string logPath;

	private const int MaximumPendingLines = 5000;

	public static string LogPath => logPath;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialise()
	{
		string folder = Path.Combine(Application.persistentDataPath, "ControlDiagnostics");
		Directory.CreateDirectory(folder);
		logPath = Path.Combine(folder, "control-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".tsv");
		File.WriteAllText(logPath, "time\tcategory\taction\ttarget\tresult\tdetail" + Environment.NewLine, Encoding.UTF8);

		GameObject writerObject = new GameObject("ControlDiagnosticsWriter")
		{
			hideFlags = HideFlags.HideAndDontSave
		};
		writerObject.AddComponent<ControlDiagnosticsWriter>();
		UnityEngine.Object.DontDestroyOnLoad(writerObject);
		Record("SYSTEM", "SESSION_START", Application.platform.ToString(), "OK", Application.unityVersion);
	}

	public static void Record(string category, string action, string target, string result, string detail)
	{
		string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" +
			Clean(category) + "\t" + Clean(action) + "\t" + Clean(target) + "\t" +
			Clean(result) + "\t" + Clean(detail);
		lock (QueueLock)
		{
			while (PendingLines.Count >= MaximumPendingLines)
			{
				PendingLines.Dequeue();
			}
			PendingLines.Enqueue(line);
		}
	}

	internal static void Flush()
	{
		if (string.IsNullOrEmpty(logPath))
		{
			return;
		}
		StringBuilder builder = null;
		lock (QueueLock)
		{
			if (PendingLines.Count == 0)
			{
				return;
			}
			builder = new StringBuilder(PendingLines.Count * 96);
			while (PendingLines.Count > 0)
			{
				builder.AppendLine(PendingLines.Dequeue());
			}
		}
		try
		{
			File.AppendAllText(logPath, builder.ToString(), Encoding.UTF8);
		}
		catch (Exception ex)
		{
			Debug.LogWarning("控制诊断日志写入失败: " + ex.Message);
		}
	}

	private static string Clean(string value)
	{
		return (value ?? string.Empty).Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
	}
}

internal sealed class ControlDiagnosticsWriter : MonoBehaviour
{
	private float nextFlushTime;

	private void Update()
	{
		if (Time.realtimeSinceStartup >= nextFlushTime)
		{
			nextFlushTime = Time.realtimeSinceStartup + 0.5f;
			ControlDiagnostics.Flush();
		}
	}

	private void OnApplicationPause(bool paused)
	{
		if (paused)
		{
			ControlDiagnostics.Record("SYSTEM", "APPLICATION_PAUSE", string.Empty, "OK", string.Empty);
			ControlDiagnostics.Flush();
		}
	}

	private void OnApplicationQuit()
	{
		ControlDiagnostics.Record("SYSTEM", "SESSION_END", string.Empty, "OK", string.Empty);
		ControlDiagnostics.Flush();
	}
}
