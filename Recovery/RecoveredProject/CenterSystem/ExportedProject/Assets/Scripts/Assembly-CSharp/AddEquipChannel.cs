using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using DG.Tweening;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class AddEquipChannel : BasePanel
{
	private Text relaykindsnametxt;

	private Text relaynametxt;

	private Dropdown RelayChannel;

	private InputField Relayname;

	private Button ConfirmBtn;

	private Button ChoiceCoderBtn;

	private Text CoderTxt;

	private Text tipText;

	private string RelayKindsname;

	private string RelayChannelname;

	private string relayname;

	private Dictionary<string, string> RepeattemoRelayname;

	private List<int> Removeusedchannel = new List<int>();

	private Dictionary<string, int> RelayChannelDic;

	private List<LightCellItemModel> ListControCellModel;

	private List<string> RemoveStringList = new List<string>();

	private CanvasGroup canvasGroup;

	private int CellCount;

	private Transform transPar;

	private ItemManager itemArr;

	private string Opencodelist = string.Empty;

	private string Closecodelist = string.Empty;

	private string RedCoder;

	private List<RelayModel> ListRelayModel;

	private int temp;

	private void Awake()
	{
		EventCenter.addlistener<string>(Eventdefine.ChangeEquipDropDownKinds, ChangeRelayDropKinds);
		RelayChannel = base.transform.Find("relayChannel/Dropdown").GetComponent<Dropdown>();
		Relayname = base.transform.Find("name/InputIP").GetComponent<InputField>();
		ConfirmBtn = base.transform.Find("BtnAdd").GetComponent<Button>();
		tipText = base.transform.Find("Tiptxt").GetComponent<Text>();
		relaykindsnametxt = base.transform.Find("relaynameTxt").GetComponent<Text>();
		ChoiceCoderBtn = base.transform.Find("ChoiceCoderBtn").GetComponent<Button>();
		CoderTxt = base.transform.Find("ChoiceCoderBtn/CoderTxt").GetComponent<Text>();
		relaynametxt = base.transform.Find("relayn/relayname").GetComponent<Text>();
	}

	private void Start()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		transPar = UITools.FindTheChildNode(base.transform.parent, "Viewport/Content");
		CellCount = transPar.childCount;
		RelayChannel.onValueChanged.AddListener(delegate(int value)
		{
			RelayChannelname = RelayChannel.options[value].text;
			Debug.LogError("通道索引:" + value + "通道名字:" + RelayChannelname);
		});
		Relayname.onEndEdit.AddListener(delegate(string str)
		{
			relayname = str;
		});
		ConfirmBtn.onClick.AddListener(delegate
		{
			GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.parent.name.Split('p')[2]);
			if (!RelayChannel.options[0].text.Contains("无") && !RelayChannel.options[0].text.Contains("COM"))
			{
				InistateCellItem(RelayKindsname, int.Parse(RelayChannel.options[0].text), relayname);
				Removeusedchannel.Remove(int.Parse(RelayChannel.options[0].text));
				for (int i = 0; i < Removeusedchannel.Count; i++)
				{
					Debug.Log(Removeusedchannel[i]);
				}
				OnClosePanel();
			}
			else if (RelayChannel.options[0].text.Contains("COM"))
			{
				InistateCOMCellItem(RelayKindsname, RelayChannel.options[0].text, relayname, GetComrelayIp(RelayKindsname));
				OnClosePanel();
			}
			else
			{
				Debug.LogError("    通道全部占用");
			}
		});
		ChoiceCoderBtn.onClick.AddListener(delegate
		{
			FileInfo[] filearr = new DirectoryInfo(Application.persistentDataPath + "/红外模块指令").GetFiles();
			base.transform.Find("ChoiceCoderPanel").gameObject.SetActive(value: true);
			for (int i = 0; i < filearr.Length; i++)
			{
				int temp = i;
				GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/itemCoder"), base.transform.Find("ChoiceCoderPanel/Viewport/Content"));
				string text = filearr[i].Name;
				string codername = text;
				obj.name = codername;
				obj.transform.Find("Text").GetComponent<Text>().text = codername;
				obj.transform.localScale = new Vector3(1f, 1f, 1f);
				obj.GetComponent<Button>().onClick.AddListener(delegate
				{
					base.transform.Find("ChoiceCoderPanel").gameObject.SetActive(value: false);
					CoderTxt.text = codername.Split('.')[0];
					Debug.Log(filearr[temp].FullName);
					RedCoder = File.ReadAllText(filearr[temp].FullName, Encoding.UTF8);
					Debug.LogError("红外指令:" + RedCoder);
				});
			}
		});
		init();
	}

	private string GetComrelayIp(string relaykindsname)
	{
		for (int i = 0; i < ListRelayModel.Count; i++)
		{
			if (ListRelayModel[i].Getrelaykinds == relaykindsname)
			{
				return ListRelayModel[i].GetrelayIp;
			}
		}
		return null;
	}

	private void init()
	{
		GameResources.Instance.RegisterEquipGameDataPath(base.transform.parent.name.Split('p')[2]);
		ListRelayModel = EquipRelayModelControl.GetInstance().GetRelayItemList(base.transform.parent.name.Split('p')[2]);
		ListControCellModel = EquipCellItemControl.GetInstance().GetEquipCellItemList(base.transform.parent.name.Split('p')[2]);
		itemArr = GameResources.Equipmanager;
		RepeattemoRelayname = new Dictionary<string, string>();
		RelayChannelDic = new Dictionary<string, int>();
		if (ListRelayModel != null)
		{
			for (int i = 0; i < ListRelayModel.Count; i++)
			{
				RepeattemoRelayname.Add(ListRelayModel[i].GetrelayName, ListRelayModel[i].Getrelaykinds);
				RelayChannelDic.Add(ListRelayModel[i].GetrelayName, ListRelayModel[i].GetAllChannelNum);
			}
		}
		else
		{
			TipShow("请先行添加继电器!", tipText);
		}
	}

	private void ChangeRelayDropKinds(string relayname)
	{
		relaykindsnametxt.text = RepeattemoRelayname[relayname];
		relaynametxt.text = relayname;
		RelayKindsname = relayname;
		Debug.LogError("自定义控制器名称：" + relayname + "   控制器名称:" + RepeattemoRelayname[relayname]);
		if (RepeattemoRelayname[relayname].Contains("学习"))
		{
			Debug.LogError("这是学习型控制器");
			ChoiceCoderBtn.gameObject.SetActive(value: true);
		}
		else
		{
			Debug.LogError("这是非学习型控制器");
			ChoiceCoderBtn.gameObject.SetActive(value: false);
		}
		int value = 0;
		List<int> list = new List<int>();
		List<uint> list2 = new List<uint>();
		if (ListControCellModel != null)
		{
			for (int i = 0; i < ListControCellModel.Count; i++)
			{
				if (ListControCellModel[i].Get_RelayName == relayname)
				{
					list2.Add(ListControCellModel[i].Get_Channel);
				}
			}
		}
		if (RelayChannelDic.TryGetValue(relayname, out value))
		{
			for (int j = 1; j < value + 1; j++)
			{
				list.Add(j);
			}
			for (int k = 0; k < list2.Count; k++)
			{
				list.Remove((int)list2[k]);
			}
		}
		if (RepeattemoRelayname[relayname].Contains("串口"))
		{
			UpdateRelayChannelDropDownItem(list, isCom: true);
		}
		else
		{
			UpdateRelayChannelDropDownItem(list);
		}
	}

	private string GetRelayIp(string relayname)
	{
		for (int i = 0; i < itemArr.dataArray.Length; i++)
		{
			if (string.Equals(itemArr.dataArray[i].relayname, relayname))
			{
				return itemArr.dataArray[i].ip;
			}
		}
		return null;
	}

	private void InistateCellItem(string Relaykindsname, int relayChannel, string InputName)
	{
		GameObject original = Resources.Load<GameObject>("Prefabs/EquipItem");
		for (int i = 0; i < CellCount; i++)
		{
			if (transPar.GetChild(i).childCount != 0)
			{
				continue;
			}
			Transform child = transPar.GetChild(i);
			GameObject ItemObj = Object.Instantiate(original);
			ItemObj.transform.SetParent(child);
			ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
			ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
			ItemObj.name = child.name.Split('l')[2];
			ItemObj.transform.Find("Text").GetComponent<Text>().text = InputName;
			FindRelayChannelCoder(RepeattemoRelayname[Relaykindsname], uint.Parse(relayChannel.ToString()));
			SaveCellItemData(child.name, ItemObj.name, Relaykindsname, relayChannel.ToString(), relayname);
			string orderopen = Opencodelist;
			string orderclose = Closecodelist;
			if (!InputName.Contains("投影仪") && !InputName.Contains("电视") && !InputName.Contains("滑轨屏") && !InputName.Contains("拍照") && InputName.Contains("红外"))
			{
				ItemObj.AddComponent<TcpControl80>();
			}
			ItemObj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
			{
				if (ison)
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-电脑-1");
					Debug.Log("姑苏城外寒山寺，夜半钟声到客船");
					Debug.LogError(GetRelayIp(RepeattemoRelayname[RelayKindsname]));
					if (Relaykindsname.Contains("红外"))
					{
						EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderopen, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
						Debug.LogError("红外1指令:" + orderopen);
					}
					else
					{
						EventCenter.BroadCast(Eventdefine.TcpSend80, orderopen, orderclose, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
					}
					Debug.Log(orderopen);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", InputName, ison);
				}
				else
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-电脑-0");
					if (Relaykindsname.Contains("红外"))
					{
						EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderopen, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
						Debug.LogError("红外1指令:" + orderopen);
					}
					else
					{
						EventCenter.BroadCast(Eventdefine.TcpSend80, orderopen, orderclose, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
					}
					Debug.Log(orderclose);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", InputName, ison);
				}
			});
			ItemObj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
				GameObject panelObj = Object.Instantiate(original2);
				panelObj.transform.SetParent(base.transform.parent);
				panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
				panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
				panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
				{
					GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('p')[2]);
					Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", ItemObj.transform.parent.name);
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
			break;
		}
	}

	private void InistateCOMCellItem(string Relaykindsname, string Com, string InputName, string ip)
	{
		Debug.LogError("串口控制器IP：" + ip);
		GameObject original = Resources.Load<GameObject>("Prefabs/EquipItem");
		for (int i = 0; i < CellCount; i++)
		{
			if (transPar.GetChild(i).childCount != 0)
			{
				continue;
			}
			Transform child = transPar.GetChild(i);
			GameObject ItemObj = Object.Instantiate(original);
			ItemObj.transform.SetParent(child);
			ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
			ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
			ItemObj.name = child.name.Split('l')[2];
			ItemObj.transform.Find("Text").GetComponent<Text>().text = InputName;
			SaveComCellItemData(child.name, ItemObj.name, Relaykindsname, Com, relayname, ip);
			ItemObj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
			{
				if (ison)
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-电脑-1");
					ItemObj.AddComponent<UDPClient>();
					EventCenter.BroadCast(Eventdefine.InitSocket, ip);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", InputName, ison);
				}
				else
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-电脑-0");
					ItemObj.AddComponent<UDPClient>();
					EventCenter.BroadCast(Eventdefine.InitSocket, ip);
					EventCenter.BroadCast(Eventdefine.Shutdown);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", InputName, ison);
				}
			});
			ItemObj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
			{
				GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
				GameObject panelObj = Object.Instantiate(original2);
				panelObj.transform.SetParent(base.transform.parent);
				panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
				panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
				panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
				{
					GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('p')[2]);
					Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", ItemObj.transform.parent.name);
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
			break;
		}
	}

	private void SaveCellItemData(bool isTV, bool isPhoto, bool isVideo, string path, string ip, string port, string strname, string strnote, string tingname)
	{
		Debug.LogError("保存成功" + base.transform.parent.name.Split('p')[2]);
		temp = FileUtil.SearchNearEmptyParentName(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt");
		string get_ParentName = "Cell" + temp;
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
		softWareModel.Get_TingName = tingname;
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

	private IEnumerator SearchNearEmptyParent()
	{
		yield return new WaitForEndOfFrame();
	}

	private void SaveCellItemData(string parentName, string name, string relayname, string channel, string dscrisname)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		lightCellItemModel.Get_CellItemName = name;
		lightCellItemModel.Get_ParentName = parentName;
		lightCellItemModel.Get_RelayName = relayname;
		lightCellItemModel.Get_Channel = uint.Parse(channel);
		lightCellItemModel.Get_DiscribName = dscrisname;
		EquipCellItemControl.GetInstance().AddEquipItem(base.transform.parent.name.Split('p')[2], lightCellItemModel);
	}

	private void SaveComCellItemData(string parentName, string name, string relayname, string channel, string dscrisname, string ip)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		lightCellItemModel.Get_CellItemName = name;
		lightCellItemModel.Get_ParentName = parentName;
		lightCellItemModel.Get_IPadress = ip;
		lightCellItemModel.Get_RelayName = relayname;
		lightCellItemModel.Get_Channel = uint.Parse(channel.Split('M')[1]);
		lightCellItemModel.Get_DiscribName = dscrisname;
		EquipCellItemControl.GetInstance().AddEquipItem(base.transform.parent.name.Split('p')[2], lightCellItemModel);
	}

	private void UpdateRelayChannelDropDownItem(List<int> showNames, bool isCom = false)
	{
		RelayChannel.options.Clear();
		for (int i = 0; i < showNames.Count; i++)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			if (!isCom)
			{
				optionData.text = showNames[i].ToString();
			}
			else
			{
				optionData.text = "COM" + showNames[i];
			}
			RelayChannel.options.Add(optionData);
		}
		if (showNames.Count != 0)
		{
			if (!isCom)
			{
				RelayChannel.captionText.text = showNames[0].ToString();
			}
			else
			{
				RelayChannel.captionText.text = "COM" + showNames[0];
			}
			RelayChannelname = RelayChannel.options[0].text;
		}
		else
		{
			RelayChannel.captionText.text = "已经无可用通道";
			Debug.LogError("该控制器通道已经被全部占用");
		}
	}

	private void FindRelayChannelCoder(string name, uint channel)
	{
		for (int i = 0; i < itemArr.dataArray.Length; i++)
		{
			int num = i;
			if (string.Equals(itemArr.dataArray[num].relayname, name))
			{
				switch (channel)
				{
				case 1u:
					Opencodelist = itemArr.dataArray[num].firstOpen;
					Closecodelist = itemArr.dataArray[num].firstClose;
					break;
				case 2u:
					Opencodelist = itemArr.dataArray[num].secondopen;
					Closecodelist = itemArr.dataArray[num].secondclose;
					break;
				case 3u:
					Opencodelist = itemArr.dataArray[num].thirdopen;
					Closecodelist = itemArr.dataArray[num].thirdclose;
					break;
				case 4u:
					Opencodelist = itemArr.dataArray[num].forthopen;
					Closecodelist = itemArr.dataArray[num].forthclose;
					break;
				case 5u:
					Opencodelist = itemArr.dataArray[num].fifthopen;
					Closecodelist = itemArr.dataArray[num].fifthclose;
					break;
				case 6u:
					Opencodelist = itemArr.dataArray[num].sixopen;
					Closecodelist = itemArr.dataArray[num].sixclose;
					break;
				case 7u:
					Opencodelist = itemArr.dataArray[num].seventhopen;
					Closecodelist = itemArr.dataArray[num].seventhclose;
					break;
				case 8u:
					Opencodelist = itemArr.dataArray[num].eightopen;
					Closecodelist = itemArr.dataArray[num].eightclose;
					break;
				}
			}
		}
	}

	public override void OnEnter()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		canvasGroup.alpha = 1f;
		canvasGroup.blocksRaycasts = true;
		base.transform.localScale = Vector3.zero;
		base.transform.DOScale(1f, 0.5f);
		if (Removeusedchannel != null && Removeusedchannel.Count != 0)
		{
			for (int i = 0; i < Removeusedchannel.Count; i++)
			{
				RemoveStringList.Add(Removeusedchannel[i].ToString());
			}
			RelayChannel.ClearOptions();
			RelayChannel.AddOptions(RemoveStringList);
			RelayChannelname = RemoveStringList[0];
		}
		RemoveStringList.Clear();
	}

	public override void OnExit()
	{
		canvasGroup.blocksRaycasts = false;
		canvasGroup.DOFade(0f, 0.5f);
		Object.Destroy(base.gameObject);
	}

	public void OnClosePanel()
	{
		UIManagerEquip.Instance.PopPanel();
	}

	private void TipShow(string msg, Text textTip)
	{
		textTip.text = msg;
		textTip.DOFade(1f, 1f).OnComplete(delegate
		{
			textTip.DOFade(0f, 5f).OnComplete(delegate
			{
			});
		});
	}

	private void OnDisable()
	{
		EventCenter.RemoveListener<string>(Eventdefine.ChangeEquipDropDownKinds, ChangeRelayDropKinds);
	}
}
