using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddPanel : MonoBehaviour
{
	private void Start()
	{
		string index = base.transform.parent.parent.name.Split('e')[2];
		base.transform.GetComponent<Button>().onClick.AddListener(delegate
		{
			Add_panel(index);
			SetTogInter();
		});
	}

	private void SetTogInter()
	{
		int childCount = base.transform.parent.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<ChinarDragSwapImage>()
					.interactable = false;
			}
		}
	}

	public void Add_panel(string index)
	{
		UIManager.Instance.PushPanel(index, UIPanelType.AddChoicePanel);
	}
}
public class addpanel : MonoBehaviour
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

	public GameObject panel03;

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
			FileUtil.AddFile(Application.persistentDataPath, "ControlData.txt", info);
		});
	}

	private void InstantiatePrefab(string name)
	{
		ControllUnitPrefab = Resources.Load<GameObject>("Prefabs/LightControllUnit");
		GameObject obj = Object.Instantiate(ControllUnitPrefab, Parent);
		obj.transform.Find("Text").GetComponent<Text>().text = name;
		obj.GetComponent<Button>().onClick.AddListener(delegate
		{
			panel03.transform.Find("Name").GetComponent<Text>().text = name.Trim();
			panel03.GetComponent<TcpController>().host = ip;
			panel03.GetComponent<TcpController>().port = int.Parse(port);
			panel03.gameObject.SetActive(value: true);
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

	private void Update()
	{
	}
}
