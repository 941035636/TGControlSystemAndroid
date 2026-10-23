using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public static class ReliableUdpCommandSender
{
	public static IEnumerator Send(string operation, string targetIp, int targetPort, string payload,
		int attempts = 3, float registrationDelaySeconds = 1f, float retryIntervalSeconds = 0.3f,
		Func<bool> shouldContinue = null)
	{
		string commandId = ControlDiagnostics.NewCommandId("UDP");
		if (string.IsNullOrWhiteSpace(targetIp) || targetPort <= 0 || targetPort > 65535 || string.IsNullOrEmpty(payload))
		{
			ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, targetIp + ":" + targetPort, "INVALID", "payload=" + payload);
			yield break;
		}

		IPEndPoint endpoint;
		try
		{
			endpoint = new IPEndPoint(IPAddress.Parse(targetIp.Trim()), targetPort);
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP target is invalid: " + targetIp + ":" + targetPort + " " + ex.Message);
			ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, targetIp + ":" + targetPort, "INVALID", ex.Message);
			yield break;
		}

		int totalAttempts = Mathf.Clamp(attempts, 1, 5);
		int successfulSends = 0;
		using (Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
		{
			sender.SendTimeout = 1000;
			ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, endpoint.ToString(), "STARTED", "attempts=" + totalAttempts + " payload=" + payload);
			TrySend(sender, endpoint, "hello", commandId, operation, 0);

			if (registrationDelaySeconds > 0f)
			{
				yield return new WaitForSecondsRealtime(registrationDelaySeconds);
			}

			for (int attempt = 1; attempt <= totalAttempts; attempt++)
			{
				if (shouldContinue != null && !shouldContinue())
				{
					ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, endpoint.ToString(),
						"CANCELLED", "superseded before attempt=" + attempt + " payload=" + payload);
					yield break;
				}
				if (TrySend(sender, endpoint, payload, commandId, operation, attempt))
				{
					successfulSends++;
				}
				if (attempt < totalAttempts && retryIntervalSeconds > 0f)
				{
					yield return new WaitForSecondsRealtime(retryIntervalSeconds);
				}
			}
		}

		ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, endpoint.ToString(),
			successfulSends > 0 ? "COMPLETED" : "FAILED",
			"socket_successes=" + successfulSends + "/" + totalAttempts + " payload=" + payload);
	}

	private static bool TrySend(Socket sender, EndPoint endpoint, string payload, string commandId, string operation, int attempt)
	{
		try
		{
			byte[] data = Encoding.ASCII.GetBytes(payload);
			int sent = sender.SendTo(data, 0, data.Length, SocketFlags.None, endpoint);
			bool complete = sent == data.Length;
			ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, endpoint.ToString(),
				complete ? "SOCKET_OK" : "PARTIAL", "attempt=" + attempt + " bytes=" + sent + "/" + data.Length + " payload=" + payload);
			return complete;
		}
		catch (Exception ex)
		{
			Debug.LogWarning("UDP reliable send failed: " + endpoint + " attempt=" + attempt + " " + ex.Message);
			ControlDiagnostics.RecordCommand(commandId, "UDP_RELIABLE", operation, endpoint.ToString(), "FAILED",
				"attempt=" + attempt + " " + ex.Message + " payload=" + payload);
			return false;
		}
	}
}
