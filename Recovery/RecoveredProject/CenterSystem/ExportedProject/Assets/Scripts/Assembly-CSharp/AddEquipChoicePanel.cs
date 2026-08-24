using DG.Tweening;
using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddEquipChoicePanel : BasePanel
{
	private Button addRelaybtn;

	private Button addChannelbtn;

	private Button addTingbtn;

	private Button addSoftWarebtn;

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
		addSoftWarebtn = base.transform.Find("addSoftControl").GetComponent<Button>();
		addSoftWarebtn.onClick.AddListener(delegate
		{
			AddSoftWare(index);
		});
		index = base.transform.parent.name.Split('p')[2];
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
		UIManagerEquip.Instance.PopPanel();
		SetTogInter();
	}

	private void SetTogInter()
	{
		int childCount = base.transform.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<EquipChinarDragSwapImage>()
					.interactable = true;
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
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddEquipRelayPanel);
	}

	public void AddChannel(string index)
	{
		OnClosePanel();
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddEquipChannelPanel);
	}

	public void AddSoftWare(string index)
	{
		OnClosePanel();
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddSoftWareConTrolPanel);
	}

	public void AddTing()
	{
		GameObject obj = Resources.Load<GameObject>("Prefabs/CellScrollpageEquip");
		Debug.LogError("AddTing");
		obj.SetActive(value: false);
		GameObject obj2 = Object.Instantiate(obj, base.transform.parent.parent);
		int childCount = base.transform.parent.parent.childCount;
		obj2.name = "CellScrollpageEquip" + childCount;
		obj2.SetActive(value: true);
		obj2.transform.localScale = new Vector3(1f, 1f, 1f);
		obj2.transform.parent.DOLocalMoveX(-1500 * childCount, 1f);
		OnClosePanel();
	}
}
