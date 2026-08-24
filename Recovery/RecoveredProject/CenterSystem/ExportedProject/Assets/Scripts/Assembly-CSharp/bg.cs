using System;
using UnityEngine;

public class bg : BasePanel
{
	private CanvasGroup canvasGroup;

	private void Start()
	{
		canvasGroup = GetComponent<CanvasGroup>();
	}

	public override void OnPause()
	{
	}

	public override void OnResume()
	{
		canvasGroup.blocksRaycasts = true;
	}

	public void OnPushPanel(string panelTypeString)
	{
		_ = (UIPanelType)Enum.Parse(typeof(UIPanelType), panelTypeString);
	}
}
