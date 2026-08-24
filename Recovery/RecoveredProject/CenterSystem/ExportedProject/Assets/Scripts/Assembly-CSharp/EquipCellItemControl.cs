using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class EquipCellItemControl
{
	private static EquipCellItemControl Instance;

	private string EquipItemPath;

	public static EquipCellItemControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new EquipCellItemControl();
		}
		return Instance;
	}

	public void AddEquipItem(string index, LightCellItemModel model)
	{
		EquipItemPath = FileUtil.CreateFolder(GameResources.EquipCellItemPath, "Equip");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(EquipItemPath, "EquipCellItem.txt"))
		{
			FileUtil.AddFile(EquipItemPath, "EquipCellItem.txt", info);
			Debug.LogError("添加完成：" + EquipItemPath);
		}
		else
		{
			FileUtil.CreateFile(EquipItemPath, "EquipCellItem.txt", info);
		}
	}

	public void UpdateEquipModel(string index, LightCellItemModel model)
	{
		EquipItemPath = FileUtil.CreateFolder(GameResources.EquipCellItemPath, "Equip");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(EquipItemPath, "EquipCellItem.txt"))
		{
			FileUtil.CoverFile(EquipItemPath, "EquipCellItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(EquipItemPath, "EquipCellItem.txt", info);
		}
	}

	public LightCellItemModel GetEquipCellItem(string index)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		EquipItemPath = FileUtil.CreateFolder(GameResources.EquipCellItemPath, "Equip");
		string text = FileUtil.LoadFile(EquipItemPath, "EquipCellItem.txt");
		if (text == null)
		{
			AddEquipItem(index, lightCellItemModel);
			return lightCellItemModel;
		}
		return JsonMapper.ToObject<LightCellItemModel>(text);
	}

	public List<LightCellItemModel> GetEquipCellItemList(string index)
	{
		List<LightCellItemModel> list = new List<LightCellItemModel>();
		EquipItemPath = FileUtil.CreateFolder(GameResources.EquipCellItemPath, "Equip");
		string[] array = FileUtil.LoadFilebyR(EquipItemPath, "EquipCellItem.txt");
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			LightCellItemModel lightCellItemModel = new LightCellItemModel();
			lightCellItemModel = JsonMapper.ToObject<LightCellItemModel>(array[i]);
			list.Add(lightCellItemModel);
		}
		_ = list.Count;
		return list;
	}
}
