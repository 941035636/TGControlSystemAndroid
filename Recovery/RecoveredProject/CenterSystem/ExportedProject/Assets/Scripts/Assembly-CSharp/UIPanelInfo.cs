using System;
using UnityEngine;

[Serializable]
public class UIPanelInfo : ISerializationCallbackReceiver
{
	[NonSerialized]
	public UIPanelType panelType;

	public string panelTypeString;

	public string path;

	public void OnAfterDeserialize()
	{
		UIPanelType uIPanelType = (UIPanelType)Enum.Parse(typeof(UIPanelType), panelTypeString);
		panelType = uIPanelType;
	}

	public void OnBeforeSerialize()
	{
	}
}
