using System.Collections.Generic;
using DG.Tweening;
using Data;
using UnityEngine;
using UnityEngine.UI;

public class AddChannel : BasePanel
{
	private Dropdown RelayChannel;

	private Text relaynametxt;

	private InputField Relayname;

	private Button ConfirmBtn;

	private string RelayKindsname;

	private string RelayChannelname;

	private string relayname;

	private Text tiptxt;

	private Dictionary<string, string> RepeattemoRelayname;

	private List<string> RemoveStringList = new List<string>();

	private CanvasGroup canvasGroup;

	private List<int> Removeusedchannel = new List<int>();

	private Dictionary<string, int> RelayChannelDic;

	private int CellCount;

	private Transform transPar;

	private ItemManager itemArr;

	private string Opencodelist = string.Empty;

	private string Closecodelist = string.Empty;

	private List<LightCellItemModel> ListControCellModel;

	private void Awake()
	{
		EventCenter.addlistener<string>(Eventdefine.ChangeDropDownKinds, ChangeRelayDropKinds);
		RelayChannel = base.transform.Find("relayChannel/Dropdown").GetComponent<Dropdown>();
		Relayname = base.transform.Find("name/InputIP").GetComponent<InputField>();
		ConfirmBtn = base.transform.Find("BtnAdd").GetComponent<Button>();
		tiptxt = base.transform.Find("tiptxt").GetComponent<Text>();
		relaynametxt = base.transform.Find("relaykinds/relayname").GetComponent<Text>();
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
			if (!RelayChannel.options[0].text.Contains("无"))
			{
				InistateCellItem(RelayKindsname, int.Parse(RelayChannelname), relayname);
				Removeusedchannel.Remove(int.Parse(RelayChannelname));
				for (int i = 0; i < Removeusedchannel.Count; i++)
				{
					Debug.Log(Removeusedchannel[i]);
				}
				OnClosePanel();
			}
			else
			{
				Debug.LogError("通道全部占用");
			}
		});
		init();
	}

	private void init()
	{
		GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('e')[2]);
		List<RelayModel> relayItemList = RelayModelControl.GetInstance().GetRelayItemList(base.transform.parent.name.Split('e')[2]);
		ListControCellModel = LightCellItemControl.GetInstance().GetLightCellItemList(base.transform.parent.name.Split('e')[2]);
		Debug.LogError("读取本地灯光文件");
		itemArr = GameResources.manager;
		RepeattemoRelayname = new Dictionary<string, string>();
		RelayChannelDic = new Dictionary<string, int>();
		if (relayItemList != null)
		{
			for (int i = 0; i < relayItemList.Count; i++)
			{
				RepeattemoRelayname.Add(relayItemList[i].GetrelayName, relayItemList[i].Getrelaykinds);
				RelayChannelDic.Add(relayItemList[i].GetrelayName, relayItemList[i].GetAllChannelNum);
			}
		}
		else
		{
			TipShow("请先行添加控制器!", tiptxt);
		}
	}

	private void ChangeRelayDropKinds(string relayname)
	{
		relaynametxt.text = relayname;
		RelayKindsname = relayname;
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
		UpdateRelayChannelDropDownItem(list);
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
		GameObject original = Resources.Load<GameObject>("Prefabs/LedItem");
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
			SaveCellItemData(child.name, ItemObj.name, Relaykindsname, relayChannel.ToString(), relayname, isopen: false);
			string orderopen = Opencodelist;
			string orderclose = Closecodelist;
			ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
			{
				if (ison)
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-LED-1");
					EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderopen, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
					Debug.Log(orderopen);
					if (!InputName.Contains("投影仪"))
					{
						if (InputName.Contains("电视机") || InputName.Contains("电视"))
						{
							NetManager.Send(new ResponseMsgControl
							{
								Code = InputName + "|打开电视"
							});
						}
						else
						{
							InputName.Contains("滑轨屏");
						}
					}
					FileUtil.UpdateLightModelFile(GameResources.LightCellItemPath, "Light/LightCellItem.txt", InputName, ison);
				}
				else
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-LED-0");
					EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderclose, GetRelayIp(RepeattemoRelayname[RelayKindsname]));
					Debug.Log(orderclose);
					FileUtil.UpdateLightModelFile(GameResources.LightCellItemPath, "Light/LightCellItem.txt", InputName, ison);
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
					GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('e')[2]);
					FileUtil.DeleContentbyLine(GameResources.LightCellItemPath, "Light/LightCellItem.txt", ItemObj.transform.parent.name);
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

	private void SaveCellItemData(string parentName, string name, string relayname, string channel, string dscrisname, bool isopen)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		lightCellItemModel.Get_CellItemName = name;
		lightCellItemModel.Get_ParentName = parentName;
		lightCellItemModel.Get_RelayName = relayname;
		lightCellItemModel.Get_Channel = uint.Parse(channel);
		lightCellItemModel.Get_DiscribName = dscrisname;
		lightCellItemModel.Get_IsOpen = isopen;
		LightCellItemControl.GetInstance().AddLightItem(base.transform.parent.name.Split('e')[2], lightCellItemModel);
	}

	private void UpdateRelayChannelDropDownItem(List<int> showNames)
	{
		RelayChannel.options.Clear();
		for (int i = 0; i < showNames.Count; i++)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			optionData.text = showNames[i].ToString();
			RelayChannel.options.Add(optionData);
		}
		if (showNames.Count != 0)
		{
			RelayChannel.captionText.text = showNames[0].ToString();
			RelayChannelname = RelayChannel.options[0].text;
		}
		else
		{
			Debug.LogError("该控制器通道已经被全部占用!");
			RelayChannel.captionText.text = "已经无可用通道!";
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

	private void TipShow(string msg, Text textTip)
	{
		textTip.text = msg;
		textTip.DOFade(1f, 1f).OnComplete(delegate
		{
			textTip.DOFade(0f, 1f).OnComplete(delegate
			{
			});
		});
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
		UIManager.Instance.PopPanel();
	}

	private void OnDisable()
	{
		EventCenter.RemoveListener<string>(Eventdefine.ChangeDropDownKinds, ChangeRelayDropKinds);
	}
}
