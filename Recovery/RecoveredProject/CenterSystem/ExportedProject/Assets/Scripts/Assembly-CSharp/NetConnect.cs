using UnityEngine;
using UnityEngine.UI;

public class NetConnect : MonoBehaviour
{
	public Toggle wifiTog;

	private Sprite Togsprite;

	private void Awake()
	{
	}

	private void Start()
	{
		wifiTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				NetManager.Connect(testTCpClient.ip, 8888);
			}
		});
	}

	private void OnTimer()
	{
		NetManager.Send(testTCpClient.clientNum);
	}

	private void Update()
	{
		if (testTCpClient.isConnect)
		{
			WifiSucc();
		}
		else
		{
			WifiFail();
		}
	}

	public void WifiSucc()
	{
		wifiTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/wifi");
		wifiTog.transform.Find("Background/ReConTxt").gameObject.SetActive(value: false);
	}

	public void WifiFail()
	{
		wifiTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/UnWIFI");
		wifiTog.transform.Find("Background/ReConTxt").gameObject.SetActive(value: true);
	}

	private void OnDestroy()
	{
	}
}
