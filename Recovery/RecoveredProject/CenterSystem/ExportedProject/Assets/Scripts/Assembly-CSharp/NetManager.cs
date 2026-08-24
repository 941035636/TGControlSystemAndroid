using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using UnityEngine;

public class NetManager : MonoBehaviour
{
	public delegate void EventListener(string str);

	public delegate void MsgListener(MsgBase msgBase);

	public enum NetEvent
	{
		ConnectSucc = 1,
		ConnectFail = 2,
		Close = 3
	}

	private static Socket socket;

	private static ByteArray readBuff;

	private static Queue<ByteArray> writeQueue;

	private static bool isConnecting = false;

	private static bool isClosing = false;

	private static Dictionary<NetEvent, EventListener> eventListeners = new Dictionary<NetEvent, EventListener>();

	private static Dictionary<string, MsgListener> msgListeners = new Dictionary<string, MsgListener>();

	private static List<MsgBase> msgList = new List<MsgBase>();

	private static int msgCount = 0;

	private static readonly int MAX_MESSSGE_FIRE = 10;

	public static bool isUsePing = true;

	public static int pingInterval = 30;

	private static float lastPingTime = 0f;

	private static float lastPongTime = 0f;

	private static NetManager Instance;

	public static NetManager GetInstance()
	{
		if (Instance != null)
		{
			Instance = new NetManager();
		}
		return Instance;
	}

	public static void AddEventListener(NetEvent netEvent, EventListener listener)
	{
		if (eventListeners.ContainsKey(netEvent))
		{
			Dictionary<NetEvent, EventListener> dictionary = eventListeners;
			dictionary[netEvent] = (EventListener)Delegate.Combine(dictionary[netEvent], listener);
		}
		else
		{
			eventListeners[netEvent] = listener;
		}
	}

	public static void RemoveEventListener(NetEvent netEvent, EventListener listener)
	{
		if (eventListeners.ContainsKey(netEvent))
		{
			Dictionary<NetEvent, EventListener> dictionary = eventListeners;
			dictionary[netEvent] = (EventListener)Delegate.Remove(dictionary[netEvent], listener);
			if (eventListeners[netEvent] == null)
			{
				eventListeners.Remove(netEvent);
			}
		}
	}

	private static void FireEvent(NetEvent netEvent, string err)
	{
		if (eventListeners.ContainsKey(netEvent))
		{
			eventListeners[netEvent](err);
		}
	}

	public static void Connect(string ip, int port)
	{
		if (socket == null || !socket.Connected)
		{
			InitState();
			socket.NoDelay = true;
			isConnecting = true;
			socket.BeginConnect(ip, port, ConnectCallback, socket);
		}
	}

	private static void InitState()
	{
		socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		readBuff = new ByteArray();
		writeQueue = new Queue<ByteArray>();
		isConnecting = false;
		isClosing = false;
		msgList = new List<MsgBase>();
		msgCount = 0;
		lastPingTime = Time.time;
		lastPongTime = Time.time;
		if (!msgListeners.ContainsKey("MsgPong"))
		{
			AddMsgListener("MsgPong", OnMsgPong);
		}
		if (!msgListeners.ContainsKey("MsgControl"))
		{
			AddMsgListener("MsgControl", OnMsgControl);
		}
		if (!msgListeners.ContainsKey("DaTiController"))
		{
			AddMsgListener("DaTiController", OnMsgDaTiControl);
		}
	}

	private static void ConnectCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			socket.EndConnect(ar);
			Debug.LogError("连接成功!");
			testTCpClient.isConnect = true;
			isConnecting = false;
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, SocketFlags.None, ReceiveCallback, socket);
		}
		catch (SocketException ex)
		{
			Debug.Log("Socket Connect fail" + ex.ToString());
			FireEvent(NetEvent.ConnectFail, ex.ToString());
			isConnecting = false;
			throw;
		}
	}

	public static void ReceiveCallback(IAsyncResult ar)
	{
		try
		{
			Socket socket = (Socket)ar.AsyncState;
			int num = socket.EndReceive(ar);
			readBuff.writeIdx += num;
			OnReceiveData();
			if (readBuff.remain < 8)
			{
				readBuff.MoveBytes();
				readBuff.ReSize(readBuff.length * 2);
			}
			socket.BeginReceive(readBuff.bytes, readBuff.writeIdx, readBuff.remain, SocketFlags.None, ReceiveCallback, socket);
		}
		catch (SocketException ex)
		{
			Debug.LogError(ex.ToString());
		}
	}

	public static void OnReceiveData()
	{
		if (readBuff.length <= 2)
		{
			return;
		}
		int readIdx = readBuff.readIdx;
		byte[] bytes = readBuff.bytes;
		short num = (short)((bytes[readIdx + 1] << 8) | bytes[readIdx]);
		if (readBuff.length < num)
		{
			return;
		}
		readBuff.readIdx += 2;
		int count = 0;
		string text = MsgBase.DecodeName(readBuff.bytes, readBuff.readIdx, out count);
		if (text == "")
		{
			Debug.Log("OnReceiveData MsgBase.DecodeName fail");
			return;
		}
		readBuff.readIdx += count;
		int num2 = num - count;
		MsgBase item = MsgBase.Decode(text, readBuff.bytes, readBuff.readIdx, num2);
		readBuff.readIdx += num2;
		readBuff.CheckAndMoveBytes();
		lock (msgList)
		{
			msgList.Add(item);
		}
		msgCount++;
		if (readBuff.length > 2)
		{
			OnReceiveData();
		}
	}

	public static void Update()
	{
		MsgUpdate();
	}

	public static void MsgUpdate()
	{
		if (msgCount == 0)
		{
			return;
		}
		for (int i = 0; i < MAX_MESSSGE_FIRE; i++)
		{
			MsgBase msgBase = null;
			lock (msgList)
			{
				if (msgList.Count > 0)
				{
					msgBase = msgList[0];
					msgList.RemoveAt(0);
					msgCount--;
				}
			}
			if (msgBase != null)
			{
				Debug.LogError("收到的协议名:" + msgBase.protoName);
				FireMsg(msgBase.protoName, msgBase);
				continue;
			}
			break;
		}
	}

	public static void Close()
	{
		if (socket != null && socket.Connected && !isConnecting)
		{
			if (writeQueue.Count > 0)
			{
				isClosing = true;
				return;
			}
			socket.Close();
			FireEvent(NetEvent.Close, "");
		}
	}

	public static void Send(MsgBase msg)
	{
		if (socket != null && socket.Connected && !isConnecting && !isClosing)
		{
			byte[] array = MsgBase.EncodeName(msg);
			byte[] array2 = MsgBase.Encode(msg);
			int num = array.Length + array2.Length;
			byte[] array3 = new byte[2 + num];
			array3[0] = (byte)(num % 256);
			array3[1] = (byte)(num / 256);
			Array.Copy(array, 0, array3, 2, array.Length);
			Array.Copy(array2, 0, array3, 2 + array.Length, array2.Length);
			ByteArray item = new ByteArray(array3);
			int num2 = 0;
			lock (writeQueue)
			{
				writeQueue.Enqueue(item);
				num2 = writeQueue.Count;
			}
			if (num2 == 1)
			{
				socket.BeginSend(array3, 0, array3.Length, SocketFlags.None, SendCallback, socket);
			}
		}
	}

	public static void SendCallback(IAsyncResult ar)
	{
		Socket socket = (Socket)ar.AsyncState;
		if (socket == null || !socket.Connected)
		{
			return;
		}
		int num = socket.EndSend(ar);
		ByteArray byteArray;
		lock (writeQueue)
		{
			byteArray = writeQueue.First();
		}
		byteArray.readIdx += num;
		if (byteArray.length == 0)
		{
			lock (writeQueue)
			{
				writeQueue.Dequeue();
				byteArray = writeQueue.First();
			}
		}
		if (byteArray != null)
		{
			socket.BeginSend(byteArray.bytes, byteArray.readIdx, byteArray.length, SocketFlags.None, SendCallback, socket);
		}
		else if (isClosing)
		{
			socket.Close();
		}
	}

	public static void AddMsgListener(string msgName, MsgListener listener)
	{
		if (msgListeners.ContainsKey(msgName))
		{
			Dictionary<string, MsgListener> dictionary = msgListeners;
			dictionary[msgName] = (MsgListener)Delegate.Combine(dictionary[msgName], listener);
		}
		else
		{
			msgListeners[msgName] = listener;
		}
	}

	public static void RemoveMsgListener(string msgName, MsgListener listener)
	{
		if (msgListeners.ContainsKey(msgName))
		{
			Dictionary<string, MsgListener> dictionary = msgListeners;
			dictionary[msgName] = (MsgListener)Delegate.Remove(dictionary[msgName], listener);
			if (msgListeners[msgName] == null)
			{
				msgListeners.Remove(msgName);
			}
		}
	}

	private static void FireMsg(string msgName, MsgBase msgBase)
	{
		if (msgListeners.ContainsKey(msgName))
		{
			msgListeners[msgName](msgBase);
		}
	}

	private static void PingUpdate()
	{
		if (isUsePing)
		{
			if (Time.time - lastPingTime > (float)pingInterval)
			{
				Send(new MsgPing());
				Debug.Log("客户端发送Ping");
				lastPingTime = Time.time;
			}
			if (Time.time - lastPongTime > (float)(pingInterval * 4))
			{
				Debug.Log("这个地方关闭连接");
				Close();
			}
		}
	}

	private static void OnMsgPong(MsgBase msgBase)
	{
		lastPongTime = Time.time;
		Debug.Log("更新客户端Pong时间");
	}

	private static void OnMsgControl(MsgBase msgBase)
	{
		Debug.Log("消息名:" + msgBase.protoName + " 消息体:" + ((MsgControl)msgBase).proName);
		ReceiverMessage.GetInstance().GetOrder(((MsgControl)msgBase).proName);
	}

	private static void OnMsgDaTiControl(MsgBase msgBase)
	{
		ReceiverMessage.GetInstance().GetOrder(((DaTiController)msgBase).Content);
	}
}
