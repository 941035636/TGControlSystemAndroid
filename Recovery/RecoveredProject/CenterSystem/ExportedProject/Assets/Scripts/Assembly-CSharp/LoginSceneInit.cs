using UnityEngine;

public class LoginSceneInit : MonoBehaviour
{
	private void Start()
	{
		GameData.Instance.curUser = "";
		AdminModel admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		Debug.Log(admin.ProductNum + "     " + Singleton<CacheManager>.Instance.Admin.IsCodeTrue());
		if (string.IsNullOrEmpty(admin.ProductNum) || !Singleton<CacheManager>.Instance.Admin.IsCodeTrue())
		{
			string productNum = Singleton<CacheManager>.Instance.Admin.SpawnProductNum();
			admin.ProductNum = productNum;
			Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
			Singleton<GameActiveManager>.Instance.ShowActivePanel();
			return;
		}
		Debug.Log("IsActive:" + admin.IsActive);
		if (!admin.IsActive)
		{
			Singleton<GameActiveManager>.Instance.ShowActivePanel();
			return;
		}
		if (string.IsNullOrEmpty(admin.AdminName))
		{
			base.transform.Find("bg/RegisterBg").gameObject.SetActive(value: true);
			return;
		}
		base.transform.Find("bg/GameActive").gameObject.SetActive(value: false);
		base.transform.Find("bg/LoginBg").gameObject.SetActive(value: true);
	}
}
