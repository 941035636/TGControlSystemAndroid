using UnityEngine;
using UnityEngine.UI;

public class ControlBg : MonoBehaviour
{
	public Toggle LightTog;

	public Toggle EauipTog;

	public Toggle ContentTog;

	public Transform Content;

	public Transform EquipMent;

	public Transform Light;

	private void Start()
	{
		LightTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Light.gameObject.SetActive(value: true);
				LightTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-灯光-1");
			}
			else
			{
				Light.gameObject.SetActive(value: false);
				LightTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-灯光-0");
			}
		});
		EauipTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				EquipMent.gameObject.SetActive(value: true);
				EauipTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-设备-1");
			}
			else
			{
				EquipMent.gameObject.SetActive(value: false);
				EauipTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-设备-0");
			}
		});
		ContentTog.onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				Content.gameObject.SetActive(value: true);
				ContentTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-内容-1");
			}
			else
			{
				Content.gameObject.SetActive(value: false);
				ContentTog.transform.Find("Background").GetComponent<Image>().sprite = Resources.Load<Sprite>("中控UI/一级/一级-内容-0");
			}
		});
	}
}
