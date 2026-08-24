using DG.Tweening;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddChoicePanel : BasePanel
{
	private Button addRelaybtn;

	private Button addChannelbtn;

	private Button addTingbtn;

	private CanvasGroup canvasGroup;

	private string index;

	private void Start()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		addRelaybtn = base.transform.Find("addrelay").GetComponent<Button>();
		addChannelbtn = base.transform.Find("addchannel").GetComponent<Button>();
		addTingbtn = base.transform.Find("addTing").GetComponent<Button>();
		index = base.transform.parent.name.Split('e')[2];
		addRelaybtn.onClick.AddListener(delegate
		{
			AddRelay(index);
		});
		addChannelbtn.onClick.AddListener(delegate
		{
			AddChannel(index);
		});
		addTingbtn.onClick.AddListener(delegate
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
		UIManager.Instance.PopPanel();
		SetTogInter(ison: true);
	}

	private void SetTogInter(bool ison)
	{
		int childCount = base.transform.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<ChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	public override void OnResume()
	{
		canvasGroup.blocksRaycasts = true;
	}

	public void AddRelay(string index)
	{
		OnClosePanel();
		UIManager.Instance.PushPanel(index, UIPanelType.AddrelayPanel);
	}

	public void AddChannel(string index)
	{
		OnClosePanel();
		UIManager.Instance.PushPanel(index, UIPanelType.AddChannel);
	}

	public void AddTing()
	{
		GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpage");
		Debug.LogError("AddTing");
		obj.SetActive(value: false);
		GameObject obj2 = Object.Instantiate(obj, base.transform.parent.parent);
		int childCount = base.transform.parent.parent.childCount;
		obj2.name = "CellScrollpage" + childCount;
		obj2.SetActive(value: true);
		obj2.transform.localScale = new Vector3(1f, 1f, 1f);
		obj2.transform.parent.DOLocalMoveX(-1714 * (childCount - 1), 0.5f);
		OnClosePanel();
	}
}
