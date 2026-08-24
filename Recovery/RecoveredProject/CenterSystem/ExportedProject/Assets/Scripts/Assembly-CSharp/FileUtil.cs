using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;

public class FileUtil
{
	public static string GetRunDirectory()
	{
		return Environment.CurrentDirectory;
	}

	public static string GetRunDirectoryInParentPath(int pathNum = 3)
	{
		DirectoryInfo parent = Directory.GetParent(GetRunDirectory());
		while (pathNum > 0 && parent.Parent != null)
		{
			parent = parent.Parent;
			pathNum--;
		}
		return parent.FullName;
	}

	public static string CreateFolder(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		return path;
	}

	public static string CreateFolder(string path, string foldName)
	{
		string text = path + "/" + foldName;
		if (!Directory.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	public static void CreateFile(string path, string fileName, string info)
	{
		FileInfo fileInfo = new FileInfo(path + "/" + fileName);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		if (fileInfo.Exists)
		{
			File.Delete(path + "/" + fileName);
		}
		StreamWriter streamWriter = fileInfo.CreateText();
		streamWriter.WriteLine(info);
		streamWriter.Close();
		streamWriter.Dispose();
	}

	public static void OnlyCreateFile(string path, string fileName)
	{
		FileInfo fileInfo = new FileInfo(path + "/" + fileName);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		if (!fileInfo.Exists)
		{
			StreamWriter streamWriter = fileInfo.CreateText();
			streamWriter.Close();
			streamWriter.Dispose();
		}
	}

	public static void AddFile(string path, string fileName, string info)
	{
		FileInfo fileInfo = new FileInfo(path + "/" + fileName);
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		StreamWriter obj = ((!fileInfo.Exists) ? fileInfo.CreateText() : fileInfo.AppendText());
		obj.WriteLine(info);
		obj.Close();
		obj.Dispose();
	}

	public static string LoadFile(string path, string fileName)
	{
		if (!IsExistsFile(path, fileName))
		{
			return null;
		}
		StreamReader streamReader = File.OpenText(path + "/" + fileName);
		string result = streamReader.ReadToEnd().Replace("\r", "").Trim();
		streamReader.Close();
		streamReader.Dispose();
		return result;
	}

	public static string[] LoadFilebyR(string path, string fileName)
	{
		if (!IsExistsFile(path, fileName))
		{
			return null;
		}
		StreamReader streamReader = new StreamReader(new FileStream(path + "/" + fileName, FileMode.Open, FileAccess.Read), Encoding.UTF8);
		string[] result = streamReader.ReadToEnd().Trim().Split('\n');
		streamReader.Close();
		streamReader.Dispose();
		return result;
	}

	public static bool IsExistsFile(string path, string fileName)
	{
		if (!Directory.Exists(path))
		{
			return false;
		}
		if (!File.Exists(path + "/" + fileName))
		{
			return false;
		}
		return true;
	}

	public static void CoverFile(string path, string fileName, string info)
	{
		File.WriteAllText(path + "/" + fileName, info);
	}

	public static void CoverLineFile(string Path, string filename, string headName, string coder)
	{
		string text = Path + "/" + filename;
		if (!File.Exists(text))
		{
			Debug.LogError("不存在路径：" + text);
			return;
		}
		StreamReader streamReader = File.OpenText(text);
		string[] array = streamReader.ReadToEnd().Trim().Split('\n');
		Debug.LogError(array.Length);
		streamReader.Close();
		streamReader.Dispose();
		DeleteFile(Path, filename);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Split(':')[0];
			Debug.Log("str1:" + text2);
			string text3 = array[i].Split(':')[1];
			Debug.Log("str2:" + text3);
			if (!string.IsNullOrEmpty(array[i].Trim()))
			{
				Debug.Log("headname:" + headName);
				if (headName.Trim() != text2)
				{
					AddFile(Path, filename, array[i].Trim());
				}
				else
				{
					AddFile(Path, filename, text2 + ":" + coder.Trim());
				}
			}
		}
	}

	public static string GetLastLine(string path, string fileName)
	{
		if (!IsExistsFile(path, fileName))
		{
			return null;
		}
		StreamReader streamReader = File.OpenText(path + "/" + fileName);
		string result = "";
		while (!streamReader.EndOfStream)
		{
			result = streamReader.ReadLine();
		}
		streamReader.Close();
		streamReader.Dispose();
		return result;
	}

	public static void DeleteFile(string path, string fileName)
	{
		string path2 = path + "/" + fileName;
		if (File.Exists(path2))
		{
			File.Delete(path2);
		}
	}

	public static void DeleteFolder(string path, string foldName)
	{
		string path2 = path + "/" + foldName;
		if (!Directory.Exists(path2))
		{
			return;
		}
		try
		{
			Directory.Delete(path2, recursive: true);
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
		}
	}

	public static void DeleContentbyLine(string Path, string filename, string headName)
	{
		string text = Path + "/" + filename;
		if (!File.Exists(text))
		{
			Debug.LogError("不存在路径：" + text);
			return;
		}
		StreamReader streamReader = File.OpenText(text);
		string[] array = streamReader.ReadToEnd().Trim().Split('\n');
		Debug.LogError(array.Length);
		streamReader.Close();
		streamReader.Dispose();
		DeleteFile(Path, filename);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Split(',')[0].Split(':')[1];
			Debug.Log("str1:" + text2);
			string text3 = text2.Split('"')[1].Trim();
			Debug.Log("str2:" + text3);
			if (!string.IsNullOrEmpty(array[i].Trim()))
			{
				Debug.Log("headname:" + headName);
				if (headName.Trim() != text3)
				{
					AddFile(Path, filename, array[i].Trim());
				}
			}
		}
	}

	public static int SearchNearEmptyParentName(string Path, string filename)
	{
		string text = Path + "/" + filename;
		Debug.LogError(text);
		int num = 0;
		if (!File.Exists(text))
		{
			Debug.LogError("不存在路径：" + text);
			return 1;
		}
		StreamReader streamReader = File.OpenText(text);
		string[] array = streamReader.ReadToEnd().Trim().Split('\n');
		Debug.LogError(array.Length);
		streamReader.Close();
		streamReader.Dispose();
		List<int> list = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			int item = int.Parse(array[i].Split(',')[0].Split(':')[1].Split('"')[1].Trim().Split('l')[2]);
			list.Add(item);
		}
		int[] array2 = list.ToArray();
		for (int j = 0; j < array2.Length - 1; j++)
		{
			for (int k = 0; k < array2.Length - 1 - j; k++)
			{
				if (array2[k] > array2[k + 1])
				{
					int num2 = array2[k];
					array2[k] = array2[k + 1];
					array2[k + 1] = num2;
				}
			}
		}
		num = array2[array2.Length - 1] + 1;
		Debug.LogError("fnaltemp:" + num);
		return num;
	}

	public static void UpdateLightModelFile(string path, string fileName, string name, bool isopen)
	{
		string text = path + "/" + fileName;
		if (!File.Exists(text))
		{
			Debug.LogError("不存在路径:" + text);
			return;
		}
		StreamReader streamReader = File.OpenText(text);
		string[] array = streamReader.ReadToEnd().Trim().Split('\n');
		Debug.LogError(array.Length);
		streamReader.Close();
		streamReader.Dispose();
		DeleteFile(path, fileName);
		for (int i = 0; i < array.Length; i++)
		{
			LightCellItemModel lightCellItemModel = JsonMapper.ToObject<LightCellItemModel>(array[i].Trim());
			if (lightCellItemModel.Get_DiscribName == name)
			{
				lightCellItemModel.Get_IsOpen = isopen;
				string text2 = JsonMapper.ToJson(lightCellItemModel);
				AddFile(path, fileName, text2.Trim());
			}
			else
			{
				AddFile(path, fileName, array[i].Trim());
			}
		}
	}

	public static void DeleRelayContentbyLine(string Path, string filename, string headName)
	{
		string text = Path + "/" + filename;
		if (!File.Exists(text))
		{
			Debug.LogError("不存在路径：" + text);
			return;
		}
		StreamReader streamReader = File.OpenText(text);
		string[] array = streamReader.ReadToEnd().Trim().Split('\n');
		Debug.LogError(array.Length);
		streamReader.Close();
		streamReader.Dispose();
		DeleteFile(Path, filename);
		for (int i = 0; i < array.Length; i++)
		{
			string text2 = array[i].Split(',')[3].Split(':')[1];
			Debug.Log("str1:" + text2);
			string text3 = text2.Split('"')[1].Trim();
			Debug.Log("str2:" + text3);
			if (!string.IsNullOrEmpty(array[i].Trim()))
			{
				Debug.Log("headname:" + headName);
				if (headName.Trim() != text3)
				{
					AddFile(Path, filename, array[i].Trim());
				}
			}
		}
	}
}
