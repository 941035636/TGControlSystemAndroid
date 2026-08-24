using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClientServer : MonoBehaviour
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

	public static MyUDPClient GetInstance()
	{
		if (Instance == null)
		{
			Instance = new MyUDPClient();
		}
		return Instance;
	}

	private void Awake()
	{
		EventCenter.addlistener(Eventdefine.EquipAllOpen, AllOpen);
		EventCenter.addlistener(Eventdefine.EquipAllClose, AllClose);
		EventCenter.addlistener(Eventdefine._1OPen, _1Open);
		EventCenter.addlistener(Eventdefine._1Close, _1Close);
		EventCenter.addlistener(Eventdefine._2OPen, _2Open);
		EventCenter.addlistener(Eventdefine._2Close, _2Close);
		EventCenter.addlistener(Eventdefine._3OPen, _3Open);
		EventCenter.addlistener(Eventdefine._3Close, _3Close);
		EventCenter.addlistener(Eventdefine._4OPen, _4Open);
		EventCenter.addlistener(Eventdefine._4Close, _4Close);
		EventCenter.addlistener(Eventdefine._5OPen, _5Open);
		EventCenter.addlistener(Eventdefine._5Close, _5Close);
		EventCenter.addlistener(Eventdefine._6OPen, _6Open);
		EventCenter.addlistener(Eventdefine._6Close, _6Close);
		EventCenter.addlistener(Eventdefine._7OPen, _7Open);
		EventCenter.addlistener(Eventdefine._7Close, _7Close);
		EventCenter.addlistener(Eventdefine._8OPen, _8Open);
		EventCenter.addlistener(Eventdefine._8Close, _8Close);
	}

	private void Start()
	{
		initsocket("192.168.20.201");
	}

	private void OnDestroy()
	{
		CloseTransport();
		EventCenter.RemoveListener(Eventdefine.EquipAllOpen, AllOpen);
		EventCenter.RemoveListener(Eventdefine.EquipAllClose, AllClose);
		EventCenter.RemoveListener(Eventdefine._1OPen, _1Open);
		EventCenter.RemoveListener(Eventdefine._1Close, _1Close);
		EventCenter.RemoveListener(Eventdefine._2OPen, _2Open);
		EventCenter.RemoveListener(Eventdefine._2Close, _2Close);
		EventCenter.RemoveListener(Eventdefine._3OPen, _3Open);
		EventCenter.RemoveListener(Eventdefine._3Close, _3Close);
		EventCenter.RemoveListener(Eventdefine._4OPen, _4Open);
		EventCenter.RemoveListener(Eventdefine._4Close, _4Close);
		EventCenter.RemoveListener(Eventdefine._5OPen, _5Open);
		EventCenter.RemoveListener(Eventdefine._5Close, _5Close);
		EventCenter.RemoveListener(Eventdefine._6OPen, _6Open);
		EventCenter.RemoveListener(Eventdefine._6Close, _6Close);
		EventCenter.RemoveListener(Eventdefine._7OPen, _7Open);
		EventCenter.RemoveListener(Eventdefine._7Close, _7Close);
		EventCenter.RemoveListener(Eventdefine._8OPen, _8Open);
		EventCenter.RemoveListener(Eventdefine._8Close, _8Close);
	}

	private void sendShutDown()
	{
		SocketSend("shutdown");
	}

	private void initsocket(string ip)
	{
		InitSocket(ip, "7000");
	}

	private void AllOpen()
	{
		SocketSend("AllOPen");
	}

	private void AllClose()
	{
		SocketSend("AllClose");
	}

	private void _1Open()
	{
		SocketSend("Open_1");
	}

	private void _1Close()
	{
		SocketSend("Close_1");
	}

	private void _2Open()
	{
		SocketSend("Open_2");
	}

	private void _2Close()
	{
		SocketSend("Close_2");
	}

	private void _3Open()
	{
		SocketSend("Open_3");
	}

	private void _3Close()
	{
		SocketSend("Close_3");
	}

	private void _4Open()
	{
		SocketSend("Open_4");
	}

	private void _4Close()
	{
		SocketSend("Close_4");
	}

	private void _5Open()
	{
		SocketSend("Open_5");
	}

	private void _5Close()
	{
		SocketSend("Close_5");
	}

	private void _6Open()
	{
		SocketSend("Open_6");
	}

	private void _6Close()
	{
		SocketSend("Close_6");
	}

	private void _7Open()
	{
		SocketSend("Open_7");
	}

	private void _7Close()
	{
		SocketSend("Close_7");
	}

	private void _8Open()
	{
		SocketSend("Open_8");
	}

	private void _8Close()
	{
		SocketSend("Close_8");
	}

	private void OnDisable()
	{
		SocketQuit();
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
			SocketSend("hello");
			connectThread = new Thread(SocketReceive)
			{
				IsBackground = true,
				Name = "EquipmentUDP-Receive"
			};
			connectThread.Start();
		}
		catch (Exception ex)
		{
			Debug.LogError("设备UDP初始化失败 " + IP + ":" + Port + " " + ex.Message);
			CloseTransport();
		}
	}

	public void SocketSend(string sendStr)
	{
		if (socket == null || ipEnd == null)
		{
			Debug.LogWarning("设备UDP尚未初始化，忽略指令: " + sendStr);
			return;
		}
		try
		{
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
		}
		catch (Exception ex)
		{
			Debug.LogError("设备UDP发送失败: " + ex.Message);
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
			}
			catch (SocketException ex)
			{
				if (receiveRunning)
				{
					Debug.LogWarning("设备UDP接收中断: " + ex.Message);
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
