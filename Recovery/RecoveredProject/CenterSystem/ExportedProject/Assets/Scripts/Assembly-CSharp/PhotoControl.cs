using UnityEngine;
using UnityEngine.UI;

public class PhotoControl : MonoBehaviour
{
	public Toggle UpDangFlag;

	public Toggle UpTuanFlag;

	public Toggle UpDuiFlag;

	public Button backBtn;

	private void Start()
	{
		UpDangFlag.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.updangFlag);
			}
		});
		UpTuanFlag.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.UpTuanFlag);
			}
		});
		UpDuiFlag.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EventCenter.BroadCast(Eventdefine.upDuiFlag);
			}
		});
		backBtn.onClick.AddListener(delegate
		{
			EventCenter.BroadCast(Eventdefine.backFlag);
		});
	}

	private void Update()
	{
	}
}
