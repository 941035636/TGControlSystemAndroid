using DG.Tweening;
using UnityEngine;

public class ControlContent : BasePanel
{
	private CanvasGroup canvasGroup;

	private void Start()
	{
		if (canvasGroup == null)
		{
			canvasGroup = GetComponent<CanvasGroup>();
		}
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
