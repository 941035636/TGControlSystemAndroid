using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddPanelEquip : MonoBehaviour
{
	private void Start()
	{
		string index = base.transform.parent.parent.name.Split('p')[2];
		base.transform.GetComponent<Button>().onClick.AddListener(delegate
		{
			Add_panel(index);
			SetTogInter();
		});
	}

	private void SetTogInter()
	{
		int childCount = base.transform.parent.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<EquipChinarDragSwapImage>()
					.interactable = false;
			}
		}
	}

	public void Add_panel(string index)
	{
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddEquipChoicePanel);
	}
}
