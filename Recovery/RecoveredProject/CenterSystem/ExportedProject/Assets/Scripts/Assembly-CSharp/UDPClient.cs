using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class UDPClient : MonoBehaviour
{
	private string editString = "hello wolrd";

	private Socket socket;

	private EndPoint serverEnd;

	private IPEndPoint ipEnd;

	private string recvStr = " ";

	private string sendStr;

	private byte[] recvData = new byte[1024];

	private byte[] sendData = new byte[1024];

	private int recvLen;

	private Thread connectThread;

	private volatile bool receiveRunning;

	private bool applicationSuspended;

	private static readonly List<UDPClient> Clients = new List<UDPClient>();

	private static UDPClient activeClient;

	private static string lastTargetIp = string.Empty;

	private static string lastTargetPort = string.Empty;

	private static bool applicationQuitting;

	private static MyUDPClient Instance;

	private GameObject _shotctl;

	private string Info = " ";

	public static MyUDPClient GetInstance()
	{
		if (Instance == null)
		{
			Instance = new MyUDPClient();
		}
		return Instance;
	}

	private void OnEnable()
	{
		applicationQuitting = false;
		applicationSuspended = false;
		_shotctl = gameObject;
		if (!Clients.Contains(this))
		{
			Clients.Add(this);
		}
		// Keep one stable owner. Dynamically-created relay prefabs also contain this
		// component; allowing the newest instance to take over made the transport
		// jump between objects and could close a healthy socket mid-command.
		if (activeClient == null)
		{
			activeClient = this;
		}
		EventCenter.addlistener<string>(Eventdefine.InitSocket, initsocket);
		EventCenter.addlistener<string, string>(Eventdefine.InitSocketIP_port, initsocketandPort);
		EventCenter.addlistener<string, string>(Eventdefine.InitRelayUdp, InitSocketRelayControl);
		EventCenter.addlistener(Eventdefine.socketQuit, SocketQuit);
		EventCenter.addlistener(Eventdefine.Shutdown, sendShutDown);
		EventCenter.addlistener<string>(Eventdefine.playvideo, playvideo);
		EventCenter.addlistener(Eventdefine.replay, replay);
		EventCenter.addlistener<string>(Eventdefine.kuaijin, kuaijin);
		EventCenter.addlistener<string>(Eventdefine.kuaitui, kuaitui);
		EventCenter.addlistener(Eventdefine.forwardplay, Forward);
		EventCenter.addlistener(Eventdefine.nextplay, NextPlay);
		EventCenter.addlistener(Eventdefine.volumeadd, volumeadd);
		EventCenter.addlistener(Eventdefine.volumesub, volumesub);
		EventCenter.addlistener<string>(Eventdefine.videoplay, videoPlay);
		EventCenter.addlistener(Eventdefine.voicestart, voicestart);
		EventCenter.addlistener(Eventdefine.voicestop, voicestop);
		EventCenter.addlistener(Eventdefine.voiceadd, voiceadd);
		EventCenter.addlistener(Eventdefine.voicesub, voicesub);
		EventCenter.addlistener(Eventdefine.videopause, videoPause);
		EventCenter.addlistener(Eventdefine.closeVideo, closeVideo);
		EventCenter.addlistener(Eventdefine.Picplay, PicPlay);
		EventCenter.addlistener(Eventdefine.Picpause, PicStop);
		EventCenter.addlistener<float>(Eventdefine.controlPic, COntrolPic);
		EventCenter.addlistener(Eventdefine.VideoSceneChange, VideoSceneChange);
		EventCenter.addlistener(Eventdefine.PicSceneChange, PicsceneChange);
		EventCenter.addlistener(Eventdefine.WebSceneChange, WebsceneChange);
		EventCenter.addlistener(Eventdefine.PPTSceneChange, PPTsceneChange);
		EventCenter.addlistener(Eventdefine.updangFlag, upDangFlag);
		EventCenter.addlistener(Eventdefine.UpTuanFlag, upTuanFlag);
		EventCenter.addlistener(Eventdefine.upDuiFlag, upDuiFlag);
		EventCenter.addlistener(Eventdefine.backFlag, backFlag);
		EventCenter.addlistener<string>(Eventdefine.OPenSoft, OpenSoft);
		EventCenter.addlistener<string>(Eventdefine.CloseSoft, CloseSoft);
		EventCenter.addlistener<string>(Eventdefine.OpenOLED, OpenOLED);
		EventCenter.addlistener<string, string, string>(Eventdefine.UDPSendCode, udp_sendcode);
	}

	private void OnDisable()
	{
		EventCenter.RemoveListener<string>(Eventdefine.OpenOLED, OpenOLED);
		EventCenter.RemoveListenerTwo<string, string>(Eventdefine.InitRelayUdp, InitSocketRelayControl);
		EventCenter.RemoveListenerThree<string, string, string>(Eventdefine.UDPSendCode, udp_sendcode);
		EventCenter.RemoveListener(Eventdefine.voiceadd, voiceadd);
		EventCenter.RemoveListener(Eventdefine.voicesub, voicesub);
		EventCenter.RemoveListener<string>(Eventdefine.OPenSoft, OpenSoft);
		EventCenter.RemoveListener<string>(Eventdefine.CloseSoft, CloseSoft);
		EventCenter.RemoveListener(Eventdefine.WebSceneChange, WebsceneChange);
		EventCenter.RemoveListener(Eventdefine.PPTSceneChange, PPTsceneChange);
		EventCenter.RemoveListener(Eventdefine.voicestart, voicestart);
		EventCenter.RemoveListener(Eventdefine.voicestop, voicestop);
		EventCenter.RemoveListener(Eventdefine.forwardplay, Forward);
		EventCenter.RemoveListener(Eventdefine.nextplay, NextPlay);
		EventCenter.RemoveListener(Eventdefine.closeVideo, closeVideo);
		EventCenter.RemoveListener<string>(Eventdefine.InitSocket, initsocket);
		EventCenter.RemoveListener(Eventdefine.Shutdown, sendShutDown);
		EventCenter.RemoveListener<string>(Eventdefine.playvideo, playvideo);
		EventCenter.RemoveListener(Eventdefine.replay, replay);
		EventCenter.RemoveListener<string>(Eventdefine.kuaijin, kuaijin);
		EventCenter.RemoveListener<string>(Eventdefine.kuaitui, kuaitui);
		EventCenter.RemoveListener(Eventdefine.volumeadd, volumeadd);
		EventCenter.RemoveListener(Eventdefine.volumesub, volumesub);
		EventCenter.RemoveListener<string>(Eventdefine.videoplay, videoPlay);
		EventCenter.RemoveListener(Eventdefine.videopause, videoPause);
		EventCenter.RemoveListener(Eventdefine.Picplay, PicPlay);
		EventCenter.RemoveListener(Eventdefine.Picpause, PicStop);
		EventCenter.RemoveListener(Eventdefine.VideoSceneChange, VideoSceneChange);
		EventCenter.RemoveListener(Eventdefine.PicSceneChange, PicsceneChange);
		EventCenter.RemoveListener(Eventdefine.socketQuit, SocketQuit);
		EventCenter.RemoveListenerTwo<string, string>(Eventdefine.InitSocketIP_port, initsocketandPort);
		EventCenter.RemoveListener<float>(Eventdefine.controlPic, COntrolPic);
		EventCenter.RemoveListener(Eventdefine.updangFlag, upDangFlag);
		EventCenter.RemoveListener(Eventdefine.UpTuanFlag, upTuanFlag);
		EventCenter.RemoveListener(Eventdefine.upDuiFlag, upDuiFlag);
		EventCenter.RemoveListener(Eventdefine.backFlag, backFlag);
		CloseTransport();
		Clients.Remove(this);
		if (activeClient == this)
		{
			activeClient = null;
			for (int i = Clients.Count - 1; i >= 0; i--)
			{
				if (Clients[i] != null && Clients[i].isActiveAndEnabled)
				{
					activeClient = Clients[i];
					break;
				}
			}
			if (!applicationQuitting && activeClient != null &&
				!string.IsNullOrWhiteSpace(lastTargetIp) && !string.IsNullOrWhiteSpace(lastTargetPort))
			{
				activeClient.InitSocket(lastTargetIp, lastTargetPort);
			}
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
		InitSocket(ip, "7002");
	}

	private void initsocketandPort(string ip, string port)
	{
		InitSocket(ip, port);
	}

	private void InitSocketRelayControl(string ip, string port)
	{
		InitSocket(ip, port);
	}

	private void playvideo(string videoName)
	{
		SocketSend("Play:" + videoName);
	}

	private void OpenOLED(string code)
	{
		SocketSend("Open" + code);
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
        Debug.Log("VideoPlay|" + name);
	}

	private void voicestart()
	{
		SocketSend("VoiceStart");
	}

	private void voicestop()
	{
		SocketSend("VoiceStop");
	}

	private void voiceadd()
	{
		SocketSend("VoiceAdd");
	}

	private void voicesub()
	{
		SocketSend("VoiceSub");
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

	private void WebsceneChange()
	{
		SocketSend("WebSceneChange");
	}

	private void PPTsceneChange()
	{
		SocketSend("PPTSceneChange");
	}

	private void upDangFlag()
	{
		SocketSend("UpDangFlag");
	}

	private void upTuanFlag()
	{
		SocketSend("UpTuanFlag");
	}

	private void upDuiFlag()
	{
		SocketSend("UpDuiFlag");
	}

	private void backFlag()
	{
		SocketSend("BackFlag");
	}

	private void OpenSoft(string name)
	{
		SocketSend(name);
	}

	private void CloseSoft(string name)
	{
		SocketSend(name);
	}

	private void ShutDown()
	{
		SocketSend("shutdown");
	}

	private void udp_sendcode(string code, string ip, string port)
	{
		SocketSendDevCode(code, ip, port);
	}

	private void InitSocket(string IP, string Port)
	{
		if (activeClient != this)
		{
			ControlDiagnostics.Record("UDP", "INITIALISE", IP + ":" + Port, "IGNORED", "inactive UDPClient instance " + name);
			return;
		}
		try
		{
			ControlDiagnostics.Record("UDP", "INITIALISE", IP + ":" + Port, "STARTED", name);
			CloseTransport();
			ipEnd = new IPEndPoint(IPAddress.Parse(IP), int.Parse(Port));
			lastTargetIp = IP;
			lastTargetPort = Port;
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			serverEnd = new IPEndPoint(IPAddress.Any, 0);
			receiveRunning = true;
			SocketSend("hello");
			connectThread = new Thread(SocketReceive)
			{
				IsBackground = true,
				Name = "UDPClient-Receive"
			};
			connectThread.Start();
			Debug.Log("UDP目标已切换: " + ipEnd);
			ControlDiagnostics.Record("UDP", "INITIALISE", ipEnd.ToString(), "OK", name);
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP初始化失败 " + IP + ":" + Port + " " + ex.Message);
			ControlDiagnostics.Record("UDP", "INITIALISE", IP + ":" + Port, "FAILED", name + " " + ex.Message);
			CloseTransport();
		}
	}

	public void SocketSend(string sendStr)
	{
		string commandId = ControlDiagnostics.NewCommandId("UDP");
		if (activeClient != this)
		{
			ControlDiagnostics.RecordCommand(commandId, "UDP", "COMMAND_SEND", ipEnd == null ? string.Empty : ipEnd.ToString(), "IGNORED", "inactive UDPClient instance " + name + " " + sendStr);
			return;
		}
		if (socket == null || ipEnd == null)
		{
			Debug.LogWarning("UDP尚未初始化，忽略指令: " + sendStr);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "COMMAND_SEND", string.Empty, "IGNORED", "transport not initialised " + sendStr);
			return;
		}
		try
		{
			sendData = Encoding.ASCII.GetBytes(sendStr);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "COMMAND_SEND", ipEnd.ToString(), "ATTEMPT", "bytes=" + sendData.Length + " payload=" + sendStr);
			socket.SendTo(sendData, sendData.Length, SocketFlags.None, ipEnd);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "COMMAND_SEND", ipEnd.ToString(), "SOCKET_OK", "bytes accepted by socket payload=" + sendStr);
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP发送失败 " + ipEnd + " 指令=" + sendStr + " " + ex.Message);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "COMMAND_SEND", ipEnd == null ? string.Empty : ipEnd.ToString(), "FAILED", ex.Message + " payload=" + sendStr);
		}
	}

	private void SocketSendDevCode(string code, string ip, string port)
	{
		string commandId = ControlDiagnostics.NewCommandId("UDP");
		if (activeClient != this)
		{
			ControlDiagnostics.RecordCommand(commandId, "UDP", "DEVICE_COMMAND_SEND", ip + ":" + port, "IGNORED", "inactive UDPClient instance " + name);
			return;
		}
		try
		{
			IPEndPoint iPEndPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
			byte[] array = strToToHexByte(code);
			if (socket == null)
			{
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			}
			Debug.Log("UDP设备指令:" + code + " 目的IP:" + iPEndPoint);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "DEVICE_COMMAND_SEND", iPEndPoint.ToString(), "ATTEMPT", "bytes=" + array.Length + " raw=" + code);
			socket.SendTo(array, array.Length, SocketFlags.None, iPEndPoint);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "DEVICE_COMMAND_SEND", iPEndPoint.ToString(), "SOCKET_OK", "bytes accepted by socket raw=" + code);
		}
		catch (Exception ex)
		{
			Debug.LogError("UDP设备指令发送失败: " + ex.Message);
			ControlDiagnostics.RecordCommand(commandId, "UDP", "DEVICE_COMMAND_SEND", ip + ":" + port, "FAILED", ex.Message + " raw=" + code);
		}
	}

	private static byte[] strToToHexByte(string hexString)
	{
		hexString = hexString.Replace(" ", "");
		if (hexString.Length % 2 != 0)
		{
			hexString += " ";
		}
		byte[] array = new byte[hexString.Length / 2];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
		}
		return array;
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
				ControlDiagnostics.Record("UDP", "RECEIVE", serverEnd == null ? string.Empty : serverEnd.ToString(), "UNMATCHED", "bytes=" + recvLen + " payload=" + recvStr);
			}
			catch (SocketException ex)
			{
				if (receiveRunning)
				{
					Debug.LogWarning("UDP接收中断: " + ex.Message);
					ControlDiagnostics.Record("UDP", "RECEIVE", serverEnd == null ? string.Empty : serverEnd.ToString(), "FAILED", ex.Message);
				}
				break;
			}
			catch (ObjectDisposedException)
			{
				break;
			}
		}
	}

	private void Start()
	{
		_shotctl = base.gameObject;
	}

	private void GetTheMessage(string message)
	{
		if (_shotctl != null)
		{
			_shotctl.SendMessage("OnClick", message, SendMessageOptions.DontRequireReceiver);
		}
	}

	private void Update()
	{
		if (Info != recvStr)
		{
			GetTheMessage(recvStr);
			Info = (recvStr = " ");
		}
	}

	private void SocketQuit()
	{
		if (activeClient == this)
		{
			lastTargetIp = string.Empty;
			lastTargetPort = string.Empty;
			CloseTransport();
		}
	}

	private void OnApplicationQuit()
	{
		applicationQuitting = true;
		CloseTransport();
	}

	private void OnApplicationPause(bool paused)
	{
		if (activeClient != this || applicationSuspended == paused)
		{
			return;
		}

		applicationSuspended = paused;
		if (paused)
		{
			ControlDiagnostics.Record("UDP", "APPLICATION_SUSPEND", lastTargetIp + ":" + lastTargetPort, "CLOSED", name);
			CloseTransport();
			return;
		}

		RestoreTransportAfterResume("APPLICATION_RESUME");
	}

	private void OnApplicationFocus(bool hasFocus)
	{
		if (hasFocus && activeClient == this && !applicationSuspended &&
			!string.IsNullOrWhiteSpace(lastTargetIp) && !string.IsNullOrWhiteSpace(lastTargetPort) &&
			(socket == null || !receiveRunning))
		{
			RestoreTransportAfterResume("APPLICATION_FOCUS_RECOVERY");
		}
	}

	private void RestoreTransportAfterResume(string action)
	{
		if (applicationQuitting || string.IsNullOrWhiteSpace(lastTargetIp) || string.IsNullOrWhiteSpace(lastTargetPort))
		{
			ControlDiagnostics.Record("UDP", action, lastTargetIp + ":" + lastTargetPort, "SKIPPED", name + " no previous target");
			return;
		}

		ControlDiagnostics.Record("UDP", action, lastTargetIp + ":" + lastTargetPort, "REINITIALISING", name);
		InitSocket(lastTargetIp, lastTargetPort);
	}

	private void CloseTransport()
	{
		string closingTarget = ipEnd == null ? string.Empty : ipEnd.ToString();
		bool hadTransport = socket != null || ipEnd != null || connectThread != null;
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
		if (hadTransport)
		{
			ControlDiagnostics.Record("UDP", "CLOSE", closingTarget, "OK", name);
		}
	}
}
