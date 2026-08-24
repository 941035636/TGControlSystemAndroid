using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class MyUDPClient : MonoBehaviour
{
	private string editString = "hello wolrd";

	private Socket socket;

	private EndPoint serverEnd;

	private IPEndPoint ipEnd;

	private string recvStr;

	private string sendStr;

	private byte[] recvData = new byte[1024];

	private byte[] sendData = new byte[1024];

	private int recvLen;

	private Thread connectThread;

	private volatile bool receiveRunning;

	private static MyUDPClient Instance;

	public GameObject _shotctl;

	private string Info = " ";

	private string message = " ";

	public static MyUDPClient GetInstance()
	{
		if (Instance == null)
		{
			Instance = UnityEngine.Object.FindObjectOfType<MyUDPClient>();
		}
		return Instance;
	}

	private void init()
	{
		EventCenter.addlistener(Eventdefine.close, sendClosePage);
		EventCenter.addlistener(Eventdefine.shutdownphoto, sendshutdown);
		EventCenter.addlistener(Eventdefine.takephoto, SendPhoto);
		EventCenter.addlistener<string, string>(Eventdefine.PhotoUpdConnect, InitSocket);
	}

	private void SendOpenTv()
	{
		SocketSend("01");
		Debug.LogError("打开电视");
	}

	public void sendClosePage()
	{
		SocketSend("Close");
	}

	public void sendshutdown()
	{
		SocketSend("shutdown");
	}

	public void SendPhoto()
	{
		SocketSend("TakePhoto");
	}

	private void Start()
	{
	}

	private void Awake()
	{
		init();
	}

	private void OnDestroy()
	{
		CloseTransport();
		EventCenter.RemoveListener(Eventdefine.close, sendClosePage);
		EventCenter.RemoveListener(Eventdefine.shutdownphoto, sendshutdown);
		EventCenter.RemoveListener(Eventdefine.takephoto, SendPhoto);
		EventCenter.RemoveListenerTwo<string, string>(Eventdefine.PhotoUpdConnect, InitSocket);
	}

	private void InitSocket(string IP, string Port)
	{
		try
		{
			CloseTransport();
			ipEnd = new IPEndPoint(IPAddress.Parse(IP), int.Parse(Port));
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			serverEnd = new IPEndPoint(IPAddress.Any, 0);
			receiveRunning = true;
			SocketSend("photo");
			connectThread = new Thread(SocketReceive)
			{
				IsBackground = true,
				Name = "PhotoUDP-Receive"
			};
			connectThread.Start();
		}
		catch (Exception ex)
		{
			Debug.LogError("拍照UDP初始化失败 " + IP + ":" + Port + " " + ex.Message);
			CloseTransport();
		}
	}

	public void SocketSend(string sendStr)
	{
		if (socket == null || ipEnd == null)
		{
			Debug.LogWarning("拍照UDP尚未初始化，忽略指令: " + sendStr);
			return;
		}
		try
		{
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
		}
		catch (Exception ex)
		{
			Debug.LogError("拍照UDP发送失败: " + ex.Message);
		}
	}

	private void SocketReceive()
	{
		while (receiveRunning)
		{
			try
			{
				recvData = new byte[1024];
				Socket currentSocket = socket;
				if (currentSocket == null)
				{
					break;
				}
				recvLen = currentSocket.ReceiveFrom(recvData, ref serverEnd);
				recvStr = Encoding.ASCII.GetString(recvData, 0, recvLen);
				message = serverEnd + ":" + recvStr;
			}
			catch (SocketException ex)
			{
				if (receiveRunning)
				{
					Debug.LogWarning("拍照UDP接收中断: " + ex.Message);
				}
				break;
			}
			catch (ObjectDisposedException)
			{
				break;
			}
		}
	}

	private void SocketQuit()
	{
		CloseTransport();
	}

	private void OnApplicationQuit()
	{
		SocketQuit();
	}

	private void GetTheMessage(string message)
	{
		if (_shotctl != null)
		{
			_shotctl.SendMessage("OnClick", message, SendMessageOptions.DontRequireReceiver);
		}
	}

	public void OnClick(string num)
	{
		Debug.Log("num:" + num);
	}

	private void Update()
	{
		if (Info != message)
		{
			GetTheMessage(message);
			Info = (message = " ");
		}
	}

	private void OnDisable()
	{
		CloseTransport();
	}

	private void CloseTransport()
	{
		receiveRunning = false;
		Socket currentSocket = socket;
		socket = null;
		if (currentSocket != null)
		{
			try
			{
				currentSocket.Close();
			}
			catch
			{
			}
		}
		if (connectThread != null && connectThread.IsAlive && Thread.CurrentThread != connectThread)
		{
			connectThread.Join(200);
		}
		connectThread = null;
		ipEnd = null;
	}
}
