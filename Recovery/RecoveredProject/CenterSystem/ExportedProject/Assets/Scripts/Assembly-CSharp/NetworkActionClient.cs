using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

public class NetworkActionClient : MonoBehaviour
{
	public static NetworkActionClient instance;

	public bool enableLog;

	public string IP = "192.168.1.165";

	public int ServerListenPort = 2389;

	private int ClientListenPort = 2390;

	private TcpClient client;

	private NetworkStream stream;

	private bool stop;

	private const int SEND_RECEIVE_COUNT = 4;

	private int dataSize;

	private byte[] dataBytesCount;

	public int CurrentSeenTimeMS;

	public int LastReceivedTimeMS;

	public bool isConnected;

	public int TimeoutThresholdMS = 5000;

	public bool autoReconnect = true;

	private bool isReachedServer;

	private int checkTimer;

	private object _asyncLockReceived = new object();

	private Queue<byte[]> _appendQueueReceived = new Queue<byte[]>();

	public UnityEvent OnConnectedEvent;

	public UnityEvent OnDisconnectedEvent;

	public UnityEventString OnReceivedStringDataEvent;

	public UnityEventByteArray OnReceivedByteDataEvent;

	private UdpClient udpClient;

	private Queue<byte[]> _appendQueueCmd = new Queue<byte[]>();

	[HideInInspector]
	public bool readySend = true;

	private object _asyncSend = new object();

	private bool triggerQuitApp;

	private bool isPaused_old;

	private bool isPaused;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void Action_SetIP(string _value)
	{
		IP = _value;
	}

	public void Action_SetServerListenPort(int _port)
	{
		ServerListenPort = _port;
		ClientListenPort = _port + 1;
	}

	private int GetCurrentMS()
	{
		return Environment.TickCount;
	}

	public void Action_ReceivedDataDebug(string _value)
	{
		Debug.Log(_value);
	}

	public void Action_RpcSend(string _cmd)
	{
		if (!isConnected || _appendQueueCmd.Count > 100)
		{
			return;
		}
		byte[] array = new byte[1] { 1 };
		byte[] bytes = Encoding.ASCII.GetBytes("Rpc");
		byte[] bytes2 = Encoding.ASCII.GetBytes(_cmd);
		byte[] array2 = new byte[array.Length + bytes.Length + bytes2.Length];
		int num = 0;
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		num += array.Length;
		Buffer.BlockCopy(bytes, 0, array2, num, bytes.Length);
		num += bytes.Length;
		Buffer.BlockCopy(bytes2, 0, array2, num, bytes2.Length);
		lock (_asyncSend)
		{
			_appendQueueCmd.Enqueue(array2);
		}
	}

	public void Action_RpcSend(byte[] _cmd)
	{
		if (!isConnected || _appendQueueCmd.Count > 1000)
		{
			return;
		}
		byte[] array = new byte[1];
		byte[] bytes = Encoding.ASCII.GetBytes("Rpc");
		byte[] array2 = new byte[array.Length + bytes.Length + _cmd.Length];
		int num = 0;
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		num += array.Length;
		Buffer.BlockCopy(bytes, 0, array2, num, bytes.Length);
		num += bytes.Length;
		Buffer.BlockCopy(_cmd, 0, array2, num, _cmd.Length);
		lock (_asyncSend)
		{
			_appendQueueCmd.Enqueue(array2);
		}
	}

	private void Start()
	{
		Application.runInBackground = true;
		ClientListenPort = ServerListenPort + 1;
		LastReceivedTimeMS = int.MinValue;
	}

	private void Update()
	{
		if (!isConnected)
		{
			OnDisconnectedEvent.Invoke();
		}
		else
		{
			OnConnectedEvent.Invoke();
		}
		CheckPause();
		CurrentSeenTimeMS = GetCurrentMS();
		if (CurrentSeenTimeMS < 0 && LastReceivedTimeMS > 0)
		{
			checkTimer = Mathf.Abs(CurrentSeenTimeMS - int.MinValue) + Mathf.Abs(int.MaxValue - LastReceivedTimeMS);
		}
		else
		{
			checkTimer = Mathf.Abs(CurrentSeenTimeMS - LastReceivedTimeMS);
		}
		if (checkTimer > TimeoutThresholdMS)
		{
			isConnected = false;
			if (autoReconnect && !triggerQuitApp)
			{
				triggerQuitApp = true;
				StartCoroutine(RelaunchApp());
			}
		}
		_ = CurrentSeenTimeMS;
		_ = LastReceivedTimeMS;
	}

	private int DataByteArrayToByteLength(byte[] dataBytesLength)
	{
		return BitConverter.ToInt32(dataBytesLength, 0);
	}

	private int ReadDataByteSize(int size)
	{
		bool flag = false;
		if (dataBytesCount == null)
		{
			dataBytesCount = new byte[size];
		}
		else if (dataBytesCount.Length > size)
		{
			dataBytesCount = new byte[size];
		}
		int num = 0;
		do
		{
			int num2 = stream.Read(dataBytesCount, num, size - num);
			if (num2 == 0)
			{
				flag = true;
				break;
			}
			num += num2;
		}
		while (num != size);
		int result = -1;
		if (!flag)
		{
			result = DataByteArrayToByteLength(dataBytesCount);
		}
		return result;
	}

	private void ReadDataByteArray(int size)
	{
		bool flag = false;
		byte[] array = new byte[size];
		int num = 0;
		do
		{
			int num2 = stream.Read(array, num, size - num);
			if (num2 == 0)
			{
				flag = true;
				break;
			}
			num += num2;
		}
		while (num != size);
		bool flag2 = false;
		if (!flag)
		{
			lock (_asyncLockReceived)
			{
				_appendQueueReceived.Enqueue(array);
			}
			flag2 = true;
		}
		while (!flag2)
		{
			Thread.Sleep(1);
		}
	}

	private void ProcessReceivedData(byte[] receivedDataBytes)
	{
		if (receivedDataBytes.Length >= 4 && (!(EnergySavingManager.instance != null) || !EnergySavingManager.instance.isSleeping))
		{
			byte[] array = new byte[4];
			Buffer.BlockCopy(receivedDataBytes, 0, array, 0, array.Length);
			string text = Encoding.ASCII.GetString(array);
			byte[] array2 = new byte[receivedDataBytes.Length - array.Length];
			Buffer.BlockCopy(receivedDataBytes, array.Length, array2, 0, array2.Length);
			if (text == "Norm")
			{
				string arg = Encoding.ASCII.GetString(array2);
				OnReceivedStringDataEvent.Invoke(arg);
			}
			if (text == "Byte")
			{
				OnReceivedByteDataEvent.Invoke(array2);
			}
		}
	}

	private IEnumerator RelaunchApp()
	{
		LOGWARNING("trigger reconnection");
		stop = true;
		while (Loom.numThreads >= Loom.maxThreads)
		{
			yield return null;
		}
		Action_ConnectServer();
		yield return new WaitForSeconds(2f);
		if (isReachedServer)
		{
			LOGWARNING("reached server, wait longer");
			yield return new WaitForSeconds(3f);
		}
		triggerQuitApp = false;
	}

	public void Action_Disconnect()
	{
		stop = true;
		if (client != null)
		{
			try
			{
				client.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		isConnected = false;
		StopCoroutine(ConnectServer());
	}

	public void Action_ConnectServer()
	{
		StartCoroutine(ConnectServer());
	}

	private IEnumerator ConnectServer()
	{
		LastReceivedTimeMS = GetCurrentMS();
		LOGWARNING("try reconnecting");
		isReachedServer = false;
		if (client != null)
		{
			try
			{
				client.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
		}
		yield return new WaitForSeconds(0.5f);
		client = new TcpClient();
		client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
		stop = false;
		yield return new WaitForSeconds(0.1f);
		Loom.RunAsync(delegate
		{
			client.BeginConnect(IPAddress.Parse(IP), ClientListenPort, ClientEndConnect, null);
			while (!client.Connected)
			{
				Thread.Sleep(1);
			}
			if (client.Connected)
			{
				if (stream != null)
				{
					stream.Close();
				}
				stream = client.GetStream();
				stream.ReadTimeout = 1000;
				stream.WriteTimeout = 200;
				LOGWARNING("Connected!");
				Loom.QueueOnMainThread(delegate
				{
					isReachedServer = true;
				});
				if (client.GetStream() != null)
				{
					byte[] array = new byte[1];
					client.GetStream().Write(array, 0, array.Length);
					client.GetStream().Flush();
				}
				while (!stop)
				{
					stream.Flush();
					dataSize = ReadDataByteSize(4);
					LOGWARNING("Received Data byte Length: " + dataSize);
					LastReceivedTimeMS = GetCurrentMS();
					isConnected = true;
					ReadDataByteArray(dataSize);
				}
			}
		});
		StartCoroutine(MainThreadSender());
		while (!stop)
		{
			while (_appendQueueReceived.Count > 0)
			{
				byte[] receivedDataBytes = new byte[1];
				lock (_asyncLockReceived)
				{
					receivedDataBytes = _appendQueueReceived.Dequeue();
				}
				ProcessReceivedData(receivedDataBytes);
			}
			yield return null;
		}
	}

	private IEnumerator MainThreadSender()
	{
		while (!stop)
		{
			Sender();
			yield return null;
		}
	}

	private void Sender()
	{
		try
		{
			if (udpClient == null)
			{
				udpClient = new UdpClient();
				udpClient.Client.SendTimeout = 200;
			}
			if (_appendQueueCmd.Count <= 0)
			{
				return;
			}
			while (_appendQueueCmd.Count > 0)
			{
				byte[] array = new byte[1];
				lock (_asyncSend)
				{
					array = _appendQueueCmd.Dequeue();
				}
				udpClient.Send(array, array.Length, new IPEndPoint(IPAddress.Parse(IP), ServerListenPort));
			}
		}
		catch (SocketException ex)
		{
			Debug.Log("error!: " + ex.ToString());
			if (udpClient != null)
			{
				udpClient.Close();
			}
			udpClient = null;
		}
	}

	private void ClientEndConnect(IAsyncResult result)
	{
		client.EndConnect(result);
	}

	private void LOGWARNING(string message)
	{
		if (enableLog)
		{
			Debug.LogWarning(message);
		}
	}

	private void OnApplicationQuit()
	{
		LOGWARNING("OnApplicationQuit");
		stop = true;
		if (client != null)
		{
			client.Close();
		}
	}

	private void OnDestroy()
	{
		Action_Disconnect();
	}

	private void CheckPause()
	{
		if (isPaused && !isPaused_old)
		{
			LOGWARNING("pause");
			Action_Disconnect();
		}
		if (!isPaused && isPaused_old)
		{
			LOGWARNING("running");
			if (!triggerQuitApp)
			{
				triggerQuitApp = true;
				StartCoroutine(RelaunchApp());
			}
		}
		isPaused_old = isPaused;
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		isPaused = !hasFocus;
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		isPaused = pauseStatus;
	}

	private void OnDisable()
	{
		Action_Disconnect();
	}

	private void OnEnable()
	{
		_ = Time.realtimeSinceStartup;
		_ = 3f;
	}
}
