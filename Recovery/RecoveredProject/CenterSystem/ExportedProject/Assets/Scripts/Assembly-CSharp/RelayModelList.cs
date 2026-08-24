using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class RelayModelList
{
	private List<RelayModel> RelayModels;

	public List<RelayModel> GetRelayModelList
	{
		get
		{
			return RelayModels;
		}
		set
		{
			RelayModels = value;
		}
	}
}
