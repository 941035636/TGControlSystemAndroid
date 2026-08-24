using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class FMServer2 : MonoBehaviour
{
	[Serializable]
	public class ConnectedClient
	{
		public string IP;

		public int Port;

		public int LastSeenTimeMS;

		public int LastSentTimeMS;

		public UdpClient Client;

		public IPEndPoint ClientEp;

		public void Send(byte[] _byte)
		{
			try
			{
				if (Client == null)
				{
					Client = new UdpClient(IP, Port);
					Client.Client.SendTimeout = 500;
				}
				Client.Send(_byte, _byte.Length);
				LastSentTimeMS = Environment.TickCount;
			}
			catch (Exception)
			{
				Close();
			}
		}

		public void Close()
		{
			if (Client != null)
			{
				Client.Close();
			}
			Client = null;
		}
	}

	[HideInInspector]
	public FMNetworkManager2 Manager;

	public static FMServer2 instance;

	[HideInInspector]
	public int ServerListenPort = 3333;

	[HideInInspector]
	public int ClientListenPort = 3334;

	public bool IsConnected;

	public int ConnectionCount;

	public List<ConnectedClient> ConnectedClients = new List<ConnectedClient>();

	[HideInInspector]
	public int CurrentSeenTimeMS;

	private Queue<FMPacket> _appendQueueReceivedPacket = new Queue<FMPacket>();

	private object _asyncLockReceived = new object();

	private Queue<FMPacket> _appendQueueSendPacket = new Queue<FMPacket>();

	private object _asyncLock = new object();

	private bool stop;

	public int CmdLength;

	[Header("[Experimental] for supported devices only")]
	public bool UseAsyncListener;

	[Header("[Experimental] suggested for mobile")]
	public bool UseMainThreadSender;

	private UdpClient Server;

	private IPEndPoint ClientEp;

	public bool ShowLog = true;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public void Action_CheckClientStatus(string _ip)
	{
		bool flag = false;
		for (int i = 0; i < ConnectedClients.Count; i++)
		{
			if (_ip == ConnectedClients[i].IP)
			{
				flag = true;
				ConnectedClients[i].IP = _ip;
				ConnectedClients[i].LastSeenTimeMS = Environment.TickCount;
			}
		}
		if (!flag)
		{
			ConnectedClient connectedClient = new ConnectedClient();
			connectedClient.IP = _ip;
			connectedClient.Port = ClientListenPort;
			connectedClient.LastSeenTimeMS = Environment.TickCount;
			connectedClient.Send(new byte[1]);
			ConnectedClients.Add(connectedClient);
		}
	}

	public void Action_AddPacket(byte[] _byteData, FMSendType _type)
	{
		byte[] array = new byte[2] { 0, 0 };
		if (_type == FMSendType.All)
		{
			array[1] = 0;
		}
		if (_type == FMSendType.Server)
		{
			array[1] = 1;
		}
		if (_type == FMSendType.Others)
		{
			array[1] = 2;
		}
		byte[] array2 = new byte[_byteData.Length + array.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(_byteData, 0, array2, 2, _byteData.Length);
		FMPacket item = new FMPacket
		{
			SendByte = array2,
			SendType = _type
		};
		lock (_asyncLock)
		{
			_appendQueueSendPacket.Enqueue(item);
		}
	}

	public void Action_AddPacket(string _stringData, FMSendType _type)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(_stringData);
		byte[] array = new byte[2] { 1, 0 };
		if (_type == FMSendType.All)
		{
			array[1] = 0;
		}
		if (_type == FMSendType.Server)
		{
			array[1] = 1;
		}
		if (_type == FMSendType.Others)
		{
			array[1] = 2;
		}
		byte[] array2 = new byte[bytes.Length + array.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(bytes, 0, array2, 2, bytes.Length);
		FMPacket item = new FMPacket
		{
			SendByte = array2,
			SendType = _type
		};
		lock (_asyncLock)
		{
			_appendQueueSendPacket.Enqueue(item);
		}
	}

	public void Action_AddPacket(byte[] _byteData, string _targetIP)
	{
		byte[] array = new byte[2] { 0, 3 };
		byte[] array2 = new byte[_byteData.Length + array.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(_byteData, 0, array2, 2, _byteData.Length);
		FMPacket item = new FMPacket
		{
			SendByte = array2,
			SendType = FMSendType.TargetIP,
			TargetIP = _targetIP
		};
		lock (_asyncLock)
		{
			_appendQueueSendPacket.Enqueue(item);
		}
	}

	public void Action_AddPacket(string _stringData, string _targetIP)
	{
		byte[] bytes = Encoding.ASCII.GetBytes(_stringData);
		byte[] array = new byte[2] { 1, 3 };
		byte[] array2 = new byte[bytes.Length + array.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(bytes, 0, array2, 2, bytes.Length);
		FMPacket item = new FMPacket
		{
			SendByte = array2,
			SendType = FMSendType.TargetIP,
			TargetIP = _targetIP
		};
		lock (_asyncLock)
		{
			_appendQueueSendPacket.Enqueue(item);
		}
	}

	public void Action_AddPacket(FMPacket _packet)
	{
		lock (_asyncLock)
		{
			_appendQueueSendPacket.Enqueue(_packet);
		}
	}

	public void Action_AddNetworkObjectPacket(byte[] _byteData, FMSendType _type)
	{
		byte[] array = new byte[2] { 2, 0 };
		if (_type == FMSendType.All)
		{
			array[1] = 0;
		}
		if (_type == FMSendType.Server)
		{
			array[1] = 1;
		}
		if (_type == FMSendType.Others)
		{
			array[1] = 2;
		}
		byte[] array2 = new byte[_byteData.Length + array.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(_byteData, 0, array2, 2, _byteData.Length);
		if (_appendQueueSendPacket.Count < 120)
		{
			FMPacket item = new FMPacket
			{
				SendByte = array2,
				SendType = _type
			};
			lock (_asyncLock)
			{
				_appendQueueSendPacket.Enqueue(item);
			}
		}
	}

	private void Start()
	{
		StartAll();
	}

	private void Update()
	{
		IsConnected = ((ConnectionCount > 0) ? true : false);
		CmdLength = _appendQueueSendPacket.Count;
	}

	public void Action_StartServer()
	{
		StartCoroutine(NetworkServerStart());
	}

	private IEnumerator NetworkServerStart()
	{
		stop = false;
		yield return new WaitForSeconds(0.5f);
		if (!UseAsyncListener)
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
						byte[] array4 = Server.Receive(ref ClientEp);
						string text = ClientEp.Address.ToString();
						if (array4.Length > 2)
						{
							FMPacket item = new FMPacket
							{
								SendByte = array4
							};
							if (array4[1] == 2)
							{
								item.SkipIP = text;
							}
							lock (_asyncLockReceived)
							{
								_appendQueueReceivedPacket.Enqueue(item);
							}
						}
						Action_CheckClientStatus(text);
					}
					catch (SocketException)
					{
						if (Server != null)
						{
							Server.Close();
						}
						Server = null;
					}
				}
			});
		}
		else
		{
			if (Server == null)
			{
				Server = new UdpClient(ServerListenPort);
				Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
				ClientEp = new IPEndPoint(IPAddress.Any, ServerListenPort);
				Server.Client.ReceiveTimeout = 1000;
				Server.Client.SendTimeout = 200;
			}
			if (!stop)
			{
				try
				{
					Server.BeginReceive(UdpReceiveCallback, null);
				}
				catch (SocketException)
				{
					if (Server != null)
					{
						Server.Close();
					}
					Server = null;
				}
			}
		}
		if (!UseMainThreadSender)
		{
			while (Loom.numThreads >= Loom.maxThreads)
			{
				yield return null;
			}
			Loom.RunAsync(delegate
			{
				while (!stop)
				{
					Sender();
					Thread.Sleep(1);
				}
				Thread.Sleep(1);
			});
		}
		else
		{
			StartCoroutine(MainThreadSender());
		}
		while (!stop)
		{
			CurrentSeenTimeMS = Environment.TickCount;
			while (_appendQueueReceivedPacket.Count > 0)
			{
				FMPacket packet = default(FMPacket);
				lock (_asyncLockReceived)
				{
					packet = _appendQueueReceivedPacket.Dequeue();
				}
				if (!(Manager != null))
				{
					continue;
				}
				byte[] sendByte = packet.SendByte;
				if (sendByte.Length <= 2)
				{
					continue;
				}
				byte[] array = new byte[2]
				{
					sendByte[0],
					sendByte[1]
				};
				if (array[1] == 3)
				{
					packet.TargetIP = new IPAddress(new byte[4]
					{
						sendByte[2],
						sendByte[3],
						sendByte[4],
						sendByte[5]
					}).ToString();
					byte[] array2 = new byte[sendByte.Length - 6];
					Buffer.BlockCopy(sendByte, 6, array2, 0, array2.Length);
					if (packet.TargetIP == Manager.ReadLocalIPAddress)
					{
						switch (array[0])
						{
						case 0:
							Manager.OnReceivedByteDataEvent.Invoke(array2);
							break;
						case 1:
							Manager.OnReceivedStringDataEvent.Invoke(Encoding.ASCII.GetString(array2));
							break;
						}
					}
					else if (packet.TargetIP != Manager.ReadLocalIPAddress)
					{
						packet.SendType = FMSendType.TargetIP;
						packet.SendByte = new byte[array.Length + array2.Length];
						packet.SendByte[0] = array[0];
						packet.SendByte[1] = array[1];
						Buffer.BlockCopy(array2, 0, packet.SendByte, 2, array2.Length);
						Action_AddPacket(packet);
					}
				}
				else
				{
					byte[] array3 = new byte[sendByte.Length - 2];
					Buffer.BlockCopy(sendByte, 2, array3, 0, array3.Length);
					switch (array[0])
					{
					case 0:
						Manager.OnReceivedByteDataEvent.Invoke(array3);
						break;
					case 1:
						Manager.OnReceivedStringDataEvent.Invoke(Encoding.ASCII.GetString(array3));
						break;
					}
					switch (array[1])
					{
					case 0:
						Action_AddPacket(packet);
						break;
					case 2:
						Action_AddPacket(packet);
						break;
					}
				}
				Manager.GetRawReceivedData.Invoke(sendByte);
			}
			yield return null;
		}
	}

	private void UdpReceiveCallback(IAsyncResult ar)
	{
		if (ar.IsCompleted)
		{
			byte[] array = Server.EndReceive(ar, ref ClientEp);
			string text = ClientEp.Address.ToString();
			if (array.Length > 2)
			{
				FMPacket item = new FMPacket
				{
					SendByte = array
				};
				if (array[1] == 2)
				{
					item.SkipIP = text;
				}
				lock (_asyncLockReceived)
				{
					_appendQueueReceivedPacket.Enqueue(item);
				}
			}
			Action_CheckClientStatus(text);
		}
		if (stop)
		{
			return;
		}
		try
		{
			Server.BeginReceive(UdpReceiveCallback, null);
		}
		catch (SocketException)
		{
			if (Server != null)
			{
				Server.Close();
			}
			Server = null;
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
		for (int i = 0; i < ConnectedClients.Count; i++)
		{
			bool flag = false;
			if (((CurrentSeenTimeMS >= 0 || ConnectedClients[i].LastSeenTimeMS <= 0) ? ((CurrentSeenTimeMS - ConnectedClients[i].LastSeenTimeMS < 3000) ? 1 : 0) : ((Mathf.Abs(CurrentSeenTimeMS - int.MinValue) + (int.MaxValue - ConnectedClients[i].LastSeenTimeMS) < 3000) ? 1 : 0)) == 0)
			{
				ConnectedClients[i].Close();
				ConnectedClients.Remove(ConnectedClients[i]);
			}
		}
		ConnectionCount = ConnectedClients.Count;
		if (_appendQueueSendPacket.Count > 0)
		{
			int num = 0;
			while (_appendQueueSendPacket.Count > 0 && num < 30)
			{
				num++;
				FMPacket fMPacket = default(FMPacket);
				lock (_asyncLock)
				{
					fMPacket = _appendQueueSendPacket.Dequeue();
				}
				if (fMPacket.SendType != FMSendType.TargetIP)
				{
					for (int j = 0; j < ConnectedClients.Count; j++)
					{
						if (ConnectedClients[j].IP != fMPacket.SkipIP)
						{
							ConnectedClients[j].Send(fMPacket.SendByte);
						}
					}
					continue;
				}
				for (int k = 0; k < ConnectedClients.Count; k++)
				{
					if (ConnectedClients[k].IP == fMPacket.TargetIP)
					{
						ConnectedClients[k].Send(fMPacket.SendByte);
					}
					else if (CurrentSeenTimeMS - ConnectedClients[k].LastSentTimeMS > 1000)
					{
						ConnectedClients[k].Send(new byte[1]);
					}
				}
			}
			return;
		}
		for (int l = 0; l < ConnectedClients.Count; l++)
		{
			if (CurrentSeenTimeMS - ConnectedClients[l].LastSentTimeMS > 1000)
			{
				ConnectedClients[l].Send(new byte[1]);
			}
		}
	}

	public void DebugLog(string _value)
	{
		if (ShowLog)
		{
			Debug.Log(_value);
		}
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
		if (!(Time.timeSinceLevelLoad <= 3f) && stop)
		{
			StartAll();
		}
	}

	private void StartAll()
	{
		stop = false;
		Action_StartServer();
	}

	private void StopAll()
	{
		stop = true;
		IsConnected = false;
		if (Server != null)
		{
			try
			{
				Server.Close();
			}
			catch (Exception ex)
			{
				DebugLog(ex.Message);
			}
			Server = null;
		}
		if (ConnectedClients.Count > 0)
		{
			foreach (ConnectedClient connectedClient in ConnectedClients)
			{
				connectedClient.Close();
			}
		}
		StopCoroutine(NetworkServerStart());
	}
}
