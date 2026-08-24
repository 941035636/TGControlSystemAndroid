using UnityEngine;

// Compatibility wrapper retained for prefabs or UnityEvents that referenced the
// original lowercase class name.
public class wakeup : MonoBehaviour
{
	private static wakeup instance;

	private void Awake()
	{
		instance = this;
	}

	public static wakeup GetInstance()
	{
		if (instance == null)
		{
			instance = Object.FindObjectOfType<wakeup>();
		}
		return instance;
	}

	public void RemoteWakeUpComputer(string macAddress)
	{
		WakeUpComputer.WakeUp(macAddress);
	}
}
