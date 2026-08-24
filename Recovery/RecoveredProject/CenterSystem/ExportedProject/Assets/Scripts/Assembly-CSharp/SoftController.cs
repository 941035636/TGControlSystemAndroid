using UnityEngine;
using UnityEngine.UI;

public class SoftController : MonoBehaviour
{
	public Button OpenBtn;

	public Button CLoseBtn;

	public string softName = string.Empty;

	private void Start()
	{
		OpenBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.OPenSoft, softName);
			Debug.LogError("打开:" + softName);
		});
		CLoseBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.CloseSoft, softName);
		});
	}
}
