using UnityEngine;

public class Global : MonoBehaviour
{
	private void Start()
	{
		Screen.sleepTimeout = -1;
		Object.DontDestroyOnLoad(this);
	}
}
