using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

/// <summary>
/// Reliable TCP command transport shared by the recovered relay and infrared
/// controllers. Commands are queued while a connection is being established
/// instead of being silently discarded.
/// </summary>
public abstract class ReliableTcpControlBase : MonoBehaviour
{
	public string host = string.Empty;

	public int port;

	[SerializeField]
	private float connectTimeoutSeconds = 3f;

	[SerializeField]
	private float sendTimeoutSeconds = 2f;

	[SerializeField]
	private float commandIntervalSeconds = 0.12f;

	[SerializeField]
	private int maxSendAttempts = 3;

	private readonly object socketLock = new object();

	private readonly Queue<PendingCommand> commandQueue = new Queue<PendingCommand>();

	private Socket socket;

	private Socket connectingSocket;

	private string connectingHost = string.Empty;

	private int connectingPort;

	private string connectedHost = string.Empty;

	private int connectedPort;

	private bool isConnecting;

	private bool shuttingDown;

	private int connectionGeneration;

	private Coroutine commandCoroutine;

	private Eventdefine connectEvent;

	private Eventdefine sendEvent;

	private Eventdefine sendSingleEvent;

	private string controllerName = "TCP控制器";

	private sealed class PendingCommand
	{
		public string CommandId;

		public string Hex;

		public string TargetIp;

		public int Attempts;

		public float DelayAfter;
	}

	private sealed class ConnectionAttempt
	{
		public Socket Socket;

		public string Host;

		public int Port;

		public int Generation;
	}

	private sealed class ReceiveState
	{
		public Socket Socket;

		public int Generation;

		public byte[] Buffer = new byte[1024];
	}

	private sealed class SendState
	{
		public Socket Socket;

		public int Generation;

		public byte[] Data;

		public int Offset;

		public SendResult Result;
	}

	private sealed class SendResult
	{
		public volatile bool Completed;

		public volatile bool Success;

		public string Error;
	}

	protected void Initialise(Eventdefine connectEvent, Eventdefine sendEvent, Eventdefine sendSingleEvent, string controllerName)
	{
		this.connectEvent = connectEvent;
		this.sendEvent = sendEvent;
		this.sendSingleEvent = sendSingleEvent;
		this.controllerName = controllerName;
		shuttingDown = false;
		EventCenter.addlistener<string, int>(connectEvent, Connect);
		EventCenter.addlistener<string, string, string>(sendEvent, Send);
		EventCenter.addlistener<string, string>(sendSingleEvent, SendSingleCode);
		if (commandCoroutine == null)
		{
			commandCoroutine = StartCoroutine(ProcessCommandQueue());
		}
	}

	protected void Shutdown()
	{
		EventCenter.RemoveListenerTwo<string, int>(connectEvent, Connect);
		EventCenter.RemoveListenerThree<string, string, string>(sendEvent, Send);
		EventCenter.RemoveListenerTwo<string, string>(sendSingleEvent, SendSingleCode);
		shuttingDown = true;
		if (commandCoroutine != null)
		{
			StopCoroutine(commandCoroutine);
			commandCoroutine = null;
		}
		commandQueue.Clear();
		CloseAllSockets();
	}

	public void Connect(string targetHost, int targetPort)
	{
		if (string.IsNullOrWhiteSpace(targetHost) || targetPort <= 0 || targetPort > 65535)
		{
			Debug.LogError(controllerName + "连接参数无效: " + targetHost + ":" + targetPort);
			ControlDiagnostics.Record("TCP", "CONNECT_REQUEST", targetHost + ":" + targetPort, "INVALID", controllerName);
			return;
		}

		host = targetHost.Trim();
		port = targetPort;
		ControlDiagnostics.Record("TCP", "CONNECT_REQUEST", host + ":" + port, "QUEUED", controllerName);
		EnsureConnection();
	}

	public void Send(string openMessage, string closeMessage, string targetIp)
	{
		Enqueue(openMessage, targetIp, 0.3f);
		Enqueue(closeMessage, targetIp, commandIntervalSeconds);
	}

	public void SendSingleCode(string message, string targetIp)
	{
		Enqueue(message, targetIp, commandIntervalSeconds);
	}

	public void DisConnect()
	{
		CloseAllSockets();
	}

	private void Enqueue(string message, string targetIp, float delayAfter)
	{
		string commandId = ControlDiagnostics.NewCommandId("TCP");
		if (string.IsNullOrWhiteSpace(message))
		{
			Debug.LogError(controllerName + "忽略空指令");
			ControlDiagnostics.RecordCommand(commandId, "TCP", "COMMAND_QUEUE", targetIp, "INVALID", controllerName + " empty command");
			return;
		}

		try
		{
			HexStringToBytes(message);
		}
		catch (Exception ex)
		{
			Debug.LogError(controllerName + "指令格式错误: " + ex.Message + " 指令=" + message);
			ControlDiagnostics.RecordCommand(commandId, "TCP", "COMMAND_QUEUE", targetIp, "INVALID", controllerName + " " + ex.Message + " " + message);
			return;
		}

		commandQueue.Enqueue(new PendingCommand
		{
			CommandId = commandId,
			Hex = message,
			TargetIp = string.IsNullOrWhiteSpace(targetIp) ? host : targetIp.Trim(),
			DelayAfter = delayAfter
		});
		ControlDiagnostics.RecordCommand(commandId, "TCP", "COMMAND_QUEUE", (string.IsNullOrWhiteSpace(targetIp) ? host : targetIp.Trim()) + ":" + port, "QUEUED", controllerName + " " + message);

		if (string.IsNullOrWhiteSpace(host) && !string.IsNullOrWhiteSpace(targetIp))
		{
			host = targetIp.Trim();
		}
		EnsureConnection();
	}

	private IEnumerator ProcessCommandQueue()
	{
		while (!shuttingDown)
		{
			if (commandQueue.Count == 0)
			{
				yield return null;
				continue;
			}

			PendingCommand command = commandQueue.Peek();
			if (!string.IsNullOrWhiteSpace(command.TargetIp) && command.TargetIp != host)
			{
				host = command.TargetIp;
			}

			float waitStarted = Time.realtimeSinceStartup;
			EnsureConnection();
			while (!shuttingDown && !IsConnectedTo(command.TargetIp))
			{
				if (Time.realtimeSinceStartup - waitStarted >= Mathf.Max(0.5f, connectTimeoutSeconds))
				{
					Debug.LogWarning(controllerName + "连接超时，准备重连: " + host + ":" + port);
					ResetConnection();
					command.Attempts++;
					if (command.Attempts >= Mathf.Max(1, maxSendAttempts))
					{
						Debug.LogError(controllerName + "指令发送失败，已达到重试上限: " + command.Hex);
						ControlDiagnostics.RecordCommand(command.CommandId, "TCP", "CONNECT", host + ":" + port, "FAILED", controllerName + " max attempts " + command.Hex);
						commandQueue.Dequeue();
						break;
					}
					waitStarted = Time.realtimeSinceStartup;
					EnsureConnection();
				}
				yield return new WaitForSecondsRealtime(0.1f);
			}

			if (shuttingDown || commandQueue.Count == 0 || commandQueue.Peek() != command)
			{
				continue;
			}

			Socket activeSocket = GetActiveSocket();
			if (activeSocket == null)
			{
				yield return null;
				continue;
			}

			ControlDiagnostics.RecordCommand(command.CommandId, "TCP", "COMMAND_SEND", command.TargetIp + ":" + port, "ATTEMPT", controllerName + " attempt=" + (command.Attempts + 1) + " " + command.Hex);
			SendResult result = BeginSend(activeSocket, HexStringToBytes(command.Hex));
			float sendStarted = Time.realtimeSinceStartup;
			while (!result.Completed && Time.realtimeSinceStartup - sendStarted < Mathf.Max(0.5f, sendTimeoutSeconds))
			{
				yield return null;
			}

			if (result.Completed && result.Success)
			{
				Debug.Log(controllerName + "发送成功 IP:" + command.TargetIp + " 指令:" + command.Hex);
				ControlDiagnostics.RecordCommand(command.CommandId, "TCP", "COMMAND_SEND", command.TargetIp + ":" + port, "SOCKET_OK", controllerName + " bytes accepted by socket " + command.Hex);
				commandQueue.Dequeue();
				yield return new WaitForSecondsRealtime(Mathf.Max(command.DelayAfter, commandIntervalSeconds));
				continue;
			}

			command.Attempts++;
			Debug.LogWarning(controllerName + "发送失败，准备重连重试: " + (result.Error ?? "发送超时"));
			ControlDiagnostics.RecordCommand(command.CommandId, "TCP", "COMMAND_SEND", command.TargetIp + ":" + port, "RETRY", controllerName + " " + (result.Error ?? "timeout") + " " + command.Hex);
			ResetConnection();
			if (command.Attempts >= Mathf.Max(1, maxSendAttempts))
			{
				Debug.LogError(controllerName + "指令发送失败，已达到重试上限: " + command.Hex);
				ControlDiagnostics.RecordCommand(command.CommandId, "TCP", "COMMAND_SEND", command.TargetIp + ":" + port, "FAILED", controllerName + " max attempts " + command.Hex);
				commandQueue.Dequeue();
			}
			yield return new WaitForSecondsRealtime(0.3f);
		}
	}

	private void EnsureConnection()
	{
		if (shuttingDown || string.IsNullOrWhiteSpace(host) || port <= 0)
		{
			return;
		}

		lock (socketLock)
		{
			if (IsSocketAlive(socket) && connectedHost == host && connectedPort == port)
			{
				return;
			}
			if (isConnecting && connectingSocket != null)
			{
				if (connectingHost == host && connectingPort == port)
				{
					return;
				}
			}

			CloseSocketNoLock(socket);
			socket = null;
			CloseSocketNoLock(connectingSocket);
			connectingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
			{
				NoDelay = true,
				SendTimeout = Mathf.CeilToInt(Mathf.Max(0.5f, sendTimeoutSeconds) * 1000f),
				ReceiveTimeout = Mathf.CeilToInt(Mathf.Max(0.5f, sendTimeoutSeconds) * 1000f)
			};
			isConnecting = true;
			connectingHost = host;
			connectingPort = port;
			int generation = ++connectionGeneration;
			ConnectionAttempt attempt = new ConnectionAttempt
			{
				Socket = connectingSocket,
				Host = host,
				Port = port,
				Generation = generation
			};
			try
			{
				connectingSocket.BeginConnect(host, port, ConnectCallback, attempt);
			}
			catch (Exception ex)
			{
				isConnecting = false;
				CloseSocketNoLock(connectingSocket);
				connectingSocket = null;
				connectingHost = string.Empty;
				connectingPort = 0;
				Debug.LogWarning(controllerName + "发起连接失败: " + ex.Message);
			}
		}
	}

	private void ConnectCallback(IAsyncResult asyncResult)
	{
		ConnectionAttempt attempt = (ConnectionAttempt)asyncResult.AsyncState;
		try
		{
			attempt.Socket.EndConnect(asyncResult);
			lock (socketLock)
			{
				if (shuttingDown || attempt.Generation != connectionGeneration || attempt.Socket != connectingSocket)
				{
					CloseSocketNoLock(attempt.Socket);
					return;
				}
				socket = attempt.Socket;
				connectingSocket = null;
				isConnecting = false;
				connectingHost = string.Empty;
				connectingPort = 0;
				connectedHost = attempt.Host;
				connectedPort = attempt.Port;
			}
			Debug.Log(controllerName + "连接成功: " + attempt.Host + ":" + attempt.Port);
			ControlDiagnostics.Record("TCP", "CONNECT", attempt.Host + ":" + attempt.Port, "OK", controllerName);
			BeginReceive(attempt.Socket, attempt.Generation);
		}
		catch (Exception ex)
		{
			lock (socketLock)
			{
				if (attempt.Socket == connectingSocket)
				{
					connectingSocket = null;
					isConnecting = false;
					connectingHost = string.Empty;
					connectingPort = 0;
				}
				CloseSocketNoLock(attempt.Socket);
			}
			Debug.LogWarning(controllerName + "连接失败: " + attempt.Host + ":" + attempt.Port + " " + ex.Message);
			ControlDiagnostics.Record("TCP", "CONNECT", attempt.Host + ":" + attempt.Port, "FAILED", controllerName + " " + ex.Message);
		}
	}

	private void BeginReceive(Socket receiveSocket, int generation)
	{
		ReceiveState state = new ReceiveState
		{
			Socket = receiveSocket,
			Generation = generation
		};
		try
		{
			receiveSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, ReceiveCallback, state);
		}
		catch (Exception ex)
		{
			Debug.LogWarning(controllerName + "启动接收失败: " + ex.Message);
			InvalidateSocket(receiveSocket, generation);
		}
	}

	private void ReceiveCallback(IAsyncResult asyncResult)
	{
		ReceiveState state = (ReceiveState)asyncResult.AsyncState;
		try
		{
			int count = state.Socket.EndReceive(asyncResult);
			if (count <= 0)
			{
				InvalidateSocket(state.Socket, state.Generation);
				return;
			}
			string receivedHex = BytesToHex(state.Buffer, count);
			Debug.Log(controllerName + "接收: " + receivedHex);
			ControlDiagnostics.Record("TCP", "RECEIVE", connectedHost + ":" + connectedPort, "UNMATCHED", controllerName + " bytes=" + count + " raw=" + receivedHex);
			state.Socket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, ReceiveCallback, state);
		}
		catch (Exception ex)
		{
			if (!shuttingDown)
			{
				Debug.LogWarning(controllerName + "接收中断: " + ex.Message);
				ControlDiagnostics.Record("TCP", "RECEIVE", connectedHost + ":" + connectedPort, "DISCONNECTED", controllerName + " " + ex.Message);
			}
			InvalidateSocket(state.Socket, state.Generation);
		}
	}

	private SendResult BeginSend(Socket sendSocket, byte[] data)
	{
		SendResult result = new SendResult();
		SendState state = new SendState
		{
			Socket = sendSocket,
			Generation = connectionGeneration,
			Data = data,
			Result = result
		};
		BeginSendRemaining(state);
		return result;
	}

	private void BeginSendRemaining(SendState state)
	{
		try
		{
			state.Socket.BeginSend(state.Data, state.Offset, state.Data.Length - state.Offset, SocketFlags.None, SendCallback, state);
		}
		catch (Exception ex)
		{
			state.Result.Error = ex.Message;
			state.Result.Success = false;
			state.Result.Completed = true;
		}
	}

	private void SendCallback(IAsyncResult asyncResult)
	{
		SendState state = (SendState)asyncResult.AsyncState;
		try
		{
			int sent = state.Socket.EndSend(asyncResult);
			if (sent <= 0)
			{
				throw new SocketException((int)SocketError.ConnectionReset);
			}
			state.Offset += sent;
			if (state.Offset < state.Data.Length)
			{
				BeginSendRemaining(state);
				return;
			}
			state.Result.Success = true;
			state.Result.Completed = true;
		}
		catch (Exception ex)
		{
			state.Result.Error = ex.Message;
			state.Result.Success = false;
			state.Result.Completed = true;
		}
	}

	private bool IsConnectedTo(string targetIp)
	{
		lock (socketLock)
		{
			return IsSocketAlive(socket) &&
				(string.IsNullOrWhiteSpace(targetIp) || connectedHost == targetIp) &&
				connectedPort == port;
		}
	}

	private Socket GetActiveSocket()
	{
		lock (socketLock)
		{
			return IsSocketAlive(socket) ? socket : null;
		}
	}

	private static bool IsSocketAlive(Socket candidate)
	{
		if (candidate == null || !candidate.Connected)
		{
			return false;
		}
		try
		{
			return !(candidate.Poll(0, SelectMode.SelectRead) && candidate.Available == 0);
		}
		catch
		{
			return false;
		}
	}

	private void ResetConnection()
	{
		lock (socketLock)
		{
			connectionGeneration++;
			CloseSocketNoLock(socket);
			CloseSocketNoLock(connectingSocket);
			socket = null;
			connectingSocket = null;
			connectedHost = string.Empty;
			connectedPort = 0;
			isConnecting = false;
			connectingHost = string.Empty;
			connectingPort = 0;
		}
	}

	private void InvalidateSocket(Socket candidate, int generation)
	{
		lock (socketLock)
		{
			if (generation != connectionGeneration || candidate != socket)
			{
				CloseSocketNoLock(candidate);
				return;
			}
			CloseSocketNoLock(socket);
			socket = null;
			connectedHost = string.Empty;
			connectedPort = 0;
		}
	}

	private void CloseAllSockets()
	{
		lock (socketLock)
		{
			connectionGeneration++;
			CloseSocketNoLock(socket);
			CloseSocketNoLock(connectingSocket);
			socket = null;
			connectingSocket = null;
			connectedHost = string.Empty;
			connectedPort = 0;
			isConnecting = false;
			connectingHost = string.Empty;
			connectingPort = 0;
		}
	}

	private static void CloseSocketNoLock(Socket candidate)
	{
		if (candidate == null)
		{
			return;
		}
		try
		{
			candidate.Shutdown(SocketShutdown.Both);
		}
		catch
		{
		}
		try
		{
			candidate.Close();
		}
		catch
		{
		}
	}

	private static byte[] HexStringToBytes(string hexString)
	{
		string compact = hexString.Replace(" ", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
		if (compact.Length % 2 != 0)
		{
			throw new FormatException("十六进制字符数必须为偶数");
		}
		byte[] data = new byte[compact.Length / 2];
		for (int i = 0; i < data.Length; i++)
		{
			data[i] = Convert.ToByte(compact.Substring(i * 2, 2), 16);
		}
		return data;
	}

	private static string BytesToHex(byte[] data, int count)
	{
		StringBuilder builder = new StringBuilder(count * 3);
		for (int i = 0; i < count; i++)
		{
			if (i > 0)
			{
				builder.Append(' ');
			}
			builder.Append(data[i].ToString("X2"));
		}
		return builder.ToString();
	}
}
