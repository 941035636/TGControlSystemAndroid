using System.Collections.Generic;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class ControlVideoPanel : MonoBehaviour
{
	private Toggle OpenTog;

	private Button ReplayBtn;

	private Toggle CloseBtn;

	private Button ShutDownBtn;

	private Button backBtn;

	private Text DescriTxt;

	private Button OpenTV;

	private Button CloseTV;

	private Button VolumeAdd;

	private Button VolumeSub;

	private Slider VolumeSlider;

	private Button KuaiJinBtn;

	private Button KuaiTuiBtn;

	private Button PhotoBtn;

	private Button VideoChangeBtn;

	private Button PicchangeBtn;

	private Toggle PicTog;

	private Text Nametxt;

	public static string ip;

	public static string port;

	public static string path;

	public new static string name;

	public static string note;

	public static float volueS;

	public static bool istv;

	public static bool isphoto;

	public static string Item_Name = string.Empty;

	public string PlayVideoName = "";

	public static float SpeedRate = 1f;

	public static List<string> ShowContentList = new List<string>();

	private Toggle[] TogArr;

	private static ControlVideoPanel Instance;

	public static ControlVideoPanel GetInstance()
	{
		if (Instance == null)
		{
			Instance = new ControlVideoPanel();
		}
		return Instance;
	}

	private void Start()
	{
		VolumeAdd = base.transform.Find("VolumeAdd").GetComponent<Button>();
		VolumeSub = base.transform.Find("VolumeSub").GetComponent<Button>();
		Nametxt = base.transform.Find("NameTxt").GetComponent<Text>();
		backBtn = base.transform.Find("Back").GetComponent<Button>();
		OpenTog = base.transform.Find("Open").GetComponent<Toggle>();
		ReplayBtn = base.transform.Find("RePlayBtn").GetComponent<Button>();
		CloseBtn = base.transform.Find("Close").GetComponent<Toggle>();
		KuaiJinBtn = base.transform.Find("KuaiJin").GetComponent<Button>();
		KuaiTuiBtn = base.transform.Find("KuaiTui").GetComponent<Button>();
		DescriTxt = base.transform.Find("TitleTxt/DescriTxt").GetComponent<Text>();
		VolumeSlider = base.transform.Find("Slider").GetComponent<Slider>();
		OpenTV = base.transform.Find("OpenTV").GetComponent<Button>();
		CloseTV = base.transform.Find("CloseTV").GetComponent<Button>();
		ShutDownBtn = base.transform.Find("ShutDown").GetComponent<Button>();
		PhotoBtn = base.transform.Find("PhotoBtn").GetComponent<Button>();
		PicTog = base.transform.Find("PicPlay").GetComponent<Toggle>();
		VideoChangeBtn = base.transform.Find("videochangeBtn").GetComponent<Button>();
		PicchangeBtn = base.transform.Find("picchangeBtn").GetComponent<Button>();
		backBtn.onClick.AddListener(delegate
		{
			Object.Destroy(base.gameObject);
			SetTogInter(ison: true);
		});
		TogArr = base.transform.Find("ResParent/Scroll View/Viewport/Content").GetComponents<Toggle>();
		PhotoBtn.gameObject.SetActive(isphoto);
		OpenTV.gameObject.SetActive(istv);
		CloseTV.gameObject.SetActive(istv);
		bool playvideo = true;
		OpenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				OpenTog.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控UI/播放控制/播放控制-视频播放器-播放-开始");
				EventCenter.BroadCast(Eventdefine.videoplay);
			}
			else
			{
				OpenTog.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控UI/播放控制/播放控制-视频播放器-播放-开始");
				EventCenter.BroadCast(Eventdefine.videopause);
			}
		});
		OpenTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (!ison)
			{
				Debug.Log(!ison);
				OpenTog.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控UI/播放控制/播放控制-视频播放器-播放-关闭");
				EventCenter.BroadCast(Eventdefine.videopause);
				playvideo = false;
			}
		});
		ReplayBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.replay);
		});
		VolumeAdd.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.volumeadd);
		});
		VolumeSub.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.volumesub);
		});
		float rate = 1f;
		KuaiJinBtn.onClick.AddListener(delegate
		{
			rate += 0.2f;
			if (rate >= 2f)
			{
				rate = 2f;
			}
			EventCenter.BroadCast(Eventdefine.kuaijin, rate.ToString());
		});
		KuaiTuiBtn.onClick.AddListener(delegate
		{
			rate -= 0.2f;
			if (rate <= 0.2f)
			{
				rate = 0.2f;
			}
			EventCenter.BroadCast(Eventdefine.kuaitui);
		});
		ShutDownBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.Shutdown);
		});
		DescriTxt.text = note;
		base.transform.Find("Tog1").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		base.transform.Find("Tog1").GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			Debug.LogError("play1");
			EventCenter.BroadCast(Eventdefine.playvideo, "1.mp4");
			PlayVideoName = " 1.mp4";
			base.transform.Find("Tog1/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-打开");
			base.transform.Find("Tog2/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog3/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog4/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog5/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
		});
		base.transform.Find("Tog2").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		base.transform.Find("Tog2").GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.playvideo, "2.mp4");
			PlayVideoName = " 2.mp4";
			Debug.LogError("play2");
			base.transform.Find("Tog1/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog2/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-打开");
			base.transform.Find("Tog3/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog4/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog5/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
		});
		base.transform.Find("Tog3").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		base.transform.Find("Tog3").GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.playvideo, "3.mp4");
			PlayVideoName = " 3.mp4";
			Debug.LogError("play3");
			base.transform.Find("Tog1/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog2/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog3/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-打开");
			base.transform.Find("Tog4/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog5/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
		});
		base.transform.Find("Tog4").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		base.transform.Find("Tog4").GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.playvideo, "4.mp4");
			PlayVideoName = " 4.mp4";
			Debug.LogError("play4");
			base.transform.Find("Tog1/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog2/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog3/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog4/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-打开");
			base.transform.Find("Tog5/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
		});
		base.transform.Find("Tog5").GetComponent<Toggle>().onValueChanged.RemoveAllListeners();
		base.transform.Find("Tog5").GetComponent<Toggle>().onValueChanged.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.playvideo, "5.mp4");
			PlayVideoName = " 5.mp4";
			Debug.LogError("play5");
			base.transform.Find("Tog1/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog2/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog3/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog4/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-关闭");
			base.transform.Find("Tog5/Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/新增播控/视频-打开");
		});
		PicTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				PicTog.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控UI/播放控制/播放控制-视频播放器-播放-开始");
				EventCenter.BroadCast(Eventdefine.Picplay);
			}
			else
			{
				PicTog.GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/中控UI/播放控制/播放控制-视频播放器-播放-关闭");
				EventCenter.BroadCast(Eventdefine.Picpause);
			}
		});
		VideoChangeBtn.onClick.AddListener(delegate
		{
			PicTog.isOn = false;
			EventCenter.BroadCast(Eventdefine.VideoSceneChange);
			EventCenter.BroadCast(Eventdefine.FMActive);
		});
		PicchangeBtn.onClick.AddListener(delegate
		{
			OpenTog.isOn = false;
			EventCenter.BroadCast(Eventdefine.PicSceneChange);
			EventCenter.BroadCast(Eventdefine.FMActive);
		});
		for (int num = 0; num < TogArr.Length; num++)
		{
			Debug.LogError("TogArr.Length:" + TogArr.Length);
			for (int num2 = 0; num2 < ShowContentList.Count; num2++)
			{
				TogArr[num].transform.Find("Text").GetComponent<Text>().text = ShowContentList[num2];
			}
		}
		Debug.LogError("ShowContentList:" + ShowContentList.Count);
	}

	private void Update()
	{
		if (!string.IsNullOrEmpty(PlayVideoName))
		{
			Nametxt.text = PlayVideoName;
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
					.GetComponent<SoftWareChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}
}
