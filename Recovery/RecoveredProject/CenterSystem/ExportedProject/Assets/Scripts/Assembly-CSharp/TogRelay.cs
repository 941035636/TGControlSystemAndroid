using UnityEngine;
using UnityEngine.UI;

public class TogRelay : MonoBehaviour
{
	private Button DeleteBtn;

	private void Start()
	{
		DeleteBtn = base.transform.Find("deleteBtn").GetComponent<Button>();
	}
}
