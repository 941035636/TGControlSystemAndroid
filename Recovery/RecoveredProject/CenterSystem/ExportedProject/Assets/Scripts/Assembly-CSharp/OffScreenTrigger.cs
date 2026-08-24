using UnityEngine;

public class OffScreenTrigger : MonoBehaviour
{
	private WorldToScreenSpace[] WSSs;

	private void Start()
	{
		WSSs = base.transform.parent.GetComponentsInChildren<WorldToScreenSpace>(includeInactive: true);
		WorldToScreenSpace[] wSSs = WSSs;
		for (int i = 0; i < wSSs.Length; i++)
		{
			wSSs[i].enabled = true;
		}
	}

	private void Update()
	{
	}

	public void Action_offscreen()
	{
		WorldToScreenSpace[] wSSs = WSSs;
		for (int i = 0; i < wSSs.Length; i++)
		{
			wSSs[i].InvokeOffScreen();
		}
	}
}
