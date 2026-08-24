using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClienthongwai : MonoBehaviour
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
		EventCenter.addlistener<string, string>(Eventdefine.InitSocketIP_portHW, initsocketandPort);
		EventCenter.addlistener(Eventdefine.socketQuitHW, SocketQuit);
		EventCenter.addlistener(Eventdefine.OPenHWPinjieping, OPenPinjiePing);
	}

	private void OnDestroy()
	{
		CloseTransport();
		EventCenter.RemoveListenerTwo<string, string>(Eventdefine.InitSocketIP_portHW, initsocketandPort);
		EventCenter.RemoveListener(Eventdefine.socketQuitHW, SocketQuit);
		EventCenter.RemoveListener(Eventdefine.OPenHWPinjieping, OPenPinjiePing);
	}

	private void sendShutDown()
	{
		SocketSend("shutdown");
	}

	private void OPenPinjiePing()
	{
		SocketSend("openPinjieping");
		Debug.Log("发送红外指令");
	}

	private void initsocket(string ip)
	{
		_ = string.Empty;
		InitSocket(ip, "1001");
	}

	private void initsocketandPort(string ip, string port)
	{
		InitSocket(ip, port);
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
				Name = "InfraredUDP-Receive"
			};
			connectThread.Start();
		}
		catch (Exception ex)
		{
			Debug.LogError("红外UDP初始化失败 " + IP + ":" + Port + " " + ex.Message);
			CloseTransport();
		}
	}

	public void SocketSend(string sendStr)
	{
		if (socket == null || ipEnd == null)
		{
			Debug.LogWarning("红外UDP尚未初始化，忽略指令: " + sendStr);
			return;
		}
		try
		{
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
		}
		catch (Exception ex)
		{
			Debug.LogError("红外UDP发送失败: " + ex.Message);
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
					Debug.LogWarning("红外UDP接收中断: " + ex.Message);
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
