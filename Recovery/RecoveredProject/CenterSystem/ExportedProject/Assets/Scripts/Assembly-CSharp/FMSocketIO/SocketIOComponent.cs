using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using WebSocketSharp;

namespace FMSocketIO
{
	public class SocketIOComponent : MonoBehaviour
	{
		public static SocketIOComponent instance;

		public string IP = "127.0.0.13";

		public int port = 3000;

		public bool sslEnabled;

		public bool portRequired = true;

		public bool socketIORequired = true;

		[HideInInspector]
		public string url = "ws://127.0.0.1:4567/socket.io/?EIO=4&transport=websocket";

		public bool autoConnect = true;

		public int reconnectDelay = 5;

		public float ackExpirationTime = 1800f;

		public float pingInterval = 25f;

		public float pingTimeout = 60f;

		private volatile bool connected;

		private volatile bool thPinging;

		private volatile bool thPong;

		private volatile bool wsConnected;

		private Thread socketThread;

		private Thread pingThread;

		private readonly ManualResetEvent shutdownEvent = new ManualResetEvent(false);

		private WebSocket ws;

		private Encoder encoder;

		private Decoder decoder;

		private Parser parser;

		private Dictionary<string, List<Action<SocketIOEvent>>> handlers;

		private List<Ack> ackList;

		private int packetId;

		private object eventQueueLock;

		private Queue<SocketIOEvent> eventQueue;

		private object ackQueueLock;

		private Queue<Packet> ackQueue;

		public UnityEventString debugMethod;

		private object RawMessageQueueLock;

		private Queue<string> RawMessageQueue;

		public bool Initialised;

		public WebSocket socket => ws;

		public string sid { get; set; }

		public bool IsConnected => connected;

		public bool IsWebSocketConnected()
		{
			return IsWebSocketConnected(ws);
		}

		private bool IsWebSocketConnected(WebSocket _ws)
		{
			if (_ws == null)
			{
				return false;
			}
			if (_ws.ReadyState != WebSocketState.Open)
			{
				return _ws.ReadyState == WebSocketState.Closing;
			}
			return true;
		}

		public void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
		}

		public void Init()
		{
			encoder = new Encoder();
			decoder = new Decoder();
			parser = new Parser();
			packetId = 0;
			handlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
			eventQueueLock = new object();
			eventQueue = new Queue<SocketIOEvent>();
			ackQueueLock = new object();
			ackQueue = new Queue<Packet>();
			ackList = new List<Ack>();
			RawMessageQueueLock = new object();
			RawMessageQueue = new Queue<string>();
			url = "ws" + (sslEnabled ? "s" : "") + "://" + IP;
			if (portRequired)
			{
				url = url + ":" + port;
			}
			if (socketIORequired)
			{
				url += "/socket.io/?EIO=4&transport=websocket";
			}
			ws = new WebSocket(url);
			ws.OnOpen += OnOpen;
			ws.OnMessage += OnMessage;
			ws.OnError += OnError;
			ws.OnClose += OnClose;
			sid = null;
			wsConnected = false;
			connected = false;
			Initialised = true;
		}

		public void Update()
		{
			if (!Initialised)
			{
				return;
			}
			lock (RawMessageQueueLock)
			{
				while (RawMessageQueue.Count > 0)
				{
					if (FMSocketIOManager.instance != null)
					{
						FMSocketIOManager.instance.OnReceivedRawMessageEvent.Invoke(RawMessageQueue.Dequeue());
					}
				}
			}
			lock (eventQueueLock)
			{
				while (eventQueue.Count > 0)
				{
					EmitEvent(eventQueue.Dequeue());
				}
			}
			lock (ackQueueLock)
			{
				while (ackQueue.Count > 0)
				{
					InvokeAck(ackQueue.Dequeue());
				}
			}
			if (wsConnected != IsWebSocketConnected(ws))
			{
				wsConnected = IsWebSocketConnected(ws);
				EmitEvent(wsConnected ? "connect" : "disconnect");
			}
			if (ackList.Count != 0 && !(DateTime.Now.Subtract(ackList[0].time).TotalSeconds < (double)ackExpirationTime))
			{
				ackList.RemoveAt(0);
			}
		}

		public void OnDestroy()
		{
			Close();
			JoinWorker(socketThread);
			JoinWorker(pingThread);
			socketThread = null;
			pingThread = null;
			if (instance == this)
			{
				instance = null;
			}
		}

		public void OnApplicationQuit()
		{
			Close();
		}

		public void Connect()
		{
			if (connected)
			{
				return;
			}
			if (!Initialised)
			{
				Init();
			}
			shutdownEvent.Reset();
			connected = true;
			socketThread = new Thread(RunSocketThread) { IsBackground = true, Name = "SocketIO-Connect" };
			socketThread.Start(ws);
			pingThread = new Thread(RunPingThread) { IsBackground = true, Name = "SocketIO-Ping" };
			pingThread.Start(ws);
		}

		public void Close()
		{
			if (connected)
			{
				EmitClose();
			}
			connected = false;
			shutdownEvent.Set();
			try
			{
				ws?.Close();
			}
			catch
			{
			}
		}

		public void On(string ev, Action<SocketIOEvent> callback)
		{
			if (!handlers.ContainsKey(ev))
			{
				handlers[ev] = new List<Action<SocketIOEvent>>();
			}
			handlers[ev].Add(callback);
		}

		public void Off(string ev, Action<SocketIOEvent> callback)
		{
			if (!handlers.ContainsKey(ev))
			{
				return;
			}
			List<Action<SocketIOEvent>> list = handlers[ev];
			if (list.Contains(callback))
			{
				list.Remove(callback);
				if (list.Count == 0)
				{
					handlers.Remove(ev);
				}
			}
		}

		public void EmitRaw(string ev)
		{
			EmitMessage(-1, ev);
		}

		public void Emit(string ev)
		{
			EmitMessage(-1, $"[\"{ev}\"]");
		}

		public void Emit(string ev, Action<string> action)
		{
			EmitMessage(++packetId, $"[\"{ev}\"]");
			ackList.Add(new Ack(packetId, action));
		}

		public void Emit(string ev, string data)
		{
			EmitMessage(-1, $"[\"{ev}\",{data}]");
		}

		public void Emit(string ev, string data, Action<string> action)
		{
			EmitMessage(++packetId, $"[\"{ev}\",{data}]");
			ackList.Add(new Ack(packetId, action));
		}

		private void RunSocketThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj;
			while (connected)
			{
				if (IsWebSocketConnected(webSocket))
				{
					if (shutdownEvent.WaitOne(Mathf.Max(100, reconnectDelay * 1000)))
					{
						break;
					}
				}
				else
				{
					try
					{
						webSocket.Connect();
					}
					catch
					{
						if (shutdownEvent.WaitOne(Mathf.Max(100, reconnectDelay * 1000)))
						{
							break;
						}
					}
				}
			}
			webSocket.Close();
		}

		private void RunPingThread(object obj)
		{
			WebSocket webSocket = (WebSocket)obj;
			float timeoutSeconds = Mathf.Max(0.1f, pingTimeout);
			int millisecondsTimeout = Mathf.FloorToInt(pingInterval * 1000f);
			while (connected)
			{
				if (!wsConnected)
				{
					if (shutdownEvent.WaitOne(Mathf.Max(100, reconnectDelay * 1000)))
					{
						break;
					}
					continue;
				}
				thPinging = true;
				thPong = false;
				EmitPacket(new Packet(EnginePacketType.PING));
				DateTime now = DateTime.Now;
				while (connected && IsWebSocketConnected(webSocket) && thPinging && DateTime.Now.Subtract(now).TotalSeconds < timeoutSeconds)
				{
					if (shutdownEvent.WaitOne(200))
					{
						break;
					}
				}
				if (!thPong)
				{
					webSocket.Close();
				}
				if (shutdownEvent.WaitOne(Mathf.Max(100, millisecondsTimeout)))
				{
					break;
				}
			}
		}

		private void EmitMessage(int id, string raw)
		{
			EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.EVENT, 0, "/", id, raw));
		}

		private void EmitClose()
		{
			EmitPacket(new Packet(EnginePacketType.MESSAGE, SocketPacketType.DISCONNECT, 0, "/", -1, JsonUtility.ToJson("")));
			EmitPacket(new Packet(EnginePacketType.CLOSE));
		}

		private void EmitPacket(Packet packet)
		{
			try
			{
				ws.Send(encoder.Encode(packet));
			}
			catch
			{
			}
		}

		private void OnOpen(object sender, EventArgs e)
		{
			EmitEvent("open");
		}

		private void OnMessage(object sender, MessageEventArgs e)
		{
			Packet packet = decoder.Decode(e);
			switch (packet.enginePacketType)
			{
			case EnginePacketType.OPEN:
				HandleOpen(packet);
				break;
			case EnginePacketType.CLOSE:
				EmitEvent("close");
				break;
			case EnginePacketType.PING:
				HandlePing();
				break;
			case EnginePacketType.PONG:
				HandlePong();
				break;
			case EnginePacketType.MESSAGE:
				HandleMessage(packet);
				break;
			}
		}

		private void HandleOpen(Packet packet)
		{
			sid = JsonUtility.FromJson<SocketOpenData>(packet.json).sid;
			EmitEvent("open");
			FMSocketIOManager.instance.Settings.socketID = sid;
		}

		private void HandlePing()
		{
			EmitPacket(new Packet(EnginePacketType.PONG));
		}

		private void HandlePong()
		{
			thPong = true;
			thPinging = false;
		}

		private void HandleMessage(Packet packet)
		{
			if (packet.json == null)
			{
				return;
			}
			if (packet.socketPacketType == SocketPacketType.ACK)
			{
				for (int i = 0; i < ackList.Count; i++)
				{
					if (ackList[i].packetId == packet.id)
					{
						lock (ackQueueLock)
						{
							ackQueue.Enqueue(packet);
							return;
						}
					}
				}
				debugMethod.Invoke("[SocketIO] Ack received for invalid Action: " + packet.id);
			}
			if (packet.socketPacketType != SocketPacketType.EVENT)
			{
				return;
			}
			if (FMSocketIOManager.instance != null)
			{
				lock (RawMessageQueueLock)
				{
					RawMessageQueue.Enqueue(packet.json);
				}
			}
			SocketIOEvent item = parser.Parse(packet.json);
			lock (eventQueueLock)
			{
				eventQueue.Enqueue(item);
			}
		}

		private void OnError(object sender, ErrorEventArgs e)
		{
			EmitEvent("error");
		}

		private void OnClose(object sender, CloseEventArgs e)
		{
			EmitEvent("close");
		}

		private void EmitEvent(string type)
		{
			EmitEvent(new SocketIOEvent(type));
		}

		private void EmitEvent(SocketIOEvent ev)
		{
			if (!handlers.ContainsKey(ev.name))
			{
				return;
			}
			foreach (Action<SocketIOEvent> item in handlers[ev.name])
			{
				try
				{
					item(ev);
				}
				catch (Exception ex)
				{
					debugMethod.Invoke(ex.ToString());
				}
			}
		}

		private void InvokeAck(Packet packet)
		{
			for (int i = 0; i < ackList.Count; i++)
			{
				if (ackList[i].packetId == packet.id)
				{
					Ack ack = ackList[i];
					ackList.RemoveAt(i);
					ack.Invoke(packet.json);
					break;
				}
			}
		}

		private static void JoinWorker(Thread worker)
		{
			if (worker != null && worker.IsAlive && Thread.CurrentThread != worker)
			{
				worker.Join(1000);
			}
		}
	}
}
