using System;
using System.Text;
using LitJson;
using UnityEngine;

public class AdminControl
{
	private static AdminControl Instance;

	private string dataPath;

	public static AdminControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new AdminControl();
		}
		return Instance;
	}

	private void AddAdmin(AdminModel model)
	{
		dataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Admin");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(dataPath, "Admin.txt"))
		{
			FileUtil.CoverFile(dataPath, "Admin.txt", info);
		}
		else
		{
			FileUtil.CreateFile(dataPath, "Admin.txt", info);
		}
	}

	public void UpdateModel(AdminModel model)
	{
		dataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Admin");
		string info = JsonMapper.ToJson(model);
		if (FileUtil.IsExistsFile(dataPath, "Admin.txt"))
		{
			FileUtil.CoverFile(dataPath, "Admin.txt", info);
		}
		else
		{
			FileUtil.CreateFile(dataPath, "Admin.txt", info);
		}
	}

	public AdminModel GetAdmin()
	{
		AdminModel adminModel = new AdminModel();
		dataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Admin");
		string text = FileUtil.LoadFile(dataPath, "Admin.txt");
		if (text == null)
		{
			AddAdmin(adminModel);
			return adminModel;
		}
		return JsonMapper.ToObject<AdminModel>(text);
	}

	public string GetAdminPwd()
	{
		AdminModel model = new AdminModel();
		dataPath = FileUtil.CreateFolder(Application.persistentDataPath + "/GameData/", "Admin");
		string text = FileUtil.LoadFile(dataPath, "Admin.txt");
		if (text == null)
		{
			AddAdmin(model);
			return "admin";
		}
		model = JsonMapper.ToObject<AdminModel>(text);
		return model.AdminPwd;
	}

	public bool IsCodeTrue()
	{
		AdminModel admin = GetAdmin();
		if (admin == null)
		{
			return false;
		}
		if (admin.ProductNum == null || admin.ActiveCode == null)
		{
			return false;
		}
		if (!IsMatch(admin.ProductNum, admin.ActiveCode))
		{
			Debug.LogError("不匹配");
			return false;
		}
		return true;
	}

	public string SpawnProductNum(int intLength, bool booNumber, bool booSign, bool booSmallword, bool booBigword)
	{
		System.Random random = new System.Random();
		int num = 0;
		int num2 = 0;
		string text = "";
		while (num < intLength)
		{
			num2 = random.Next(1, 5);
			if (num2 == 1 && booNumber)
			{
				text = random.Next(0, 10) + text;
				num++;
				continue;
			}
			if (num2 == 2 && booSign)
			{
				num2 = random.Next(1, 5);
				switch (num2)
				{
				case 1:
					num2 = random.Next(33, 48);
					text = (char)num2 + text;
					num++;
					continue;
				case 2:
					num2 = random.Next(58, 65);
					text = (char)num2 + text;
					num++;
					continue;
				case 3:
					num2 = random.Next(91, 97);
					text = (char)num2 + text;
					num++;
					continue;
				case 4:
					num2 = random.Next(123, 127);
					text = (char)num2 + text;
					num++;
					continue;
				}
			}
			if (num2 == 3 && booSmallword)
			{
				num2 = random.Next(97, 123);
				text = (char)num2 + text;
				num++;
			}
			else if (num2 == 4 && booBigword)
			{
				num2 = random.Next(65, 89);
				text = (char)num2 + text;
				num++;
			}
		}
		return text;
	}

	public string SpawnProductNum()
	{
		return SystemInfo.deviceUniqueIdentifier.Substring(0, 8);
	}

	private bool IsMatch(string productNum, string activeCode)
	{
		string pwd = GetPwd3(productNum);
		Debug.Log("激活码:" + pwd);
		if (activeCode != pwd)
		{
			return false;
		}
		return true;
	}

	private string SpawnActiveCode(string productNum)
	{
		StringBuilder stringBuilder = new StringBuilder();
		_ = new int[5] { 15, 35, 24, 33, 11 };
		for (int i = 0; i < productNum.Length; i++)
		{
			stringBuilder.Append($"{productNum[i] | 0x3F2:X}");
		}
		return stringBuilder.ToString();
	}

	public static string GetPwd3(string spwanCode)
	{
		StringBuilder stringBuilder = new StringBuilder();
		_ = new int[5] { 15, 35, 24, 33, 11 };
		for (int i = 0; i < spwanCode.Length; i++)
		{
			stringBuilder.Append($"{spwanCode[i] | 0x3F2:X}");
		}
		return stringBuilder.ToString().Substring(0, 16);
	}

	public bool IsMatchAdminPwd(string pwd)
	{
		AdminModel admin = GetAdmin();
		if (pwd == admin.AdminPwd)
		{
			return true;
		}
		return false;
	}
}
