using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class SoftWareArr
{
	public List<SoftWare> softWares;

	public List<SoftWare> GetSoftWare
	{
		get
		{
			return softWares;
		}
		set
		{
			softWares = value;
		}
	}
}
