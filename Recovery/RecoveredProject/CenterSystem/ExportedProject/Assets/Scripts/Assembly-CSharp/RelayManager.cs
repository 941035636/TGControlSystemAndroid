using System.Collections.Generic;
using UnityEngine;

public class RelayManager : MonoBehaviour
{
	private List<RelayModel> ListRelayModel;

	private void OnEnable()
	{
		ListRelayModel = new List<RelayModel>();
		ListRelayModel.Clear();
		ListRelayModel = EquipRelayModelControl.GetInstance().GetRelayItemList(base.transform.name.Split('p')[2]);
	}
}
