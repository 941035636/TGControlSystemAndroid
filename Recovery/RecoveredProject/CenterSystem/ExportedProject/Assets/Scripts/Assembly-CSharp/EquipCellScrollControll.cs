using System.Collections;
using System.Collections.Generic;
using Data;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class EquipCellScrollControll : MonoBehaviour
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

	private List<GameObject> relayObjList = new List<GameObject>();

	private List<LightCellItemModel> ListModel = new List<LightCellItemModel>();

	private List<RelayModel> ListRelayModel;

	private Transform ContentrelayTrans;

	private GameObject Obj;

	private string Opencode = string.Empty;

	private string Closecode = string.Empty;

	private Dictionary<string, string> RepeattemoRelayname;

	private void Start()
	{
		itemArr = GameResources.Equipmanager;
		AllOn = base.transform.Find("TogOnoff/TogOn").GetComponent<Toggle>();
		AllOff = base.transform.Find("TogOnoff/TogOff").GetComponent<Toggle>();
		BtnAdd = base.transform.Find("Manager/BtnAdd").GetComponent<Button>();
		deleteTog = base.transform.Find("Manager/DeleteTog").GetComponent<Toggle>();
		GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
		InitCell();
		init();
		InitCellItemByData();
	}

	private void OnEnable()
	{
		ListRelayModel = new List<RelayModel>();
		ContentrelayTrans = UITools.FindTheChildNode(base.transform, "Contentrelay");
		ListRelayModel.Clear();
		GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
		ListRelayModel = EquipRelayModelControl.GetInstance().GetRelayItemList(base.transform.name.Split('p')[2]);
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
			}
			Debug.LogError("设备继电器重新建立链接");
		}
	}

	private IEnumerator StarOpenTog(Toggle[] togarr)
	{
		Debug.LogError(TogArr.Length);
		Debug.LogError(togarr.Length);
		for (int i = 0; i < togarr.Length; i++)
		{
			togarr[i].GetComponent<EquipChinarDragSwapImage>().isOn = true;
			yield return new WaitForSeconds(0.5f);
		}
	}

	private IEnumerator StarCloseTog(Toggle[] togarr)
	{
		for (int i = 0; i < togarr.Length; i++)
		{
			TogArr[i].GetComponent<EquipChinarDragSwapImage>().isOn = false;
			yield return new WaitForSeconds(0.5f);
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
					.GetComponent<EquipChinarDragSwapImage>()
					.interactable = ison;
			}
		}
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

	private void InitCellItemByData()
	{
		ListModel.Clear();
		ListModel = EquipCellItemControl.GetInstance().GetEquipCellItemList(base.transform.name.Split('p')[2]);
		if (ListModel != null && ListModel.Count == 0 && ListRelayModel.Count == 0)
		{
			return;
		}
		if (ListRelayModel != null && ListRelayModel.Count != 0)
		{
			for (int i = 0; i < ListRelayModel.Count; i++)
			{
				InstantiateRelayItem(ListRelayModel[i].GetrelayName, ListRelayModel[i].GetrelayIp, ListRelayModel[i].GetRelayPort);
			}
		}
		if (ListModel == null || ListModel.Count == 0)
		{
			return;
		}
		foreach (LightCellItemModel item in ListModel)
		{
			if (item.Get_RelayName == "")
			{
				Debug.LogError("这是软件");
				InstantiateCellItem(item.Get_CellItemName, item.Get_ParentName, item.Get_DiscribName, item.Get_IPadress, item.Get_Port, item.Get_RelayName, item.Get_Channel, item.Get_IsOpen, item.Get_Coder);
			}
			else
			{
				InstantiateCellItem(item.Get_CellItemName, item.Get_ParentName, item.Get_DiscribName, item.Get_IPadress, item.Get_Port, RepeattemoRelayname[item.Get_RelayName], item.Get_Channel, item.Get_IsOpen, item.Get_Coder);
			}
		}
	}

	private void InstantiateRelayItem(string name, string ip, int port)
	{
		GameObject original = Resources.Load<GameObject>("Prefabs/Togrelay");
		GameObject ItemObj = Object.Instantiate(original);
		ItemObj.transform.SetParent(ContentrelayTrans);
		ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
		ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
		ItemObj.transform.Find("Text").GetComponent<Text>().text = name;
		ItemObj.GetComponent<TcpControl80>().Connect(ip, port);
		Debug.LogError("IP:" + ip + "   port:" + port);
		ItemObj.transform.GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			AddChannel(base.transform.name.Split('p')[2], ItemObj.transform.GetChild(0).GetComponent<Text>().text);
			Debug.LogError("点击了控制器：" + ItemObj.transform.GetChild(0).GetComponent<Text>().text);
		});
		ItemObj.transform.Find("deleteBtn").GetComponent<Button>().onClick.AddListener(delegate
		{
			GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
			GameObject panelObj = Object.Instantiate(original2);
			panelObj.transform.SetParent(base.transform);
			panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
			panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
			panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
			{
				GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
				FileUtil.DeleRelayContentbyLine(GameResources.EquipRelayItemPath, "Relay/RelayItem.txt", ip);
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
	}

	private void InstantiateCellItem(string name, string ParentName, string discribname, string ip, string port, string relayname, uint channel, bool isopen, string Coder)
	{
		Debug.LogError("IP:" + ip);
		Transform parent = base.transform.Find("Viewport/Content").Find(ParentName);
		GameObject original = Resources.Load<GameObject>("Prefabs/EquipItem");
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
			if (relayname.Contains("红外"))
			{
				ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-红外-1");
			}
			else if (relayname == "")
			{
				ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-1");
			}
			else
			{
				ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-1");
			}
		}
		else if (relayname.Contains("红外"))
		{
			ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-红外-0");
		}
		else if (relayname == "")
		{
			ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-0");
		}
		else
		{
			ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-0");
		}
		ItemObj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
		{
			Debug.LogError("ItemName:" + ItemObj.name);
			GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
			if (ison)
			{
				if (relayname == "")
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-1");
					Debug.LogError("这个设备是软件控制开机的");
				}
				else
				{
					Debug.LogError("relayname" + relayname);
					if (relayname.Contains("红外"))
					{
						ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-红外-1");
						EventCenter.BroadCast(Eventdefine.TcpSendSingle80, Coder, GetRelayIp(ListRelayModel, relayname));
						Debug.LogError("红外指令:" + Coder);
					}
					else if (relayname.Contains("串口"))
					{
						ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-1");
						if (ItemObj.GetComponent<UDPClient>() == null)
						{
							ItemObj.AddComponent<UDPClient>();
						}
					}
					else
					{
						ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-1");
						Debug.LogError("OpenCode:" + FindRelayChannelCoder(relayname, channel).Split(':')[0]);
						EventCenter.BroadCast(Eventdefine.TcpSend80, FindRelayChannelCoder(relayname, channel).Split(':')[0], FindRelayChannelCoder(relayname, channel).Split(':')[1], GetRelayIp(ListRelayModel, relayname));
					}
				}
				Debug.LogError("这个设备是:" + discribname);
				FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", discribname, ison);
			}
			else
			{
				if (relayname == "")
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-0");
					if (ItemObj.GetComponent<UDPClient>() == null)
					{
						ItemObj.AddComponent<UDPClient>();
					}
					EventCenter.BroadCast(Eventdefine.InitSocket, ip);
					EventCenter.BroadCast(Eventdefine.Shutdown);
					Debug.LogError("这个设备是软件控制关机的:" + name + "|" + ip + "|关机");
				}
				else if (relayname.Contains("串口"))
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-0");
					if (ItemObj.GetComponent<UDPClient>() == null)
					{
						ItemObj.AddComponent<UDPClient>();
					}
					EventCenter.BroadCast(Eventdefine.InitSocket, ip);
					EventCenter.BroadCast(Eventdefine.Shutdown);
					Debug.LogError("这个设备是串口控制关机的:" + name + "|" + ip + "|关机");
				}
				else
				{
					Debug.LogError("relayname" + relayname);
					if (relayname.Contains("红外"))
					{
						ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-红外-0");
						EventCenter.BroadCast(Eventdefine.TcpSendSingle80, Coder, GetRelayIp(ListRelayModel, relayname));
						Debug.LogError("红外指令:" + Coder);
					}
					else
					{
						Debug.LogError("CloseCoder：" + FindRelayChannelCoder(relayname, channel).Split(':')[1]);
						ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-继电器-0");
						EventCenter.BroadCast(Eventdefine.TcpSend80, FindRelayChannelCoder(relayname, channel).Split(':')[1], FindRelayChannelCoder(relayname, channel).Split(':')[1], GetRelayIp(ListRelayModel, relayname));
					}
				}
				FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", discribname, ison);
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
				Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
				GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
				FileUtil.DeleContentbyLine(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", ItemObj.transform.parent.name);
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
			GameResources.Instance.RegisterEquipGameDataPath(base.transform.name.Split('p')[2]);
			FileUtil.CoverFile(GameResources.EquipCellPath, "cell.txt", childCount.ToString());
			Debug.LogError(GameResources.EquipCellPath);
		});
	}

	private IEnumerator StartOpenComputer(Toggle toggle)
	{
		yield return new WaitForSeconds(0.3f);
		toggle.isOn = false;
		Debug.LogError("断开");
	}

	public void AddChannel(string index, string temp)
	{
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddEquipChannelPanel);
		StartCoroutine(LateToCall(temp));
	}

	private IEnumerator LateToCall(string temp)
	{
		yield return new WaitForEndOfFrame();
		EventCenter.BroadCast(Eventdefine.ChangeEquipDropDownKinds, temp);
	}

	private void SaveCellItemData(bool isTV, bool isPhoto, bool isVideo, string path, string ip, string port, string strname, string strnote)
	{
		Debug.LogError("保存成功" + base.transform.name.Split('p')[2]);
		GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.name.Split('p')[2]);
		string get_ParentName = "Cell" + FileUtil.SearchNearEmptyParentName(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt");
		SoftWareModel softWareModel = new SoftWareModel();
		softWareModel.GetPath = path;
		softWareModel.Get_ParentName = get_ParentName;
		softWareModel.GetPort = port;
		softWareModel.GetIp = ip;
		softWareModel.GetName = strname;
		softWareModel.GetNote = strnote;
		softWareModel.GetIsVideo = isVideo;
		softWareModel.GetIsSheBei = isTV;
		softWareModel.GetIsPhoto = isPhoto;
		SoftWareModelControl.GetInstance().AddSoftWareItem(softWareModel);
	}

	private IEnumerator SendToCenterControlServer(string discribname)
	{
		yield return new WaitForSeconds(1f);
		NetManager.Send(new ResponseMsgControl
		{
			Code = discribname
		});
	}

	private void InitCell()
	{
		string text = FileUtil.LoadFile(GameResources.EquipCellPath, "cell.txt");
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

	private void OnDisable()
	{
		for (int i = 0; i < ContentrelayTrans.childCount; i++)
		{
			ContentrelayTrans.GetChild(i).GetComponent<TcpControl80>().DisConnect();
			Debug.LogError("设备继电器已离线");
		}
	}
}
