using System.Collections;
using System.Collections.Generic;
using Data;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class LightCellScrollControll : MonoBehaviour
{
	private Toggle AllOn;

	private Toggle AllOff;

	private Toggle[] TogArr;

	private Button BtnAdd;

	private Toggle deleteTog;

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

	private List<GameObject> relayObjList = new List<GameObject>();

	private List<LightCellItemModel> ListModel = new List<LightCellItemModel>();

	private List<RelayModel> ListRelayModel;

	private Transform ContentrelayTrans;

	private string Opencode = string.Empty;

	private string Closecode = string.Empty;

	private Dictionary<string, string> RepeattemoRelayname;

	private void Start()
	{
		itemArr = GameResources.manager;
		AllOn = base.transform.Find("TogOnoff/TogOn").GetComponent<Toggle>();
		AllOff = base.transform.Find("TogOnoff/TogOff").GetComponent<Toggle>();
		BtnAdd = base.transform.Find("Manager/BtnAdd").GetComponent<Button>();
		deleteTog = base.transform.Find("Manager/DeleteTog").GetComponent<Toggle>();
		GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
		InitCell();
		init();
		InitCellItemByData();
		EventCenter.addlistener<bool>(Eventdefine.SetCellitemInterable, SetCellitemInterable);
	}

	private void OnEnable()
	{
		ListRelayModel = new List<RelayModel>();
		ContentrelayTrans = UITools.FindTheChildNode(base.transform, "Contentrelay");
		GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
		ListRelayModel = RelayModelControl.GetInstance().GetRelayItemList(base.transform.name.Split('e')[2]);
		RepeattemoRelayname = new Dictionary<string, string>();
		if (ListRelayModel != null)
		{
			for (int i = 0; i < ListRelayModel.Count; i++)
			{
				RepeattemoRelayname.Add(ListRelayModel[i].GetrelayName, ListRelayModel[i].Getrelaykinds);
			}
		}
		if (ListRelayModel != null && ListRelayModel.Count != 0 && ContentrelayTrans.childCount != 0)
		{
			for (int j = 0; j < ListRelayModel.Count; j++)
			{
				ContentrelayTrans.GetChild(j).GetComponent<TcpControl80>().Connect(ListRelayModel[j].GetrelayIp, ListRelayModel[j].GetRelayPort);
				Debug.LogError("灯光继电器:" + ListRelayModel[j].GetrelayName + "建立连接");
			}
		}
	}

	private IEnumerator StarOpenTog(Toggle[] togarr)
	{
		for (int i = 0; i < togarr.Length; i++)
		{
			TogArr[i].GetComponent<ChinarDragSwapImage>().isOn = true;
			TogArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-1");
			yield return new WaitForSeconds(0.8f);
		}
	}

	private IEnumerator StarCloseTog(Toggle[] togarr)
	{
		for (int i = 0; i < togarr.Length; i++)
		{
			TogArr[i].GetComponent<ChinarDragSwapImage>().isOn = false;
			TogArr[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-0");
			yield return new WaitForSeconds(0.8f);
		}
	}

	private void init()
	{
		CellCount = base.transform.Find("Viewport/Content").childCount;
		ContentrelayTrans = UITools.FindTheChildNode(base.transform, "Contentrelay");
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
		deleteTog.onValueChanged.AddListener(delegate(bool ison)
		{
			ControlBool(ison);
			SetTogInter(!ison);
		});
	}

	private void ControlBool(bool ison)
	{
		base.transform.parent.parent.parent.parent.Find("DeieteTing").gameObject.SetActive(ison);
		for (int i = 0; i < CellCount; i++)
		{
			if (base.transform.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.Find("Viewport/Content").GetChild(i).GetChild(0)
					.Find("DeleteBtn")
					.gameObject.SetActive(ison);
			}
		}
		for (int j = 0; j < ContentrelayTrans.childCount; j++)
		{
			ContentrelayTrans.GetChild(j).Find("deleteBtn").gameObject.SetActive(ison);
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
					.GetComponent<ChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	private void InitCellItemByData()
	{
		ListModel.Clear();
		ListModel = LightCellItemControl.GetInstance().GetLightCellItemList(base.transform.name.Split('e')[2]);
		if (ListRelayModel == null)
		{
			return;
		}
		for (int i = 0; i < ListRelayModel.Count; i++)
		{
			InstantiateRelayItem(ListRelayModel[i].GetrelayName, ListRelayModel[i].GetrelayIp, ListRelayModel[i].GetRelayPort, i);
		}
		if (ListModel != null)
		{
			for (int j = 0; j < ListModel.Count; j++)
			{
				int num = j;
				InstantiateCellItem(num, ListModel[num].Get_CellItemName, ListModel[num].Get_ParentName, ListModel[num].Get_DiscribName, ListModel[num].Get_IPadress, ListModel[num].Get_Port, RepeattemoRelayname[ListModel[num].Get_RelayName], ListModel[num].Get_Channel, ListModel[num].Get_IsOpen);
			}
		}
	}

	private void InstantiateRelayItem(string name, string ip, int port, int temp)
	{
		GameObject original = Resources.Load<GameObject>("Prefabs/Togrelay");
		GameObject ItemObj = Object.Instantiate(original);
		ItemObj.transform.SetParent(ContentrelayTrans);
		ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
		ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
		ItemObj.transform.Find("Text").GetComponent<Text>().text = name;
		ItemObj.GetComponent<TcpControl80>().Connect(ip, port);
		ItemObj.transform.Find("deleteBtn").GetComponent<Button>().onClick.AddListener(delegate
		{
			GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
			GameObject panelObj = Object.Instantiate(original2);
			panelObj.transform.SetParent(base.transform);
			panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
			panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
			panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
			{
				GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
				FileUtil.DeleRelayContentbyLine(GameResources.RelayItemPath, "Relay/RelayItem.txt", ip);
				Object.Destroy(ItemObj);
				panelObj.GetComponent<CanvasGroup>().alpha = 0f;
				panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
				ControlBool(ison: false);
				SetTogInter(ison: true);
			});
			panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(delegate
			{
				panelObj.GetComponent<CanvasGroup>().alpha = 0f;
				panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
			});
		});
		ItemObj.transform.GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			AddChannel(base.transform.name.Split('e')[2], ItemObj.transform.GetChild(0).GetComponent<Text>().text);
		});
	}

	public void AddChannel(string index, string temp)
	{
		UIManager.Instance.PushPanel(index, UIPanelType.AddChannel);
		StartCoroutine(LateToCall(temp));
	}

	private IEnumerator LateToCall(string temp)
	{
		yield return new WaitForEndOfFrame();
		EventCenter.BroadCast(Eventdefine.ChangeDropDownKinds, temp);
	}

	private void InstantiateCellItem(int item, string name, string ParentName, string discribname, string ip, string port, string relayname, uint channel, bool isopen)
	{
		Transform parent = base.transform.Find("Viewport/Content").Find(ParentName);
		GameObject original = Resources.Load<GameObject>("Prefabs/LedItem");
		GameObject ItemObj = Object.Instantiate(original);
		ItemObj.transform.SetParent(parent);
		ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
		ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
		ItemObj.name = name;
		ItemObj.transform.Find("Text").GetComponent<Text>().text = discribname;
		relayObjList.Add(ItemObj);
		ItemObj.GetComponent<Toggle>().isOn = isopen;
		if (isopen)
		{
			ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-1");
		}
		else
		{
			ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-0");
		}
		ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
		{
			GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
			Debug.LogError(base.transform.name.Split('e')[2]);
			if (ison)
			{
				ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-1");
				Debug.Log("ItemName:" + ItemObj.name);
				EventCenter.BroadCast(Eventdefine.TcpSendSingle80, FindRelayChannelCoder(relayname, channel).Split(':')[0], GetRelayIp(ListRelayModel, relayname));
				Debug.Log("开灯指令:" + FindRelayChannelCoder(relayname, channel).Split(':')[0]);
				Debug.LogError("这是灯光:" + discribname + "  " + GameResources.LightCellItemPath + "/Light/LightCellItem.txt");
				FileUtil.UpdateLightModelFile(GameResources.LightCellItemPath, "Light/LightCellItem.txt", discribname, ison);
			}
			else
			{
				ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-0");
				Debug.Log("ItemName:" + ItemObj.name);
				EventCenter.BroadCast(Eventdefine.TcpSendSingle80, FindRelayChannelCoder(relayname, channel).Split(':')[1], GetRelayIp(ListRelayModel, relayname));
				Debug.Log("关灯指令:" + FindRelayChannelCoder(relayname, channel).Split(':')[1]);
				FileUtil.UpdateLightModelFile(GameResources.LightCellItemPath, "Light/LightCellItem.txt", discribname, ison);
			}
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
				GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
				FileUtil.DeleContentbyLine(GameResources.LightCellItemPath, "Light/LightCellItem.txt", ItemObj.transform.parent.name);
				Object.Destroy(ItemObj);
				panelObj.GetComponent<CanvasGroup>().alpha = 0f;
				panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
				ControlBool(ison: false);
				SetTogInter(ison: true);
			});
			panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(delegate
			{
				panelObj.GetComponent<CanvasGroup>().alpha = 0f;
				panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
			});
		});
		_ = base.transform.Find("Viewport/Content").childCount;
		base.transform.Find("Viewport/Content").Find("CellAdd").GetComponent<Button>()
			.onClick.AddListener(delegate
		{
			int childCount = base.transform.Find("Viewport/Content").childCount;
			Debug.Log(childCount);
			GameObject original2 = Resources.Load<GameObject>("Prefabs/Cell");
			Debug.Log(childCount);
			GameObject gameObject = Object.Instantiate(original2);
			gameObject.name = ("Cell" + childCount).Split('(')[0];
			gameObject.transform.SetParent(base.transform.Find("Viewport/Content"));
			gameObject.transform.localPosition = new Vector3(1f, 1f, 1f);
			gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
			base.transform.Find("Viewport/Content").Find("CellAdd").transform.SetAsLastSibling();
			GameResources.Instance.RegisterGameDataPath(base.transform.name.Split('e')[2]);
			FileUtil.CoverFile(GameResources.CellPath, "cell.txt", childCount.ToString());
			Debug.LogError(GameResources.CellPath);
		});
	}

	private string GetRelayIp(List<RelayModel> listrelaymodel, string relayname)
	{
		for (int i = 0; i < listrelaymodel.Count; i++)
		{
			if (string.Equals(listrelaymodel[i].Getrelaykinds, relayname))
			{
				return listrelaymodel[i].GetrelayIp;
			}
		}
		return null;
	}

	private void InitCell()
	{
		string text = FileUtil.LoadFile(GameResources.CellPath, "cell.txt");
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

	private string FindRelayChannelCoder(string name, uint channel)
	{
		for (int i = 0; i < itemArr.dataArray.Length; i++)
		{
			int num = i;
			if (string.Equals(itemArr.dataArray[num].relayname, name))
			{
				switch (channel)
				{
				case 1u:
					Opencode = itemArr.dataArray[num].firstOpen;
					Closecode = itemArr.dataArray[num].firstClose;
					return Opencode + ":" + Closecode;
				case 2u:
					Opencode = itemArr.dataArray[num].secondopen;
					Closecode = itemArr.dataArray[num].secondclose;
					return Opencode + ":" + Closecode;
				case 3u:
					Opencode = itemArr.dataArray[num].thirdopen;
					Closecode = itemArr.dataArray[num].thirdclose;
					return Opencode + ":" + Closecode;
				case 4u:
					Opencode = itemArr.dataArray[num].forthopen;
					Closecode = itemArr.dataArray[num].forthclose;
					return Opencode + ":" + Closecode;
				case 5u:
					Opencode = itemArr.dataArray[num].fifthopen;
					Closecode = itemArr.dataArray[num].fifthclose;
					return Opencode + ":" + Closecode;
				case 6u:
					Opencode = itemArr.dataArray[num].sixopen;
					Closecode = itemArr.dataArray[num].sixclose;
					return Opencode + ":" + Closecode;
				case 7u:
					Opencode = itemArr.dataArray[num].seventhopen;
					Closecode = itemArr.dataArray[num].seventhclose;
					return Opencode + ":" + Closecode;
				case 8u:
					Opencode = itemArr.dataArray[num].eightopen;
					Closecode = itemArr.dataArray[num].eightclose;
					return Opencode + ":" + Closecode;
				}
			}
		}
		return null;
	}

	private void SetCellitemInterable(bool ison)
	{
		for (int i = 0; i < base.transform.Find("Viewport/Content").childCount; i++)
		{
			if (base.transform.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<Toggle>()
					.interactable = ison;
			}
		}
	}

	private void OnDisable()
	{
		for (int i = 0; i < ContentrelayTrans.childCount; i++)
		{
			ContentrelayTrans.GetChild(i).GetComponent<TcpControl80>().DisConnect();
			Debug.LogError("灯光继电器断开建立链接");
		}
		EventCenter.RemoveListener<bool>(Eventdefine.SetCellitemInterable, SetCellitemInterable);
	}
}
