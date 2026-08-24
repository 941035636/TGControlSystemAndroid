using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ControlUnit : BasePanel
{
	private CanvasGroup canvasGroup;

	public Toggle LightTog;

	public Toggle EquipTog;

	public Toggle ContentTog;

	public Transform LightScroll;

	public Transform EquipScroll;

	public Transform ContentScroll;

	public Button ExitBtn;

	private void Start()
	{
		LightTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-灯光-1");
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
		LightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			LightScroll.gameObject.SetActive(ison);
			if (ison)
			{
				LightScroll.DOScale(1f, 0.5f);
				LightTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-灯光-1");
			}
			else
			{
				LightScroll.DOScale(0f, 0.5f);
				LightTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-灯光-0");
			}
		});
		EquipTog.onValueChanged.AddListener(delegate(bool ison)
		{
			EquipScroll.gameObject.SetActive(ison);
			if (ison)
			{
				EquipScroll.DOScale(1f, 0.5f);
				EquipTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-设备-1");
			}
			else
			{
				EquipScroll.DOScale(0f, 0.5f);
				EquipTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-设备-0");
			}
		});
		ContentTog.onValueChanged.AddListener(delegate(bool ison)
		{
			ContentScroll.gameObject.SetActive(ison);
			if (ison)
			{
				ContentScroll.DOScale(1f, 0.5f);
				ContentTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-内容-1");
			}
			else
			{
				ContentScroll.DOScale(0f, 0.5f);
				ContentTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-内容-0");
			}
		});
		ExitBtn.onClick.AddListener(delegate
		{
			Application.Quit();
		});
	}

	public override void OnEnter()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
			canvasGroup.alpha = 0f;
			canvasGroup.blocksRaycasts = true;
			canvasGroup.DOFade(1f, 0.5f);
		}
	}

	public override void OnExit()
	{
		canvasGroup.blocksRaycasts = false;
		canvasGroup.DOFade(0f, 0.5f);
	}

	public void OnClosePanel()
	{
		UIManager.Instance.PopPanel();
	}
}
