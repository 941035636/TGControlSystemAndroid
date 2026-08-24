using System.Collections.Generic;
using LitJson;
using UnityEngine;

public class RelayModelControl : MonoBehaviour
{
	private static RelayModelControl Instance;

	private string RelayItemPath;

	public static RelayModelControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new RelayModelControl();
		}
		return Instance;
	}

	public void AddRelayItem(string index, RelayModel model)
	{
		RelayItemPath = FileUtil.CreateFolder(GameResources.RelayItemPath, "Relay");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(RelayItemPath, "RelayItem.txt"))
		{
			FileUtil.AddFile(RelayItemPath, "RelayItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(RelayItemPath, "RelayItem.txt", info);
		}
	}

	public void UpdateRelayModel(string index, RelayModel model)
	{
		RelayItemPath = FileUtil.CreateFolder(GameResources.RelayItemPath, "Relay");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(RelayItemPath, "RelayItem.txt"))
		{
			FileUtil.CoverFile(RelayItemPath, "RelayItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(RelayItemPath, "RelayItem.txt", info);
		}
	}

	public RelayModel GetRelayItem(string index)
	{
		RelayModel relayModel = new RelayModel();
		RelayItemPath = FileUtil.CreateFolder(GameResources.RelayItemPath, "Relay");
		string text = FileUtil.LoadFile(RelayItemPath, "RelayItem.txt");
		if (text == null)
		{
			AddRelayItem(index, relayModel);
			return relayModel;
		}
		return JsonMapper.ToObject<RelayModel>(text);
	}

	public List<RelayModel> GetRelayItemList(string index)
	{
		List<RelayModel> list = new List<RelayModel>();
		RelayItemPath = FileUtil.CreateFolder(GameResources.RelayItemPath, "Relay");
		string[] array = FileUtil.LoadFilebyR(RelayItemPath, "RelayItem.txt");
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			RelayModel relayModel = new RelayModel();
			relayModel = JsonMapper.ToObject<RelayModel>(array[i]);
			list.Add(relayModel);
		}
		_ = list.Count;
		return list;
	}

	public List<RelayModel> GetEquipRelayItemList(string index)
	{
		List<RelayModel> list = new List<RelayModel>();
		RelayItemPath = FileUtil.CreateFolder(GameResources.EquipRelayItemPath, "Relay");
		string[] array = FileUtil.LoadFilebyR(RelayItemPath, "RelayItem.txt");
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			RelayModel relayModel = new RelayModel();
			relayModel = JsonMapper.ToObject<RelayModel>(array[i]);
			list.Add(relayModel);
		}
		_ = list.Count;
		return list;
	}
}
