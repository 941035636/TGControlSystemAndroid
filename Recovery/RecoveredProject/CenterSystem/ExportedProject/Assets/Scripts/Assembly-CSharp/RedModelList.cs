using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RedModelList
{
	private string msg;

	private List<RedModel> RedModels;

	public string Msg
	{
		get
		{
			return msg;
		}
		set
		{
			msg = value;
		}
	}

	public List<RedModel> GetRedModelList
	{
		get
		{
			return RedModels;
		}
		set
		{
			RedModels = value;
		}
	}
}
