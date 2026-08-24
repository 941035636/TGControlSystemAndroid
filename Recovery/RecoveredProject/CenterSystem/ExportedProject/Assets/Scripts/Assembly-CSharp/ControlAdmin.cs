using UnityEngine;

public class ControlAdmin : MonoBehaviour
{
	private void Start()
	{
		EventCenter.addlistener<bool>(Eventdefine.AdminControl, Controleditor);
	}

	private void OnDestroy()
	{
		EventCenter.RemoveListener<bool>(Eventdefine.AdminControl, Controleditor);
	}

	public void Controleditor(bool temp)
	{
		int childCount = base.transform.Find("LightScroll/Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (temp)
			{
				base.transform.Find("LightScroll/Viewport/Content").GetChild(i).Find("Manager")
					.gameObject.GetComponent<CanvasGroup>().alpha = 1f;
			}
			else
			{
				base.transform.Find("LightScroll/Viewport/Content").GetChild(i).Find("Manager")
					.gameObject.GetComponent<CanvasGroup>().alpha = 0f;
			}
		}
	}
}
