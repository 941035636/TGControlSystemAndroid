using System;
using System.Collections.Generic;
using FMSocketIO;
using UnityEngine;

public class SocketIOComponentWebGL : MonoBehaviour
{
	public static SocketIOComponentWebGL instance;

	public string sid;

	public string IP = "127.0.0.1";

	public int port = 3000;

	public bool sslEnabled;

	public bool portRequired = true;

	public bool socketIORequired = true;

	private int packetId;

	private Dictionary<string, List<Action<SocketIOEvent>>> eventHandlers;

	private List<Ack> ackList;

	private bool _WebSocketConnected;

	private bool Ready;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
	}

	public bool IsWebSocketConnected()
	{
		return _WebSocketConnected;
	}

	private void Update()
	{
	}

	public void Init()
	{
		if (socketIORequired)
		{
			eventHandlers = new Dictionary<string, List<Action<SocketIOEvent>>>();
			ackList = new List<Ack>();
			AddSocketIO();
			AddEventListeners();
		}
	}

	private void OnConnected(SocketIOEvent e)
	{
		Debug.Log("[Event] SocketIO connected");
	}

	private void AddSocketIO()
	{
	}

	private void AddEventListeners()
	{
	}

	public void Connect()
	{
		Debug.Log(">>> start connecting");
	}

	public void Close()
	{
		Ready = false;
	}

	public void Emit(string e)
	{
	}

	public void Emit(string e, string data)
	{
	}

	public void Emit(string e, Action<string> action)
	{
		_ = socketIORequired;
	}

	public void Emit(string e, string data, Action<string> action)
	{
		_ = socketIORequired;
	}

	public void On(string e, Action<SocketIOEvent> callback)
	{
		_ = socketIORequired;
	}

	public void Off(string e, Action<SocketIOEvent> callback)
	{
		if (!eventHandlers.ContainsKey(e))
		{
			return;
		}
		List<Action<SocketIOEvent>> list = eventHandlers[e];
		if (list.Contains(callback))
		{
			list.Remove(callback);
			if (list.Count == 0)
			{
				eventHandlers.Remove(e);
			}
		}
	}

	public void InvokeAck(string ackJson)
	{
		Ack ack = JsonUtility.FromJson<Ack>(ackJson);
		for (int i = 0; i < ackList.Count; i++)
		{
			if (ackList[i].packetId == ack.packetId)
			{
				Ack ack2 = ackList[i];
				ackList.RemoveAt(i);
				ack2.Invoke(ackJson);
				break;
			}
		}
	}

	public void OnOpen()
	{
		Ready = true;
	}

	public void SetSocketID(string socketID)
	{
		sid = socketID;
		FMSocketIOManager.instance.Settings.socketID = sid;
	}

	public void InvokeEventCallback(string eventJson)
	{
		EventJson eventJson2 = JsonUtility.FromJson<EventJson>(eventJson);
		if (eventHandlers.ContainsKey(eventJson2.socketEvent))
		{
			for (int i = 0; i < eventHandlers[eventJson2.socketEvent].Count; i++)
			{
				SocketIOEvent obj = new SocketIOEvent(eventJson2.socketEvent, eventJson2.eventData);
				eventHandlers[eventJson2.socketEvent][i](obj);
			}
		}
	}

	public void RegOnOpen()
	{
		Ready = true;
	}

	public void RegOnClose()
	{
		Ready = false;
		_WebSocketConnected = false;
	}

	public void RegOnMessage(string _msg)
	{
		if (FMSocketIOManager.instance != null)
		{
			FMSocketIOManager.instance.OnReceivedRawMessageEvent.Invoke(_msg);
		}
	}

	public void RegOnError(string _msg)
	{
		Debug.Log(">>> UNITY: (Error) " + _msg);
	}

	public void RegWebSocketConnected()
	{
		_WebSocketConnected = true;
	}

	public void RegWebSocketDisconnected()
	{
		_WebSocketConnected = false;
	}
}
