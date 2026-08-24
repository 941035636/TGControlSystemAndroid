using System.IO;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PaneSerialNum : BasePanel
{
	[SerializeField]
	private Text tip;

	[SerializeField]
	private Text productNum;

	[SerializeField]
	private InputField activeCode;

	[SerializeField]
	private Button btnEnsure;

	private AdminModel model;

	public GameObject LoginBg;

	public GameObject RegisterBg;

	private void Start()
	{
		model = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		productNum.text = model.ProductNum;
		Debug.Log(AdminControl.GetPwd3(model.ProductNum));
		Debug.LogError(model.ProductNum);
		btnEnsure.onClick.RemoveAllListeners();
		btnEnsure.onClick.AddListener(OnEnsureClick);
	}

	private void CreatCodePath()
	{
		string path = Application.persistentDataPath + "/Code.txt";
		if (!File.Exists(path))
		{
			File.Create(path).Close();
			model.ProductNum += Random.Range(0, 10000);
			File.WriteAllText(path, model.ProductNum);
			Singleton<CacheManager>.Instance.Admin.UpdateModel(model);
			return;
		}
		Debug.Log("path:" + File.Exists(path));
		string text = File.ReadAllText(path);
		if (text == "")
		{
			model.ProductNum += Random.Range(0, 10000);
			File.WriteAllText(path, model.ProductNum);
		}
		else
		{
			model.ProductNum = text;
			Singleton<CacheManager>.Instance.Admin.UpdateModel(model);
		}
	}

	public void OnEnsureClick()
	{
		if (string.IsNullOrEmpty(activeCode.text))
		{
			TipShow("请输入激活码");
			return;
		}
		model.ActiveCode = activeCode.text;
		model.ProductNum = productNum.text;
		Singleton<CacheManager>.Instance.Admin.UpdateModel(model);
		if (Singleton<CacheManager>.Instance.Admin.IsCodeTrue())
		{
			base.gameObject.SetActive(value: false);
			model.IsActive = true;
			RegisterBg.SetActive(value: true);
		}
		else
		{
			TipShow("您输入的激活码不正确，请重新输入激活码。");
			model.IsActive = false;
		}
		Singleton<CacheManager>.Instance.Admin.UpdateModel(model);
	}

	private void TipShow(string msg)
	{
		tip.text = msg;
		tip.DOFade(1f, 1f).OnComplete(delegate
		{
			tip.DOFade(0f, 1f).OnComplete(delegate
			{
			});
		});
	}

	public override void OnEnter()
	{
	}

	public override void OnPause()
	{
	}

	public override void OnResume()
	{
	}

	public override void OnExit()
	{
	}
}
