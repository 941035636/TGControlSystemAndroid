using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Globel : MonoBehaviour
{
	public Text netStateTxt;

	public static bool IsAdmin;

	public static bool IsConnect;

	[DllImport("winInet.dll")]
	private static extern bool InternetGetConnectedState(ref int dwFlag, int dwReserved);

	private void Awake()
	{
		Debug.LogError(AdminControl.GetPwd3("c14ad43f"));
		GameResources.Instance.Register();
		Screen.sleepTimeout = -1;
	}

	private void Start()
	{
		SceneManager.LoadScene("Start");
		IsConnect = ChinarPing(new string[1] { "58.56.208.202" }, out var _);
		if (IsConnect)
		{
			netStateTxt.text = "设备已连接外网";
		}
		else
		{
			netStateTxt.text = "外网连接断开";
		}
		Object.DontDestroyOnLoad(this);
	}

	private void Update()
	{
	}

	private bool IsConnectedInternet()
	{
		int dwFlag = 0;
		if (!InternetGetConnectedState(ref dwFlag, 0))
		{
			netStateTxt.text = "当前没有联网，请您先联网后再进行操作！";
			Debug.Log("当前没有联网，请您先联网后再进行操作！");
			if ((dwFlag & 0x14) == 0)
			{
				return false;
			}
			Debug.LogWarning("本地系统处于脱机模式。");
			netStateTxt.text = "本地系统处于脱机模式。";
			return false;
		}
		if ((dwFlag & 1) != 0)
		{
			netStateTxt.text = "调制解调器上网。";
			Debug.Log("调制解调器上网。");
			return true;
		}
		if ((dwFlag & 2) != 0)
		{
			netStateTxt.text = "网卡上网。";
			Debug.Log("网卡上网。");
			return true;
		}
		if ((dwFlag & 4) != 0)
		{
			netStateTxt.text = "代理服务器上网。";
			Debug.Log("代理服务器上网。");
			return true;
		}
		if ((dwFlag & 0x40) != 0)
		{
			netStateTxt.text = "虽然可以联网，但可能链接也可能不连接。";
			Debug.Log("虽然可以联网，但可能链接也可能不连接。");
			return true;
		}
		return false;
	}

	public bool ChinarPing(string url)
	{
		bool result = true;
		System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
		try
		{
			PingReply pingReply = ping.Send(url);
			if (pingReply != null && pingReply.Status != IPStatus.Success)
			{
				result = false;
			}
			if (pingReply != null)
			{
				MonoBehaviour.print("Ping 网址：<" + url + ">------状态：" + pingReply.Status);
			}
		}
		catch
		{
			result = false;
		}
		return result;
	}

	public static bool ChinarPing(string[] urls, out int errorCount)
	{
		bool result = true;
		System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
		errorCount = 0;
		try
		{
			for (int i = 0; i < urls.Length; i++)
			{
				PingReply pingReply = ping.Send(urls[i]);
				if (pingReply != null && pingReply.Status != IPStatus.Success)
				{
					result = false;
					errorCount++;
				}
				if (pingReply != null)
				{
					MonoBehaviour.print("Ping 网址：<" + urls[i] + ">------状态：" + pingReply.Status);
				}
			}
		}
		catch
		{
			result = false;
			errorCount = urls.Length;
		}
		return result;
	}
}
