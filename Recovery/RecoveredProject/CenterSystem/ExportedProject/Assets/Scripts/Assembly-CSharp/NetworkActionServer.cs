using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkActionServer : MonoBehaviour
{
	public static NetworkActionServer instance;

	private object _asyncLock = new object();

	private Queue<byte[]> _appendQueueCmd = new Queue<byte[]>();

	private object _asyncLockReceived = new object();

	private Queue<byte[]> _appendQueueReceived = new Queue<byte[]>();

	public bool enableLog;

	[Header("[Network Settings]")]
	public int ServerListenPort = 2389;

	private int ClientListenPort = 2390;

	public bool isConnected;

	public int ConnectionCount;

	public UnityEventInt GetServerListenPort_Event;

	private TcpListener listener;

	private bool stop;

	private List<TcpClient> clients = new List<TcpClient>();

	private List<NetworkStream> streams = new List<NetworkStream>();

	private const int SEND_RECEIVE_COUNT = 4;

	private List<int> dataLostTimers = new List<int>();

	private UdpClient Server;

	private IPEndPoint ClientEp;

	private object _UdpLock = new object();

	private Queue<string> commands = new Queue<string>();

	[Header("[Experimental] for supported devices only")]
	public bool UseAsyncUdpReceiver;

	public UnityEventString OnReceivedStringDataEvent;

	public UnityEventByteArray OnReceivedByteDataEvent;

	private bool CreatedServer;

	[Header("[Experimental] Tested on Mobile")]
	public bool UseAsyncTcpListener = true;

	[Header("[Experimental] suggested for mobile")]
	public bool UseMainThreadSender;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void Action_AddCmd(string _cmd)
	{
		if (!isConnected)
		{
			return;
		}
		_ = Time.realtimeSinceStartup;
		byte[] bytes = Encoding.ASCII.GetBytes("Norm");
		byte[] bytes2 = Encoding.ASCII.GetBytes(_cmd);
		byte[] array = new byte[bytes.Length + bytes2.Length];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		Buffer.BlockCopy(bytes2, 0, array, bytes.Length, bytes2.Length);
		if (_appendQueueCmd.Count >= 30)
		{
			return;
		}
		lock (_asyncLock)
		{
			_appendQueueCmd.Enqueue(array);
		}
	}

	public void Action_AddCmd(byte[] _byteData)
	{
		if (!isConnected)
		{
			return;
		}
		byte[] bytes = Encoding.ASCII.GetBytes("Byte");
		byte[] array = new byte[bytes.Length + _byteData.Length];
		Buffer.BlockCopy(bytes, 0, array, 0, bytes.Length);
		Buffer.BlockCopy(_byteData, 0, array, bytes.Length, _byteData.Length);
		if (_appendQueueCmd.Count >= 1000)
		{
			return;
		}
		lock (_asyncLock)
		{
			_appendQueueCmd.Enqueue(array);
		}
	}

	private IEnumerator NetworkServerStart()
	{
		stop = false;
		yield return new WaitForSeconds(1f);
		if (UseAsyncUdpReceiver)
		{
			Server = new UdpClient(ServerListenPort);
			Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
			Server.Client.ReceiveTimeout = 1000;
			if (!stop)
			{
				try
				{
					Server.BeginReceive(UdpReceiveCallback, null);
				}
				catch (SocketException ex)
				{
					Debug.Log("Socket exception: " + ex);
				}
			}
		}
		else
		{
			while (Loom.numThreads >= Loom.maxThreads)
			{
				yield return null;
			}
			Loom.RunAsync(delegate
			{
				while (!stop)
				{
					try
					{
						if (Server == null)
						{
							Server = new UdpClient(ServerListenPort);
							Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
							Server.Client.EnableBroadcast = true;
							ClientEp = new IPEndPoint(IPAddress.Any, ServerListenPort);
						}
						byte[] item2 = Server.Receive(ref ClientEp);
						lock (_asyncLockReceived)
						{
							_appendQueueReceived.Enqueue(item2);
						}
					}
					catch (SocketException ex2)
					{
						Debug.Log("Socket exception: " + ex2);
						if (Server != null)
						{
							Server.Close();
						}
						Server = null;
					}
				}
				Thread.Sleep(1);
			});
		}
		while (!stop)
		{
			while (_appendQueueReceived.Count > 0)
			{
				byte[] array = new byte[1];
				lock (_asyncLockReceived)
				{
					array = _appendQueueReceived.Dequeue();
				}
				byte[] array2 = new byte[1] { array[0] };
				byte[] array3 = new byte[3]
				{
					array[1],
					array[2],
					array[3]
				};
				byte[] array4 = new byte[array.Length - array2.Length - array3.Length];
				Buffer.BlockCopy(array, 4, array4, 0, array4.Length);
				string text = Encoding.ASCII.GetString(array3);
				if (array2[0] == 0)
				{
					if (text.Contains("Rpc"))
					{
						lock (_UdpLock)
						{
							Action_AddCmd(array4);
						}
						OnReceivedByteDataEvent.Invoke(array4);
					}
				}
				else if (text.Contains("Rpc"))
				{
					string text2 = Encoding.ASCII.GetString(array4);
					string item = "Cmd" + text2;
					lock (_UdpLock)
					{
						commands.Enqueue(item);
					}
					OnReceivedStringDataEvent.Invoke(text2);
				}
			}
			yield return null;
		}
	}

	private void UdpReceiveCallback(IAsyncResult ar)
	{
		if (ar.IsCompleted)
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, ServerListenPort);
			byte[] item = Server.EndReceive(ar, ref remoteEP);
			lock (_asyncLockReceived)
			{
				_appendQueueReceived.Enqueue(item);
			}
		}
		if (!stop)
		{
			try
			{
				Server.BeginReceive(UdpReceiveCallback, null);
			}
			catch (SocketException ex)
			{
				Debug.Log("Socket exception: " + ex);
			}
		}
	}

	private void StopServerListener()
	{
		if (Server != null)
		{
			Server.Close();
		}
		StopCoroutine(NetworkServerStart());
	}

	private void Start()
	{
		Application.runInBackground = true;
		ClientListenPort = ServerListenPort + 1;
		StartCoroutine(NetworkServerStart());
		StartCoroutine(initServer());
	}

	private void Update()
	{
		GetServerListenPort_Event.Invoke(ServerListenPort);
		isConnected = ((ConnectionCount > 0) ? true : false);
		while (commands.Count > 0)
		{
			lock (_UdpLock)
			{
				Action_AddCmd(commands.Dequeue());
			}
		}
	}

	private IEnumerator initServer()
	{
		if (!CreatedServer)
		{
			CreatedServer = true;
			listener = new TcpListener(IPAddress.Any, ClientListenPort);
			listener.Start();
			listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
			if (UseAsyncTcpListener)
			{
				listener.BeginAcceptSocket(AcceptCallback, listener);
			}
			else
			{
				Loom.RunAsync(delegate
				{
					while (!stop)
					{
						clients.Add(listener.AcceptTcpClient());
						clients[clients.Count - 1].NoDelay = true;
						isConnected = true;
						streams.Add(clients[clients.Count - 1].GetStream());
						streams[streams.Count - 1].WriteTimeout = 500;
						dataLostTimers.Add(0);
						Loom.QueueOnMainThread(delegate
						{
							isConnected = true;
						});
						Thread.Sleep(1);
					}
				});
			}
			StartCoroutine(senderCOR());
		}
		yield return null;
	}

	private void AcceptCallback(IAsyncResult ar)
	{
		TcpClient tcpClient = ((TcpListener)ar.AsyncState).EndAcceptTcpClient(ar);
		tcpClient.NoDelay = true;
		isConnected = true;
		clients.Add(tcpClient);
		streams.Add(tcpClient.GetStream());
		dataLostTimers.Add(0);
		ConnectionCount = clients.Count;
		if (!stop)
		{
			listener.BeginAcceptSocket(AcceptCallback, listener);
		}
	}

	private IEnumerator senderCOR()
	{
		while (!isConnected)
		{
			yield return null;
		}
		LOG("Connected!");
		bool readyToGetFrame = true;
		while (!stop)
		{
			if (clients.Count > 0)
			{
				readyToGetFrame = false;
				if (UseMainThreadSender)
				{
					Sender();
					readyToGetFrame = true;
				}
				else
				{
					while (Loom.numThreads >= Loom.maxThreads)
					{
						yield return null;
					}
					Loom.RunAsync(delegate
					{
						Sender();
						readyToGetFrame = true;
					});
				}
			}
			float startCheckTime = Time.realtimeSinceStartup;
			while (!readyToGetFrame)
			{
				LOG("Waiting To get new frame");
				if (Time.realtimeSinceStartup - startCheckTime > 2f)
				{
					for (int num = 0; num < clients.Count; num++)
					{
						if (!clients[num].Connected)
						{
							RemoveClientConnection(num);
						}
					}
					readyToGetFrame = true;
				}
				ConnectionCount = clients.Count;
				yield return null;
			}
			yield return null;
		}
	}

	private void Sender()
	{
		byte[] array = new byte[4];
		byte[] array2 = new byte[1];
		for (int i = 0; i < clients.Count; i++)
		{
			try
			{
				if (clients[i].Available <= 0)
				{
					dataLostTimers[i]++;
					if (dataLostTimers[i] > 0)
					{
						RemoveClientConnection(i);
						ConnectionCount = clients.Count;
					}
				}
				else
				{
					dataLostTimers[i] = 0;
				}
				if (streams[i].CanWrite)
				{
					streams[i].WriteTimeout = 200;
				}
			}
			catch (SocketException ex)
			{
				LOG("Socket exception: " + i + " >> " + ex);
				RemoveClientConnection(i);
				ConnectionCount = clients.Count;
			}
		}
		if (_appendQueueCmd.Count > 0)
		{
			while (_appendQueueCmd.Count > 0)
			{
				lock (_asyncLock)
				{
					array2 = _appendQueueCmd.Dequeue();
				}
				byteLengthToDataByteArray(array2.Length, array);
				for (int j = 0; j < clients.Count; j++)
				{
					StreamWrite(streams[j], array2, array);
				}
			}
		}
		else
		{
			array2 = new byte[1];
			byteLengthToDataByteArray(array2.Length, array);
			for (int k = 0; k < clients.Count; k++)
			{
				StreamWrite(streams[k], array2, array);
			}
		}
		ConnectionCount = clients.Count;
	}

	private void byteLengthToDataByteArray(int byteLength, byte[] fullBytes)
	{
		Array.Clear(fullBytes, 0, fullBytes.Length);
		BitConverter.GetBytes(byteLength).CopyTo(fullBytes, 0);
	}

	private void StreamWrite(NetworkStream _stream, byte[] _dataBytes, byte[] _dataBytesLength)
	{
		if (_stream.CanWrite)
		{
			try
			{
				_stream.Write(_dataBytesLength, 0, _dataBytesLength.Length);
			}
			catch (SocketException ex)
			{
				LOG("Socket exception: " + ex);
			}
			try
			{
				_stream.Write(_dataBytes, 0, _dataBytes.Length);
			}
			catch (SocketException ex2)
			{
				LOG("Socket exception: " + ex2);
			}
			_stream.Flush();
		}
		else
		{
			_stream.Flush();
			LOG("can't write?");
		}
	}

	private void RemoveClientConnection(int _id)
	{
		try
		{
			streams[_id].Close();
			clients[_id].Close();
		}
		catch (SocketException ex)
		{
			LOG("Socket exception: " + ex);
		}
		streams.Remove(streams[_id]);
		clients.Remove(clients[_id]);
		dataLostTimers.Remove(dataLostTimers[_id]);
	}

	private void LOG(string messsage)
	{
		if (enableLog)
		{
			Debug.Log(messsage);
		}
	}

	private void StopAll()
	{
		stop = true;
		if (listener != null)
		{
			listener.Stop();
		}
		for (int i = 0; i < clients.Count; i++)
		{
			RemoveClientConnection(i);
		}
		StopCoroutine(senderCOR());
		StopServerListener();
		CreatedServer = false;
	}

	private void OnApplicationQuit()
	{
		StopAll();
	}

	private void OnDisable()
	{
		StopAll();
	}

	private void OnDestroy()
	{
		StopAll();
	}

	private void OnEnable()
	{
		if (!(Time.realtimeSinceStartup <= 3f) && !CreatedServer)
		{
			stop = false;
			isConnected = false;
			if (clients != null)
			{
				clients.Clear();
			}
			StartCoroutine(NetworkServerStart());
			StartCoroutine(initServer());
		}
	}
}
