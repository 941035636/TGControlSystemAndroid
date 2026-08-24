using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InitSoftWareTing : MonoBehaviour
{
	private DirectoryInfo[] dirarr;

	private void Start()
	{
		GameResources.Instance.RegisterSoftWareGameDataPath("1");
	}

	private void OnEnable()
	{
		int num = 0;
		Debug.Log(base.transform.childCount);
		for (int i = 0; i < base.transform.childCount; i++)
		{
			Debug.Log("删除:" + i);
			Object.DestroyImmediate(base.transform.GetChild(i).gameObject);
		}
		if (Directory.Exists(Application.persistentDataPath + "/SoftWare"))
		{
			dirarr = new DirectoryInfo(Application.persistentDataPath + "/SoftWare").GetDirectories();
		}
		if (dirarr != null)
		{
			num = dirarr.Length;
		}
		List<string> list = new List<string>();
		for (int j = 0; j < num; j++)
		{
			if (dirarr[j].Name != "Unity")
			{
				list.Add(dirarr[j].Name);
			}
		}
		Debug.Log("TingList:" + list.Count);
		if (list.Count == 0)
		{
			return;
		}
		for (int k = 0; k < list.Count; k++)
		{
			GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpageSoftWare");
			obj.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(obj, base.transform);
			gameObject.name = "CellScrollpageSoftWare" + list[k];
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			gameObject.SetActive(value: true);
			if (Globel.IsAdmin)
			{
				gameObject.transform.Find("Manager").gameObject.GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				gameObject.transform.Find("Manager").gameObject.GetComponent<CanvasGroup>().alpha = 0f;
			}
		}
	}
}
