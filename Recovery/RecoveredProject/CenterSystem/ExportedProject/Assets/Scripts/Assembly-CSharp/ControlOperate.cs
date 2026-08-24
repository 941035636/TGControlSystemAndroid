using System.Diagnostics;
using UnityEngine;

public class ControlOperate : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(this);
	}

	public void OnClick(string message)
	{
		UnityEngine.Debug.LogError("接收到的么:" + message);
		if (!message.Contains("photo") && message.Contains("shutdown"))
		{
			UnityEngine.Debug.LogError("关机啊!");
			Process process = new Process();
			process.StartInfo.FileName = "cmd.exe";
			process.StartInfo.Arguments = "/c shutdown -p";
			process.Start();
		}
	}
}
