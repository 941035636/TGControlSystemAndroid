using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DemoScreenshot : MonoBehaviour
{
	private int order;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void SaveScreenshot()
	{
		string text = Directory.GetParent(Application.dataPath).ToString() + "/Screenshots/";
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		string text2 = text + SceneManager.GetActiveScene().name + order + ".png";
		ScreenCapture.CaptureScreenshot(text2);
		order++;
		MonoBehaviour.print(text2);
	}
}
