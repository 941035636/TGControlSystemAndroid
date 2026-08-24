public class GameActiveManager : Singleton<GameActiveManager>
{
	public void ShowActivePanel()
	{
		base.transform.Find("PanelSerialNum").gameObject.SetActive(value: true);
	}

	public void HideActivePanel()
	{
		base.transform.Find("PanelSerialNum").gameObject.SetActive(value: false);
	}
}
