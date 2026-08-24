using System.Collections.Generic;
using System.Text.RegularExpressions;
using DG.Tweening;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddSoftWareDetail : BasePanel
{
	public InputField Ip;

	public InputField Port;

	public InputField Name;

	public Dropdown ControlKindsdrop;

	public InputField Note;

	public Button ConfirmBtn;

	public Toggle TVTog;

	public Toggle PhotoTog;

	public bool isVideo;

	private string strName = string.Empty;

	private string strNote = string.Empty;

	private string strPath = string.Empty;

	private string strIp = string.Empty;

	private string controlkind = "视频控制";

	private string strPort = string.Empty;

	private bool SheBeiControl;

	private bool PhotoControl;

	private Transform transPar;

	private int CellCount;

	private CanvasGroup canvasGroup;

	private void ControlKindDrop()
	{
		List<string> options = new List<string> { "视频控制", "拍照控制" };
		ControlKindsdrop.ClearOptions();
		ControlKindsdrop.AddOptions(options);
	}

	private void SetTogInter(bool ison)
	{
		int childCount = base.transform.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<SoftWareChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	private void Init()
	{
		SetTogInter(ison: false);
		ControlKindDrop();
		transPar = UITools.FindTheChildNode(base.transform.parent, "Viewport/Content");
		CellCount = transPar.childCount;
		isVideo = false;
		Name.onEndEdit.AddListener(delegate(string text)
		{
			strName = text;
		});
		Note.onEndEdit.AddListener(delegate(string text)
		{
			strNote = text;
		});
		Ip.onEndEdit.AddListener(delegate(string txt)
		{
			strIp = txt;
		});
		Port.onEndEdit.AddListener(delegate(string txt)
		{
			strPort = txt;
		});
		ControlKindsdrop.onValueChanged.AddListener(delegate(int Value)
		{
			controlkind = ControlKindsdrop.options[Value].text;
		});
		ConfirmBtn.onClick.AddListener(delegate
		{
			if (IsIP(strIp))
			{
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.parent.name.Split('e')[3]);
				Debug.Log("设备类型:" + controlkind);
				if (controlkind == "视频控制")
				{
					SheBeiControl = false;
					PhotoControl = false;
					isVideo = true;
				}
				else if (controlkind == "拍照控制")
				{
					SheBeiControl = false;
					PhotoControl = true;
					isVideo = false;
				}
				InistateCellItem(SheBeiControl, PhotoControl, isVideo, strIp, "8888", "", strName, strNote);
				OnClosePanel();
				SetTogInter(ison: true);
			}
			else
			{
				Debug.LogError("ip地址或者端口输入不合法!");
			}
		});
	}

	private void InistateCellItem(bool isShebei, bool isPhoto, bool isVideo, string strip, string strport, string strpath, string strname, string strnote)
	{
		Debug.Log("设备:" + isShebei);
		Debug.Log("拍照:" + isPhoto);
		Debug.Log("视频:" + isVideo);
		for (int i = 0; i < CellCount; i++)
		{
			if (transPar.GetChild(i).childCount != 0)
			{
				continue;
			}
			if (isShebei)
			{
				GameObject original = Resources.Load<GameObject>("Prefabs/TVItem");
				Transform child = transPar.GetChild(i);
				GameObject ItemObj = Object.Instantiate(original);
				ItemObj.transform.SetParent(child);
				ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
				ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
				ItemObj.name = child.name.Split('l')[2];
				ItemObj.transform.Find("Text").GetComponent<Text>().text = strname;
				ItemObj.GetComponent<Button>().onClick.AddListener(delegate
				{
					SetTogInter(ison: false);
					ControlSoftWarePanel.ip = strip;
					ControlSoftWarePanel.port = strport;
					ControlSoftWarePanel.path = strpath;
					ControlSoftWarePanel.name = strname;
					ControlSoftWarePanel.note = strnote;
					Debug.Log("打开控制面板,ip:" + strip);
					GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlShebeiPanel"));
					obj.transform.SetParent(base.transform.parent);
					obj.transform.localPosition = new Vector3(1f, 1f, 1f);
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
					obj.transform.Find("TitleTxt").GetComponent<Text>().text = base.name;
					obj.AddComponent<UDPClient>();
					EventCenter.BroadCast(Eventdefine.InitSocket, strip);
				});
				SaveCellItemData(isShebei, isPhoto, isVideo, child.name, strpath, strip, strport, strname, strnote);
				ItemObj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj.transform.parent.name);
					Object.Destroy(ItemObj);
				});
			}
			else if (isPhoto)
			{
				GameObject original2 = Resources.Load<GameObject>("Prefabs/softwareItem");
				Transform child2 = transPar.GetChild(i);
				GameObject ItemObj2 = Object.Instantiate(original2);
				ItemObj2.transform.SetParent(child2);
				ItemObj2.transform.localPosition = new Vector3(1f, 1f, 1f);
				ItemObj2.transform.localScale = new Vector3(1f, 1f, 1f);
				ItemObj2.name = child2.name.Split('l')[2];
				ItemObj2.transform.Find("Text").GetComponent<Text>().text = strname;
				ItemObj2.GetComponent<Button>().onClick.AddListener(delegate
				{
					SetTogInter(ison: false);
					ControlSoftWarePanel.ip = strip;
					ControlSoftWarePanel.port = strport;
					ControlSoftWarePanel.path = strpath;
					ControlSoftWarePanel.name = strname;
					ControlSoftWarePanel.note = strnote;
					ControlSoftWarePanel.IsTV = false;
					ControlSoftWarePanel.IsPhoto = false;
					Debug.Log("打开控制面板,ip:" + strip);
					GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlPhotoPanel"));
					obj.transform.SetParent(base.transform.parent);
					obj.transform.localPosition = new Vector3(-175f, 1f, 1f);
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
					EventCenter.BroadCast(Eventdefine.PhotoUpdConnect, strip, "7014");
					base.transform.parent.parent.parent.parent.parent.Find("PhoteNetPar").gameObject.SetActive(value: true);
				});
				SaveCellItemData(isShebei, isPhoto, isVideo, child2.name, strpath, strip, strport, strname, strnote);
				ItemObj2.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Debug.LogError("删除控制单元:" + ItemObj2.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj2.transform.parent.name);
					Object.Destroy(ItemObj2);
				});
			}
			else if (isVideo)
			{
				GameObject original3 = Resources.Load<GameObject>("Prefabs/VideoItem");
				Transform child3 = transPar.GetChild(i);
				GameObject ItemObj3 = Object.Instantiate(original3);
				ItemObj3.transform.SetParent(child3);
				ItemObj3.transform.localPosition = new Vector3(1f, 1f, 1f);
				ItemObj3.transform.localScale = new Vector3(1f, 1f, 1f);
				ItemObj3.name = child3.name.Split('l')[2];
				ItemObj3.transform.Find("Text").GetComponent<Text>().text = strname;
				ItemObj3.GetComponent<Button>().onClick.AddListener(delegate
				{
					SetTogInter(ison: false);
					ControlVideoPanel.ip = strIp;
					ControlVideoPanel.port = strPort;
					ControlVideoPanel.path = strPath;
					ControlVideoPanel.name = strname;
					ControlVideoPanel.note = strNote;
					ControlVideoPanel.istv = false;
					ControlVideoPanel.isphoto = false;
					ControlVideoPanel.Item_Name = strname;
					Debug.Log("打开控制面板,ip:" + strip);
					GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlVideoPanel"));
					obj.transform.SetParent(base.transform.parent);
					obj.transform.localPosition = new Vector3(-175f, 1f, 1f);
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
					obj.AddComponent<UDPClient>();
					EventCenter.BroadCast(Eventdefine.InitSocket, strip);
				});
				SaveCellItemData(isShebei, isPhoto, isVideo, child3.name, strpath, strip, strport, strname, strnote);
				ItemObj3.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Debug.LogError("删除控制单元:" + ItemObj3.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj3.transform.parent.name);
					Object.Destroy(ItemObj3);
				});
			}
			break;
		}
	}

	private void SaveCellItemData(bool isShebei, bool isPhoto, bool isVideo, string parentName, string path, string ip, string port, string strname, string strnote)
	{
		SoftWareModel softWareModel = new SoftWareModel();
		softWareModel.GetPath = path;
		softWareModel.Get_ParentName = parentName;
		softWareModel.GetPort = port;
		softWareModel.GetIp = ip;
		softWareModel.GetName = strname;
		softWareModel.GetNote = strnote;
		softWareModel.GetIsVideo = isVideo;
		softWareModel.GetIsSheBei = isShebei;
		softWareModel.GetIsPhoto = isPhoto;
		SoftWareModelControl.GetInstance().AddSoftWareItem(softWareModel);
	}

	public static bool IsIP(string ip)
	{
		return Regex.IsMatch(ip, "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$");
	}

	public static bool IsWholeNumber(string strNumber)
	{
		return new Regex("^[0-9]\\d*$").IsMatch(strNumber);
	}

	private void Start()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		Init();
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
		UIManagerSoftWare.Instance.PopPanel();
		SetTogInter(ison: true);
	}
}
