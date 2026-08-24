using System;
using System.Collections.Generic;
using UnityEngine;

public class UIManager
{
	[Serializable]
	private class UIPanelTypeJson
	{
		public List<UIPanelInfo> infoList;
	}

	private static UIManager _instance;

	private Transform canvasTransform;

	private Dictionary<UIPanelType, string> panelPathDict;

	private Dictionary<UIPanelType, BasePanel> panelDict;

	private Stack<BasePanel> panelStack;

	public static UIManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new UIManager();
			}
			return _instance;
		}
	}

	private Transform CanvasTransform
	{
		get
		{
			if (canvasTransform == null)
			{
				canvasTransform = GameObject.Find("Canvas/bg/LightScroll/Viewport/Content/CellScroll").transform;
			}
			return canvasTransform;
		}
	}

	private Transform GetCanvasTrans(string index)
	{
		canvasTransform = GameObject.Find("Canvas/bg/LightScroll/Viewport/Content/CellScrollpage" + index).transform;
		return canvasTransform;
	}

	private UIManager()
	{
		ParseUIPanelTypeJson();
	}

	public void PushPanel(string index, UIPanelType panelType)
	{
		if (panelStack == null)
		{
			panelStack = new Stack<BasePanel>();
		}
		if (panelStack.Count > 0)
		{
			panelStack.Peek().OnPause();
		}
		BasePanel panel5 = GetPanel(index, panelType);
		panel5.OnEnter();
		panelStack.Push(panel5);
	}

	public void PopPanel()
	{
		if (panelStack == null)
		{
			panelStack = new Stack<BasePanel>();
		}
		if (panelStack.Count > 0)
		{
			panelStack.Pop().OnExit();
			if (panelStack.Count > 0)
			{
				panelStack.Peek().OnResume();
			}
		}
	}

	private BasePanel GetPanel(string index, UIPanelType panelType)
	{
		if (panelDict == null)
		{
			panelDict = new Dictionary<UIPanelType, BasePanel>();
		}
		BasePanel basePanel = panelDict.TryGet(panelType);
		if (basePanel == null)
		{
			GameObject obj = UnityEngine.Object.Instantiate(Resources.Load(panelPathDict.TryGet(panelType))) as GameObject;
			obj.transform.SetParent(GetCanvasTrans(index), worldPositionStays: false);
			return obj.GetComponent<BasePanel>();
		}
		Debug.LogError("不用重新加载");
		return basePanel;
	}

	private void ParseUIPanelTypeJson()
	{
		panelPathDict = new Dictionary<UIPanelType, string>();
		foreach (UIPanelInfo info in JsonUtility.FromJson<UIPanelTypeJson>(Resources.Load<TextAsset>("UIPanelType").text).infoList)
		{
			panelPathDict.Add(info.panelType, info.path);
		}
	}
}
