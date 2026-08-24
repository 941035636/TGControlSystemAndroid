using UnityEngine;
using UnityEngine.UI;

public class ConnectionDebugText : MonoBehaviour
{
	public Text DebugText;

	public NetworkDiscovery ND;

	public NetworkActionClient NC;

	private void Start()
	{
	}

	private void Update()
	{
		if (!(DebugText == null))
		{
			string text = "";
			text = text + "num thread: " + Loom.numThreads + " / " + Loom.maxThreads + "\n";
			if (ND != null)
			{
				text += "Network Discovery\n";
				text = text + "-server IP: " + ND.ServerIP + "\n";
				text = text + "-client IP: " + ND.ClientIP + "\n";
				text = text + "-server received: " + ND.ServerStr + "\n";
				text = text + "-client received: " + ND.ClientStr + "\n";
			}
			if (NC != null)
			{
				text += "Network TCP Client\n";
				text = text + "-connected: " + NC.isConnected + "\n";
				text = text + "-Ip: " + NC.IP + "\n";
			}
			DebugText.text = text;
		}
	}
}
