using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InitEquipTing : MonoBehaviour
{
	private void OnEnable()
	{
		GameResources.Instance.RegisterEquipGameDataPath("1");
	}

	private void Start()
	{
		DirectoryInfo[] directories = new DirectoryInfo(Application.persistentDataPath + "/Equip").GetDirectories();
		int num = directories.Length;
		List<string> list = new List<string>();
		for (int i = 0; i < num; i++)
		{
			if (directories[i].Name != "Unity")
			{
				list.Add(directories[i].Name);
			}
		}
		if (list.Count == 0)
		{
			return;
		}
		for (int j = 0; j < list.Count; j++)
		{
			GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpageEquip");
			obj.SetActive(value: false);
			GameObject gameObject = Object.Instantiate(obj, base.transform);
			gameObject.name = "CellScrollpageEquip" + list[j];
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
