using DG.Tweening;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddSoftWareConTrolPanel : BasePanel
{
	private Button closeBtn;

	private InputField InputName;

	private InputField InputIP;

	private InputField InputPort;

	private Button AddBtn;

	private CanvasGroup canvasGroup;

	private string index;

	private new string name = string.Empty;

	private string ip = string.Empty;

	private string port = string.Empty;

	private Transform transPar;

	private int CellCount;

	private void Start()
	{
		SetTogInter(ison: false);
		transPar = UITools.FindTheChildNode(base.transform.parent, "Viewport/Content");
		CellCount = transPar.childCount;
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		InputName = base.transform.Find("NameTxt/InputField").GetComponent<InputField>();
		InputIP = base.transform.Find("IPTxt/InputField").GetComponent<InputField>();
		InputPort = base.transform.Find("PortTxt/InputField").GetComponent<InputField>();
		closeBtn = base.transform.Find("BtnExit").GetComponent<Button>();
		AddBtn = base.transform.Find("AddBtn").GetComponent<Button>();
		index = base.transform.parent.name.Split('p')[2];
		closeBtn.onClick.AddListener(delegate
		{
			OnClosePanel();
		});
		InputName.onEndEdit.AddListener(delegate(string value)
		{
			name = value;
		});
		InputIP.onEndEdit.AddListener(delegate(string value)
		{
			ip = value;
		});
		InputPort.onEndEdit.AddListener(delegate(string value)
		{
			port = value;
		});
		AddBtn.onClick.AddListener(delegate
		{
			Debug.LogError(base.transform.parent.name.Split('p')[2]);
			GameResources.Instance.RegisterEquipGameDataPath(base.transform.parent.name.Split('p')[2]);
			InistateCellItem();
			OnClosePanel();
			SetTogInter(ison: true);
		});
	}

	private void SetTogInter(bool ison)
	{
		int childCount = base.transform.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<EquipChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	private void InistateCellItem()
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
			ItemObj.transform.Find("Text").GetComponent<Text>().text = name;
			SaveCellItemData(ip, child.name, ItemObj.name, "", "", name);
			ItemObj.GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
			ItemObj.GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
			{
				if (ison)
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-1");
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", name, ison);
				}
				else
				{
					ItemObj.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控增项/设备按钮/二级-设备-软件-0");
					Debug.LogError("软件控制关闭：" + name + "|" + ip + "|关机");
					FileUtil.UpdateLightModelFile(GameResources.EquipCellItemPath, "Equip/EquipCellItem.txt", name, ison);
					ItemObj.AddComponent<UDPClient>();
					EventCenter.BroadCast(Eventdefine.InitSocket, ip);
					EventCenter.BroadCast(Eventdefine.Shutdown);
				}
			});
			ItemObj.GetComponent<Toggle>().isOn = true;
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

	private void SaveCellItemData(string ip, string parentName, string name, string relayname, string channel, string dscrisname)
	{
		LightCellItemModel lightCellItemModel = new LightCellItemModel();
		lightCellItemModel.Get_CellItemName = name;
		lightCellItemModel.Get_ParentName = parentName;
		lightCellItemModel.Get_RelayName = relayname;
		lightCellItemModel.Get_Channel = 0u;
		lightCellItemModel.Get_DiscribName = dscrisname;
		lightCellItemModel.Get_IPadress = ip;
		Debug.LogError("正在添加：" + base.transform.parent.name.Split('p')[2]);
		EquipCellItemControl.GetInstance().AddEquipItem(base.transform.parent.name.Split('p')[2], lightCellItemModel);
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
		UIManagerEquip.Instance.PopPanel();
		SetTogInter(ison: true);
	}

	public override void OnResume()
	{
		canvasGroup.blocksRaycasts = true;
	}
}
