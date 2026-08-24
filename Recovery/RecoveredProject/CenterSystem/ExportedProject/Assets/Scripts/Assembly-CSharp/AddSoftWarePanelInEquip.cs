using UnityEngine;
using UnityEngine.UI;

public class AddSoftWarePanelInEquip : MonoBehaviour
{
	private void Start()
	{
		string index = base.transform.parent.name.Split('p')[2];
		base.transform.GetComponent<Button>().onClick.AddListener(delegate
		{
			Add_panel(index);
		});
	}

	public void Add_panel(string index)
	{
		UIManagerEquip.Instance.PushPanel(index, UIPanelType.AddSoftWareConTrolPanel);
	}
}
