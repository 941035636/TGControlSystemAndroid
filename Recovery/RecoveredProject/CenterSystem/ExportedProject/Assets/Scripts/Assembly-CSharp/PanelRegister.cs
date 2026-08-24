using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PanelRegister : MonoBehaviour
{
	public InputField InputAcc;

	private string acc = string.Empty;

	public InputField InputPwd;

	private string pwd1 = string.Empty;

	public InputField InputPwd2;

	private string pwd2 = string.Empty;

	public Button BtnLogin;

	public Text txtTip;

	private bool isRemPwd = true;

	public void Start()
	{
		BtnLogin.onClick.AddListener(delegate
		{
			if (InputAcc.text == "" || InputPwd.text == "" || InputPwd2.text == "")
			{
				TipShow("账号或密码不能为空！");
			}
			else
			{
				OnLoginClick();
			}
		});
		InputAcc.onEndEdit.AddListener(delegate(string text)
		{
			acc = text;
		});
		InputPwd.onEndEdit.AddListener(delegate(string text)
		{
			pwd1 = text;
		});
		InputPwd2.onEndEdit.AddListener(delegate(string text)
		{
			pwd2 = text;
			if (pwd1 != pwd2)
			{
				TipShow("两次输入的密码不一致，请重新输入");
				text = "";
			}
		});
	}

	private void OnLoginClick()
	{
		if (string.IsNullOrEmpty(InputAcc.text))
		{
			TipShow("请输入账号");
			return;
		}
		if (string.IsNullOrEmpty(InputPwd.text))
		{
			TipShow("请输入密码");
			return;
		}
		if (string.IsNullOrEmpty(InputPwd2.text))
		{
			TipShow("请输入确认密码");
			return;
		}
		if (!Globel.IsConnect)
		{
			TipShow("设备未连接到外网，无法注册用户信息");
			return;
		}
		AdminModel admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		string registerdate = DateTime.Now.ToString();
		string deadlinedate = DateTime.Now.AddDays(10.0).ToString();
		string msg = SqlManager.GetInstance().Add(acc, pwd1, registerdate, 20, 1, deadlinedate, 0, admin.ProductNum, admin.ActiveCode);
		TipShow(msg);
		admin.LastUser = acc;
		admin.IsRemPwd = isRemPwd;
		admin.AdminPwd = pwd1;
		admin.Registerdate = registerdate;
		admin.Prescription = 20;
		admin.Authority = 1;
		admin.Deadlinedate = deadlinedate;
		admin.Longtime = 0;
		admin.LastNetUser = acc;
		admin.LastNetUserPwd = pwd1;
		admin.AdminName = acc;
		admin.AdminPwd = pwd1;
		admin.LastNetUser = acc;
		Singleton<CacheManager>.Instance.Admin.UpdateModel(admin);
		base.transform.parent.parent.Find("LoginBg").gameObject.SetActive(value: true);
		base.transform.parent.gameObject.SetActive(value: false);
	}

	private void TipShow(string msg)
	{
		txtTip.text = msg;
		txtTip.DOFade(1f, 1f).OnComplete(delegate
		{
			txtTip.DOFade(0f, 1f).OnComplete(delegate
			{
			});
		});
	}
}
