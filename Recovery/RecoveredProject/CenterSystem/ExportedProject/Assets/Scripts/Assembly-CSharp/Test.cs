using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
	public Dropdown dropdown;

	private void Start()
	{
		dropdown.onValueChanged.AddListener(delegate(int value)
		{
			Debug.Log(value);
		});
	}

	private void Update()
	{
	}
}
