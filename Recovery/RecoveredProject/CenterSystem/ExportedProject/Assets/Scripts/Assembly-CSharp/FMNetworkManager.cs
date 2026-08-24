using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FMNetworkManager : MonoBehaviour
{
	[Serializable]
	public class FMServerSettings
	{
		public int ServerListenPort = 3333;

		[Tooltip("(( on supported devices only ))")]
		public bool UseAsyncListener;

		[Tooltip("(( suggested for low-end mobile, but not recommend for streaming large data ))")]
		public bool UseMainThreadSender = true;

		public int ConnectionCount;
	}

	[Serializable]
	public class FMClientSettings
	{
		public int ClientListenPort = 3334;

		[Tooltip("(( suggested for low-end mobile, but not recommend for streaming large data ))")]
		public bool UseMainThreadSender = true;

		[Tooltip("(( true by default ))")]
		public bool AutoNetworkDiscovery;

		[Tooltip("(( only applied when Auto Network Discovery is off ))")]
		public string ServerIP;

		public bool IsConnected;
	}

	private string _localIP;

	public static FMNetworkManager instance;

	public bool AutoInit = true;

	[HideInInspector]
	public bool Initialised;

	[Tooltip("Initialise as Server or Client")]
	public FMNetworkType NetworkType;

	[HideInInspector]
	public FMServer Server;

	[HideInInspector]
	public FMClient Client;

	[Tooltip("Network Settings for Server")]
	public FMServerSettings ServerSettings;

	[Tooltip("Network Settings for Client")]
	public FMClientSettings ClientSettings;

	public bool ShowLog = true;

	[TextArea(1, 10)]
	public string Status;

	public Text UIStatus;

	public UnityEventByteArray OnReceivedByteDataEvent;

	public UnityEventString OnReceivedStringDataEvent;

	public UnityEventByteArray GetRawReceivedData;

	[Header("[ Sync ] Server => Client")]
	[Tooltip("Sync Transformation of Network Objects. # Both Server and Clients should have same number of NetworkObjects")]
	public GameObject[] NetworkObjects;

	private FMNetworkTransform[] NetworkTransform;

	private float SyncFrequency = 0.05f;

	[Range(1f, 60f)]
	public float SyncFPS = 20f;

	private float SyncTimer;

	private float LastReceivedTimestamp;

	private float TargetTimestamp;

	private float CurrentTimestamp;

	public string ReadLocalIPAddress
	{
		get
		{
			if (_localIP == null)
			{
				_localIP = LocalIPAddress();
			}
			if (_localIP.Length <= 3)
			{
				_localIP = LocalIPAddress();
			}
			return _localIP;
		}
	}

	public string LocalIPAddress()
	{
		string text = "0.0.0.0";
		NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
		foreach (NetworkInterface networkInterface in allNetworkInterfaces)
		{
			if (networkInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && networkInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
			{
				continue;
			}
			foreach (UnicastIPAddressInformation unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
			{
				if (unicastAddress.Address.AddressFamily != AddressFamily.InterNetwork || !unicastAddress.IsDnsEligible)
				{
					continue;
				}
				try
				{
					if (unicastAddress.AddressValidLifetime / 2 != int.MaxValue)
					{
						text = unicastAddress.Address.ToString();
						break;
					}
					if (text == "0.0.0.0")
					{
						text = unicastAddress.Address.ToString();
					}
				}
				catch (Exception)
				{
					text = unicastAddress.Address.ToString();
					break;
				}
			}
		}
		return text;
	}

	private void Action_SendNetworkObjectTransform()
	{
		if (NetworkType == FMNetworkType.Server)
		{
			byte[] bytes = BitConverter.GetBytes(Time.realtimeSinceStartup);
			byte[] array = new byte[NetworkObjects.Length * 10 * 4];
			byte[] array2 = new byte[bytes.Length + array.Length];
			int num = 0;
			Buffer.BlockCopy(bytes, 0, array2, num, bytes.Length);
			num += bytes.Length;
			GameObject[] networkObjects = NetworkObjects;
			foreach (GameObject obj in networkObjects)
			{
				byte[] array3 = EncodeTransformByte(obj);
				Buffer.BlockCopy(array3, 0, array2, num, array3.Length);
				num += array3.Length;
			}
			Server.Action_AddNetworkObjectPacket(array2, FMSendType.Others);
		}
	}

	private byte[] EncodeTransformByte(GameObject obj)
	{
		byte[] array = new byte[40];
		Vector3 position = obj.transform.position;
		Quaternion rotation = obj.transform.rotation;
		Vector3 localScale = obj.transform.localScale;
		Buffer.BlockCopy(new float[10] { position.x, position.y, position.z, rotation.x, rotation.y, rotation.z, rotation.w, localScale.x, localScale.y, localScale.z }, 0, array, 0, array.Length);
		return array;
	}

	private float[] DecodeByteToFloatArray(byte[] _data, int _offset)
	{
		float[] array = new float[10];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = BitConverter.ToSingle(_data, i * 4 + _offset);
		}
		return array;
	}

	public void Action_SyncNetworkObjectTransform(byte[] _data)
	{
		float num = BitConverter.ToSingle(_data, 0);
		int num2 = 4;
		if (num > LastReceivedTimestamp)
		{
			LastReceivedTimestamp = TargetTimestamp;
			TargetTimestamp = num;
			CurrentTimestamp = LastReceivedTimestamp;
			for (int i = 0; i < NetworkObjects.Length; i++)
			{
				float[] array = DecodeByteToFloatArray(_data, num2 + i * 40);
				NetworkTransform[i].position = new Vector3(array[0], array[1], array[2]);
				NetworkTransform[i].rotation = new Quaternion(array[3], array[4], array[5], array[6]);
				NetworkTransform[i].localScale = new Vector3(array[7], array[8], array[9]);
			}
		}
	}

	public void Action_InitAsServer()
	{
		NetworkType = FMNetworkType.Server;
		Init();
	}

	public void Action_InitAsClient()
	{
		NetworkType = FMNetworkType.Client;
		Init();
	}

	private void Init()
	{
		if (NetworkType == FMNetworkType.Server)
		{
			Server = base.gameObject.AddComponent<FMServer>();
			Server.Manager = this;
			Server.ServerListenPort = ServerSettings.ServerListenPort;
			Server.ClientListenPort = ClientSettings.ClientListenPort;
			Server.UseAsyncListener = ServerSettings.UseAsyncListener;
			Server.UseMainThreadSender = ServerSettings.UseMainThreadSender;
		}
		else
		{
			Client = base.gameObject.AddComponent<FMClient>();
			Client.Manager = this;
			Client.ServerListenPort = ServerSettings.ServerListenPort;
			Client.ClientListenPort = ClientSettings.ClientListenPort;
			Client.UseMainThreadSender = ClientSettings.UseMainThreadSender;
			Client.AutoNetworkDiscovery = false;
			if (ClientSettings.ServerIP == "")
			{
				ClientSettings.ServerIP = "1.1.1.1";
			}
			if (!Client.AutoNetworkDiscovery)
			{
				Client.ServerIP = ClientSettings.ServerIP;
			}
			NetworkTransform = new FMNetworkTransform[NetworkObjects.Length];
			for (int i = 0; i < NetworkTransform.Length; i++)
			{
				NetworkTransform[i] = default(FMNetworkTransform);
				NetworkTransform[i].position = Vector3.zero;
				NetworkTransform[i].rotation = Quaternion.identity;
				NetworkTransform[i].localScale = new Vector3(1f, 1f, 1f);
			}
		}
		Initialised = true;
	}

	private void Awake()
	{
		Application.runInBackground = true;
		if (instance == null)
		{
			instance = this;
		}
	}

	private void Start()
	{
		if (AutoInit)
		{
			Init();
		}
	}

	private void Update()
	{
		if (!Initialised)
		{
			return;
		}
		if (NetworkType == FMNetworkType.Server)
		{
			if (Server.ConnectionCount > 0 && NetworkObjects.Length != 0)
			{
				SyncFrequency = 1f / SyncFPS;
				SyncTimer += Time.deltaTime;
				if (SyncTimer > SyncFrequency)
				{
					Action_SendNetworkObjectTransform();
					SyncTimer %= SyncFrequency;
				}
			}
			Server.ShowLog = ShowLog;
		}
		else
		{
			if (Client.IsConnected && NetworkObjects.Length != 0)
			{
				for (int i = 0; i < NetworkObjects.Length; i++)
				{
					CurrentTimestamp += Time.deltaTime;
					float value = (CurrentTimestamp - LastReceivedTimestamp) / (TargetTimestamp - LastReceivedTimestamp);
					value = Mathf.Clamp(value, 0f, 1f);
					NetworkObjects[i].transform.position = Vector3.Slerp(NetworkObjects[i].transform.position, NetworkTransform[i].position, value);
					NetworkObjects[i].transform.rotation = Quaternion.Slerp(NetworkObjects[i].transform.rotation, NetworkTransform[i].rotation, value);
					NetworkObjects[i].transform.localScale = Vector3.Slerp(NetworkObjects[i].transform.localScale, NetworkTransform[i].localScale, value);
				}
			}
			Client.ShowLog = ShowLog;
		}
		string text = "";
		text = text + "Thread: " + Loom.numThreads + " / " + Loom.maxThreads + "\n";
		text = text + "Network Type: " + NetworkType.ToString() + "\n";
		text = text + "Local IP: " + ReadLocalIPAddress + "\n";
		if (NetworkType == FMNetworkType.Server)
		{
			ServerSettings.ConnectionCount = Server.ConnectionCount;
			text = text + "Connection Count: " + ServerSettings.ConnectionCount + "\n";
			text = text + "Async Listener: " + ServerSettings.UseAsyncListener + "\n";
			text = text + "Use Main Thread Sender: " + ServerSettings.UseMainThreadSender + "\n";
			foreach (FMServer.ConnectedClient connectedClient in Server.ConnectedClients)
			{
				if (connectedClient != null)
				{
					text = text + "connected ip: " + connectedClient.IP + "\n";
					text = text + "last seen: " + connectedClient.LastSeenTimeMS + "\n";
					text = text + "last send: " + connectedClient.LastSentTimeMS + "\n";
				}
				else
				{
					text += "Connected Client: null/unknown issue\n";
				}
			}
		}
		else
		{
			ClientSettings.IsConnected = Client.IsConnected;
			text = text + "Is Connected: " + ClientSettings.IsConnected + "\n";
			text = text + "Use Main Thread Sender: " + ClientSettings.UseMainThreadSender + "\n";
			text = text + "last send: " + Client.LastSentTimeMS + "\n";
			text = text + "last received: " + Client.LastReceivedTimeMS + "\n";
		}
		Status = text;
		if (UIStatus != null)
		{
			UIStatus.text = Status;
		}
	}

	public void Send(byte[] _byteData, FMSendType _type)
	{
		Send(_byteData, _type, null);
	}

	public void Send(string _stringData, FMSendType _type)
	{
		Send(_stringData, _type, null);
	}

	public void SendToAll(byte[] _byteData)
	{
		Send(_byteData, FMSendType.All, null);
	}

	public void SendToServer(byte[] _byteData)
	{
		Send(_byteData, FMSendType.Server, null);
	}

	public void SendToOthers(byte[] _byteData)
	{
		Send(_byteData, FMSendType.Others, null);
	}

	public void SendToAll(string _stringData)
	{
		Send(_stringData, FMSendType.All, null);
	}

	public void SendToServer(string _stringData)
	{
		Send(_stringData, FMSendType.Server, null);
	}

	public void SendToOthers(string _stringData)
	{
		Send(_stringData, FMSendType.Others, null);
	}

	public void SendToTarget(byte[] _byteData, string _targetIP)
	{
		if (_targetIP == ReadLocalIPAddress || _targetIP == "127.0.0.1" || _targetIP == "localhost")
		{
			OnReceivedByteDataEvent.Invoke(_byteData);
		}
		else
		{
			Send(_byteData, FMSendType.TargetIP, _targetIP);
		}
	}

	public void SendToTarget(string _stringData, string _targetIP)
	{
		if (_targetIP == ReadLocalIPAddress || _targetIP == "127.0.0.1" || _targetIP == "localhost")
		{
			OnReceivedStringDataEvent.Invoke(_stringData);
		}
		else
		{
			Send(_stringData, FMSendType.TargetIP, _targetIP);
		}
	}

	private void Send(byte[] _byteData, FMSendType _type, string _targetIP)
	{
		if (!Initialised || (NetworkType == FMNetworkType.Client && !Client.IsConnected))
		{
			return;
		}
		switch (_type)
		{
		case FMSendType.All:
			if (NetworkType == FMNetworkType.Server)
			{
				Server.Action_AddPacket(_byteData, _type);
				OnReceivedByteDataEvent.Invoke(_byteData);
			}
			else
			{
				Client.Action_AddPacket(_byteData, _type);
			}
			break;
		case FMSendType.Server:
			if (NetworkType == FMNetworkType.Server)
			{
				OnReceivedByteDataEvent.Invoke(_byteData);
			}
			else
			{
				Client.Action_AddPacket(_byteData, _type);
			}
			break;
		case FMSendType.Others:
			if (NetworkType == FMNetworkType.Server)
			{
				Server.Action_AddPacket(_byteData, _type);
			}
			else
			{
				Client.Action_AddPacket(_byteData, _type);
			}
			break;
		case FMSendType.TargetIP:
			if (NetworkType == FMNetworkType.Server)
			{
				if (_targetIP.Length > 4)
				{
					Server.Action_AddPacket(_byteData, _targetIP);
				}
			}
			else if (_targetIP.Length > 4)
			{
				Client.Action_AddPacket(_byteData, _targetIP);
			}
			break;
		}
	}

	private void Send(string _stringData, FMSendType _type, string _targetIP)
	{
		if (!Initialised || (NetworkType == FMNetworkType.Client && !Client.IsConnected))
		{
			return;
		}
		switch (_type)
		{
		case FMSendType.All:
			if (NetworkType == FMNetworkType.Server)
			{
				Server.Action_AddPacket(_stringData, _type);
				OnReceivedStringDataEvent.Invoke(_stringData);
			}
			else
			{
				Client.Action_AddPacket(_stringData, _type);
			}
			break;
		case FMSendType.Server:
			if (NetworkType == FMNetworkType.Server)
			{
				OnReceivedStringDataEvent.Invoke(_stringData);
			}
			else
			{
				Client.Action_AddPacket(_stringData, _type);
			}
			break;
		case FMSendType.Others:
			if (NetworkType == FMNetworkType.Server)
			{
				Server.Action_AddPacket(_stringData, _type);
			}
			else
			{
				Client.Action_AddPacket(_stringData, _type);
			}
			break;
		case FMSendType.TargetIP:
			if (NetworkType == FMNetworkType.Server)
			{
				if (_targetIP.Length > 6)
				{
					Server.Action_AddPacket(_stringData, _targetIP);
				}
			}
			else if (_targetIP.Length > 6)
			{
				Client.Action_AddPacket(_stringData, _targetIP);
			}
			break;
		}
	}

	public void Action_ReloadScene()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
