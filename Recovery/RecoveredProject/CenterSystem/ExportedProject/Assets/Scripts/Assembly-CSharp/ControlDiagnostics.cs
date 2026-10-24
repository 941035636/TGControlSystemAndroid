using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public static class ControlDiagnostics
{
	private static readonly object QueueLock = new object();

	private static readonly Queue<string> PendingLines = new Queue<string>();

	private static readonly object StatusLock = new object();

	private static string logPath;

	private static string sessionId = string.Empty;

	private static long sequence;

	private static long latestStatusVersion;

	private static string latestStatusText = string.Empty;

	private static bool latestStatusIsError;

	private static readonly System.Diagnostics.Stopwatch SessionClock = new System.Diagnostics.Stopwatch();

	private const int MaximumPendingLines = 5000;

	public static string LogPath => logPath;

	public static string SessionId => sessionId;

	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Initialise()
	{
		sessionId = DateTime.Now.ToString("yyyyMMdd-HHmmss") + "-" + Guid.NewGuid().ToString("N").Substring(0, 8);
		sequence = 0L;
		latestStatusVersion = 0L;
		latestStatusText = string.Empty;
		latestStatusIsError = false;
		SessionClock.Restart();
		string folder = Path.Combine(Application.persistentDataPath, "ControlDiagnostics");
		Directory.CreateDirectory(folder);
		logPath = Path.Combine(folder, "control-" + sessionId + ".tsv");
		File.WriteAllText(logPath, "time\telapsed_ms\tsession_id\tsequence\tthread_id\tcommand_id\tcategory\taction\ttarget\tresult\tdetail" + Environment.NewLine, Encoding.UTF8);

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
		RecordCommand(string.Empty, category, action, target, result, detail);
	}

	public static void PublishStatus(string message, bool isError)
	{
		if (string.IsNullOrWhiteSpace(message))
		{
			return;
		}
		lock (StatusLock)
		{
			latestStatusVersion++;
			latestStatusText = DateTime.Now.ToString("HH:mm:ss") + "  " + message.Trim();
			latestStatusIsError = isError;
		}
	}

	public static bool TryGetLatestStatus(ref long observedVersion, out string message, out bool isError)
	{
		lock (StatusLock)
		{
			if (observedVersion == latestStatusVersion || string.IsNullOrEmpty(latestStatusText))
			{
				message = string.Empty;
				isError = false;
				return false;
			}
			observedVersion = latestStatusVersion;
			message = latestStatusText;
			isError = latestStatusIsError;
			return true;
		}
	}

	public static string NewCommandId(string protocol)
	{
		long value = Interlocked.Increment(ref sequence);
		string prefix = string.IsNullOrWhiteSpace(protocol) ? "CMD" : protocol.Trim().ToUpperInvariant();
		return prefix + "-" + sessionId + "-" + value.ToString("D6");
	}

	public static void RecordCommand(string commandId, string category, string action, string target, string result, string detail)
	{
		long lineSequence = Interlocked.Increment(ref sequence);
		long elapsedMilliseconds = SessionClock.IsRunning ? SessionClock.ElapsedMilliseconds : 0L;
		string line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\t" +
			elapsedMilliseconds + "\t" + Clean(sessionId) + "\t" + lineSequence + "\t" +
			Thread.CurrentThread.ManagedThreadId + "\t" + Clean(commandId) + "\t" +
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
		ControlDiagnostics.Record("SYSTEM", paused ? "APPLICATION_PAUSE" : "APPLICATION_RESUME", Application.internetReachability.ToString(), "OK", string.Empty);
		ControlDiagnostics.Flush();
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		ControlDiagnostics.Record("SYSTEM", hasFocus ? "APPLICATION_FOCUS_GAINED" : "APPLICATION_FOCUS_LOST", Application.internetReachability.ToString(), "OK", string.Empty);
		ControlDiagnostics.Flush();
	}

	private void OnApplicationQuit()
	{
		ControlDiagnostics.Record("SYSTEM", "SESSION_END", string.Empty, "OK", string.Empty);
		ControlDiagnostics.Flush();
	}
}
