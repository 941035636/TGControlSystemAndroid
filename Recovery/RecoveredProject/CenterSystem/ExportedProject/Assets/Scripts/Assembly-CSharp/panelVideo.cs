using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class panelVideo : MonoBehaviour
{
	private new string name;

	private string ip;

	private string port;

	private int count;

	private string[] StrArr;

	public Transform Parent;

	public GameObject plane05;

	private GameObject ControllUnitPrefab;

	public Button ExitBtn;

	public Toggle SubBtn;

	private void Start()
	{
		instantiateFormFile(readFromFile());
		ExitBtn.onClick.AddListener(delegate
		{
			Application.Quit();
		});
	}

	private string[] readFromFile()
	{
		try
		{
			StrArr = FileUtil.LoadFilebyR(Application.persistentDataPath, "ControlVideoData.txt");
		}
		catch (Exception)
		{
			throw;
		}
		return StrArr;
	}

	private void instantiateFormFile(string[] arr)
	{
		if (!File.Exists(Application.persistentDataPath + "/ControlVideoData.txt"))
		{
			return;
		}
		try
		{
			for (int i = 0; i < arr.Length - 1; i++)
			{
				if (string.IsNullOrEmpty(arr[i].Trim().Split('|')[0]))
				{
					continue;
				}
				ControllUnitPrefab = Resources.Load<GameObject>("Prefabs/LightControllUnit");
				GameObject obj = UnityEngine.Object.Instantiate(ControllUnitPrefab, Parent);
				obj.transform.Find("Text").GetComponent<Text>().text = arr[i].Split('|')[0];
				string name = arr[i].Split('|')[0];
				_ = arr[i].Split('|')[1];
				_ = arr[i].Split('|')[2];
				obj.GetComponent<Button>().onClick.RemoveAllListeners();
				obj.GetComponent<Button>().onClick.AddListener(delegate
				{
					plane05.transform.Find("Name").GetComponent<Text>().text = name.Trim();
					plane05.gameObject.SetActive(value: true);
				});
				SubBtn.onValueChanged.AddListener(delegate(bool ison)
				{
					if (ison)
					{
						if ((bool)obj)
						{
							obj.transform.Find("DeleteBtn").gameObject.SetActive(value: true);
						}
					}
					else if ((bool)obj)
					{
						obj.transform.Find("DeleteBtn").gameObject.SetActive(value: false);
					}
				});
				obj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					UnityEngine.Object.Destroy(obj);
				});
			}
		}
		catch (Exception)
		{
			throw;
		}
	}
}
