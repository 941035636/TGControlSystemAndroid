using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class AddSoftWareChoicePanel : BasePanel
{
	private Button addSoftWareControlbtn;

	private Button addVideoControlbtn;

	private Button addTVbtn;

	private Button closeBtn;

	private Button addTing;

	private CanvasGroup canvasGroup;

	private string index;

	private void Start()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		addSoftWareControlbtn = base.transform.Find("addSoftwareControl").GetComponent<Button>();
		addVideoControlbtn = base.transform.Find("addVideoControl").GetComponent<Button>();
		addTVbtn = base.transform.Find("addTV").GetComponent<Button>();
		closeBtn = base.transform.Find("BtnExit").GetComponent<Button>();
		addTing = base.transform.Find("addTing").GetComponent<Button>();
		index = base.transform.parent.name.Split('e')[3];
		addSoftWareControlbtn.onClick.AddListener(delegate
		{
			AddSoftWare(index);
		});
		addVideoControlbtn.onClick.AddListener(delegate
		{
			AddVideo(index);
		});
		addTVbtn.onClick.AddListener(delegate
		{
			AddTV(index);
		});
		closeBtn.onClick.AddListener(delegate
		{
			OnClosePanel();
		});
		addTing.onClick.AddListener(delegate
		{
			AddTing();
		});
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

	public override void OnResume()
	{
		canvasGroup.blocksRaycasts = true;
	}

	public void AddSoftWare(string index)
	{
		OnClosePanel();
		UIManagerSoftWare.Instance.PushPanel(index, UIPanelType.AddSoftWareDetail);
	}

	public void AddVideo(string index)
	{
		OnClosePanel();
		UIManagerSoftWare.Instance.PushPanel(index, UIPanelType.AddVideoDetail);
	}

	public void AddTV(string index)
	{
		OnClosePanel();
		UIManagerSoftWare.Instance.PushPanel(index, UIPanelType.AddSoftWareDetail);
	}

	public void AddTing()
	{
		GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpageSoftWare");
		Debug.LogError("AddTing");
		obj.SetActive(value: false);
		GameObject obj2 = Object.Instantiate(obj, base.transform.parent.parent);
		int childCount = base.transform.parent.parent.childCount;
		obj2.name = "CellScrollpageSoftWare" + childCount;
		obj2.SetActive(value: true);
		obj2.transform.localScale = new Vector3(1f, 1f, 1f);
		obj2.transform.parent.DOLocalMoveX(-1500 * childCount, 1f);
		OnClosePanel();
	}
}
