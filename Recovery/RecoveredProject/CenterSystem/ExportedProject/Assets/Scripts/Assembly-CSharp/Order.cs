using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class Order
{
	public string allopen;

	public string allclose;

	public List<string> OPenList;

	public List<string> CloseList;

	public string Allopen
	{
		get
		{
			return allopen;
		}
		set
		{
			allopen = value;
		}
	}

	public string Allclose
	{
		get
		{
			return allclose;
		}
		set
		{
			allclose = value;
		}
	}

	public List<string> OpenArr
	{
		get
		{
			return OPenList;
		}
		set
		{
			OPenList = value;
		}
	}

	public List<string> CloseArr
	{
		get
		{
			return CloseList;
		}
		set
		{
			CloseList = value;
		}
	}
}
