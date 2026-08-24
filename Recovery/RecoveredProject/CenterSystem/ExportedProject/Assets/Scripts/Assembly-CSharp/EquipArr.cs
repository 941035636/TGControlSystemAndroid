using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class EquipArr
{
	public List<Equip> Equips;

	public List<Equip> GetSoftWare
	{
		get
		{
			return Equips;
		}
		set
		{
			Equips = value;
		}
	}
}
