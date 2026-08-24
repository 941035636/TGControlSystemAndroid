using UnityEngine;
using UnityEngine.UI;

public class NetworkActions_Debug : MonoBehaviour
{
	public Text text;

	public void Action_TextUpdate(string _value)
	{
		text.text = _value;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
