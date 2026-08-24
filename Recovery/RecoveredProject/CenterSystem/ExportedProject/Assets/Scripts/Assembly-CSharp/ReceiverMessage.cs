using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReceiverMessage : MonoBehaviour
{
	private bool isplay = true;

	public AsyncOperation async;

	private uint nowprocess;

	public static string VideoName = string.Empty;

	public static string TiIndex = string.Empty;

	public static string TiContent = string.Empty;

	private static ReceiverMessage Instance;

	public static ReceiverMessage GetInstance()
	{
		if (Instance == null)
		{
			Instance = new ReceiverMessage();
		}
		return Instance;
	}

	public void GetOrder(string str)
	{
		if (str.Contains("Index|"))
		{
			TiIndex = str;
			SceneManager.LoadScene(6);
		}
		if (str.Contains("Qes"))
		{
			TiContent = "[" + str.Split('[')[1];
			SceneManager.LoadScene(6);
		}
		string empty = string.Empty;
		empty = str;
		Debug.Log(empty);
		try
		{
			switch (int.Parse(empty))
			{
			case 1006:
				LoadScene_PlayPcAndPico(1006, "长征5.mp4");
				break;
			case 1004:
				LoadScene_PlayPcAndPico(1004, "复兴号_0.mp4");
				break;
			case 1009:
				LoadScene_PlayPcAndPico(1009, "ludingqiao-quanjingbeifen1.mp4");
				break;
			case 1003:
				LoadScene_PlayPcAndPico(1003, "jiaolong_0.mp4");
				break;
			case 1005:
				LoadScene_PlayPcAndPico(1005, "liaoning.mp4");
				break;
			case 1001:
				LoadScene_PlayPcAndPico(1001, "PlanetEarth_1_0.mp4");
				break;
			case 1002:
				LoadScene_PlayPcAndPico(1002, "长征系列第一集.mp4");
				break;
			case 1007:
				LoadScene_PlayPcAndPico(1007, "天眼.mp4");
				break;
			case 1020:
				LoadScene_PlayPcAndPico(1020, "血战湘江.mp4");
				break;
			case 1021:
				LoadScene_PlayPcAndPico(1021, "良庄.mp4");
				break;
			case 1008:
				openPackage("com.dongyi.SKG");
				NetManager.Send(new ShutDownController
				{
					Content = "shutdown"
				});
				break;
			case 1010:
				Debug.Log("打开特钢厂");
				openPackage("com.ShiHeng.TeGang1");
				NetManager.Send(new ShutDownController
				{
					Content = "shutdown"
				});
				break;
			case 101:
				SceneManager.LoadScene("Login");
				break;
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.ToString());
		}
	}

	private void LoadScene_PlayPcAndPico(int number, string videoName)
	{
		NetManager.Send(new ResponseMsgControl
		{
			Code = "shutdown|" + number
		});
		VideoName = videoName;
		SceneManager.LoadSceneAsync("360video");
	}

	private void openPackage(string pkgName)
	{
		using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		using AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
		using AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getPackageManager", Array.Empty<object>());
		using AndroidJavaObject androidJavaObject3 = androidJavaObject2.Call<AndroidJavaObject>("getLaunchIntentForPackage", new object[1] { pkgName });
		if (androidJavaObject3 != null)
		{
			androidJavaObject.Call("startActivity", androidJavaObject3);
		}
	}
}
