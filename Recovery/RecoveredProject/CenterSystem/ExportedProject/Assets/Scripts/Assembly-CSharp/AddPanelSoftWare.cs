using QmDreamer.UI;
using UnityEngine;
using UnityEngine.UI;

public class AddPanelSoftWare : MonoBehaviour
{
	private void Start()
	{
		string index = base.transform.parent.parent.name.Split('e')[3];
		base.transform.GetComponent<Button>().onClick.AddListener(delegate
		{
			Add_panel(index);
			SetTogInter(ison: false);
		});
	}

	private void SetTogInter(bool ison)
	{
		int childCount = base.transform.parent.parent.Find("Viewport/Content").childCount;
		for (int i = 0; i < childCount; i++)
		{
			if (base.transform.parent.parent.Find("Viewport/Content").GetChild(i).childCount != 0)
			{
				base.transform.parent.parent.Find("Viewport/Content").GetChild(i).GetChild(0)
					.GetComponent<SoftWareChinarDragSwapImage>()
					.interactable = ison;
			}
		}
	}

	public void Add_panel(string index)
	{
		UIManagerSoftWare.Instance.PushPanel(index, UIPanelType.AddSoftWareDetail);
	}
}
public class addpanelSoftWare : MonoBehaviour
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
			FileUtil.AddFile(Application.persistentDataPath, "ControlSoftWareData.txt", info);
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
