using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;

public class AccControl
{
	private string allAccStr;

	private string accDataPath;

	private string Testdatapath;

	public void AddAccount(AccountModel model)
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(accDataPath, "Accounts.txt"))
		{
			FileUtil.AddFile(accDataPath, "Accounts.txt", info);
		}
		else
		{
			FileUtil.CreateFile(accDataPath, "Accounts.txt", info);
		}
	}

	public AccountModel GetAccount(string acc)
	{
		AccountModel result = new AccountModel();
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string text = FileUtil.LoadFile(accDataPath, "Accounts.txt");
		if (text == null)
		{
			return result;
		}
		string[] array = text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			AccountModel accountModel = JsonMapper.ToObject<AccountModel>(array[i]);
			if (accountModel == null)
			{
				i++;
				break;
			}
			if (accountModel.Account == acc)
			{
				return accountModel;
			}
		}
		return result;
	}

	public List<AccountModel> GetAccountList()
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		FileUtil.CreateFile(accDataPath, "/Accounts.txt", "");
		string text = FileUtil.LoadFile(accDataPath, "Accounts.txt");
		if (text == null)
		{
			return null;
		}
		string[] array = text.Split('\n');
		List<AccountModel> list = new List<AccountModel>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				AccountModel item = JsonMapper.ToObject<AccountModel>(array[i]);
				list.Add(item);
			}
		}
		return list;
	}

	public List<AccountModel> GetAccountTestDataList()
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string text = FileUtil.LoadFile(accDataPath, "Accounts.txt");
		if (text == null)
		{
			return null;
		}
		string[] array = text.Split('\n');
		List<AccountModel> list = new List<AccountModel>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]))
			{
				if (array[i].Contains("NaN"))
				{
					array[i].Replace("NaN", "0");
				}
				AccountModel accountModel = JsonMapper.ToObject<AccountModel>(array[i]);
				if (accountModel.testtime != "")
				{
					list.Add(accountModel);
				}
			}
		}
		return list;
	}

	public bool Match(string acc, string pwd)
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string text = FileUtil.LoadFile(accDataPath, "Accounts.txt");
		if (text == null)
		{
			return false;
		}
		string[] array = text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			AccountModel accountModel = JsonMapper.ToObject<AccountModel>(array[i]);
			if (accountModel.Account == acc && accountModel.Pwd == pwd)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsExistAcc(string acc)
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string text = FileUtil.LoadFile(accDataPath, "Accounts.txt");
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		string[] array = text.Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			AccountModel accountModel = JsonMapper.ToObject<AccountModel>(array[i]);
			if (accountModel == null)
			{
				i++;
				break;
			}
			if (accountModel.Account == acc)
			{
				return true;
			}
		}
		return false;
	}

	public void DeleteAccount()
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string[] array = FileUtil.LoadFile(accDataPath, "Accounts.txt").Split('\n');
		for (int i = 0; i < array.Length; i++)
		{
			JsonMapper.ToObject<AccountModel>(array[i]);
		}
		StringBuilder stringBuilder = new StringBuilder();
		FileUtil.CoverFile(accDataPath, "Accounts.txt", stringBuilder.ToString());
	}

	public void DeleteUserAccount(int index)
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath, "Account");
		Debug.Log("本地数据删除成功" + index);
		List<string> list = new List<string>();
		string[] array = File.ReadAllLines(accDataPath + "/Accounts.txt");
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		Debug.Log("静态传值:" + index);
		list.RemoveAt(index);
		File.WriteAllLines(accDataPath + "/Accounts.txt", list.ToArray());
	}

	public void UpdateAccount(AccountModel model)
	{
		accDataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Account");
		string[] array = FileUtil.LoadFile(accDataPath, "Accounts.txt").Split('\n');
		List<AccountModel> list = new List<AccountModel>();
		for (int i = 0; i < array.Length; i++)
		{
			AccountModel accountModel = JsonMapper.ToObject<AccountModel>(array[i]);
			if (model.Account == accountModel.Account)
			{
				list.Add(model);
			}
			else
			{
				list.Add(accountModel);
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int j = 0; j < list.Count; j++)
		{
			stringBuilder.Append(JsonMapper.ToJson(list[j]));
			stringBuilder.Append("\n");
		}
		FileUtil.CoverFile(accDataPath, "Accounts.txt", stringBuilder.ToString());
	}
}
