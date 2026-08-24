using System.Text.RegularExpressions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AddVideoDetail : BasePanel
{
	public InputField Ip;

	public InputField Path;

	public InputField Port;

	public InputField Name;

	public InputField Note;

	public Button ConfirmBtn;

	public Button ExitBtn;

	public bool isVideo;

	public Toggle tvTog;

	public Toggle PhotoTog;

	private string strName = string.Empty;

	private string strNote = string.Empty;

	private string strPath = string.Empty;

	private string strIp = string.Empty;

	private string strPort = string.Empty;

	private bool istv;

	private bool isphoto;

	private Transform transPar;

	private int CellCount;

	private CanvasGroup canvasGroup;

	private void Init()
	{
		transPar = UITools.FindTheChildNode(base.transform.parent, "Viewport/Content");
		CellCount = transPar.childCount;
		isVideo = true;
		tvTog.onValueChanged.AddListener(delegate(bool ison)
		{
			istv = ison;
			ControlVideoPanel.istv = ison;
		});
		PhotoTog.onValueChanged.AddListener(delegate(bool ison)
		{
			isphoto = ison;
			ControlVideoPanel.isphoto = ison;
		});
		Name.onEndEdit.AddListener(delegate(string text)
		{
			strName = text;
		});
		Note.onEndEdit.AddListener(delegate(string text)
		{
			strNote = text;
		});
		Path.onEndEdit.AddListener(delegate(string txt)
		{
			strPath = txt;
		});
		Ip.onEndEdit.AddListener(delegate(string txt)
		{
			strIp = txt;
		});
		Port.onEndEdit.AddListener(delegate(string txt)
		{
			strPort = txt;
		});
		ConfirmBtn.onClick.AddListener(delegate
		{
			if (IsIP(strIp) && IsWholeNumber(strPort))
			{
				GameResources.Instance.RegisterSoftWareGameDataPath(base.transform.parent.name.Split('e')[3]);
				InistateCellItem(istv, isphoto, isVideo, strIp, strPort, strPath, strName, strNote);
				OnClosePanel();
			}
			else
			{
				Debug.LogError("ip地址或者端口输入不合法!");
			}
		});
		ExitBtn.onClick.AddListener(delegate
		{
			Object.Destroy(base.gameObject);
		});
	}

	private void InistateCellItem(bool istv, bool isphoto, bool isvideo, string strip, string strport, string strpath, string strname, string strnote)
	{
		GameObject original = Resources.Load<GameObject>("Prefabs/VideoItem");
		for (int i = 0; i < CellCount; i++)
		{
			if (transPar.GetChild(i).childCount == 0)
			{
				Transform child = transPar.GetChild(i);
				GameObject ItemObj = Object.Instantiate(original);
				ItemObj.transform.SetParent(child);
				ItemObj.transform.localPosition = new Vector3(1f, 1f, 1f);
				ItemObj.transform.localScale = new Vector3(1f, 1f, 1f);
				ItemObj.name = child.name.Split('l')[2];
				ItemObj.transform.Find("Text").GetComponent<Text>().text = strname;
				SaveCellItemData(istv, isphoto, isvideo, child.name, strpath, strip, strport, strname, strnote);
				ItemObj.GetComponent<Button>().onClick.AddListener(delegate
				{
					ControlVideoPanel.ip = strip;
					ControlVideoPanel.port = strport;
					ControlVideoPanel.path = strpath;
					ControlVideoPanel.name = strname;
					ControlVideoPanel.note = strnote;
					ControlVideoPanel.istv = istv;
					ControlVideoPanel.isphoto = isphoto;
					Debug.Log("打开控制面板,ip:" + strip);
					GameObject obj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/ControlVideoPanel"));
					obj.transform.SetParent(base.transform.parent);
					obj.transform.localPosition = new Vector3(1f, 1f, 1f);
					obj.transform.localScale = new Vector3(1f, 1f, 1f);
				});
				ItemObj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
				{
					Debug.LogError("删除控制单元:" + ItemObj.transform.parent.name);
					FileUtil.DeleContentbyLine(GameResources.SoftWareCellItemPath, "software/SoftwareItem.txt", ItemObj.transform.parent.name);
					Object.Destroy(ItemObj);
				});
				break;
			}
		}
	}

	private void SaveCellItemData(bool istv, bool isphoto, bool isvideo, string parentName, string path, string ip, string port, string strname, string strnote)
	{
		SoftWareModel softWareModel = new SoftWareModel();
		softWareModel.GetPath = path;
		softWareModel.Get_ParentName = parentName;
		softWareModel.GetPort = port;
		softWareModel.GetIp = ip;
		softWareModel.GetName = strname;
		softWareModel.GetNote = strnote;
		softWareModel.GetIsVideo = isvideo;
		softWareModel.GetIsSheBei = istv;
		softWareModel.GetIsPhoto = isphoto;
		Debug.Log("save:" + istv + isphoto);
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
	}
}
