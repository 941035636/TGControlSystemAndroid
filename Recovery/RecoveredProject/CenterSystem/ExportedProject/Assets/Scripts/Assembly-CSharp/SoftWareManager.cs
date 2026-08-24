using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class SoftWareManager : MonoBehaviour
{
	public SoftWareArr SoftWareModeList;

	public Toggle xuTingTog;

	public Toggle xuanchuantog;

	private string ip_xuanchuan;

	private int port_xuanchuan;

	public Toggle zhengRongTog;

	public Toggle lingdaotog;

	public Toggle chukongtog;

	public Toggle menzhutog;

	private string ip_texiao;

	private int port_texiao;

	private string ip_lingdao;

	private int port_lingdao;

	private string ip_chukong;

	private int port_chukong;

	private string ip_menzhu;

	private int port_menzhu;

	public Toggle rongChuangTog;

	public Toggle rongyutog;

	private string ip_rongyu;

	private int port_rongyu;

	public Toggle chanPinTog;

	public Toggle banjuantog;

	public Toggle juegangtog;

	public Toggle jianzhutog;

	public Toggle maogangtog;

	public Toggle tezhonggangtog;

	private string ip_banjuan;

	private int port_banjuan;

	private string ip_juegang;

	private int port_juegang;

	private string ip_jianzhu;

	private int port_jianzhu;

	private string ip_maogang;

	private int port_maogang;

	private string ip_tezhonggang;

	private int port_tezhonggang;

	public Toggle yuanYvZhouTog;

	public Toggle _3ATog;

	public Toggle chanpinyingyongtog;

	public Toggle gangtieyingyongtog;

	public Toggle _3ATog_1;

	public Toggle chanpinyingyongtog_1;

	public Toggle gangtieyingyongtog_1;

	private string ip_3A_1;

	private int port_3A_1;

	private string ip_3A_2;

	private int port_3A_2;

	private string ip_TGchanpin_1;

	private int port_TGchanpin_1;

	private string ip_gangTe_1;

	private int port_gangTe_1;

	private string ip_TGchanpin_2;

	private int port_TGchanpin_2;

	private string ip_gangTe_2;

	private int port_gangTe_2;

	public Toggle shuziTog;

	public Toggle shuzhitog;

	public Toggle ditantog;

	private string ip_shuzhi;

	private int port_shuzhi;

	private string ip_ditan;

	private int port_ditan;

	public Toggle ganDangTog;

	private string ip_wenhua;

	private int port_wenhua;

	public Toggle wenhuatog;

	public Toggle dangJianTog;

	public Toggle dangjiantog;

	public Toggle paizhaotog;

	private string ip_dangjian;

	private int port_dangjian;

	private string ip_paizhao;

	private int port_paizhao;

	public Transform VideoPlayContent;

	public Text tipps;

	public List<string> XuanchuanVideoList = new List<string>();

	public List<string> TexiaoVideoList = new List<string>();

	public List<string> yuanyyVideoList = new List<string>();

	public List<string> shengqiVideoList = new List<string>();

	public static string ChoiceVideoName;

	private string HwIP;

	private string HWPort;

	private IEnumerator Start()
	{
		string json = null;
		yield return LoadStreamingText("softController/SoftWare.json", value => json = value);
		try
		{
			SoftWareModeList = JsonConvert.DeserializeObject<SoftWareArr>(json);
		}
		catch (Exception ex)
		{
			Debug.LogError("软件控制配置解析失败: " + ex.Message);
			yield break;
		}
		if (SoftWareModeList == null)
		{
			Debug.LogError("软件控制配置为空，停止初始化软件控制界面");
			yield break;
		}
		ReadIPs(SoftWareModeList);
		init();
	}

	public SoftWareArr GetRelayItemList()
	{
		try
		{
			new SoftWareArr();
			using StreamReader streamReader = new StreamReader("jar.file://" + Application.dataPath + "!/assets/softController/SoftWare.json");
			SoftWareArr result = JsonConvert.DeserializeObject<SoftWareArr>(streamReader.ReadToEnd());
			tipps.text = "没毛病";
			return result;
		}
		catch (Exception ex)
		{
			tipps.text = "故障:" + ex.ToString();
			Debug.LogError(ex.ToString());
			return null;
		}
	}

	public SoftWareArr UnityWebRequestJsonString(string fileName)
	{
	#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.LogWarning("安卓端软件配置已改为异步初始化，返回当前缓存: " + fileName);
		return SoftWareModeList;
	#else
		string path = Path.Combine(Application.streamingAssetsPath, fileName);
		return JsonConvert.DeserializeObject<SoftWareArr>(File.ReadAllText(path));
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
				Debug.LogError("读取软件控制配置失败 " + fileName + ": " + request.error);
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
			Debug.LogError("读取软件控制配置失败 " + path + ": " + ex.Message);
		}
		completed(text);
		yield break;
	#endif
	}

	private void ReadIPs(SoftWareArr SoftWareModeList)
	{
		for (int i = 0; i < SoftWareModeList.GetSoftWare.Count; i++)
		{
			switch (SoftWareModeList.GetSoftWare[i].GetName)
			{
			case "宣传片":
				ip_xuanchuan = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_xuanchuan = SoftWareModeList.GetSoftWare[i].GetPort1;
				XuanchuanVideoList = SoftWareModeList.GetSoftWare[i].GetVideoList;
				break;
			case "特效投影":
				ip_texiao = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_texiao = SoftWareModeList.GetSoftWare[i].GetPort1;
				TexiaoVideoList = SoftWareModeList.GetSoftWare[i].GetVideoList;
				break;
			case "老领导话石特":
				ip_lingdao = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_lingdao = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "触控一体机":
				ip_chukong = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_chukong = SoftWareModeList.GetSoftWare[i].GetPort1;
				HwIP = SoftWareModeList.GetSoftWare[i].GetHWIP.Split(':')[0];
				HWPort = SoftWareModeList.GetSoftWare[i].GetHWIP.Split(':')[1];
				break;
			case "门柱特效":
				ip_menzhu = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_menzhu = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "领导关怀企业荣誉":
				ip_rongyu = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_rongyu = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "板卷产品介绍":
				ip_banjuan = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_banjuan = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "电力角钢产品介绍":
				ip_juegang = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_juegang = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "建筑材料产品介绍":
				ip_jianzhu = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_jianzhu = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "锚杆钢产品介绍":
				ip_maogang = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_maogang = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "特种钢产品介绍":
				ip_tezhonggang = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_tezhonggang = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "3A沉浸式旅游参观":
				ip_3A_1 = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_3A_1 = SoftWareModeList.GetSoftWare[i].GetPort1;
				ip_3A_2 = SoftWareModeList.GetSoftWare[i].GetIP2;
				port_3A_2 = SoftWareModeList.GetSoftWare[i].GetPort2;
				yuanyyVideoList = SoftWareModeList.GetSoftWare[i].GetVideoList;
				break;
			case "特钢产品应用场景":
				ip_TGchanpin_1 = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_TGchanpin_1 = SoftWareModeList.GetSoftWare[i].GetPort1;
				ip_TGchanpin_2 = SoftWareModeList.GetSoftWare[i].GetIP2;
				port_TGchanpin_2 = SoftWareModeList.GetSoftWare[i].GetPort2;
				yuanyyVideoList = SoftWareModeList.GetSoftWare[i].GetVideoList;
				break;
			case "特钢是怎样炼成的":
				ip_gangTe_1 = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_gangTe_1 = SoftWareModeList.GetSoftWare[i].GetPort1;
				ip_gangTe_2 = SoftWareModeList.GetSoftWare[i].GetIP2;
				port_gangTe_2 = SoftWareModeList.GetSoftWare[i].GetPort2;
				yuanyyVideoList = SoftWareModeList.GetSoftWare[i].GetVideoList;
				break;
			case "数智领航":
				ip_shuzhi = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_shuzhi = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "绿色低碳":
				ip_ditan = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_ditan = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "企业文化":
				ip_wenhua = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_wenhua = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "党建工作":
				ip_dangjian = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_dangjian = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			case "拍照留言":
				ip_paizhao = SoftWareModeList.GetSoftWare[i].GetIP1;
				port_paizhao = SoftWareModeList.GetSoftWare[i].GetPort1;
				break;
			}
		}
	}

	private IEnumerator ChangeIPVideoPlay(bool ischange, string ip = "", string port = "")
	{
		if (VideoPlayContent.Find("videoPlayControl") != null)
		{
			yield return new WaitForSeconds(0.02f);
			if (ischange)
			{
				EventCenter.BroadCast(Eventdefine.socketQuit);
				yield return new WaitForEndOfFrame();
				EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port);
			}
			else
			{
				UnityEngine.Object.Destroy(VideoPlayContent.Find("videoPlayControl").gameObject);
			}
		}
		if (VideoPlayContent.Find("videoPlayControl2") != null)
		{
			EventCenter.BroadCast(Eventdefine.closeVideo2);
			yield return new WaitForSeconds(0.02f);
			if (ischange)
			{
				EventCenter.BroadCast(Eventdefine.socketQuit2);
				yield return new WaitForEndOfFrame();
				EventCenter.BroadCast(Eventdefine.InitSocketIP_port2, ip, port);
			}
			else
			{
				UnityEngine.Object.Destroy(VideoPlayContent.Find("videoPlayControl2").gameObject);
			}
		}
	}

	private IEnumerator ChangeIPSoftControl(bool ischange, string ip = "", string port = "")
	{
		if (VideoPlayContent.Find("SoftControl") != null)
		{
			yield return new WaitForSeconds(0.2f);
			if (ischange)
			{
				EventCenter.BroadCast(Eventdefine.socketQuit);
				yield return new WaitForEndOfFrame();
				EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port);
			}
			else
			{
				UnityEngine.Object.Destroy(VideoPlayContent.Find("SoftControl").gameObject);
			}
		}
	}

	private IEnumerator ChangeIPFlagControl(bool ischange, string ip = "", string port = "")
	{
		if (VideoPlayContent.Find("FlagControl") != null)
		{
			yield return new WaitForSeconds(0.02f);
			if (ischange)
			{
				EventCenter.BroadCast(Eventdefine.socketQuit);
				yield return new WaitForEndOfFrame();
				EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port);
			}
			else
			{
				UnityEngine.Object.Destroy(VideoPlayContent.Find("FlagControl").gameObject);
			}
		}
	}

	private IEnumerator DealySetFMIP(string IP)
	{
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.FMIP, IP);
	}

	private IEnumerator DealySetFMIP2(string IP)
	{
		yield return new WaitForSeconds(1f);
		EventCenter.BroadCast(Eventdefine.FMIP2, IP);
	}

	private IEnumerator DelayGenVideoPlay(string ip1, int port1, List<string> PlayList, bool addudp2 = false, string ip2 = "", int port2 = 0)
	{
		yield return new WaitForSeconds(0.3f);
		if (!(VideoPlayContent.Find("videoPlayControl") == null))
		{
			yield break;
		}
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TGPrefab/videoPlayControl"));
		gameObject.name = "videoPlayControl";
		gameObject.transform.SetParent(VideoPlayContent);
		gameObject.transform.localPosition = new Vector3(0f, 7f, 0f);
		gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
		for (int i = 0; i < PlayList.Count; i++)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TGPrefab/item"));
			obj.transform.SetParent(VideoPlayContent.Find("videoPlayControl/ListScroll/Viewport/Content"));
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			string _videoname = PlayList[i];
			Debug.LogError("选择了：" + _videoname);
			obj.transform.Find("nameTxt").GetComponent<Text>().text = _videoname;
			obj.GetComponent<Toggle>().onValueChanged.AddListener(delegate
			{
				if (_videoname != null)
				{
					switch (_videoname)
					{
					case "一路向新":
						ChoiceVideoName = "yiluxiangxin";
						break;
					case "3A沉浸式旅游参观":
						ChoiceVideoName = "3A";
						break;
					case "特钢产品应用场景":
						ChoiceVideoName = "tgscene";
						break;
					case "特钢是怎样炼成的":
						ChoiceVideoName = "tgdo";
						break;
					}
				}
			});
			obj.GetComponent<Toggle>().group = VideoPlayContent.Find("videoPlayControl/ListScroll/Viewport/Content").GetComponent<ToggleGroup>();
		}
		gameObject.AddComponent<UDPClient>();
		EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip1, port1.ToString());
		Debug.LogError("远端IP,port" + ip1 + "," + port1);
		StartCoroutine(DealySetFMIP(ip1));
		if (addudp2)
		{
			gameObject.AddComponent<UDPClient2>();
			EventCenter.BroadCast(Eventdefine.InitSocketIP_port2, ip2, port2.ToString());
			gameObject.transform.Find("yyzcontroller").gameObject.SetActive(value: true);
		}
	}

	private IEnumerator DelayGen2VideoPlay(string ip1, int port1, string ip2, int port2)
	{
		yield return new WaitForSeconds(0.3f);
		if (VideoPlayContent.Find("videoPlayControl2") == null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TGPrefab/videoPlayControl2"));
			obj.name = "videoPlayControl2";
			obj.transform.SetParent(VideoPlayContent);
			obj.transform.localPosition = new Vector3(1.7251f, -8.19f, 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.AddComponent<UDPClient>();
			obj.AddComponent<UDPClient2>();
			EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip1, port1.ToString());
			EventCenter.BroadCast(Eventdefine.InitSocketIP_port2, ip2, port2.ToString());
			StartCoroutine(DealySetFMIP(ip1));
			StartCoroutine(DealySetFMIP2(ip2));
		}
	}

	private IEnumerator DelayGenSoftControl(string ip, int port, string openname)
	{
		yield return new WaitForSeconds(0.3f);
		if (VideoPlayContent.Find("SoftControl") == null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TGPrefab/SoftControl"));
			obj.name = "SoftControl";
			obj.transform.SetParent(VideoPlayContent);
			obj.transform.localPosition = new Vector3(1.7251f, -8.19f, 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.AddComponent<UDPClient>();
			EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port.ToString());
			obj.GetComponent<SoftController>().softName = openname;
		}
	}

	private IEnumerator DelayGenFlagControl(string ip, int port)
	{
		yield return new WaitForSeconds(0.3f);
		if (VideoPlayContent.Find("FlagControl") == null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("TGPrefab/FlagControl"));
			obj.name = "FlagControl";
			obj.transform.SetParent(VideoPlayContent);
			obj.transform.localPosition = new Vector3(-812f, 103f, 0f);
			obj.transform.localScale = new Vector3(1f, 1f, 1f);
			obj.AddComponent<UDPClient>();
			EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port.ToString());
		}
	}

	private void init()
	{
		EventCenter.addlistener<string, int>(Eventdefine.XuanChuanPian, udpCNN);
		xuTingTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				xuanchuantog.isOn = true;
			}
			else
			{
				xuanchuantog.isOn = false;
			}
		});
		xuanchuantog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenVideoPlay(ip_xuanchuan, port_xuanchuan, XuanchuanVideoList));
			}
			else
			{
				StartCoroutine(ChangeIPVideoPlay(ischange: false));
			}
		});
		lingdaotog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_lingdao, port_lingdao, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		chukongtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_chukong, port_chukong, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		menzhutog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenVideoPlay(ip_menzhu, port_menzhu, TexiaoVideoList));
			}
			else
			{
				StartCoroutine(ChangeIPVideoPlay(ischange: false));
			}
		});
		zhengRongTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				menzhutog.isOn = true;
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
			else
			{
				lingdaotog.isOn = false;
				chukongtog.isOn = false;
				menzhutog.isOn = false;
			}
		});
		rongChuangTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				rongyutog.isOn = true;
			}
			else
			{
				rongyutog.isOn = false;
			}
		});
		rongyutog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_rongyu, port_rongyu, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		banjuantog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_banjuan, port_banjuan, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		juegangtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_juegang, port_juegang, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		jianzhutog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_jianzhu, port_jianzhu, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		maogangtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_maogang, port_maogang, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		tezhonggangtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_tezhonggang, port_tezhonggang, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		chanPinTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				banjuantog.isOn = true;
			}
			else
			{
				banjuantog.isOn = false;
				juegangtog.isOn = false;
				jianzhutog.isOn = false;
				maogangtog.isOn = false;
				tezhonggangtog.isOn = false;
			}
		});
		yuanYvZhouTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				_3ATog.isOn = true;
				StartCoroutine(DelayGenVideoPlay(ip_3A_1, port_3A_1, yuanyyVideoList, addudp2: true, ip_3A_2, port_3A_2));
			}
			else
			{
				StartCoroutine(ChangeIPVideoPlay(ischange: false));
			}
		});
		_3ATog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "3A";
			}
		});
		_3ATog_1.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "3A_1";
			}
		});
		chanpinyingyongtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "tgscene";
			}
		});
		chanpinyingyongtog_1.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "tgscene_1";
			}
		});
		gangtieyingyongtog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "tgdo";
			}
		});
		gangtieyingyongtog_1.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				ChoiceVideoName = "tgdo_1";
			}
		});
		shuziTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				shuzhitog.isOn = true;
			}
			else
			{
				shuzhitog.isOn = false;
				ditantog.isOn = false;
			}
		});
		shuzhitog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_shuzhi, port_shuzhi, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		ditantog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_ditan, port_ditan, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		ganDangTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				wenhuatog.isOn = true;
			}
			else
			{
				wenhuatog.isOn = false;
			}
		});
		wenhuatog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_wenhua, port_wenhua, ""));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
		dangJianTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				dangjiantog.isOn = true;
			}
			else
			{
				dangjiantog.isOn = false;
				paizhaotog.isOn = false;
			}
		});
		dangjiantog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenVideoPlay(ip_dangjian, port_dangjian, shengqiVideoList));
				StartCoroutine(DelayGenFlagControl(ip_dangjian, port_dangjian));
			}
			else
			{
				StartCoroutine(ChangeIPFlagControl(ischange: false));
				StartCoroutine(ChangeIPVideoPlay(ischange: false));
			}
		});
		paizhaotog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(DelayGenSoftControl(ip_paizhao, port_paizhao, "paizhao"));
			}
			else
			{
				StartCoroutine(ChangeIPSoftControl(ischange: false));
			}
		});
	}

	private void udpCNN(string ip, int port)
	{
		StartCoroutine(udpConnect(ip, port));
	}

	public IEnumerator udpConnect(string ip, int port)
	{
		yield return new WaitForSeconds(0.5f);
		EventCenter.BroadCast(Eventdefine.InitSocketIP_port, ip, port.ToString());
	}

	private void OnDisable()
	{
		EventCenter.RemoveListenerTwo<string, int>(Eventdefine.XuanChuanPian, udpCNN);
	}
}
