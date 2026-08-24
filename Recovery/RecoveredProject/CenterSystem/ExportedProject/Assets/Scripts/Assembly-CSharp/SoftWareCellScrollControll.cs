using System.Collections;
using System.Collections.Generic;
using Data;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class SoftWareCellScrollControll : MonoBehaviour
{
	private Toggle deleteTog;

	private Toggle AllOn;

	private Toggle AllOff;

	private Toggle[] TogArr;

	private Button BtnAdd;

	private InputField InputTingName;

	private InputField InputName;

	private InputField IpAdress;

	private InputField Port;

	private Button btnAdd;

	private Button btnCancle;

	private int CellCount;

	private ItemManager itemArr;

	private List<string> Opencodelist = new List<string>();

	private List<string> Closecodelist = new List<string>();

	private List<SoftWareModel> ListModel;

	private Transform ContentrelayTrans;

	private void Awake()
	{
		deleteTog = base.transform.Find("Manager/DeleteTog").GetComponent<Toggle>();
		CellCount = base.transform.Find("Viewport/Content").childCount;
		deleteTog.onValueChanged.AddListener(delegate(bool ison)
		{
			for (int i = 0; i < CellCount; i++)
			{
				if (base.transform.Find("Viewport/Content").GetChild(i).childCount != 0)
				{
					base.transform.Find("Viewport/Content").GetChild(i).GetChild(0)
						.Find("DeleteBtn")
						.gameObject.SetActive(ison);
				}
			}
		});
	}

	private void OnEnable()
	{
		ListModel = new List<SoftWareModel>();
		itemArr = GameResources.manager;
		AllOn = base.transform.Find("TogOnoff/TogOn").GetComponent<Toggle>();
		AllOff = base.transform.Find("TogOnoff/TogOff").GetComponent<Toggle>();
		BtnAdd = base.transform.Find("Manager/BtnAdd").GetComponent<Button>();
		GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
		InitCell();
		InitCellItemByData();
	}

	private IEnumerator StarOpenTog(Toggle[] togarr)
	{
		for (int i = 0; i < togarr.Length; i++)
		{
			TogArr[i].GetComponent<ChinarDragSwapImage>().isOn = true;
			TogArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-LED-1");
			yield return new WaitForSeconds(0.1f);
		}
	}

	private IEnumerator StarCloseTog(Toggle[] togarr)
	{
		for (int i = 0; i < togarr.Length; i++)
		{
			TogArr[i].GetComponent<ChinarDragSwapImage>().isOn = false;
			TogArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-LED-0");
			yield return new WaitForSeconds(0.1f);
		}
	}

	private void init()
	{
		CellCount = base.transform.Find("Viewport/Content").childCount;
		AllOn.onValueChanged.AddListener(delegate(bool ison)
		{
			TogArr = base.transform.Find("Viewport/Content").GetComponentsInChildren<Toggle>();
			if (ison)
			{
				AllOn.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/其他/一键开关/一键开关-打开-1");
				StartCoroutine(StarOpenTog(TogArr));
			}
			else
			{
				AllOn.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/其他/一键开关/一键开关-打开-0");
			}
		});
		AllOff.onValueChanged.AddListener(delegate(bool ison)
		{
			TogArr = base.transform.Find("Viewport/Content").GetComponentsInChildren<Toggle>();
			if (ison)
			{
				AllOff.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/其他/一键开关/一键开关-关闭-1");
				StartCoroutine(StarCloseTog(TogArr));
			}
			else
			{
				AllOff.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/其他/一键开关/一键开关-关闭-0");
			}
		});
	}

	private void InitCellItemByData()
	{
		ListModel.Clear();
		ListModel = SoftWareModelControl.GetInstance().GetSoftWareCellItemList(base.transform.name.Split('e')[3]);
		if (ListModel != null)
		{
			Debug.LogError(ListModel.Count);
		}
		if (ListModel != null)
		{
			for (int i = 0; i < ListModel.Count; i++)
			{
				int index = i;
				InstantiateCellItem(ListModel[index].GetIsSheBei, ListModel[index].GetIsPhoto, ListModel[index].GetIsVideo, ListModel[index].GetPath, ListModel[index].Get_ParentName, ListModel[index].GetIp, ListModel[index].GetPort, ListModel[index].GetName, ListModel[index].GetNote, ListModel[index].Get_TingName, ListModel[index].Get_ShowContenList);
			}
		}
	}

	private void SetTogInter(bool ison)
	{
		int childCount = base.transform.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<SoftWareChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	private void InstantiateCellItem(bool isshebei, bool isPhoto, bool isvideo, string path, string ParentName, string ip, string port, string name, string note, string tingname, List<string> showContentList)
	{
		Transform parent = base.transform.Find("Viewport/Content").Find(ParentName);
		if (isvideo)
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/VideoItem");
			GameObject ItemObj = Object.Instantiate(original);
			ItemObj.transform.SetParent(parent);
			ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
			ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
			ItemObj.name = name;
			ItemObj.transform.Find("Text").GetComponent<Text>().text = name;
			ItemObj.GetComponent<Button>().onClick.AddListener(delegate
			{
				ControlVideoPanel.ip = ip;
				ControlVideoPanel.port = port;
				ControlVideoPanel.path = path;
				ControlVideoPanel.name = name;
				ControlVideoPanel.note = note;
				ControlVideoPanel.istv = isshebei;
				ControlVideoPanel.isphoto = isPhoto;
				ControlVideoPanel.ShowContentList = showContentList;
				Debug.LogError("showContentList.count:" + showContentList.Count);
				Debug.Log("打开控制面板,ip:" + ip);
				GameObject original2 = Resources.Load<GameObject>("Prefabs/ControlVideoPanel");
				ControlVideoPanel.Item_Name = name;
				GameObject obj = Object.Instantiate(original2);
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = new Vector3(-175f, 1f, 1f);
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
				obj.AddComponent<UDPClient>();
				EventCenter.BroadCast(Eventdefine.InitSocket, ip);
				SetTogInter(ison: false);
			});
			ItemObj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
				GameObject panelObj = Object.Instantiate(original2);
				panelObj.transform.SetParent(base.transform);
				panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
				panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
				panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
				{
					GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
					FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj.transform.parent.name);
					Object.Destroy(ItemObj);
					panelObj.GetComponent<CanvasGroup>().alpha = 0f;
					panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
				});
				panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(delegate
				{
					panelObj.GetComponent<CanvasGroup>().alpha = 0f;
					panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
				});
			});
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.RemoveAllListeners();
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.AddListener(delegate
			{
				int childCount = base.transform.Find("Viewport/Content").childCount;
				Debug.Log(childCount);
				GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Cell"));
				gameObject.name = ("Cell" + childCount).Split('(')[0];
				gameObject.transform.SetParent(base.transform.Find("Viewport/Content"));
				gameObject.transform.localPosition = new Vector3(1f, 1f, 1f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				base.transform.Find("Viewport/Content").Find("CellAdd").transform.SetAsLastSibling();
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
				FileUtil.CoverFile(GameResources.SoftWareCellPath, "cell.txt", childCount.ToString());
				Debug.LogError(GameResources.SoftWareCellPath);
			});
		}
		else if (isPhoto)
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/softwareItem");
			GameObject ItemObj2 = Object.Instantiate(original);
			ItemObj2.transform.SetParent(parent);
			ItemObj2.transform.localPosition = new Vector3(1f, 1f, 1f);
			ItemObj2.transform.localScale = new Vector3(1f, 1f, 1f);
			ItemObj2.name = name;
			ItemObj2.transform.Find("Text").GetComponent<Text>().text = name;
			ItemObj2.GetComponent<Button>().onClick.AddListener(delegate
			{
				ControlSoftWarePanel.ip = ip;
				ControlSoftWarePanel.port = port;
				ControlSoftWarePanel.path = path;
				ControlSoftWarePanel.name = name;
				ControlSoftWarePanel.note = note;
				Debug.Log("打开控制面板,ip:" + ip);
				Debug.LogError(isshebei + " " + isPhoto);
				ControlSoftWarePanel.IsTV = isshebei;
				ControlSoftWarePanel.IsPhoto = false;
				GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlPhotoPanel"));
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = new Vector3(-175f, 1f, 1f);
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
				EventCenter.BroadCast(Eventdefine.PhotoUpdConnect, ip, "7014");
				base.transform.parent.parent.parent.parent.Find("PhoteNetPar").gameObject.SetActive(value: true);
				SetTogInter(ison: false);
			});
			ItemObj2.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				Debug.LogError("删除控制单元:" + ItemObj2.transform.parent.name);
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
				FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj2.transform.parent.name);
				Object.Destroy(ItemObj2);
			});
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.RemoveAllListeners();
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.AddListener(delegate
			{
				int childCount = base.transform.Find("Viewport/Content").childCount;
				Debug.Log(childCount);
				GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Cell"));
				gameObject.name = ("Cell" + childCount).Split('(')[0];
				gameObject.transform.SetParent(base.transform.Find("Viewport/Content"));
				gameObject.transform.localPosition = new Vector3(1f, 1f, 1f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				base.transform.Find("Viewport/Content").Find("CellAdd").transform.SetAsLastSibling();
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
				FileUtil.CoverFile(GameResources.SoftWareCellPath, "cell.txt", childCount.ToString());
				Debug.LogError(GameResources.SoftWareCellPath);
			});
		}
		else if (isshebei)
		{
			GameObject original = Resources.Load<GameObject>("Prefabs/TVItem");
			GameObject ItemObj3 = Object.Instantiate(original);
			ItemObj3.transform.SetParent(parent);
			ItemObj3.transform.localPosition = new Vector3(1f, 1f, 1f);
			ItemObj3.transform.localScale = new Vector3(1f, 1f, 1f);
			ItemObj3.name = name;
			ItemObj3.transform.Find("Text").GetComponent<Text>().text = name;
			ItemObj3.GetComponent<Button>().onClick.AddListener(delegate
			{
				ControlSoftWarePanel.ip = ip;
				ControlSoftWarePanel.port = port;
				ControlSoftWarePanel.path = path;
				ControlSoftWarePanel.name = name;
				ControlSoftWarePanel.note = note;
				Debug.Log("打开控制面板,ip:" + ip);
				Debug.LogError(isshebei + " " + isPhoto);
				GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlShebeiPanel"));
				obj.transform.SetParent(base.transform);
				obj.transform.localPosition = new Vector3(-175f, 1f, 1f);
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
				obj.transform.Find("TitleTxt").GetComponent<Text>().text = name;
				obj.AddComponent<UDPClient>();
				EventCenter.BroadCast(Eventdefine.InitSocket, ip);
			});
			ItemObj3.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				Debug.LogError("删除控制单元:" + ItemObj3.transform.parent.name);
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
				FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj3.transform.parent.name);
				Object.Destroy(ItemObj3);
			});
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.RemoveAllListeners();
			base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
				.onClick.AddListener(delegate
			{
				int childCount = base.transform.Find("Viewport/Content").childCount;
				Debug.Log(childCount);
				GameObject gameObject = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Cell"));
				gameObject.name = ("Cell" + childCount).Split('(')[0];
				gameObject.transform.SetParent(base.transform.Find("Viewport/Content"));
				gameObject.transform.localPosition = new Vector3(1f, 1f, 1f);
				gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
				base.transform.Find("Viewport/Content").Find("CellAdd").transform.SetAsLastSibling();
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('e')[3]);
				FileUtil.CoverFile(GameResources.SoftWareCellPath, "cell.txt", childCount.ToString());
				Debug.LogError(GameResources.SoftWareCellPath);
			});
		}
	}

	private void InitCell()
	{
		string text = FileUtil.LoadFile(GameResources.SoftWareCellPath, "cell.txt");
		if (string.IsNullOrEmpty(text))
		{
			return;
		}
		int num = int.Parse(text);
		if (num > 21)
		{
			int num2 = num - 21;
			for (int i = 0; i < num2; i++)
			{
				GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Cell"));
				obj.name = "Cell" + (22 + i);
				obj.transform.SetParent(base.transform.Find("Viewport/Content"));
				obj.transform.localPosition = new Vector3(1f, 1f, 1f);
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
			}
			base.transform.Find("Viewport/Content").Find("CellAdd").transform.SetAsLastSibling();
		}
	}
}
