using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Data;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddrelayPanel : BasePanel
{
	private Dropdown relaykinds;

	private InputField ReNameInput;

	private InputField relayIp;

	private InputField relayPort;

	private Button BtnAdd;

	private CanvasGroup canvasGroup;

	private List<string> temoNames;

	private string Ip;

	private string rename;

	private int port;

	private List<RelayModel> ListModel;

	private ItemManager manager;

	private string index = string.Empty;

	private Transform ContentrelayTrans;

	private string Opencodelist = string.Empty;

	private string Closecodelist = string.Empty;

	private Transform transPar;

	private int CellCount;

	private string RelayChannelname;

	private List<string> RepeattemoRelayname;

	private List<int> Removeusedchannel = new List<int>();

	private Dictionary<string, int> RelayChannelDic;

	private string RelayKindsname;

	private List<int> ChannelCountList1;

	private List<int> ChannelCountList2;

	private List<int> ChannelCountList3;

	private List<int> ChannelCountList4;

	private List<int> ChannelCountList5;

	private List<int> ChannelCountList6;

	private List<int> ChannelCountList7;

	private List<int> ChannelCountList8;

	private List<int> ChannelCountList9;

	private List<int> ChannelCountList10;

	private List<int> ChannelCountList11;

	private List<int> ChannelCountList12;

	private Dictionary<string, List<int>> temoChannelDic1;

	private Dictionary<string, List<int>> temoChannelDic2;

	private Dictionary<string, List<int>> temoChannelDic3;

	private Dictionary<string, List<int>> temoChannelDic4;

	private Dictionary<string, List<int>> temoChannelDic5;

	private Dictionary<string, List<int>> temoChannelDic6;

	private Dictionary<string, List<int>> temoChannelDic7;

	private Dictionary<string, List<int>> temoChannelDic8;

	private Dictionary<string, List<int>> temoChannelDic9;

	private Dictionary<string, List<int>> temoChannelDic10;

	private Dictionary<string, List<int>> temoChannelDic11;

	private Dictionary<string, List<int>> temoChannelDic12;

	private List<LightCellItemModel> ListControCellModel;

	private void Awake()
	{
		relaykinds = base.transform.Find("relaykinds/Dropdown").GetComponent<Dropdown>();
		relayIp = base.transform.Find("relayIp/InputIP").GetComponent<InputField>();
		relayPort = base.transform.Find("relayPort/InputIP").GetComponent<InputField>();
		ReNameInput = base.transform.Find("Name").GetComponent<InputField>();
		BtnAdd = base.transform.Find("BtnAdd").GetComponent<Button>();
	}

	private void Start()
	{
		SetTogInter(ison: false);
		ContentrelayTrans = base.transform.parent.Find("Manager/relayScroll/Viewport/Contentrelay");
		manager = GameResources.manager;
		index = base.transform.parent.name.Split('e')[2];
		Init();
		GameResources.Instance.RegisterGameDataPath(index);
		Debug.LogError(index);
		ListModel = RelayModelControl.GetInstance().GetRelayItemList(index);
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		ItemManager itemArr = GameResources.manager;
		temoNames = new List<string>();
		for (int i = 0; i < itemArr.dataArray.Length; i++)
		{
			temoNames.Add(itemArr.dataArray[i].relayname);
		}
		UpdateDropDownItem(temoNames);
		relayIp.onEndEdit.AddListener(delegate(string text)
		{
			Ip = text;
		});
		ReNameInput.onEndEdit.AddListener(delegate(string text)
		{
			rename = text;
		});
		relaykinds.onValueChanged.AddListener(delegate(int temp)
		{
			for (int j = 0; j < manager.dataArray.Length; j++)
			{
				if (manager.dataArray[j].relayname == relaykinds.options[temp].text)
				{
					port = (int)manager.dataArray[j].port;
				}
			}
			Debug.LogError("RepeatNAme:" + RepeattemoRelayname.Count);
			for (int k = 0; k < RepeattemoRelayname.Count; k++)
			{
				if (RepeattemoRelayname[k] == relaykinds.options[temp].text)
				{
					Removeusedchannel.Clear();
					if (temoChannelDic1.ContainsKey(relaykinds.options[temp].text))
					{
						int value = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList1.Count);
							for (int l = 1; l < value + 1; l++)
							{
								Removeusedchannel.Add(l);
							}
							for (int m = 0; m < ChannelCountList1.Count; m++)
							{
								Removeusedchannel.Remove(ChannelCountList1[m]);
							}
						}
					}
					if (temoChannelDic2.ContainsKey(relaykinds.options[temp].text))
					{
						int value2 = 0;
						if (temoChannelDic2.Keys.First() != null && RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value2))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList2.Count);
							for (int n = 1; n < value2 + 1; n++)
							{
								Removeusedchannel.Add(n);
							}
							for (int num = 0; num < ChannelCountList2.Count; num++)
							{
								Removeusedchannel.Remove(ChannelCountList2[num]);
							}
						}
					}
					if (temoChannelDic3.ContainsKey(relaykinds.options[temp].text))
					{
						int value3 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value3))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList3.Count);
							for (int num2 = 1; num2 < value3 + 1; num2++)
							{
								Removeusedchannel.Add(num2);
							}
							for (int num3 = 0; num3 < ChannelCountList3.Count; num3++)
							{
								Removeusedchannel.Remove(ChannelCountList3[num3]);
							}
						}
					}
					if (temoChannelDic4.ContainsKey(relaykinds.options[temp].text))
					{
						int value4 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value4))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList4.Count);
							for (int num4 = 1; num4 < value4 + 1; num4++)
							{
								Removeusedchannel.Add(num4);
							}
							for (int num5 = 0; num5 < ChannelCountList4.Count; num5++)
							{
								Removeusedchannel.Remove(ChannelCountList4[num5]);
							}
						}
					}
					if (temoChannelDic5.ContainsKey(relaykinds.options[temp].text))
					{
						int value5 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value5))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList5.Count);
							for (int num6 = 1; num6 < value5 + 1; num6++)
							{
								Removeusedchannel.Add(num6);
							}
							for (int num7 = 0; num7 < ChannelCountList5.Count; num7++)
							{
								Removeusedchannel.Remove(ChannelCountList5[num7]);
							}
						}
					}
					if (temoChannelDic6.ContainsKey(relaykinds.options[temp].text))
					{
						int value6 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value6))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList6.Count);
							for (int num8 = 1; num8 < value6 + 1; num8++)
							{
								Removeusedchannel.Add(num8);
							}
							for (int num9 = 0; num9 < ChannelCountList6.Count; num9++)
							{
								Removeusedchannel.Remove(ChannelCountList6[num9]);
							}
						}
					}
					if (temoChannelDic7.ContainsKey(relaykinds.options[temp].text))
					{
						int value7 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value7))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList7.Count);
							for (int num10 = 1; num10 < value7 + 1; num10++)
							{
								Removeusedchannel.Add(num10);
							}
							for (int num11 = 0; num11 < ChannelCountList7.Count; num11++)
							{
								Removeusedchannel.Remove(ChannelCountList7[num11]);
							}
						}
					}
					if (temoChannelDic8.ContainsKey(relaykinds.options[temp].text))
					{
						int value8 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value8))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList8.Count);
							for (int num12 = 1; num12 < value8 + 1; num12++)
							{
								Removeusedchannel.Add(num12);
							}
							for (int num13 = 0; num13 < ChannelCountList8.Count; num13++)
							{
								Removeusedchannel.Remove(ChannelCountList8[num13]);
							}
						}
					}
					if (temoChannelDic9.ContainsKey(relaykinds.options[temp].text))
					{
						int value9 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value9))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList9.Count);
							for (int num14 = 1; num14 < value9 + 1; num14++)
							{
								Removeusedchannel.Add(num14);
							}
							for (int num15 = 0; num15 < ChannelCountList9.Count; num15++)
							{
								Removeusedchannel.Remove(ChannelCountList9[num15]);
							}
						}
					}
					if (temoChannelDic10.ContainsKey(relaykinds.options[temp].text))
					{
						int value10 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value10))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList10.Count);
							for (int num16 = 1; num16 < value10 + 1; num16++)
							{
								Removeusedchannel.Add(num16);
							}
							for (int num17 = 0; num17 < ChannelCountList10.Count; num17++)
							{
								Removeusedchannel.Remove(ChannelCountList10[num17]);
							}
						}
					}
					if (temoChannelDic11.ContainsKey(relaykinds.options[temp].text))
					{
						int value11 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value11))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList11.Count);
							for (int num18 = 1; num18 < value11 + 1; num18++)
							{
								Removeusedchannel.Add(num18);
							}
							for (int num19 = 0; num19 < ChannelCountList11.Count; num19++)
							{
								Removeusedchannel.Remove(ChannelCountList11[num19]);
							}
						}
					}
					if (temoChannelDic12.ContainsKey(relaykinds.options[temp].text))
					{
						int value12 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value12))
						{
							Debug.Log(" ChannelCountList1.Count:" + ChannelCountList12.Count);
							for (int num20 = 1; num20 < value12 + 1; num20++)
							{
								Removeusedchannel.Add(num20);
							}
							for (int num21 = 0; num21 < ChannelCountList12.Count; num21++)
							{
								Removeusedchannel.Remove(ChannelCountList12[num21]);
							}
						}
					}
					else if (ListControCellModel == null)
					{
						int value13 = 0;
						if (RelayChannelDic.TryGetValue(relaykinds.options[temp].text, out value13))
						{
							for (int num22 = 1; num22 < value13 + 1; num22++)
							{
								Removeusedchannel.Add(num22);
							}
							Removeusedchannel.Clear();
							RelayChannelDic.Clear();
						}
					}
				}
			}
			RelayKindsname = relaykinds.options[temp].text;
			Debug.LogError("Relayname:" + RelayKindsname);
		});
		bool isExitSameRelay = false;
		BtnAdd.onClick.AddListener(delegate
		{
			if (ListModel != null)
			{
				for (int j = 0; j < ListModel.Count; j++)
				{
					if (ListModel[j].Getrelaykinds == relaykinds.captionText.text && ListModel[j].GetrelayIp == relayIp.text)
					{
						isExitSameRelay = true;
						Debug.Log("已经存在IP=" + relayIp.text + "的继电器，IP不能重复!");
						OnClosePanel();
						break;
					}
				}
			}
			if (!isExitSameRelay)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/Togrelay");
				GameObject relayObj = Object.Instantiate(original);
				relayObj.transform.SetParent(UITools.FindTheChildNode(base.transform.parent, "Contentrelay"), worldPositionStays: false);
				relayObj.transform.Find("Text").GetComponent<Text>().text = rename;
				relayObj.GetComponent<TcpControl80>().Connect(Ip, port);
				relayObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
				{
					if (ison)
					{
						AddChannel(base.transform.parent.name.Split('e')[2], relayObj.transform.GetChild(0).GetComponent<Text>().text);
					}
				});
				OnClosePanel();
				RelayModel relayModel = new RelayModel
				{
					Getrelaykinds = relaykinds.captionText.text,
					GetrelayIp = Ip,
					GetRelayPort = port,
					GetrelayName = rename
				};
				Debug.Log(itemArr.dataArray.Length);
				Debug.Log(relaykinds.captionText.text);
				for (int num = 0; num < itemArr.dataArray.Length; num++)
				{
					Debug.Log(itemArr.dataArray[num].relayname);
					if (relaykinds.captionText.text == itemArr.dataArray[num].relayname)
					{
						relayModel.GetAllChannelNum = (int)itemArr.dataArray[num].ChannelNum;
					}
				}
				Debug.LogError(base.transform.parent.name.Split('e')[2]);
				RelayModelControl.GetInstance().AddRelayItem(base.transform.parent.name.Split('e')[2], relayModel);
				relayObj.transform.Find("deleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					GameObject original2 = Resources.Load<GameObject>("Prefabs/DeletePanel");
					GameObject panelObj = Object.Instantiate(original2);
					panelObj.transform.SetParent(base.transform.parent);
					panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
					panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
					panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
					{
						Debug.LogError("删除控制单元:" + relayObj.transform.parent.name);
						GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('e')[2]);
						FileUtil.DeleRelayContentbyLine(GameResources.RelayItemPath, "Relay/RelayItem.txt", Ip);
						Object.Destroy(relayObj);
						panelObj.GetComponent<CanvasGroup>().alpha = 0f;
						panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
						ControlBool(ison: false);
						SetTogInter(ison: true);
					});
					panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(delegate
					{
						panelObj.GetComponent<CanvasGroup>().alpha = 0f;
						panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
						SetTogInter(ison: true);
					});
				});
			}
		});
	}

	public void AddChannel(string index)
	{
		OnClosePanel();
		UIManager.Instance.PushPanel(index, UIPanelType.AddChannel);
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
			FindRelayChannelCoder(RelayKindsname, uint.Parse(relayChannel.ToString()));
			SaveCellItemData(child.name, ItemObj.name, Relaykindsname, relayChannel.ToString(), base.name);
			string orderopen = Opencodelist;
			string orderclose = Closecodelist;
			ItemObj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
			{
				if (ison)
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-1");
					Debug.LogError(GetRelayIp(RelayKindsname));
					EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderopen, GetRelayIp(RelayKindsname));
					Debug.Log("打开指令:" + orderopen);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Light/LightCellItem.txt", InputName, ison);
				}
				else
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/二级/内容/二级-设备-灯光-0");
					EventCenter.BroadCast(Eventdefine.TcpSendSingle80, orderclose, GetRelayIp(RelayKindsname));
					Debug.Log("关闭指令: " + orderclose);
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Light/LightCellItem.txt", InputName, ison);
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
					Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
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
					SetTogInter(ison: true);
				});
			});
			break;
		}
	}

	private void ControlBool(bool ison)
	{
		base.transform.parent.parent.parent.parent.parent.Find("DeieteTing").gameObject.SetActive(ison);
		for (int i = 0; i < CellCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
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
		int childCount = base.transform.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<ChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	private IEnumerator SendToCenterControlServer(string discribname)
	{
		yield return new WaitForSeconds(1f);
		NetManager.Send(new ResponseMsgControl
		{
			Code = discribname
		});
	}

	private string GetRelayIp(string relayname)
	{
		for (int i = 0; i < manager.dataArray.Length; i++)
		{
			if (string.Equals(manager.dataArray[i].relayname, relayname))
			{
				return manager.dataArray[i].ip;
			}
		}
		return null;
	}

	private void FindRelayChannelCoder(string name, uint channel)
	{
		for (int i = 0; i < manager.dataArray.Length; i++)
		{
			int num = i;
			if (string.Equals(manager.dataArray[num].relayname, name))
			{
				switch (channel)
				{
				case 1u:
					Opencodelist = manager.dataArray[num].firstOpen;
					Closecodelist = manager.dataArray[num].firstClose;
					break;
				case 2u:
					Opencodelist = manager.dataArray[num].secondopen;
					Closecodelist = manager.dataArray[num].secondclose;
					break;
				case 3u:
					Opencodelist = manager.dataArray[num].thirdopen;
					Closecodelist = manager.dataArray[num].thirdclose;
					break;
				case 4u:
					Opencodelist = manager.dataArray[num].forthopen;
					Closecodelist = manager.dataArray[num].forthclose;
					break;
				case 5u:
					Opencodelist = manager.dataArray[num].fifthopen;
					Closecodelist = manager.dataArray[num].fifthclose;
					break;
				case 6u:
					Opencodelist = manager.dataArray[num].sixopen;
					Closecodelist = manager.dataArray[num].sixclose;
					break;
				case 7u:
					Opencodelist = manager.dataArray[num].seventhopen;
					Closecodelist = manager.dataArray[num].seventhclose;
					break;
				case 8u:
					Opencodelist = manager.dataArray[num].eightopen;
					Closecodelist = manager.dataArray[num].eightclose;
					break;
				}
			}
		}
	}

	private void SaveCellItemData(string parentName, string name, string relayname, string channel, string dscrisname)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		lightCellItemModel.Get_CellItemName = name;
		lightCellItemModel.Get_ParentName = parentName;
		lightCellItemModel.Get_RelayName = relayname;
		lightCellItemModel.Get_Channel = uint.Parse(channel);
		lightCellItemModel.Get_DiscribName = dscrisname;
		LightCellItemControl.GetInstance().AddLightItem(base.transform.parent.name.Split('e')[2], lightCellItemModel);
	}

	private void Init()
	{
		transPar = UITools.FindTheChildNode(base.transform.parent, "Viewport/Content");
		CellCount = transPar.childCount;
		GameResources.Instance.RegisterGameDataPath(base.transform.parent.name.Split('e')[2]);
		List<Item> list = manager.dataArray.ToList();
		ListControCellModel = LightCellItemControl.GetInstance().GetLightCellItemList(base.transform.parent.name.Split('e')[2]);
		RepeattemoRelayname = new List<string>();
		temoChannelDic1 = new Dictionary<string, List<int>>();
		temoChannelDic2 = new Dictionary<string, List<int>>();
		temoChannelDic3 = new Dictionary<string, List<int>>();
		temoChannelDic4 = new Dictionary<string, List<int>>();
		temoChannelDic5 = new Dictionary<string, List<int>>();
		temoChannelDic6 = new Dictionary<string, List<int>>();
		temoChannelDic7 = new Dictionary<string, List<int>>();
		temoChannelDic8 = new Dictionary<string, List<int>>();
		temoChannelDic9 = new Dictionary<string, List<int>>();
		temoChannelDic10 = new Dictionary<string, List<int>>();
		temoChannelDic11 = new Dictionary<string, List<int>>();
		temoChannelDic12 = new Dictionary<string, List<int>>();
		RelayChannelDic = new Dictionary<string, int>();
		if (list != null)
		{
			for (int i = 0; i < list.Count; i++)
			{
				Debug.Log("控制器名称:" + list[i].relayname);
				RepeattemoRelayname.Add(list[i].relayname);
				RelayChannelDic.Add(list[i].relayname, (int)list[i].ChannelNum);
			}
		}
		else
		{
			Debug.LogError("第一次创建");
		}
		if (ListControCellModel != null && ListControCellModel.Count != 0)
		{
			for (int j = 0; j < ListControCellModel.Count; j++)
			{
				for (int k = 0; k < RepeattemoRelayname.Count; k++)
				{
					switch (k)
					{
					case 0:
						if (ChannelCountList1 == null)
						{
							ChannelCountList1 = new List<int>();
							temoChannelDic1.Add(RepeattemoRelayname[k], ChannelCountList1);
						}
						break;
					case 1:
						if (ChannelCountList2 == null)
						{
							ChannelCountList2 = new List<int>();
							temoChannelDic2.Add(RepeattemoRelayname[k], ChannelCountList2);
						}
						break;
					case 2:
						if (ChannelCountList3 == null)
						{
							ChannelCountList3 = new List<int>();
							temoChannelDic3.Add(RepeattemoRelayname[k], ChannelCountList3);
						}
						break;
					case 3:
						if (ChannelCountList4 == null)
						{
							ChannelCountList4 = new List<int>();
							temoChannelDic4.Add(RepeattemoRelayname[k], ChannelCountList4);
						}
						break;
					case 4:
						if (ChannelCountList5 == null)
						{
							ChannelCountList5 = new List<int>();
							temoChannelDic5.Add(RepeattemoRelayname[k], ChannelCountList5);
						}
						break;
					case 5:
						if (ChannelCountList6 == null)
						{
							ChannelCountList6 = new List<int>();
							temoChannelDic6.Add(RepeattemoRelayname[k], ChannelCountList6);
						}
						break;
					case 6:
						if (ChannelCountList7 == null)
						{
							ChannelCountList7 = new List<int>();
							temoChannelDic7.Add(RepeattemoRelayname[k], ChannelCountList7);
						}
						break;
					case 7:
						if (ChannelCountList8 == null)
						{
							ChannelCountList8 = new List<int>();
							temoChannelDic8.Add(RepeattemoRelayname[k], ChannelCountList8);
						}
						break;
					case 8:
						if (ChannelCountList9 == null)
						{
							ChannelCountList9 = new List<int>();
							temoChannelDic9.Add(RepeattemoRelayname[k], ChannelCountList9);
						}
						break;
					case 9:
						if (ChannelCountList10 == null)
						{
							ChannelCountList10 = new List<int>();
							temoChannelDic10.Add(RepeattemoRelayname[k], ChannelCountList10);
						}
						break;
					case 10:
						if (ChannelCountList11 == null)
						{
							ChannelCountList11 = new List<int>();
							temoChannelDic11.Add(RepeattemoRelayname[k], ChannelCountList11);
						}
						break;
					case 11:
						if (ChannelCountList12 == null)
						{
							ChannelCountList12 = new List<int>();
							temoChannelDic12.Add(RepeattemoRelayname[k], ChannelCountList12);
						}
						break;
					}
					if (RepeattemoRelayname[k] == ListControCellModel[j].Get_RelayName)
					{
						if (temoChannelDic1.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList1.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic2.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList2.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic3.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList3.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic4.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList4.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic5.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList5.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic6.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList6.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic7.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList7.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic8.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList8.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic9.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList9.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic10.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList10.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic11.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList11.Add((int)ListControCellModel[j].Get_Channel);
						}
						else if (temoChannelDic12.ContainsKey(RepeattemoRelayname[k]))
						{
							ChannelCountList12.Add((int)ListControCellModel[j].Get_Channel);
						}
					}
				}
			}
		}
		else
		{
			Debug.LogError("第一次创建");
		}
	}

	private void UpdateDropDownItem(List<string> showNames)
	{
		relaykinds.options.Clear();
		for (int i = 0; i < showNames.Count; i++)
		{
			Dropdown.OptionData optionData = new Dropdown.OptionData();
			optionData.text = showNames[i];
			relaykinds.options.Add(optionData);
		}
		relaykinds.captionText.text = showNames[0];
		Debug.Log("继电器种类:  " + relaykinds.captionText.text);
		for (int j = 0; j < manager.dataArray.Length; j++)
		{
			if (manager.dataArray[j].relayname == relaykinds.captionText.text)
			{
				Ip = manager.dataArray[j].ip;
				port = (int)manager.dataArray[j].port;
			}
		}
		RelayKindsname = relaykinds.options[0].text;
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
	}

	public override void OnExit()
	{
		canvasGroup.blocksRaycasts = false;
		canvasGroup.DOFade(0f, 0.5f);
	}

	public void OnClosePanel()
	{
		UIManager.Instance.PopPanel();
		SetTogInter(ison: true);
	}
}
