using UnityEngine;
using UnityEngine.UI;

public class FMWebSocketNetwork_debug : MonoBehaviour
{
	public Text debugText;

	private string _received = "";

	public void Action_SendStringAll(string _string)
	{
		FMSocketIOManager.instance.SendToAll(_string);
	}

	public void Action_SendStringServer(string _string)
	{
		FMSocketIOManager.instance.SendToServer(_string);
	}

	public void Action_SendStringOthers(string _string)
	{
		FMSocketIOManager.instance.SendToOthers(_string);
	}

	public void Action_SendByteAll()
	{
		FMSocketIOManager.instance.SendToAll(new byte[3]);
	}

	public void Action_SendByteServer()
	{
		FMSocketIOManager.instance.SendToServer(new byte[4]);
	}

	public void Action_SendByteOthers()
	{
		FMSocketIOManager.instance.SendToOthers(new byte[5]);
	}

	private void Start()
	{
	}

	private void Update()
	{
		debugText.text = "";
		if (FMSocketIOManager.instance != null)
		{
			Text text = debugText;
			text.text = text.text + "[connected: " + FMSocketIOManager.instance.Ready + "]";
		}
		debugText.text += _received;
	}

	public void Action_OnReceivedData(string _string)
	{
		_received = "received: " + _string;
	}

	public void Action_OnReceivedData(byte[] _byte)
	{
		_received = "received(byte): " + _byte.Length;
	}
}
