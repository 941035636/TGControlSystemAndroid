using System.IO;
using Data;
using UnityEngine;

public class GameResources
{
	private static GameResources instance;

	public static ItemManager manager;

	public static ItemManager Equipmanager;

	public static string LightCellItemPath;

	public static string RelayItemPath;

	public static string CellPath;

	public static string EquipCellPath;

	public static string EquipCellItemPath;

	public static string EquipRelayItemPath;

	public static string SoftWareCellPath;

	public static string SoftWareCellItemPath;

	private static string ConfigName = "/item.xlsx";

	public static GameResources Instance
	{
		get
		{
			if (instance == null)
			{
				instance = new GameResources();
			}
			return instance;
		}
	}

	public void Register()
	{
		manager = new ItemManager();
		Equipmanager = new ItemManager();
	}

	public void RegisterGameDataPath(string index)
	{
		FileUtil.OnlyCreateFile(Application.persistentDataPath + "/Light/" + index, "/TingName.text");
		LightCellItemPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Light/" + index, "LightCellItem");
		RelayItemPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Light/" + index, "Relay");
		CellPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Light/" + index, "Cell");
		if (!Directory.Exists(Application.persistentDataPath + "/灯光控制器"))
		{
			CopyFoderFile("灯光控制器", Application.streamingAssetsPath + "/灯光控制器/万智八路继电器.txt", "万智八路继电器.txt", 0);
		}
		if (!Directory.Exists(Application.persistentDataPath + "/设备控制器"))
		{
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/八路WIFI网络学习型红外模块(XM_IR_08).txt", "八路WIFI网络学习型红外模块(XM_IR_08).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/八路网口控制器(JY-DAMXXXX系列).txt", "八路网口控制器(JY-DAMXXXX系列).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器101((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器101((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器102((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器102((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器103((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器103((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器104((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器104((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器105((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器105((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器106((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器106((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器107((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器107((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器108((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器108((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器109((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器109((JY-DAMXXXX系列)).txt", 0);
			CopyFoderFile("设备控制器", Application.streamingAssetsPath + "/设备控制器/两路WIFI远程智能控制器110((JY-DAMXXXX系列)).txt", "两路WIFI远程智能控制器110((JY-DAMXXXX系列)).txt", 0);
		}
		if (!Directory.Exists(Application.persistentDataPath + "/红外模块指令"))
		{
			CopyFoderFile("红外模块指令", Application.streamingAssetsPath + "/红外模块指令/投影开.txt", "投影开.txt", 0);
			CopyFoderFile("红外模块指令", Application.streamingAssetsPath + "/红外模块指令/投影关.txt", "投影关.txt", 0);
			CopyFoderFile("红外模块指令", Application.streamingAssetsPath + "/红外模块指令/电视一.txt", "电视一.txt", 0);
			CopyFoderFile("红外模块指令", Application.streamingAssetsPath + "/红外模块指令/电视二.txt", "电视二.txt", 0);
			CopyFoderFile("红外模块指令", Application.streamingAssetsPath + "/红外模块指令/电视三.txt", "电视三.txt", 0);
		}
		manager.dataArray = ExcelTool.CreateItemArrayWithTxt("控制器类型");
		Equipmanager.dataArray = ExcelTool.CreateItemArrayWithTxt("控制器类型");
	}

	public static void CopyFoderFile(string kindname, string path, string filename, int imageType)
	{
		string destination = Path.Combine(Application.persistentDataPath, kindname, filename);
		Directory.CreateDirectory(Path.GetDirectoryName(destination));
		if (Application.platform == RuntimePlatform.Android)
		{
			CopyAndroidAsset(kindname + "/" + filename, destination);
			return;
		}
		File.Copy(path, destination, overwrite: true);
	}

	public static void CopyFile(string fileName, int imageType)
	{
		string destination = Path.Combine(Application.persistentDataPath, fileName);
		Directory.CreateDirectory(Path.GetDirectoryName(destination));
		if (Application.platform == RuntimePlatform.Android)
		{
			CopyAndroidAsset(fileName, destination);
			return;
		}
		File.Copy(Path.Combine(Application.streamingAssetsPath, fileName), destination, overwrite: true);
	}

	private static void CopyAndroidAsset(string assetRelativePath, string destination)
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
			using (AndroidJavaObject assets = activity.Call<AndroidJavaObject>("getAssets"))
			using (AndroidJavaObject input = assets.Call<AndroidJavaObject>("open", assetRelativePath.Replace('\\', '/')))
			using (FileStream output = File.Create(destination))
			{
				byte[] buffer = new byte[8192];
				int count;
				while ((count = input.Call<int>("read", buffer)) > 0)
				{
					output.Write(buffer, 0, count);
				}
				input.Call("close");
			}
		}
		catch (System.Exception ex)
		{
			Debug.LogError("复制安卓内置资源失败 " + assetRelativePath + ": " + ex.Message);
		}
	#else
		Debug.LogError("CopyAndroidAsset 只能在安卓设备上调用: " + assetRelativePath);
	#endif
	}

	public void RegisterEquipGameDataPath(string index)
	{
		FileUtil.OnlyCreateFile(Application.persistentDataPath + "/Equip/" + index, "/TingName.text");
		EquipCellPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Equip/" + index, "Cell");
		EquipCellItemPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Equip/" + index, "EquipCellItem");
		EquipRelayItemPath = FileUtil.CreateFolder(Application.persistentDataPath + "/Equip/" + index, "Relay");
	}

	public void RegisterSoftWareGameDataPath(string index)
	{
		SoftWareCellPath = FileUtil.CreateFolder(Application.persistentDataPath + "/SoftWare/" + index, "Cell");
		SoftWareCellItemPath = FileUtil.CreateFolder(Application.persistentDataPath + "/SoftWare/" + index, "SoftWareCellItem");
	}
}
