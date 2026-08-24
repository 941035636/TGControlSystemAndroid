using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class testTCpClient : MonoBehaviour
{
	private string Per_path = string.Empty;

	public static string num = string.Empty;

	public static string ip = string.Empty;

	public static string port = string.Empty;

	public static bool isConnect = false;

	public static ClientNum clientNum;

	private void OnEnable()
	{
		Per_path = Application.streamingAssetsPath + "/IP.txt";
		if (File.Exists(Per_path))
		{
			num = File.ReadAllText(Per_path).Split('|')[2].Trim();
			ip = File.ReadAllText(Per_path).Split('|')[0].Trim();
			port = File.ReadAllText(Per_path).Split('|')[1].Trim();
		}
		clientNum = new ClientNum();
		clientNum.num = num;
	}

	private void Start()
	{
		NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
		NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
	}

	private void OnApplicationPause(bool pause)
	{
		if (!pause && string.IsNullOrEmpty(ip))
		{
			_ = isConnect;
		}
	}

	private void OnTimer()
	{
		NetManager.Send(clientNum);
	}

	private void Update()
	{
		NetManager.Update();
	}

	public void OnConnectClick()
	{
	}

	public void OnConnectSucc(string str)
	{
		Debug.Log("OnConnectSucc");
		_ = SceneManager.GetActiveScene().name == "Login";
	}

	public void OnConnectFail(string err)
	{
		Debug.Log("OnConnectFail:" + err);
	}

	public void OnConnectClose(string str)
	{
		isConnect = false;
	}

	private void OnApplicationQuit()
	{
		NetManager.Close();
	}
}
