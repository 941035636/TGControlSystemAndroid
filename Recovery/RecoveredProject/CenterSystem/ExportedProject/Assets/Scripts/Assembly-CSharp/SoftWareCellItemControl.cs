using System.Collections.Generic;
using LitJson;

public class SoftWareCellItemControl
{
	private static SoftWareCellItemControl Instance;

	private string SoftWareItemPath;

	public static SoftWareCellItemControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new SoftWareCellItemControl();
		}
		return Instance;
	}

	public void AddSoftWareItem(string index, SoftWareModel model)
	{
		SoftWareItemPath = FileUtil.CreateFolder(GameResources.SoftWareCellItemPath, "software");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(SoftWareItemPath, "SoftwareItem.txt"))
		{
			FileUtil.AddFile(SoftWareItemPath, "SoftwareItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(SoftWareItemPath, "SoftwareItem.txt", info);
		}
	}

	public void UpdateSoftWareModel(string index, SoftWareModel model)
	{
		SoftWareItemPath = FileUtil.CreateFolder(GameResources.SoftWareCellItemPath, "software");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(SoftWareItemPath, "SoftwareItem.txt"))
		{
			FileUtil.CoverFile(SoftWareItemPath, "SoftwareItem.txt", info);
		}
		else
		{
			FileUtil.CreateFile(SoftWareItemPath, "SoftwareItem.txt", info);
		}
	}

	public SoftWareModel GetSoftWareCellItem(string index)
	{
		SoftWareModel softWareModel = new SoftWareModel();
		SoftWareItemPath = FileUtil.CreateFolder(GameResources.SoftWareCellItemPath, "software");
		string text = FileUtil.LoadFile(SoftWareItemPath, "SoftwareItem.txt");
		if (text == null)
		{
			AddSoftWareItem(index, softWareModel);
			return softWareModel;
		}
		return JsonMapper.ToObject<SoftWareModel>(text);
	}

	public List<SoftWareModel> GetSoftWareCellItemList(string index)
	{
		List<SoftWareModel> list = new List<SoftWareModel>();
		SoftWareItemPath = FileUtil.CreateFolder(GameResources.SoftWareCellItemPath, "software");
		string[] array = FileUtil.LoadFilebyR(SoftWareItemPath, "SoftwareItem.txt");
		if (array == null)
		{
			return null;
		}
		for (int i = 0; i < array.Length; i++)
		{
			SoftWareModel softWareModel = new SoftWareModel();
			softWareModel = JsonMapper.ToObject<SoftWareModel>(array[i]);
			list.Add(softWareModel);
		}
		_ = list.Count;
		return list;
	}
}
