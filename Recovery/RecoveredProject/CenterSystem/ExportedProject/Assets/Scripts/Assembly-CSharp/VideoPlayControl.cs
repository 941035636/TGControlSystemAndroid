using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VideoPlayControl : MonoBehaviour
{
	public RawImage videoRawImage;

	public Text showTips;

	public Text playshowTxt;

	public Toggle playContainervideo;

	public Toggle netTog;

	public Button ForwardBtn;

	public Button fastuiBtn;

	public Toggle playTog;

	public Toggle pauseTog;

	public Toggle voiceTog;

	public Button fastjinBtn;

	public Button nextBtn;

	public Toggle playContainerpic;

	public Button VoiceAddBtn;

	public Button VoiceSubBtn;

	public Button Pic_playBtn;

	public Button Pic_pauseBtn;

	public Toggle playContainerPPT;

	public Button PPT_Up;

	public Button PPT_Down;

	public Slider controlSlider;

	public Toggle playContainerWeb;

	public GameObject yyzControl;

	public Toggle windOpenTog;

	public Toggle windCloseTog;

	private static VideoPlayControl Instance;

	private bool IsOpenX;

	private static bool ISYuanyvzhou;

	private string XiangxunOpenCode = "FF FF 00 4B 02 FD 00 01 2A 00 01 A4 2E AB 33 38 33 38 33 38 33 39 32 39 32 39 32 39 35 A8 33 39 35 A8 33 38 35 36 33 38 33 38 33 39 32 AB 38 A6 38 32 36 36 32 AB 36 A8 35 36 35 36 35 A8 35 36 36 35 36 35 36 A8 35 36 35 36 33 38 35 36 35 36 35 36 38 33 35 36 35 36 36 35 36 35 36 35 36 35 36 35 36 A8 35 36 35 36 36 35 36 35 36 35 36 35 36 35 36 35 36 35 36 36 35 36 35 36 35 36 38 33 35 A8 33 38 39 A5 33 38 35 36 38 33 36 35 36 A8 35 36 35 36 35 36 35 36 35 36 36 35 36 35 36 36 35 A8 36 A8 35 36 35 36 35 36 36 35 36 35 36 35 36 35 36 36 35 36 35 36 35 36 38 33 35 36 36 35 36 35 36 35 36 36 35 36 35 36 35 36 38 33 35 36 36 35 36 35 36 35 39 32 36 36 38 33 38 33 38 33 39 33 38 33 38 33 38 33 38 33 39 33 38 33 38 33 38 A6 38 A6 38 33 38 33 38 33 39 32 39 32 39 32 39 A8 36 A8 35 33 38 33 39 32 38 36 36 32 39 35 36 35 36 35 36 36 35 36 38 33 38 33 38 33 36 35 38 33 39 32 39 32 39 32 39 32 36 35 36 36 35 36 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 39 32 39 32 39 32 39 33 38 33 38 33 38 33 38 33 38 A6 38 A5 39 32 39 32 39 32 39 32 39 32 39 32 39 A5 38 A5 39 32 39 32 39 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 39 32 39 32 39 32 39 2F 3C 33 38 2F 3C 33 35 33 3B 33 38 30 3B 33 35 33 3C 2F 3C 32 39 2F 3C 32 39 2F 3C 32 38 30 3C 2F 3C 32 3A 2E 3C 33 34 34 3B A2 3C A2 3B 30 3B 30 3B 30 3B 30 3B 30 3B 30 3C A2 3B A2 3B 30 3B 30 31 3D 38 30 3C 2F 3B 33 39 2F 3B 30 3B A2 3C 2F 3C 2F 3B 30 3B 30 3B 30 3B 30 3B 30 3B A2 31 3A 3C 2F 3B 30 31 3B 30 3B 30 3B 31 3A 31 AD 30 3B 31 3A 31 3B 30 3B 31 3A 31 3A 31 3B 30 AD 31 3D 2E 3B 30 3B 30 AD 31 AD 30 3B 30 AD 31 AD 30 3E 2E 3A 31 3D 2E AD 30 3B 31 AC 31 3B 30 AD 31 3A 31 AD 30 3E 2E 3D 2E AD 31 3A 31 AD 30 3B 30 AD 31 3B 30 B0 2E AD 30 3D 2E AD 31 3B 30 AD 31 3D 2E AF 2E 3E 00 01 3C 16";

	private string XiangxunCloseCode = "FF FF 00 4B 02 FD 00 01 27 00 01 A8 37 2F 37 A7 3C 2F 37 34 37 34 38 33 38 33 38 34 37 A6 3C 2F 38 A6 3C 2E 38 33 38 34 37 34 37 34 37 A7 3B 2F 38 A8 3A 2E 38 A8 3A A4 39 2F 37 34 37 A7 3C 2E 38 33 38 33 38 A6 3C 2F 37 34 37 34 37 34 37 34 38 33 38 33 37 34 38 33 37 34 37 34 37 34 37 34 37 34 37 A7 3B 2F 38 33 38 33 38 33 37 34 38 33 37 34 38 33 38 33 37 34 38 33 38 33 38 33 38 33 38 A6 37 34 37 A7 37 33 38 33 38 33 38 33 38 A6 37 34 37 34 37 34 38 33 38 33 38 33 38 33 38 33 38 A6 37 A9 35 33 38 33 38 33 38 33 38 33 35 37 35 36 37 34 35 36 35 36 35 36 35 36 35 36 35 36 35 36 35 36 35 37 35 36 35 36 35 36 35 36 35 36 35 36 35 36 35 36 35 36 35 36 38 33 38 33 38 33 38 33 35 36 38 34 35 36 35 36 35 36 35 36 35 36 35 A9 35 A9 34 37 32 39 32 39 32 39 33 38 33 39 32 AB 33 AE 2F 39 32 39 32 39 32 39 33 38 33 38 33 38 33 39 32 39 32 39 35 36 35 36 35 36 35 36 35 36 35 37 34 37 35 36 35 36 35 36 35 36 35 36 35 36 35 37 35 39 32 39 35 36 35 36 35 36 38 33 36 35 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 A6 35 A8 36 35 36 36 35 36 38 33 38 33 38 33 38 A5 39 A5 38 33 38 33 38 33 39 32 39 32 39 32 39 32 39 32 39 32 39 32 39 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 39 32 39 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 38 33 37 35 35 36 38 33 38 33 38 33 38 33 38 33 39 33 38 A2 3C A1 3C 33 38 2F 3C 32 35 33 3C 32 35 33 3C A1 3C A2 3B 30 3C 32 34 34 3B 30 3C 32 38 30 3B 30 3C 32 38 A3 3B 30 3B 33 37 31 3B 33 39 2F 3B 30 31 3D 38 A3 3A 31 31 3D 39 2F 31 3E 39 2F 31 3D 39 2F 3B A3 3B 33 39 2F 3B 33 39 2F 3C 2F 3C 32 39 30 3B A2 3B 30 3B 31 3A 31 38 33 31 AC 3C A2 33 AB 33 AB 33 38 33 39 33 38 33 AB 33 38 33 AB 33 38 33 AB 33 39 33 AA 33 39 33 38 33 AB 33 38 33 AB 33 38 33 AB 33 38 33 AB 33 AA 33 39 33 AA 33 39 33 AA 33 38 33 AB 33 38 00 01 41 16";

	private string FengOpenCode = "FF 01 00 48 01 FD 86 28 84 28 2C 7E 84 28 84 28 2C 7E 2D 7E 2C 7E 2D 7E 2C 7E 2D 7E 83 00 02 E5 81 28 83 28 2D 7E 83 29 83 28 2D 7E 2D 7D 2D 7E 2C 7E 2D 7E 2D 7D 84 00 03 4A 81 28 84 28 2D 7D 84 28 84 28 2D 7D 2D 7E 2D 7D 2D 7E 2C 7E 2D 7E 83 00 02 E5 81 28 83 28 2D 7E 84 28 84 27 2D 7E 2D 7D 2D 7E 2D 7D 2D 7E 2D 7D 84 00 03 4B 81 28 83 28 2D 7E 83 29 83 28 2D 7E 2D 7D 2D 7E 2D 7D 2D 7E 2D 7D 84 00 02 E4 81 28 84 28 2D 7D 81 2B 84 28 2D 7D 2D 7E 2C 7E 2D 7E 2C 7E 2D 7E 81 00 03 4D 7F 2A 81 2B 2C 7E 81 2B 81 2B 29 82 29 81 29 82 29 82 26 84 29 82 81 00 02 E7 7E 2B 81 2A 2A 81 81 2B 81 2A 2A 81 2A 81 29 81 2A 81 29 82 29 81 81 00 03 4E 7D 2C 80 2B 29 82 80 2C 80 2B 29 82 29 82 29 82 28 82 29 82 29 82 80 00 02 E8 7D 2C 80 2B 29 82 80 2C 80 2C 28 82 28 83 26 85 28 85 26 85 23 88 7A 00 03 54 77 32 7A 32 23 88 7A 32 7A 32 22 89 22 88 23 8B 20 8B 20 8A 21 8A 78 00 02 F0 75 35 77 35 20 8B 77 35 77 35 20 8B 20 8B 20 8B 20 8B 20 8D 1D 8E 75 00 03 5C 70 37 74 38 1B 92 70 3D 6F 3D 18 98 13 9B 10 16";

	private string FengCloseCode = "FF 01 00 48 01 FD 86 28 84 28 2C 7E 84 28 84 28 2C 7E 2D 7E 2C 7E 2D 7E 2C 7E 2D 7E 83 00 02 E5 81 28 83 28 2D 7E 83 29 83 28 2D 7E 2D 7D 2D 7E 2C 7E 2D 7E 2D 7D 84 00 03 4A 81 28 84 28 2D 7D 84 28 84 28 2D 7D 2D 7E 2D 7D 2D 7E 2C 7E 2D 7E 83 00 02 E5 81 28 83 28 2D 7E 84 28 84 27 2D 7E 2D 7D 2D 7E 2D 7D 2D 7E 2D 7D 84 00 03 4B 81 28 83 28 2D 7E 83 29 83 28 2D 7E 2D 7D 2D 7E 2D 7D 2D 7E 2D 7D 84 00 02 E4 81 28 84 28 2D 7D 81 2B 84 28 2D 7D 2D 7E 2C 7E 2D 7E 2C 7E 2D 7E 81 00 03 4D 7F 2A 81 2B 2C 7E 81 2B 81 2B 29 82 29 81 29 82 29 82 26 84 29 82 81 00 02 E7 7E 2B 81 2A 2A 81 81 2B 81 2A 2A 81 2A 81 29 81 2A 81 29 82 29 81 81 00 03 4E 7D 2C 80 2B 29 82 80 2C 80 2B 29 82 29 82 29 82 28 82 29 82 29 82 80 00 02 E8 7D 2C 80 2B 29 82 80 2C 80 2C 28 82 28 83 26 85 28 85 26 85 23 88 7A 00 03 54 77 32 7A 32 23 88 7A 32 7A 32 22 89 22 88 23 8B 20 8B 20 8A 21 8A 78 00 02 F0 75 35 77 35 20 8B 77 35 77 35 20 8B 20 8B 20 8B 20 8B 20 8D 1D 8E 75 00 03 5C 70 37 74 38 1B 92 70 3D 6F 3D 18 98 13 9B 10 16";

	private float _3AStartimer = 23f;

	private float tgsceneStartWind = 55f;

	private float tgdoStartWind = 32f;

	public static VideoPlayControl GetInstance()
	{
		if (Instance == null)
		{
			Instance = new VideoPlayControl();
		}
		return Instance;
	}

	private void Start()
	{
		playContainervideo.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.VideoSceneChange);
			}
		});
		playTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.videoplay2, SoftWareManager.ChoiceVideoName);
				}
				Debug.LogError("开始播放:" + SoftWareManager.ChoiceVideoName);
				EventCenter.BroadCast(Eventdefine.videoplay, SoftWareManager.ChoiceVideoName);
				if (SoftWareManager.ChoiceVideoName == "3A" || SoftWareManager.ChoiceVideoName == "tgscene" || SoftWareManager.ChoiceVideoName == "tgdo")
				{
					ISYuanyvzhou = true;
					Debug.LogError("ISYuanyvzhou设置为：" + ISYuanyvzhou);
				}
			}
			else
			{
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.closeVideo2);
				}
				EventCenter.BroadCast(Eventdefine.closeVideo);
			}
		});
		pauseTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.videopause2);
				}
				EventCenter.BroadCast(Eventdefine.videopause);
			}
			else
			{
                Debug.LogError("从暂停转为播放:" + SoftWareManager.ChoiceVideoName);
                EventCenter.BroadCast(Eventdefine.videoplay, SoftWareManager.ChoiceVideoName);
				if (base.transform.GetComponent<UDPClient2>() != null)
                {
                    Debug.LogError("从暂停转为播放udpClient2:" + SoftWareManager.ChoiceVideoName);
                    EventCenter.BroadCast(Eventdefine.videoplay2, SoftWareManager.ChoiceVideoName);
				}
			}
		});
		voiceTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.voicestop2);
				}
				EventCenter.BroadCast(Eventdefine.voicestop);
			}
			else
			{
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.voicestart2);
				}
				EventCenter.BroadCast(Eventdefine.voicestart);
			}
		});
		VoiceAddBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.voiceadd);
		});
		VoiceSubBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.voicesub);
		});
		ForwardBtn.onClick.AddListener(delegate
		{
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.forwardplay2);
			}
			EventCenter.BroadCast(Eventdefine.forwardplay);
		});
		float rate = 1f;
		fastjinBtn.onClick.AddListener(delegate
		{
			rate += 0.2f;
			if (rate >= 2f)
			{
				rate = 2f;
			}
			showTips.text = "X" + rate;
			EventCenter.BroadCast(Eventdefine.kuaijin, rate.ToString());
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.kuaijin2, rate.ToString());
			}
		});
		fastuiBtn.onClick.AddListener(delegate
		{
			rate -= 0.2f;
			rate = Mathf.Round(rate * 10f) / 10f;
			if (rate <= 0.2f)
			{
				rate = 0.2f;
			}
			showTips.text = "X" + rate;
			EventCenter.BroadCast(Eventdefine.kuaitui, rate.ToString());
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.kuaitui2, rate.ToString());
			}
		});
		nextBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.nextplay);
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.nextplay2);
			}
		});
		playContainerpic.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.PicSceneChange);
				if (base.transform.GetComponent<UDPClient2>() != null)
				{
					EventCenter.BroadCast(Eventdefine.PicSceneChange2);
				}
			}
		});
		Pic_playBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.Picplay);
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.Picplay2);
			}
		});
		Pic_pauseBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.Picpause);
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.Picpause2);
			}
		});
		controlSlider.onValueChanged.AddListener(delegate(float value)
		{
			Debug.LogError("Value:" + value);
			EventCenter.BroadCast(Eventdefine.controlPic, value);
			if (base.transform.GetComponent<UDPClient2>() != null)
			{
				EventCenter.BroadCast(Eventdefine.controlPic2, value);
			}
		});
		playContainerWeb.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.WebSceneChange);
			}
		});
		playContainerPPT.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.PPTSceneChange);
			}
		});
		windOpenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, FengOpenCode, "192.168.86.90");
			}
		});
		windCloseTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, FengCloseCode, "192.168.86.90");
			}
		});
	}

	private void Update()
	{
		if (!ISYuanyvzhou)
		{
			return;
		}
		_ = Time.time;
		if (SoftWareManager.ChoiceVideoName == "3A")
		{
			_3AStartimer -= Time.deltaTime;
			if (_3AStartimer <= 0f && !IsOpenX)
			{
				IsOpenX = true;
				Debug.LogError("打开香薰:");
				StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, XiangxunOpenCode, "192.168.86.90");
			}
			if (_3AStartimer < -5f && IsOpenX)
			{
				IsOpenX = false;
				Debug.LogError("关闭香薰:");
				ISYuanyvzhou = false;
				_3AStartimer = 25f;
				StartCoroutine(TCPConn2Send90("192.168.86.90", 502));
				EventCenter.BroadCast(Eventdefine.TcpSendSingle90, XiangxunCloseCode, "192.168.86.90");
			}
		}
	}

	private IEnumerator TCPConn2Send90(string ip, int port)
	{
		EventCenter.BroadCast(Eventdefine.TcpConnect90, ip, port);
		yield return new WaitForSeconds(2f);
	}

	private void OnDestroy()
	{
	}
}
