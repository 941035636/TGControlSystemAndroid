using System;
using System.Collections;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class ControlSoftWarePanel : MonoBehaviour
{
	private Button OpenBtn;

	private Button CloseBtn;

	private Button ShutDownBtn;

	private Button backBtn;

	private Button OpenTV;

	private Button CloseTV;

	private Button PhotoBtn;

	private Button TakePhotoBtn;

	private Text DescriTxt;

	private Text NameTxt;

	public static string ip;

	public static string port;

	public static string path;

	public new static string name;

	public static string note;

	public static bool IsTV;

	public static bool IsPhoto;

	private static ControlSoftWarePanel Instance;

	public static ControlSoftWarePanel GetInstance()
	{
		if (Instance == null)
		{
			Instance = new ControlSoftWarePanel();
		}
		return Instance;
	}

	private void TVControl(bool Tvcontrol)
	{
	}

	private void PhotoControl(bool PhotoControl)
	{
	}

	private void Awake()
	{
	}

	private void Start()
	{
		NameTxt = base.transform.Find("NameTxt").GetComponent<Text>();
		backBtn = base.transform.Find("Back").GetComponent<Button>();
		OpenBtn = base.transform.Find("Open").GetComponent<Button>();
		CloseBtn = base.transform.Find("Close").GetComponent<Button>();
		ShutDownBtn = base.transform.Find("ShutDown").GetComponent<Button>();
		OpenTV = base.transform.Find("OpenTV").GetComponent<Button>();
		CloseTV = base.transform.Find("CloseTV").GetComponent<Button>();
		DescriTxt = base.transform.Find("TitleTxt/DescriTxt").GetComponent<Text>();
		PhotoBtn = base.transform.Find("PhotoBtn").GetComponent<Button>();
		backBtn.onClick.AddListener(delegate
		{
			try
			{
				base.transform.parent.parent.parent.parent.parent.Find("PhoteNetPar").gameObject.SetActive(value: false);
				UnityEngine.Object.Destroy(base.gameObject);
				EventCenter.BroadCast(Eventdefine.close);
				SetTogInter(ison: true);
			}
			catch (Exception ex)
			{
				SetTogInter(ison: true);
				Debug.LogError(ex.ToString());
			}
		});
		TakePhotoBtn = base.transform.parent.parent.parent.parent.parent.Find("PhotoBtn").GetComponent<Button>();
		OpenTV.gameObject.SetActive(IsTV);
		CloseTV.gameObject.SetActive(IsTV);
		PhotoBtn.gameObject.SetActive(IsPhoto);
		Debug.LogError("电视:" + IsTV + "拍照" + IsPhoto);
		ShutDownBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.shutdownphoto);
		});
		TakePhotoBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.takephoto);
		});
		DescriTxt.text = note;
		NameTxt.text = name;
	}

	private IEnumerator ControlPhoto()
	{
		EventCenter.BroadCast(Eventdefine.close);
		yield return new WaitForSeconds(0.5f);
		base.transform.parent.parent.parent.parent.parent.Find("PhoteNetPar").gameObject.SetActive(value: false);
		UnityEngine.Object.Destroy(base.gameObject);
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

	private void OnDestroy()
	{
	}
}
