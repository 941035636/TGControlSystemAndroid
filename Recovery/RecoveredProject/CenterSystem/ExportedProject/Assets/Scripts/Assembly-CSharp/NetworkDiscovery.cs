using System;
using System.Collections;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class NetworkDiscovery : MonoBehaviour
{
	public enum NetworkType
	{
		Server = 0,
		Client = 1
	}

	public NetworkType NT;

	[HideInInspector]
	public string ServerStr;

	[HideInInspector]
	public string ClientStr;

	public int port = 2222;

	public string ServerIP = "0,0,0,0";

	public string ClientIP = "0,0,0,0";

	[Header("[milliseconds (MS)]")]
	public int frequency = 100;

	public int StreamingPort;

	public bool isStreaming;

	public bool isListening;

	[Header("[CLIENT] will invoke when found server")]
	public UnityEventInt GetStreamingServerPort_Event;

	[Header("[CLIENT] will invoke when found server")]
	public UnityEventString GetServerIP_Event;

	[Header("[SERVER] will invoke when found client")]
	public UnityEventString GetClientIP_Event;

	[Header("[Client] suggested \"true\" when less than 3 threads")]
	public bool StopAfterFoundIP;

	private bool stop;

	[Header("[Experimental] for supported devices only")]
	public bool UseAsyncServer;

	private UdpClient Server;

	private UdpClient Client;

	public bool ShowLog = true;

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
				catch (Exception exception)
				{
					text = unicastAddress.Address.ToString();
					Debug.LogException(exception);
					break;
				}
			}
		}
		return text;
	}

	public void SetStreamingPort(int _port)
	{
		StreamingPort = _port;
	}

	public void Action_SetIsStreaming(bool _value)
	{
		isStreaming = _value;
		if (!isStreaming)
		{
			return;
		}
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
		}
		isListening = false;
	}

	private void Start()
	{
		StartAll();
	}

	private void Update()
	{
		if (NT == NetworkType.Client && !isStreaming && !isListening)
		{
			Action_StartClient();
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			StopAll();
		}
	}

	public void Action_StartServer()
	{
		StartCoroutine(NetworkServerStart());
	}

	public void Action_StartClient()
	{
		StartCoroutine(NetworkClientStart());
	}

	private IEnumerator NetworkServerStart()
	{
		stop = false;
		yield return new WaitForSeconds(2.5f);
		if (!UseAsyncServer)
		{
			while (Loom.numThreads >= Loom.maxThreads)
			{
				yield return null;
			}
			Loom.RunAsync(delegate
			{
				while (!stop)
				{
					byte[] bytes = Encoding.ASCII.GetBytes(StreamingPort.ToString());
					try
					{
						Server = new UdpClient(port);
						Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
						IPEndPoint ClientEp = new IPEndPoint(IPAddress.Any, port);
						byte[] bytes2 = Server.Receive(ref ClientEp);
						string ClientRequest = "CMD" + Encoding.ASCII.GetString(bytes2);
						Loom.QueueOnMainThread(delegate
						{
							ClientIP = ClientEp.Address.ToString();
							GetClientIP_Event.Invoke(ClientIP);
							ServerStr = "from client " + ClientEp.Address?.ToString() + " : " + ClientRequest;
						});
						Server.Client.EnableBroadcast = true;
						Server.Send(bytes, bytes.Length, ClientEp);
						Server.Close();
					}
					catch (SocketException ex2)
					{
						DebugLog("Socket exception: " + ex2);
					}
					Thread.Sleep(frequency);
				}
			});
			yield break;
		}
		Server = new UdpClient(port);
		Server.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, optionValue: true);
		Server.Client.ReceiveTimeout = 1000;
		Server.Client.SendTimeout = 200;
		if (stop)
		{
			yield break;
		}
		try
		{
			Server.BeginReceive(UdpReceiveCallback, null);
		}
		catch (SocketException ex)
		{
			Debug.Log("Socket exception: " + ex);
		}
	}

	private void UdpReceiveCallback(IAsyncResult ar)
	{
		if (ar.IsCompleted)
		{
			IPEndPoint remoteEP = new IPEndPoint(IPAddress.Any, port);
			byte[] bytes = Server.EndReceive(ar, ref remoteEP);
			string text = "CMD" + Encoding.ASCII.GetString(bytes);
			ClientIP = remoteEP.Address.ToString();
			GetClientIP_Event.Invoke(ClientIP);
			ServerStr = "from client " + remoteEP.Address?.ToString() + " : " + text;
			byte[] bytes2 = Encoding.ASCII.GetBytes(StreamingPort.ToString());
			Server.Client.EnableBroadcast = true;
			try
			{
				Server.Send(bytes2, bytes2.Length, remoteEP);
			}
			catch (SocketException ex)
			{
				DebugLog("Socket exception: " + ex);
			}
		}
		if (!stop)
		{
			try
			{
				Server.BeginReceive(UdpReceiveCallback, null);
			}
			catch (SocketException ex2)
			{
				DebugLog("network discovery: can't find any server: " + ex2.ToString());
			}
		}
	}

	private IEnumerator NetworkClientStart()
	{
		stop = false;
		isListening = true;
		yield return new WaitForSeconds(2.5f);
		string LastSeenTime = DateTime.Now.ToString("mm:ss");
		while (Loom.numThreads >= Loom.maxThreads)
		{
			yield return new WaitForEndOfFrame();
		}
		Loom.RunAsync(delegate
		{
			while (!isStreaming && !stop)
			{
				try
				{
					Client = new UdpClient();
					Client.Client.SendTimeout = 200;
					Client.Client.ReceiveTimeout = 500;
					byte[] bytes = Encoding.ASCII.GetBytes("last: " + LastSeenTime);
					IPEndPoint ServerEp = new IPEndPoint(IPAddress.Any, port);
					Client.EnableBroadcast = true;
					Client.Send(bytes, bytes.Length, new IPEndPoint(IPAddress.Broadcast, port));
					byte[] bytes2 = Client.Receive(ref ServerEp);
					string ServerResponse = Encoding.ASCII.GetString(bytes2);
					Client.Close();
					if (ServerResponse.Length > 0)
					{
						Loom.QueueOnMainThread(delegate
						{
							LastSeenTime = DateTime.Now.ToString("mm:ss");
							if (StopAfterFoundIP)
							{
								stop = true;
							}
							StreamingPort = int.Parse(ServerResponse);
							GetStreamingServerPort_Event.Invoke(StreamingPort);
							ServerIP = ServerEp.Address.ToString();
							GetServerIP_Event.Invoke(ServerIP);
							ClientStr = "from server: " + ServerResponse + " : " + ServerIP;
						});
					}
				}
				catch (SocketException ex)
				{
					DebugLog("network discovery: can't find any server: " + ex.ToString());
				}
				Thread.Sleep(frequency);
			}
			Thread.Sleep(1);
		});
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
		if (Time.frameCount > 0 && stop)
		{
			StartAll();
		}
	}

	private void StartAll()
	{
		isStreaming = false;
		isListening = false;
		stop = false;
		if (NT == NetworkType.Server)
		{
			ServerIP = LocalIPAddress();
			Action_StartServer();
		}
		if (NT == NetworkType.Client)
		{
			ClientIP = LocalIPAddress();
			Action_StartClient();
		}
	}

	private void StopAll()
	{
		stop = true;
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
		}
		if (Server != null)
		{
			try
			{
				Server.Close();
			}
			catch (Exception ex2)
			{
				Debug.Log(ex2.Message);
			}
		}
		if (NT == NetworkType.Server)
		{
			StopCoroutine(NetworkServerStart());
		}
		if (NT == NetworkType.Client)
		{
			StopCoroutine(NetworkClientStart());
		}
	}
}
