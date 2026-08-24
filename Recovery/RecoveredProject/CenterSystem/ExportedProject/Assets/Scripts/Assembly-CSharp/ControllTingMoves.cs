using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ControllTingMoves : CustomScrollRect
{
	private GameObject Scroll;

	private ScrollRect sc;

	private float OriginPosX;

	private float offetX;

	private int CurIndex;

	private bool isDragging;

	private bool TimerFlag;

	private float Timer;

	private InputField Tingname;

	private Button DeleteTingBtn;

	protected new void OnEnable()
	{
		Tingname = base.transform.parent.parent.parent.Find("InputTingName").GetComponent<InputField>();
		DeleteTingBtn = base.transform.parent.parent.parent.Find("DeieteTing").GetComponent<Button>();
		Tingname.text = FileUtil.LoadFile(Application.persistentDataPath + "/Light/1", "/TingName.text");
		DeleteTingBtn.onClick.RemoveAllListeners();
		DeleteTingBtn.onClick.AddListener(delegate
		{
			DeletePanel(0f);
		});
		ChangeTingName(0f);
	}

	public new void Start()
	{
		CurIndex = 0;
		Scroll = base.gameObject;
		sc = Scroll.transform.parent.parent.gameObject.GetComponent<CustomScrollRect>();
	}

	public override void OnBeginDrag(PointerEventData eventData)
	{
		sc.OnBeginDrag(eventData);
		OriginPosX = base.transform.localPosition.x;
		CurIndex = (int)(Mathf.Abs(base.transform.localPosition.x) / 1740f);
		offetX = 0f;
		DirectoryInfo[] directories = new DirectoryInfo(Application.persistentDataPath + "/Light").GetDirectories();
		int num = directories.Length;
		List<string> list = new List<string>();
		for (int i = 0; i < num; i++)
		{
			if (directories[i].Name != "Unity")
			{
				list.Add(directories[i].Name);
			}
		}
		if (list.Count != 0)
		{
			Tingname.text = FileUtil.LoadFile(Application.persistentDataPath + "/Light/" + list[CurIndex], "/TingName.text");
		}
	}

	public override void OnDrag(PointerEventData eventData)
	{
		sc.OnDrag(eventData);
	}

	public override void OnEndDrag(PointerEventData eventData)
	{
		int num = 0;
		sc.OnEndDrag(eventData);
		offetX = base.transform.localPosition.x - OriginPosX;
		if (Mathf.Abs(offetX) % 1740f < 300f)
		{
			num = (int)(Mathf.Abs(offetX) / 857f);
			int num2 = 0;
			num2 = ((!(offetX < 0f)) ? (CurIndex - num) : (CurIndex + num));
			num2 = ((num2 >= 0 && num2 <= Scroll.transform.childCount - 1) ? num2 : ((num2 >= 0) ? (Scroll.transform.childCount - 1) : 0));
			Debug.LogError("本次位移目标" + num2 + "初始" + CurIndex);
			float num3 = -num2 * 1740;
			float Tingnum = Mathf.Abs(num3 / 1740f);
			base.transform.DOLocalMoveX(num3, 0.5f).SetEase(Ease.OutBack);
			Debug.LogError(Tingnum);
			ChangeTingName(Tingnum);
			DeleteTingBtn.onClick.RemoveAllListeners();
			DeleteTingBtn.onClick.AddListener(delegate
			{
				DeletePanel(Tingnum);
			});
		}
		else
		{
			num = (int)(Mathf.Abs(offetX) / 1740f);
			num++;
			int num4 = 0;
			num4 = ((!(offetX < 0f)) ? (CurIndex - num) : (CurIndex + num));
			num4 = ((num4 >= 0 && num4 <= Scroll.transform.childCount - 1) ? num4 : ((num4 >= 0) ? (Scroll.transform.childCount - 1) : 0));
			float num5 = -num4 * 1740;
			float Tingnum2 = Mathf.Abs(num5 / 1740f);
			base.transform.DOLocalMoveX(num5, 0.5f).SetEase(Ease.OutBack);
			Debug.LogError(Tingnum2);
			ChangeTingName(Tingnum2);
			DeleteTingBtn.onClick.RemoveAllListeners();
			DeleteTingBtn.onClick.AddListener(delegate
			{
				DeletePanel(Tingnum2);
			});
		}
	}

	private void DeletePanel(float Tingnum)
	{
		DirectoryInfo[] dirarr = new DirectoryInfo(Application.persistentDataPath + "/Light").GetDirectories();
		int num = dirarr.Length;
		List<string> TingList = new List<string>();
		for (int i = 0; i < num; i++)
		{
			if (dirarr[i].Name != "Unity")
			{
				TingList.Add(dirarr[i].Name);
			}
		}
		GameObject original = Resources.Load<GameObject>("Prefabs/DeleteTingPanel");
		GameObject panelObj = Object.Instantiate(original);
		panelObj.transform.SetParent(base.transform.parent.parent.parent);
		panelObj.transform.localPosition = new Vector3(1f, 1f, 1f);
		panelObj.transform.localScale = new Vector3(1f, 1f, 1f);
		panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.RemoveAllListeners();
		panelObj.transform.Find("BtnAdd").GetComponent<Button>().onClick.AddListener(delegate
		{
			Debug.LogError(Application.persistentDataPath + "/Light/" + TingList[(int)Tingnum]);
			FileUtil.DeleteFolder(Application.persistentDataPath + "/Light/", TingList[(int)Tingnum]);
			Object.DestroyImmediate(base.transform.GetChild((int)Tingnum).gameObject);
			if ((int)Tingnum != dirarr.Length - 1)
			{
				SortDeleteFolder((int)Tingnum);
			}
			Object.DestroyImmediate(panelObj);
		});
		panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.RemoveAllListeners();
		panelObj.transform.Find("BtnExit").GetComponent<Button>().onClick.AddListener(delegate
		{
			panelObj.GetComponent<CanvasGroup>().alpha = 0f;
			panelObj.GetComponent<CanvasGroup>().blocksRaycasts = false;
		});
	}

	private void SortDeleteFolder(int value)
	{
		DirectoryInfo[] directories = new DirectoryInfo(Application.persistentDataPath + "/Light").GetDirectories();
		int num = directories.Length;
		for (int i = value; i < num; i++)
		{
			int num2 = i;
			if (directories[i].Name != "Unity")
			{
				directories[i].MoveTo(Application.persistentDataPath + "/Light/" + (num2 + 1));
			}
		}
	}

	private void ChangeTingName(float num)
	{
		DirectoryInfo[] directories = new DirectoryInfo(Application.persistentDataPath + "/Light").GetDirectories();
		int num2 = directories.Length;
		List<string> TingList = new List<string>();
		for (int i = 0; i < num2; i++)
		{
			if (directories[i].Name != "Unity")
			{
				TingList.Add(directories[i].Name);
			}
		}
		Tingname.onEndEdit.RemoveAllListeners();
		Tingname.onEndEdit.AddListener(delegate(string text)
		{
			Debug.LogError("保存厅名字:" + Application.persistentDataPath + "/Light/" + TingList[(int)num]);
			FileUtil.CoverFile(Application.persistentDataPath + "/Light/" + TingList[(int)num], "/TingName.text", text);
		});
		Tingname.text = FileUtil.LoadFile(Application.persistentDataPath + "/Light/" + TingList[(int)num], "/TingName.text");
	}
}
