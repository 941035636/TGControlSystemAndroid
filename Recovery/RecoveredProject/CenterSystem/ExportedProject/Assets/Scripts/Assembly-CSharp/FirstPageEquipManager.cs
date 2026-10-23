using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FirstPageEquipManager : MonoBehaviour
{
	public Toggle AllLightOpenTog;

	public Toggle AllLightCloseTog;

	public Toggle AllEquipOpenTog;

	public Toggle AllEquipCloseTog;

	public Toggle xuTingTogComTog;

	public Toggle LaoLeaderComTog;

	public Toggle HistoryLEDTog;

	public Toggle HistoryComTog;

	public Toggle MenzhuComTog;

	public Toggle Zhangui1Tog;

	public Toggle Zhangui2Tog;

	public Toggle Zhangui3Tog;

	public Toggle Zhangui4Tog;

	public Toggle Zhangui5Tog;

	public Toggle HuanMuComTog;

	public Toggle DiMuComTogB;

	public Toggle shuzhiCOmTog;

	public Toggle ditanComTog;

	public Toggle feipingTVTog;

	public Toggle feipingComTog;

	public Toggle shengqiComTog;

	public Toggle paizhaoLEDTog;

	public Toggle xuTingTogComTogClose;

	public Toggle LaoLeaderComTogClose;

	public Toggle HistoryLEDTogClose;

	public Toggle HistoryComTogClose;

	public Toggle MenzhuComTogClose;

	public Toggle Zhangui1TogClose;

	public Toggle Zhangui2TogClose;

	public Toggle Zhangui3TogClose;

	public Toggle Zhangui4TogClose;

	public Toggle Zhangui5TogClose;

	public Toggle HuanMuComTogClose;

	public Toggle DiMuComTogBClose;

	public Toggle shuzhiCOmTogClose;

	public Toggle ditanComTogClose;

	public Toggle feipingTVTogClose;

	public Toggle feipingComTogClose;

	public Toggle shengqiComTogClose;

	public Toggle paizhaoLEDTogClose;

	public Toggle xutingLightTog;

	public Toggle GanDangLightTog;

	public Toggle RongChuangLightTog;

	public Toggle ZhengRongLightTog;

	public Toggle DJfunengLightTog;

	public Toggle ChanpinLightTog;

	public Button CloseBtn;

	public Toggle xutingLightTogClose;

	public Toggle GanDangLightTogClose;

	public Toggle RongChuangLightTogClose;

	public Toggle ZhengRongLightTogClose;

	public Toggle DJfunengLightTogClose;

	public Toggle ChanpinLightTogClose;

	public Text receiverTxt;

	private List<RelayModel> RelayModelList;

	private List<RedModel> RedModelList;

	private Toggle[] TogArrEquip;

	private Toggle[] TogArrEquipClose;

	private Toggle[] TogArrLightOpen;

	private Toggle[] TogArrLightClose;

	public Transform RelayController;

	private string r_xuTingIP;

	private int r_xuTingPort;

	private int r_xuTingCom;

	private string r_horstoryIP;

	private int r_horstoryPort;

	private int r_horstoryCom;

	private string r_MenzhuIP;

	private int r_MenzhuPort;

	private int r_MenzhuCom;

	private string r_Zhangui1IP;

	private int r_Zhangui1Port;

	private int r_Zhangui1Com;

	private string r_Zhangui2IP;

	private int r_Zhangui2Port;

	private int r_Zhangui2Com;

	private string r_Zhangui3IP;

	private int r_Zhangui3Port;

	private int r_Zhangui3Com;

	private string r_Zhangui4IP;

	private int r_Zhangui4Port;

	private int r_Zhangui4Com;

	private string r_Zhangui5IP;

	private int r_Zhangui5Port;

	private int r_Zhangui5Com;

	private string r_HuanmuIP;

	private int r_HuanmuPort;

	private int r_HuanmuCom;

	private string r_DimuIP;

	private int r_DimuPort;

	private int r_DimuCom;

	private string r_DiTanIP;

	private int r_DiTanPort;

	private int r_DiTanCom;

	private string r_DangjianIP;

	private int r_DangjianPort;

	private int r_DangjianCom;

	private string r_FeiPingIP;

	private int r_FeiPingPort;

	private int r_FeiPingCom;

	private string HwIP = string.Empty;

	private string HWPort = string.Empty;

	private float _3AStartimer = 25f;

	private float tgsceneStartWind = 62f;

	private float tgdoStartWind = 42f;

	private bool IsOpenX;

	private static bool ISYuanyvzhou = true;

	private Coroutine equipBatchCoroutine;

	private Coroutine lightBatchCoroutine;

	private Toggle activeEquipBatchToggle;

	private Toggle activeLightBatchToggle;

	private readonly Dictionary<Toggle, bool> equipBatchInteractableStates = new Dictionary<Toggle, bool>();

	private readonly Dictionary<Toggle, bool> lightBatchInteractableStates = new Dictionary<Toggle, bool>();

	public EquipArr EquipModeList;

	private IEnumerator RelayConnectTCP()
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect80, "192.168.86.80", 10000);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpConnect81, "192.168.86.81", 10000);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpConnect82, "192.168.86.82", 10000);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpConnect85, "192.168.86.85", 10000);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpConnect86, "192.168.86.86", 10000);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator RedConnectTcp()
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect90, "192.168.86.90", 502);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpConnect91, "192.168.86.91", 502);
	}

	private IEnumerator Start()
	{
		string equipJson = null;
		string relayJson = null;
		string redJson = null;
		yield return LoadStreamingText("softController/Equip.json", value => equipJson = value);
		yield return LoadStreamingText("设备控制器/Relay.json", value => relayJson = value);
		yield return LoadStreamingText("设备控制器/Red.json", value => redJson = value);
		try
		{
			EquipModeList = JsonConvert.DeserializeObject<EquipArr>(equipJson);
			RelayModelList relayModels = JsonConvert.DeserializeObject<RelayModelList>(relayJson);
			RedModelList redModels = JsonConvert.DeserializeObject<RedModelList>(redJson);
			RelayModelList = relayModels == null ? null : relayModels.GetRelayModelList;
			RedModelList = redModels == null ? null : redModels.GetRedModelList;
		}
		catch (Exception ex)
		{
			Debug.LogError("中控配置解析失败: " + ex.Message);
			yield break;
		}
		if (EquipModeList == null || RelayModelList == null || RedModelList == null)
		{
			Debug.LogError("中控配置不完整，停止初始化设备控制界面");
			yield break;
		}
		ReadIPs(EquipModeList);
		StartCoroutine(RelayConnectTCP());
		StartCoroutine(RedConnectTcp());
		TogArrEquip = base.transform.Find("equipKindsSrollview/Viewport/Content").GetComponentsInChildren<Toggle>();
		TogArrEquipClose = base.transform.Find("equipKindsSrollviewClose/Viewport/Content").GetComponentsInChildren<Toggle>();
		TogArrLightOpen = base.transform.Find("lightKindsSrollview/Viewport/ContentOpen").GetComponentsInChildren<Toggle>();
		TogArrLightClose = base.transform.Find("lightKindsSrollview/Viewport/ContentClose").GetComponentsInChildren<Toggle>();
		AttachToggleDiagnostics(TogArrEquip, "EQUIPMENT_OPEN");
		AttachToggleDiagnostics(TogArrEquipClose, "EQUIPMENT_CLOSE");
		AttachToggleDiagnostics(TogArrLightOpen, "LIGHT_OPEN");
		AttachToggleDiagnostics(TogArrLightClose, "LIGHT_CLOSE");
		ControlDiagnostics.Record("UI", "CONTROL_TOGGLE_DISCOVERY", name, "OK",
			"equipmentOpen=" + TogArrEquip.Length +
			" equipmentClose=" + TogArrEquipClose.Length +
			" lightOpen=" + TogArrLightOpen.Length +
			" lightClose=" + TogArrLightClose.Length);
		AllEquipOpenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartEquipBatch(TogArrEquip, AllEquipOpenTog);
			}
		});
		AllEquipCloseTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartEquipBatch(TogArrEquipClose, AllEquipCloseTog);
			}
		});
		AllLightOpenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Debug.Log("所有灯光全部上电");
				StartLightBatch(TogArrLightOpen, AllLightOpenTog);
			}
		});
		AllLightCloseTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartLightBatch(TogArrLightClose, AllLightCloseTog);
				Debug.Log("所有灯光全部断电");
			}
		});
		CloseBtn.onClick.AddListener(delegate
		{
			Application.Quit();
		});
		HistoryComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send80(r_horstoryIP, 10000, RelayModelList[0].GetOrder.OPenList[r_horstoryCom]));
				HistoryComTogClose.isOn = false;
			}
		});
		HistoryComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send80(r_horstoryIP, 10000, RelayModelList[0].GetOrder.CloseList[r_horstoryCom]));
				HistoryComTog.isOn = false;
			}
		});
		xuTingTogComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send80(r_xuTingIP, 10000, RelayModelList[0].GetOrder.OPenList[r_xuTingCom]));
				xuTingTogComTogClose.isOn = false;
			}
		});
		xuTingTogComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send80(r_xuTingIP, 10000, RelayModelList[0].GetOrder.CloseList[r_xuTingCom]));
				xuTingTogComTog.isOn = false;
			}
		});
		ditanComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send80(r_DiTanIP, 10000, RelayModelList[0].GetOrder.OPenList[r_DiTanCom]));
				ditanComTogClose.isOn = false;
				StartCoroutine(HandleIrcleOpen());
			}
		});
		ditanComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ditanComTog.isOn = false;
				StartCoroutine(TCPConn2Send80(r_DiTanIP, 10000, RelayModelList[0].GetOrder.CloseList[r_DiTanCom]));
				StartCoroutine(HandleIrcleClose());
			}
		});
		DiMuComTogB.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_DimuIP, 10000, RelayModelList[1].GetOrder.OPenList[r_DimuCom]));
				DiMuComTogBClose.isOn = false;
			}
		});
		DiMuComTogBClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_DimuIP, 10000, RelayModelList[1].GetOrder.CloseList[r_DimuCom]));
				DiMuComTogB.isOn = false;
			}
		});
		HuanMuComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_HuanmuIP, 10000, RelayModelList[1].GetOrder.OPenList[r_HuanmuCom]));
				HuanMuComTogClose.isOn = false;
			}
		});
		HuanMuComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_HuanmuIP, 10000, RelayModelList[1].GetOrder.CloseList[r_HuanmuCom]));
				HuanMuComTog.isOn = false;
			}
		});
		MenzhuComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_MenzhuIP, 10000, RelayModelList[1].GetOrder.OPenList[r_MenzhuCom]));
				MenzhuComTogClose.isOn = false;
			}
		});
		MenzhuComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_MenzhuIP, 10000, RelayModelList[1].GetOrder.CloseList[r_MenzhuCom]));
				MenzhuComTog.isOn = false;
			}
		});
		shengqiComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send81(r_DangjianIP, 10000, RelayModelList[1].GetOrder.OPenList[r_DangjianCom]));
				shengqiComTogClose.isOn = false;
			}
		});
		shengqiComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				shengqiComTog.isOn = false;
				StartCoroutine(TCPConn2Send81(r_DangjianIP, 10000, RelayModelList[1].GetOrder.CloseList[r_DangjianCom]));
			}
		});
		feipingComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				feipingComTogClose.isOn = false;
				StartCoroutine(TCPConn2Send82(r_FeiPingIP, 10000, RelayModelList[4].GetOrder.OPenList[r_FeiPingCom]));
			}
		});
		feipingComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send82(r_FeiPingIP, 10000, RelayModelList[4].GetOrder.CloseList[r_FeiPingCom]));
				feipingComTog.isOn = false;
			}
		});
		xutingLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(XuTingLightOpen());
				xutingLightTogClose.isOn = false;
			}
		});
		xutingLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(XuTingLightClose());
				xutingLightTog.isOn = false;
			}
		});
		DJfunengLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DangJianFunnegLightOpen());
				DJfunengLightTogClose.isOn = false;
			}
		});
		DJfunengLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DangJianFunnegLightClose());
				DJfunengLightTog.isOn = false;
			}
		});
		GanDangLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(GanDangLightOpen());
				GanDangLightTogClose.isOn = false;
			}
		});
		GanDangLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(GanDangLightClose());
				GanDangLightTog.isOn = false;
			}
		});
		ZhengRongLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ZhengRongLightOpen());
				ZhengRongLightTogClose.isOn = false;
			}
		});
		ZhengRongLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ZhengRongLightClose());
				ZhengRongLightTog.isOn = false;
			}
		});
		RongChuangLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(RongChuangLightOpen());
				RongChuangLightTogClose.isOn = false;
			}
		});
		RongChuangLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(RongChuangLightClose());
				RongChuangLightTog.isOn = false;
			}
		});
		ChanpinLightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ChanpinLightOpen());
				ChanpinLightTogClose.isOn = false;
			}
		});
		ChanpinLightTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ChanpinLightClose());
				ChanpinLightTog.isOn = false;
			}
		});
		LaoLeaderComTog.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open3, RedModelList[0].GetredIp);
				LaoLeaderComTogClose.isOn = false;
			}
		});
		LaoLeaderComTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close3, RedModelList[0].GetredIp);
				LaoLeaderComTog.isOn = false;
			}
		});
		HistoryLEDTog.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				StartCoroutine(ControlHistoryLedOpen());
				HistoryLEDTogClose.isOn = false;
			}
		});
		HistoryLEDTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				StartCoroutine(ControlHistoryLedClose());
				HistoryLEDTog.isOn = false;
			}
		});
		shuzhiCOmTog.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open5, RedModelList[0].GetredIp);
				shuzhiCOmTogClose.isOn = false;
			}
		});
		shuzhiCOmTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close5, RedModelList[0].GetredIp);
				shuzhiCOmTog.isOn = false;
			}
		});
		feipingTVTog.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send91("192.168.86.91", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle91, RedModelList[1].GetOrder.Open1, RedModelList[1].GetredIp);
				feipingTVTogClose.isOn = false;
			}
		});
		feipingTVTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send91("192.168.86.91", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle91, RedModelList[1].GetOrder.Close1, RedModelList[1].GetredIp);
				feipingTVTog.isOn = false;
			}
		});
		paizhaoLEDTog.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open6, RedModelList[0].GetredIp);
				paizhaoLEDTogClose.isOn = false;
			}
		});
		paizhaoLEDTogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close6, RedModelList[0].GetredIp);
				paizhaoLEDTog.isOn = false;
			}
		});
		Zhangui1Tog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ControlOpenEquip("1"));
				Zhangui1TogClose.isOn = false;
			}
		});
		Zhangui1TogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ControlCLoseEquip("192.168.86.50"));
				Zhangui1Tog.isOn = false;
			}
		});
		Zhangui2Tog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Zhangui2TogClose.isOn = false;
				StartCoroutine(ControlOpenEquip("2"));
			}
		});
		Zhangui2TogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Zhangui2Tog.isOn = false;
				StartCoroutine(ControlCLoseEquip("192.168.86.51"));
			}
		});
		Zhangui3Tog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ControlOpenEquip("3"));
				Zhangui3TogClose.isOn = false;
			}
		});
		Zhangui3TogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Zhangui3Tog.isOn = false;
				StartCoroutine(ControlCLoseEquip("192.168.86.52"));
			}
		});
		Zhangui4Tog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ControlOpenEquip("4"));
				Zhangui4TogClose.isOn = false;
			}
		});
		Zhangui4TogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Zhangui4Tog.isOn = false;
				StartCoroutine(ControlCLoseEquip("192.168.86.53"));
			}
		});
		Zhangui5Tog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(ControlOpenEquip("5"));
				Zhangui5TogClose.isOn = false;
			}
		});
		Zhangui5TogClose.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Zhangui5Tog.isOn = false;
				StartCoroutine(ControlCLoseEquip("192.168.86.54"));
			}
		});
	}

	private IEnumerator TCPConn2Send80(string ip, int port, string code)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect80, ip, port);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle80, code, ip);
	}

	private IEnumerator TCPConn2Send81(string ip, int port, string code)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect81, ip, port);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle81, code, ip);
	}

	private IEnumerator TCPConn2Send82(string ip, int port, string code)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect82, ip, port);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle82, code, ip);
	}

	private IEnumerator TCPConn2Send85(string ip, int port)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect85, ip, port);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator TCPConn2Send86(string ip, int port)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect86, ip, port);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator TCPConn2Send90(string ip, int port)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect90, ip, port);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator TCPConn2Send91(string ip, int port)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect91, ip, port);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator ControlHistoryLedOpen()
	{
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open4, RedModelList[0].GetredIp);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open4, RedModelList[0].GetredIp);
	}

	private IEnumerator ControlHistoryLedClose()
	{
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close4, RedModelList[0].GetredIp);
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close4, RedModelList[0].GetredIp);
	}

	private IEnumerator ControlCLoseEquip(string ip)
	{
		yield return ReliableUdpCommandSender.Send("TRANSPARENT_CABINET_SHUTDOWN", ip, 7006, "shutdown");
	}

	private IEnumerator ControlOpenEquip(string code)
	{
		yield return ReliableUdpCommandSender.Send("TRANSPARENT_CABINET_WAKE", "192.168.86.40", 7002, "Open" + code);
	}

	private IEnumerator XuTingLightOpen()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[0], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[1], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator XuTingLightClose()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[0], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[1], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator GanDangLightOpen()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[7], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[9], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[2], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[3], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[4], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator GanDangLightClose()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[7], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[9], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[2], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[3], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[4], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator DangJianFunnegLightOpen()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[6], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator DangJianFunnegLightClose()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[6], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ZhengRongLightOpen()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[10], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[11], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[12], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[13], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[15], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ZhengRongLightClose()
	{
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[10], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[11], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[12], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[13], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[15], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator HandleIrcleOpen()
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.OPenList[8], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator HandleIrcleClose()
	{
		yield return new WaitForSeconds(1f);
		StartCoroutine(TCPConn2Send85(RelayModelList[2].GetrelayIp, 10000));
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle85, RelayModelList[2].GetOrder.CloseList[8], RelayModelList[2].GetrelayIp);
		yield return new WaitForSeconds(1f);
	}

	private IEnumerator RongChuangLightOpen()
	{
		StartCoroutine(TCPConn2Send86(RelayModelList[3].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.OPenList[1], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator RongChuangLightClose()
	{
		StartCoroutine(TCPConn2Send86(RelayModelList[3].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.CloseList[1], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ChanpinLightOpen()
	{
		StartCoroutine(TCPConn2Send86(RelayModelList[3].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.OPenList[2], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.OPenList[3], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private IEnumerator ChanpinLightClose()
	{
		StartCoroutine(TCPConn2Send86(RelayModelList[3].GetrelayIp, 10000));
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.CloseList[2], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.TcpSendSingle86, RelayModelList[3].GetOrder.CloseList[3], RelayModelList[3].GetrelayIp);
		yield return new WaitForSeconds(0.5f);
	}

	private void ReadIPs(EquipArr EquipModeList)
	{
		for (int i = 0; i < EquipModeList.GetSoftWare.Count; i++)
		{
			switch (EquipModeList.GetSoftWare[i].GetName)
			{
			case "序厅主机":
				r_xuTingIP = EquipModeList.GetSoftWare[i].GetIP;
				r_xuTingPort = EquipModeList.GetSoftWare[i].GetPort;
				r_xuTingCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "历史沿革主机":
				r_horstoryIP = EquipModeList.GetSoftWare[i].GetIP;
				r_horstoryPort = EquipModeList.GetSoftWare[i].GetPort;
				r_horstoryCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "门柱屏主机":
				r_MenzhuIP = EquipModeList.GetSoftWare[i].GetIP;
				r_MenzhuPort = EquipModeList.GetSoftWare[i].GetPort;
				r_MenzhuCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "透明展柜1":
				r_Zhangui1IP = EquipModeList.GetSoftWare[i].GetIP;
				r_Zhangui1Port = EquipModeList.GetSoftWare[i].GetPort;
				r_Zhangui1Com = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "透明展柜2":
				r_Zhangui2IP = EquipModeList.GetSoftWare[i].GetIP;
				r_Zhangui2Port = EquipModeList.GetSoftWare[i].GetPort;
				r_Zhangui2Com = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "透明展柜3":
				r_Zhangui3IP = EquipModeList.GetSoftWare[i].GetIP;
				r_Zhangui3Port = EquipModeList.GetSoftWare[i].GetPort;
				r_Zhangui3Com = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "透明展柜4":
				r_Zhangui4IP = EquipModeList.GetSoftWare[i].GetIP;
				r_Zhangui4Port = EquipModeList.GetSoftWare[i].GetPort;
				r_Zhangui4Com = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "透明展柜5":
				r_Zhangui5IP = EquipModeList.GetSoftWare[i].GetIP;
				r_Zhangui5Port = EquipModeList.GetSoftWare[i].GetPort;
				r_Zhangui5Com = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "环幕主机":
				r_HuanmuIP = EquipModeList.GetSoftWare[i].GetIP;
				r_HuanmuPort = EquipModeList.GetSoftWare[i].GetPort;
				r_HuanmuCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "地幕主机":
				r_DimuIP = EquipModeList.GetSoftWare[i].GetIP;
				r_DimuPort = EquipModeList.GetSoftWare[i].GetPort;
				r_DimuCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "绿色低碳主机":
				r_DiTanIP = EquipModeList.GetSoftWare[i].GetIP;
				r_DiTanPort = EquipModeList.GetSoftWare[i].GetPort;
				r_DiTanCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "党建赋能主机":
				r_DangjianIP = EquipModeList.GetSoftWare[i].GetIP;
				r_DangjianPort = EquipModeList.GetSoftWare[i].GetPort;
				r_DangjianCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			case "互动飞屏一体机":
				r_FeiPingIP = EquipModeList.GetSoftWare[i].GetIP;
				r_FeiPingPort = EquipModeList.GetSoftWare[i].GetPort;
				r_FeiPingCom = EquipModeList.GetSoftWare[i].GetCom;
				break;
			}
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Q))
		{
			EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open1, RedModelList[0].GetredIp);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close1, RedModelList[0].GetredIp);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Open2, RedModelList[0].GetredIp);
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			EventCenter.BroadCast(Eventdefine.TcpSendSingle90, RedModelList[0].GetOrder.Close2, RedModelList[0].GetredIp);
		}
	}

	private void ReceveTxt(string code)
	{
		receiverTxt.text = code;
		if (code != null)
		{
			switch (code)
			{
			case "FE 05 00 00 FF 00 98 35":
				break;
			case "FE 05 00 00 00 00 D9 C5":
				break;
			case "FE 05 00 01 FF 00 C9 F5":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 01 00 00 88 05":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 02 FF 00 39 F5":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 02 00 00 78 05":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 03 FF 00 68 35":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 03 00 00 29 C5":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 04 FF 00 D9 F4":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 04 00 00 98 04":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 05 FF 00 88 34":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 05 00 00 C9 C4":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 06 FF 00 78 34":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 06 00 00 39 C4":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			case "FE 05 00 07 FF 00 29 F4":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = true;
				break;
			case "FE 05 00 07 00 00 68 04":
				LaoLeaderComTog.transform.Find("state").GetComponent<Toggle>().isOn = false;
				break;
			}
		}
	}

	public EquipArr UnityWebRequestJsonString(string fileName)
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.LogWarning("安卓端配置读取已改为异步初始化，返回当前缓存: " + fileName);
		return EquipModeList;
	#else
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		return JsonConvert.DeserializeObject<EquipArr>(File.ReadAllText(path));
	#endif
	}

	public List<RelayModel> GetRelayItemList()
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		return RelayModelList;
	#else
		string path = Path.Combine(Application.streamingAssetsPath, "设备控制器", "Relay.json");
		return JsonConvert.DeserializeObject<RelayModelList>(File.ReadAllText(path)).GetRelayModelList;
	#endif
	}

	public List<RedModel> GetRedItemList()
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		return RedModelList;
	#else
		string path = Path.Combine(Application.streamingAssetsPath, "设备控制器", "Red.json");
		return JsonConvert.DeserializeObject<RedModelList>(File.ReadAllText(path)).GetRedModelList;
	#endif
	}

	private IEnumerator LoadStreamingText(string fileName, Action<string> completed)
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		string uri = "jar:file://" + Application.dataPath + "!/assets/" + fileName;
		using (UnityWebRequest request = UnityWebRequest.Get(uri))
		{
			request.timeout = 10;
			yield return request.SendWebRequest();
			if (request.result != UnityWebRequest.Result.Success)
			{
				Debug.LogError("读取中控配置失败 " + fileName + ": " + request.error);
				completed(null);
				yield break;
			}
			completed(request.downloadHandler.text);
		}
	#else
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		string text = null;
		try
		{
			text = File.ReadAllText(path);
		}
		catch (Exception ex)
		{
			Debug.LogError("读取中控配置失败 " + path + ": " + ex.Message);
		}
		completed(text);
		yield break;
	#endif
	}

	private void OnDestroy()
	{
	}

	private void OnDisable()
	{
		if (equipBatchCoroutine != null)
		{
			StopCoroutine(equipBatchCoroutine);
			equipBatchCoroutine = null;
		}
		if (lightBatchCoroutine != null)
		{
			StopCoroutine(lightBatchCoroutine);
			lightBatchCoroutine = null;
		}
		if (activeEquipBatchToggle != null)
		{
			activeEquipBatchToggle.SetIsOnWithoutNotify(false);
			activeEquipBatchToggle = null;
		}
		if (activeLightBatchToggle != null)
		{
			activeLightBatchToggle.SetIsOnWithoutNotify(false);
			activeLightBatchToggle = null;
		}
		RestoreBatchControls(equipBatchInteractableStates);
		RestoreBatchControls(lightBatchInteractableStates);
	}

	private static void AttachToggleDiagnostics(Toggle[] toggles, string action)
	{
		if (toggles == null)
		{
			return;
		}
		for (int i = 0; i < toggles.Length; i++)
		{
			Toggle toggle = toggles[i];
			if (toggle == null)
			{
				continue;
			}
			string toggleName = toggle.name;
			toggle.onValueChanged.AddListener(delegate(bool isOn)
			{
				ControlDiagnostics.Record("UI", action, toggleName, isOn ? "ON" : "OFF", "active=" + toggle.gameObject.activeInHierarchy + " interactable=" + toggle.interactable);
			});
		}
	}

	private void StartEquipBatch(Toggle[] toggles, Toggle batchToggle)
	{
		if (toggles == null || toggles.Length == 0)
		{
			Debug.LogWarning("Equipment batch has no controls to execute");
			ControlDiagnostics.Record("BATCH", "EQUIPMENT_START", batchToggle == null ? string.Empty : batchToggle.name, "EMPTY", string.Empty);
			if (batchToggle != null)
			{
				batchToggle.SetIsOnWithoutNotify(false);
			}
			return;
		}
		if (equipBatchCoroutine != null)
		{
			Debug.LogWarning("Equipment batch command ignored because another equipment batch is still running");
			ControlDiagnostics.Record("BATCH", "EQUIPMENT_START", batchToggle == null ? string.Empty : batchToggle.name, "BUSY", "active=" + (activeEquipBatchToggle == null ? string.Empty : activeEquipBatchToggle.name));
			if (batchToggle != null && batchToggle != activeEquipBatchToggle)
			{
				batchToggle.SetIsOnWithoutNotify(false);
			}
			return;
		}
		ControlDiagnostics.Record("BATCH", "EQUIPMENT_START", batchToggle == null ? string.Empty : batchToggle.name, "STARTED", "count=" + (toggles == null ? 0 : toggles.Length));
		LockBatchControls(true);
		activeEquipBatchToggle = batchToggle;
		equipBatchCoroutine = StartCoroutine(RunToggleBatch(toggles, batchToggle, true));
	}

	private void StartLightBatch(Toggle[] toggles, Toggle batchToggle)
	{
		if (toggles == null || toggles.Length == 0)
		{
			Debug.LogWarning("Light batch has no controls to execute");
			ControlDiagnostics.Record("BATCH", "LIGHT_START", batchToggle == null ? string.Empty : batchToggle.name, "EMPTY", string.Empty);
			if (batchToggle != null)
			{
				batchToggle.SetIsOnWithoutNotify(false);
			}
			return;
		}
		if (lightBatchCoroutine != null)
		{
			Debug.LogWarning("Light batch command ignored because another light batch is still running");
			ControlDiagnostics.Record("BATCH", "LIGHT_START", batchToggle == null ? string.Empty : batchToggle.name, "BUSY", "active=" + (activeLightBatchToggle == null ? string.Empty : activeLightBatchToggle.name));
			if (batchToggle != null && batchToggle != activeLightBatchToggle)
			{
				batchToggle.SetIsOnWithoutNotify(false);
			}
			return;
		}
		ControlDiagnostics.Record("BATCH", "LIGHT_START", batchToggle == null ? string.Empty : batchToggle.name, "STARTED", "count=" + (toggles == null ? 0 : toggles.Length));
		LockBatchControls(false);
		activeLightBatchToggle = batchToggle;
		lightBatchCoroutine = StartCoroutine(RunToggleBatch(toggles, batchToggle, false));
	}

	private IEnumerator RunToggleBatch(Toggle[] toggles, Toggle batchToggle, bool isEquipmentBatch)
	{
		if (toggles == null || toggles.Length == 0)
		{
			Debug.LogWarning("一键控制未找到可执行的设备开关");
			ResetBatchState(batchToggle, isEquipmentBatch);
			yield break;
		}

		for (int i = 0; i < toggles.Length; i++)
		{
			Toggle target = toggles[i];
			if (target == null || !target.gameObject.activeInHierarchy)
			{
				continue;
			}

			// A Toggle does not invoke onValueChanged when it is already true.
			// Reset it without notification first so a batch command always sends.
			target.SetIsOnWithoutNotify(false);
			target.isOn = true;
			ControlDiagnostics.Record("BATCH", isEquipmentBatch ? "EQUIPMENT_ITEM" : "LIGHT_ITEM", target.name, "TRIGGERED", "index=" + i);
			yield return new WaitForSecondsRealtime(isEquipmentBatch ? 2.6f : 0.8f);
		}

		ResetBatchState(batchToggle, isEquipmentBatch);
	}

	private void ResetBatchState(Toggle batchToggle, bool isEquipmentBatch)
	{
		ControlDiagnostics.Record("BATCH", isEquipmentBatch ? "EQUIPMENT_END" : "LIGHT_END", batchToggle == null ? string.Empty : batchToggle.name, "COMPLETED", string.Empty);
		if (batchToggle != null)
		{
			batchToggle.SetIsOnWithoutNotify(false);
		}
		if (isEquipmentBatch)
		{
			RestoreBatchControls(equipBatchInteractableStates);
			equipBatchCoroutine = null;
			activeEquipBatchToggle = null;
		}
		else
		{
			RestoreBatchControls(lightBatchInteractableStates);
			lightBatchCoroutine = null;
			activeLightBatchToggle = null;
		}
	}

	private void LockBatchControls(bool isEquipmentBatch)
	{
		Dictionary<Toggle, bool> states = isEquipmentBatch ? equipBatchInteractableStates : lightBatchInteractableStates;
		states.Clear();
		if (isEquipmentBatch)
		{
			RememberAndDisable(TogArrEquip, states);
			RememberAndDisable(TogArrEquipClose, states);
			RememberAndDisable(AllEquipOpenTog, states);
			RememberAndDisable(AllEquipCloseTog, states);
		}
		else
		{
			RememberAndDisable(TogArrLightOpen, states);
			RememberAndDisable(TogArrLightClose, states);
			RememberAndDisable(AllLightOpenTog, states);
			RememberAndDisable(AllLightCloseTog, states);
		}
	}

	private static void RememberAndDisable(Toggle[] toggles, Dictionary<Toggle, bool> states)
	{
		if (toggles == null)
		{
			return;
		}
		for (int i = 0; i < toggles.Length; i++)
		{
			RememberAndDisable(toggles[i], states);
		}
	}

	private static void RememberAndDisable(Toggle toggle, Dictionary<Toggle, bool> states)
	{
		if (toggle == null || states.ContainsKey(toggle))
		{
			return;
		}
		states.Add(toggle, toggle.interactable);
		toggle.interactable = false;
	}

	private static void RestoreBatchControls(Dictionary<Toggle, bool> states)
	{
		foreach (KeyValuePair<Toggle, bool> state in states)
		{
			if (state.Key != null)
			{
				state.Key.interactable = state.Value;
			}
		}
		states.Clear();
	}

	private IEnumerator StartAllRelayOpen()
	{
		for (int i = 0; i < RelayModelList.Count; i++)
		{
			for (int j = 0; j < RelayModelList[i].GetOrder.OPenList.Count; j++)
			{
				yield return new WaitForSeconds(1f);
				EventCenter.BroadCast(Eventdefine.TcpSendSingle80, RelayModelList[i].GetOrder.OPenList[j], RelayModelList[i].GetrelayIp);
			}
		}
	}

	private IEnumerator StartAllRelayClose()
	{
		for (int i = 0; i < RelayModelList.Count; i++)
		{
			for (int j = 0; j < RelayModelList[i].GetOrder.CloseList.Count; j++)
			{
				yield return new WaitForSeconds(1f);
				EventCenter.BroadCast(Eventdefine.TcpSendSingle80, RelayModelList[i].GetOrder.CloseList[j], RelayModelList[i].GetrelayIp);
			}
		}
	}
}
