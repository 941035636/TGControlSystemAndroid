using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClient2 : MonoBehaviour
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

	private static UDPClient2 activeClient;

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
		EventCenter.addlistener<string>(Eventdefine.InitSocket2, initsocket);
		EventCenter.addlistener<string, string>(Eventdefine.InitSocketIP_port2, initsocketandPort);
		EventCenter.addlistener(Eventdefine.socketQuit2, SocketQuit);
		EventCenter.addlistener(Eventdefine.Shutdown2, sendShutDown);
		EventCenter.addlistener<string>(Eventdefine.playvideo2, playvideo);
		EventCenter.addlistener(Eventdefine.replay2, replay);
		EventCenter.addlistener<string>(Eventdefine.kuaijin2, kuaijin);
		EventCenter.addlistener<string>(Eventdefine.kuaitui2, kuaitui);
		EventCenter.addlistener(Eventdefine.forwardplay2, Forward);
		EventCenter.addlistener(Eventdefine.nextplay2, NextPlay);
		EventCenter.addlistener(Eventdefine.volumeadd2, volumeadd);
		EventCenter.addlistener(Eventdefine.volumesub2, volumesub);
		EventCenter.addlistener<string>(Eventdefine.videoplay2, videoPlay);
		EventCenter.addlistener(Eventdefine.voicestart2, voicestart);
		EventCenter.addlistener(Eventdefine.voicestop2, voicestop);
		EventCenter.addlistener(Eventdefine.videopause2, videoPause);
		EventCenter.addlistener(Eventdefine.closeVideo2, closeVideo);
		EventCenter.addlistener(Eventdefine.Picplay2, PicPlay);
		EventCenter.addlistener(Eventdefine.Picpause2, PicStop);
		EventCenter.addlistener<float>(Eventdefine.controlPic2, COntrolPic);
		EventCenter.addlistener(Eventdefine.VideoSceneChange2, VideoSceneChange);
		EventCenter.addlistener(Eventdefine.PicSceneChange2, PicsceneChange);
	}

	private void OnEnable()
	{
		if (activeClient != null && activeClient != this)
		{
			activeClient.CloseTransport();
		}
		activeClient = this;
	}

	private void OnDestroy()
	{
		CloseTransport();
		EventCenter.RemoveListener(Eventdefine.voicestart2, voicestart);
		EventCenter.RemoveListener(Eventdefine.voicestop2, voicestop);
		EventCenter.RemoveListener(Eventdefine.forwardplay2, Forward);
		EventCenter.RemoveListener(Eventdefine.nextplay2, NextPlay);
		EventCenter.RemoveListener(Eventdefine.closeVideo2, closeVideo);
		EventCenter.RemoveListener<string>(Eventdefine.InitSocket2, initsocket);
		EventCenter.RemoveListener(Eventdefine.Shutdown2, sendShutDown);
		EventCenter.RemoveListener<string>(Eventdefine.playvideo2, playvideo);
		EventCenter.RemoveListener(Eventdefine.replay2, replay);
		EventCenter.RemoveListener<string>(Eventdefine.kuaijin2, kuaijin);
		EventCenter.RemoveListener<string>(Eventdefine.kuaitui2, kuaitui);
		EventCenter.RemoveListener(Eventdefine.volumeadd2, volumeadd);
		EventCenter.RemoveListener(Eventdefine.volumesub2, volumesub);
		EventCenter.RemoveListener<string>(Eventdefine.videoplay2, videoPlay);
		EventCenter.RemoveListener(Eventdefine.videopause2, videoPause);
		EventCenter.RemoveListener(Eventdefine.Picplay2, PicPlay);
		EventCenter.RemoveListener(Eventdefine.Picpause2, PicStop);
		EventCenter.RemoveListener(Eventdefine.VideoSceneChange2, VideoSceneChange);
		EventCenter.RemoveListener(Eventdefine.PicSceneChange2, PicsceneChange);
		EventCenter.RemoveListener(Eventdefine.socketQuit2, SocketQuit);
		EventCenter.RemoveListenerTwo<string, string>(Eventdefine.InitSocketIP_port2, initsocketandPort);
		EventCenter.RemoveListener<float>(Eventdefine.controlPic2, COntrolPic);
		if (activeClient == this)
		{
			activeClient = null;
		}
	}

	private void sendShutDown()
	{
		SocketSend("shutdown");
	}

	private void initsocket(string ip)
	{
		_ = string.Empty;
		if (ip != null)
		{
			_ = ip == "192.168.20.64";
		}
		Debug.LogError("初始2");
		InitSocket(ip, "7001");
	}

	private void initsocketandPort(string ip, string port)
	{
		InitSocket(ip, port);
	}

	private void playvideo(string videoName)
	{
		SocketSend("Play:" + videoName);
	}

	private void closeVideo()
	{
		SocketSend("CloseVideo");
	}

	private void replay()
	{
		SocketSend("Replay");
	}

	private void kuaijin(string value)
	{
		SocketSend("kuaijin|" + value);
	}

	private void kuaitui(string value)
	{
		SocketSend("kuaitui|" + value);
	}

	private void Forward()
	{
		SocketSend("ForwardPlay");
	}

	private void NextPlay()
	{
		SocketSend("NextPlay");
	}

	private void volumeadd()
	{
		SocketSend("volumeadd");
	}

	private void volumesub()
	{
		SocketSend("volumesub");
	}

	private void videoPause()
	{
		SocketSend("VideoPause");
	}

	private void videoPlay(string name)
	{
		SocketSend("VideoPlay|" + name);
	}

	private void voicestart()
	{
		SocketSend("VoiceStart");
	}

	private void voicestop()
	{
		SocketSend("VoiceStop");
	}

	private void PicPlay()
	{
		SocketSend("picturePlay");
	}

	private void PicStop()
	{
		SocketSend("picturePause");
	}

	private void COntrolPic(float value)
	{
		SocketSend("picturecontrol|" + value);
	}

	private void VideoSceneChange()
	{
		SocketSend("videoSceneChange");
	}

	private void PicsceneChange()
	{
		SocketSend("picSceneChange");
	}

	private void OnDisable()
	{
		CloseTransport();
	}

	private void InitSocket(string IP, string Port)
	{
		if (activeClient != this)
		{
			return;
		}
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
				Name = "UDPClient2-Receive"
			};
			connectThread.Start();
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP2初始化失败 " + IP + ":" + Port + " " + ex.Message);
			CloseTransport();
		}
	}

	public void SocketSend(string sendStr)
	{
		if (activeClient != this)
		{
			return;
		}
		if (socket == null || ipEnd == null)
		{
			Debug.LogWarning("UDP2尚未初始化，忽略指令: " + sendStr);
			return;
		}
		try
		{
			sendData = Encoding.ASCII.GetBytes(sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP2发送失败: " + ex.Message);
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
					Debug.LogWarning("UDP2接收中断: " + ex.Message);
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
		if (activeClient == this)
		{
			CloseTransport();
		}
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
