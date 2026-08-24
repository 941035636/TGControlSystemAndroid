using System.Collections.Generic;
using LitJson;

public class LightCellItemControl
{
	private static LightCellItemControl Instance;

	private string LightItemPath;

	public static LightCellItemControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new LightCellItemControl();
		}
		return Instance;
	}

	public void AddLightItem(string index, LightCellItemModel model)
	{
		LightItemPath = FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(LightItemPath, "LightCellItem.txt"))
		{
			FileUtil.AddFile(LightItemPath, "LightCellItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(LightItemPath, "LightCellItem.txt", info);
		}
	}

	public void UpdateLightModel(string index, LightCellItemModel model)
	{
		LightItemPath = FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(LightItemPath, "LightCellItem.txt"))
		{
			FileUtil.CoverFile(LightItemPath, "LightCellItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(LightItemPath, "LightCellItem.txt", info);
		}
	}

	public LightCellItemModel GetLightCellItem(string index)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		LightItemPath = FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light");
		string text = FileUtil.LoadFile(LightItemPath, "LightCellItem.txt");
		if (text == null)
		{
			AddLightItem(index, lightCellItemModel);
			return lightCellItemModel;
		}
		return JsonMapper.ToObject<LightCellItemModel>(text);
	}

	public List<LightCellItemModel> GetLightCellItemList(string index)
	{
		List<LightCellItemModel> list = new List<LightCellItemModel>();
		LightItemPath = FileUtil.CreateFolder(GameResources.LightCellItemPath, "Light");
		string[] array = FileUtil.LoadFilebyR(LightItemPath, "LightCellItem.txt");
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
