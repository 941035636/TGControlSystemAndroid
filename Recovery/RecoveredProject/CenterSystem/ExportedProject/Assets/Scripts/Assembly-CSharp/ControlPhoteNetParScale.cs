using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ControlPhoteNetParScale : MonoBehaviour
{
	private void Start()
	{
		GetComponent<RectTransform>().sizeDelta = new Vector2(607f, 280f);
		GetComponent<Toggle>().onValueChanged.AddListener(delegate(bool ison)
		{
			if (ison)
			{
				GetComponent<RectTransform>().DOSizeDelta(new Vector2(2000f, 1200f), 1f);
				base.transform.parent.DOLocalMove(new Vector3(0f, 0f, 0f), 1f);
				base.transform.parent.Find("Text").gameObject.SetActive(value: false);
				base.transform.parent.parent.Find("PhotoBtn").gameObject.SetActive(value: true);
			}
			else
			{
				GetComponent<RectTransform>().DOSizeDelta(new Vector2(607f, 280f), 1f);
				base.transform.parent.DOLocalMove(new Vector3(-124f, -166f, 0f), 1f);
				base.transform.parent.Find("Text").gameObject.SetActive(value: true);
				base.transform.parent.parent.Find("PhotoBtn").gameObject.SetActive(value: false);
			}
		});
	}
}
