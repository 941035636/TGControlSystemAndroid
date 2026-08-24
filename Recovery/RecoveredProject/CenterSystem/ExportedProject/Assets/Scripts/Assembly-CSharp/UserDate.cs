using System;
using UnityEngine;
using UnityEngine.UI;

public class UserDate : MonoBehaviour
{
	private float curYear;

	private float curMonth;

	private float curDay;

	private float curHours;

	private float curMin;

	private float curSec;

	public float year;

	public float month;

	public float day;

	public float hours;

	public float min;

	public float sec;

	public Text dateshow;

	private AdminModel admin;

	public Transform Globelt;

	public static bool IsDateOut;

	private DateTime TimeStringToFromat(string dateTimeString, string newDataTimeFormat)
	{
		return DateTime.Parse(dateTimeString);
	}

	private void Update()
	{
	}

	private void CheckAurtoh()
	{
		admin = Singleton<CacheManager>.Instance.Admin.GetAdmin();
		if (!string.IsNullOrEmpty(admin.Deadlinedate))
		{
			year = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Year;
			month = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Month;
			day = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Day;
			hours = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Hour;
			min = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Minute;
			sec = TimeStringToFromat(admin.Deadlinedate, "yyyy-MM-dd HH:mm").Second;
			dateshow.text = "体验版：截止日期为" + year + "年" + month + "月" + day + "日" + hours + "时" + min + "分" + sec + "秒";
		}
		if (string.IsNullOrEmpty(admin.Registerdate))
		{
			return;
		}
		curYear = DateTime.Now.Year;
		curMonth = DateTime.Now.Month;
		curDay = DateTime.Now.Day;
		curHours = DateTime.Now.Hour;
		curMin = DateTime.Now.Minute;
		curSec = DateTime.Now.Second;
		if (curYear > year)
		{
			IsDateOut = true;
			Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
		}
		else
		{
			if (curYear < year || curYear != year)
			{
				return;
			}
			if (curMonth > month)
			{
				IsDateOut = true;
				Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
			}
			else
			{
				if (curMonth < month || curMonth != month)
				{
					return;
				}
				if (curDay > day)
				{
					IsDateOut = true;
					Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
				}
				else
				{
					if (curDay < day || curDay != day)
					{
						return;
					}
					if (curHours > hours)
					{
						IsDateOut = true;
						Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
					}
					else
					{
						if (curHours < hours || curHours != hours)
						{
							return;
						}
						if (curMin > min)
						{
							IsDateOut = true;
							Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
						}
						else if (!(curMin < min) && curMin == min)
						{
							if (curSec > sec)
							{
								IsDateOut = true;
								Globelt.Find("MaskDateOut").gameObject.SetActive(value: true);
							}
							else
							{
								_ = curSec;
								_ = sec;
							}
						}
					}
				}
			}
		}
	}
}
