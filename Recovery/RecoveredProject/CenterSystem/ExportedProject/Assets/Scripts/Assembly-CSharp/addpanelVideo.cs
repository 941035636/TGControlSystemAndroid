using UnityEngine;
using UnityEngine.UI;

public class addpanelVideo : MonoBehaviour
{
	public Transform Parent;

	public InputField InputName;

	public InputField InputIp;

	public InputField InputPort;

	public Toggle SubBtn;

	public GameObject ControllUnitPrefab;

	private new string name;

	private string ip;

	private string port;

	public GameObject panel05;

	private void Start()
	{
		InputName.onEndEdit.AddListener(delegate(string str)
		{
			name = str;
		});
		InputIp.onEndEdit.AddListener(delegate(string str)
		{
			ip = str;
		});
		InputPort.onEndEdit.AddListener(delegate(string str)
		{
			port = str;
		});
		base.transform.parent.Find("confirmBtn").GetComponent<Button>().onClick.AddListener(delegate
		{
			InstantiatePrefab(name);
			base.transform.parent.gameObject.SetActive(value: false);
			string info = name + "|" + ip + "|" + port;
			FileUtil.AddFile(Application.persistentDataPath, "ControlVideoData.txt", info);
		});
	}

	private void InstantiatePrefab(string name)
	{
		ControllUnitPrefab = Resources.Load<GameObject>("Prefabs/LightControllUnit");
		GameObject obj = Object.Instantiate(ControllUnitPrefab, Parent);
		obj.transform.Find("Text").GetComponent<Text>().text = name;
		obj.GetComponent<Button>().onClick.AddListener(delegate
		{
			panel05.transform.Find("Name").GetComponent<Text>().text = name.Trim();
			panel05.gameObject.SetActive(value: true);
		});
		SubBtn.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				if ((bool)obj)
				{
					obj.transform.Find("DeleteBtn").gameObject.SetActive(value: true);
				}
			}
			else if ((bool)obj)
			{
				obj.transform.Find("DeleteBtn").gameObject.SetActive(value: false);
			}
		});
		obj.transform.Find("DeleteBtn").GetComponent<Button>().onClick.AddListener(delegate
		{
			Object.Destroy(obj);
		});
	}
}
