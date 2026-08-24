using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class InitTing : MonoBehaviour
{
	private void OnEnable()
	{
		GameResources.Instance.RegisterGameDataPath("1");
	}

	private void Start()
	{
		DirectoryInfo[] directories = new DirectoryInfo(Application.persistentDataPath + "/Light").GetDirectories();
		int num = directories.Length;
		List<string> list = new List<string>();
		for (int i = 0; i < num; i++)
		{
			if (directories[i].Name != "Unity")
			{
				list.Add(directories[i].Name);
			}
		}
		if (list.Count != 0)
		{
			for (int j = 0; j < list.Count; j++)
			{
				GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpage");
				obj.SetActive(value: false);
				GameObject obj2 = Object.Instantiate(obj, base.transform);
				obj2.name = "CellScrollpage" + list[j];
				obj2.transform.localScale = new Vector3(1f, 1f, 1f);
				obj2.SetActive(value: true);
			}
		}
	}
}
