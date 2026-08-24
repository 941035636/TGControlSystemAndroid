using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class FMClient2 : MonoBehaviour
{
	public FMNetworkManager2 Manager;

	public static FMClient2 instance;

	[HideInInspector]
	public int ServerListenPort = 3333;

	[HideInInspector]
	public int ClientListenPort = 3334;

	public string ServerIP = "0,0,0,0";

	public bool AutoNetworkDiscovery;

	[HideInInspector]
	public string ClientIP = "0,0,0,0";

	public bool IsConnected;

	public bool FoundServer;

	[HideInInspector]
	public int CurrentSeenTimeMS;

	public int LastSentTimeMS;

	public int LastReceivedTimeMS;

	[Header("[Experimental] suggested for mobile")]
	public bool UseMainThreadSender;

	private Queue<FMPacket2> _appendQueueReceivedPacket = new Queue<FMPacket2>();

	private object _asyncLockReceived = new object();

	private Queue<FMPacket2> _appendQueueSendPacket = new Queue<FMPacket2>();

	private object _asyncLock = new object();

	private bool stop;

	private UdpClient Client;

	private UdpClient ClientListener;

	private IPEndPoint ServerEp;

	public int ReceivedCount;

	public bool ShowLog = true;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	private void SetserverIP(string ip_)
	{
		Debug.LogError("给FMIP:" + ip_);
		ServerIP = ip_;
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
		FMPacket2 item = new FMPacket2
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
		FMPacket2 item = new FMPacket2
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
		if (ServerIP == _targetIP)
		{
			Action_AddPacket(_byteData, FMSendType.Server);
			return;
		}
		byte[] array = new byte[2] { 0, 3 };
		byte[] addressBytes = IPAddress.Parse(_targetIP).GetAddressBytes();
		byte[] array2 = new byte[_byteData.Length + array.Length + addressBytes.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(addressBytes, 0, array2, 2, addressBytes.Length);
		Buffer.BlockCopy(_byteData, 0, array2, 6, _byteData.Length);
		FMPacket2 item = new FMPacket2
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
		if (ServerIP == _targetIP)
		{
			Action_AddPacket(_stringData, FMSendType.Server);
			return;
		}
		byte[] bytes = Encoding.ASCII.GetBytes(_stringData);
		byte[] array = new byte[2] { 1, 3 };
		byte[] addressBytes = IPAddress.Parse(_targetIP).GetAddressBytes();
		byte[] array2 = new byte[bytes.Length + array.Length + addressBytes.Length];
		Buffer.BlockCopy(array, 0, array2, 0, array.Length);
		Buffer.BlockCopy(addressBytes, 0, array2, 2, addressBytes.Length);
		Buffer.BlockCopy(bytes, 0, array2, 6, bytes.Length);
		FMPacket2 item = new FMPacket2
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

	private void Start()
	{
		StartAll();
	}

	private void Update()
	{
		if (CurrentSeenTimeMS < 0 && LastReceivedTimeMS > 0)
		{
			IsConnected = ((Mathf.Abs(CurrentSeenTimeMS - int.MinValue) + (int.MaxValue - LastReceivedTimeMS) < 3000) ? true : false);
		}
		else
		{
			IsConnected = ((CurrentSeenTimeMS - LastReceivedTimeMS < 3000) ? true : false);
		}
	}

	public void Action_StartClient()
	{
		StartCoroutine(NetworkClientStart());
	}

	private IEnumerator NetworkClientStart()
	{
		LastSentTimeMS = Environment.TickCount;
		LastReceivedTimeMS = Environment.TickCount;
		stop = false;
		yield return new WaitForSeconds(0.5f);
		if (UseMainThreadSender)
		{
			StartCoroutine(MainThreadSender());
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
					Sender();
					Thread.Sleep(FoundServer ? 1 : 200);
				}
				Thread.Sleep(1);
			});
		}
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
					if (ClientListener == null)
					{
						ClientListener = new UdpClient(ClientListenPort);
						ClientListener.Client.ReceiveTimeout = 2000;
						ServerEp = new IPEndPoint(IPAddress.Any, ClientListenPort);
					}
					byte[] array2 = ClientListener.Receive(ref ServerEp);
					LastReceivedTimeMS = Environment.TickCount;
					if (!FoundServer)
					{
						FoundServer = true;
					}
					if (array2.Length > 2)
					{
						FMPacket2 item = new FMPacket2
						{
							SendByte = array2
						};
						lock (_asyncLockReceived)
						{
							_appendQueueReceivedPacket.Enqueue(item);
						}
					}
				}
				catch (SocketException)
				{
					if (ClientListener != null)
					{
						ClientListener.Close();
					}
					ClientListener = null;
				}
			}
			Thread.Sleep(1);
		});
		while (!stop)
		{
			CurrentSeenTimeMS = Environment.TickCount;
			while (_appendQueueReceivedPacket.Count > 0)
			{
				ReceivedCount = _appendQueueReceivedPacket.Count;
				FMPacket2 fMPacket = default(FMPacket2);
				lock (_asyncLockReceived)
				{
					fMPacket = _appendQueueReceivedPacket.Dequeue();
				}
				if (!(Manager != null))
				{
					continue;
				}
				byte[] sendByte = fMPacket.SendByte;
				if (sendByte.Length > 2)
				{
					byte[] obj = new byte[2]
					{
						sendByte[0],
						sendByte[1]
					};
					byte[] array = new byte[sendByte.Length - 2];
					Buffer.BlockCopy(sendByte, 2, array, 0, array.Length);
					switch (obj[0])
					{
					case 0:
						Manager.OnReceivedByteDataEvent.Invoke(array);
						break;
					case 1:
						Manager.OnReceivedStringDataEvent.Invoke(Encoding.ASCII.GetString(array));
						break;
					case 2:
						Manager.Action_SyncNetworkObjectTransform(array);
						break;
					}
					Manager.GetRawReceivedData.Invoke(sendByte);
				}
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
			if (Client == null)
			{
				Client = new UdpClient();
				Client.Client.SendTimeout = 200;
				Client.EnableBroadcast = true;
			}
			byte[] array = new byte[1];
			if (!FoundServer && AutoNetworkDiscovery)
			{
				if (CurrentSeenTimeMS - LastSentTimeMS > 2000)
				{
					Client.Send(array, array.Length, new IPEndPoint(IPAddress.Broadcast, ServerListenPort));
					LastSentTimeMS = Environment.TickCount;
				}
			}
			else if (_appendQueueSendPacket.Count > 0)
			{
				int num = 0;
				while (_appendQueueSendPacket.Count > 0 && num < 30)
				{
					num++;
					FMPacket2 fMPacket = default(FMPacket2);
					lock (_asyncLock)
					{
						fMPacket = _appendQueueSendPacket.Dequeue();
					}
					Client.Send(fMPacket.SendByte, fMPacket.SendByte.Length, new IPEndPoint(IPAddress.Parse(ServerIP), ServerListenPort));
					LastSentTimeMS = Environment.TickCount;
				}
			}
			else if (CurrentSeenTimeMS - LastSentTimeMS > 2000)
			{
				Client.Send(array, array.Length, new IPEndPoint(IPAddress.Parse(ServerIP), ServerListenPort));
				LastSentTimeMS = Environment.TickCount;
			}
		}
		catch (SocketException)
		{
			if (Client != null)
			{
				Client.Close();
			}
			Client = null;
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
		EventCenter.RemoveListener<string>(Eventdefine.FMIP2, SetserverIP);
	}

	private void OnDestroy()
	{
		StopAll();
	}

	private void OnEnable()
	{
		EventCenter.addlistener<string>(Eventdefine.FMIP2, SetserverIP);
		if (!(Time.timeSinceLevelLoad <= 3f) && stop)
		{
			StartAll();
		}
	}

	private void StartAll()
	{
		stop = false;
		Action_StartClient();
	}

	private void StopAll()
	{
		stop = true;
		IsConnected = false;
		FoundServer = false;
		if (Client != null)
		{
			try
			{
				Client.Close();
			}
			catch (Exception ex)
			{
				Debug.Log(ex.Message);
			}
			Client = null;
		}
		StopCoroutine(NetworkClientStart());
	}
}
