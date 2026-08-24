using System;
using System.Diagnostics;
using DG.Tweening;
using LitJson;
using UnityEngine;
using UnityEngine.UI;

public class PanelLogin : MonoBehaviour
{
	[SerializeField]
	private Button btnLogin;

	[SerializeField]
	private InputField inAcc4Login;

	[SerializeField]
	private InputField inPwd4Login;

	public Text txtTip;

	[SerializeField]
	private Toggle btnRemPwd;

	private bool isRemPwd;

	public static string jsonUser;

	public Transform Globeltrs;

	private void Start()
	{
		btnLogin.onClick.AddListener(delegate
		{
			UserLoginData obj = new UserLoginData(inAcc4Login.text, inPwd4Login.text);
			if (inAcc4Login.text != "" && inPwd4Login.text != "")
			{
				jsonUser = JsonMapper.ToJson(obj);
			}
			if (inAcc4Login.text == "" || inPwd4Login.text == "")
			{
				TipShow("账号或密码不能为空！");
			}
			else
			{
				OnLoginClick();
			}
		});
		btnRemPwd.onValueChanged.AddListener(delegate(bool ison)
		{
			OnRemPwdClick(ison);
		});
		inAcc4Login.onEndEdit.AddListener(delegate(string acc)
		{
			OnInAccEndEdit(acc);
		});
		inPwd4Login.onEndEdit.AddListener(delegate(string pwd)
		{
			OnInPwdEndEdit(pwd);
		});
		isRemPwd = true;
		string lastUser = Singleton<CacheManager>.Instance.Admin.GetAdmin().LastUser;
		if (!string.IsNullOrEmpty(lastUser))
		{
			inAcc4Login.text = lastUser;
			if (Singleton<CacheManager>.Instance.Admin.GetAdmin().IsRemPwd)
			{
				inPwd4Login.text = Singleton<CacheManager>.Instance.Admin.GetAdminPwd();
				isRemPwd = true;
			}
			else
			{
				isRemPwd = false;
			}
		}
	}

	private void UpdateUseData()
	{
		if (UserDate.IsDateOut && Globel.IsConnect)
		{
			AdminModel admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
			string adminName = admin.AdminName;
			string text = SqlManager.GetInstance().Get(adminName);
			if (CompanyDate(text, DateTime.Now.ToString()))
			{
				admin.Deadlinedate = text;
				Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
				Globeltrs.Find("MaskDateOut").gameObject.SetActive(value: false);
				UserDate.IsDateOut = false;
			}
		}
	}

	public bool CompanyDate(string dateStr1, string dateStr2)
	{
		DateTime t = Convert.ToDateTime(dateStr1);
		DateTime t2 = Convert.ToDateTime(dateStr2);
		int num = DateTime.Compare(t, t2);
		if (num > 0)
		{
			return true;
		}
		if (num == 0)
		{
			return false;
		}
		_ = 0;
		return false;
	}

	private void Update()
	{
		UpdateUseData();
	}

	private void OnInAccEndEdit(string acc)
	{
		if (string.IsNullOrEmpty(acc))
		{
			TipShow("账号不能为空");
		}
		else if (acc != "dysk" && !Singleton<CacheManager>.Instance.Account.IsExistAcc(acc))
		{
			TipShow("账号不存在,请注册账号后再登录");
		}
	}

	private void OnInPwdEndEdit(string pwd)
	{
		if (string.IsNullOrEmpty(inAcc4Login.text))
		{
			TipShow("请先输入账号");
		}
		else if (string.IsNullOrEmpty(pwd))
		{
			TipShow("请输入密码");
		}
		else if (inAcc4Login.text == "dysk")
		{
			if (!Singleton<CacheManager>.Instance.Admin.IsMatchAdminPwd(inPwd4Login.text))
			{
				TipShow("密码不正确，请重新输入密码");
			}
		}
		else if (!Singleton<CacheManager>.Instance.Account.Match(inAcc4Login.text, pwd))
		{
			TipShow("密码不正确，请重新输入密码");
		}
	}

	private void OnLoginClick()
	{
		if (string.IsNullOrEmpty(inAcc4Login.text))
		{
			TipShow("请输入账号");
			return;
		}
		if (string.IsNullOrEmpty(inPwd4Login.text))
		{
			TipShow("请输入密码");
			return;
		}
		if (!Singleton<CacheManager>.Instance.Admin.IsMatchAdminPwd(inPwd4Login.text))
		{
			TipShow("密码不正确，请重新输入密码");
			inPwd4Login.text = string.Empty;
			return;
		}
		AdminModel admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		admin.LastUser = inAcc4Login.text;
		admin.IsRemPwd = isRemPwd;
		Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
		base.transform.parent.gameObject.SetActive(value: false);
		Globel.IsAdmin = true;
		GameData.Instance.curUser = inAcc4Login.text;
		EventCenter.BroadCast(Eventdefine.AdminControl, Globel.IsAdmin);
	}

	private void OnRemPwdClick(bool ison)
	{
		AdminModel admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		if (ison)
		{
			isRemPwd = true;
			admin.IsRemPwd = true;
			Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
		}
		else
		{
			isRemPwd = false;
			admin.IsRemPwd = false;
			Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
		}
	}

	public void OnBtnCloseClick()
	{
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.Arguments = "/c shutdown -s -t 1";
		process.Start();
	}

	private void TipShow(string msg)
	{
		txtTip.text = msg;
		txtTip.DOFade(1f, 0.3f).OnComplete(delegate
		{
			txtTip.DOFade(0f, 0.5f).OnComplete(delegate
			{
			});
		});
	}

	private void OnDestroy()
	{
	}

	public void ShutDownComputer()
	{
		Process process = new Process();
		process.StartInfo.FileName = "cmd.exe";
		process.StartInfo.Arguments = "/c shutdown -s -t 3";
		process.Start();
	}
}
