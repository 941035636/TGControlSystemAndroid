using System.Collections;
using UnityEngine;

public class FMController : MonoBehaviour
{
	public GameObject FM;

	private void Start()
	{
		EventCenter.addlistener(Eventdefine.FMActive, SetActive);
	}

	private void OnDisable()
	{
		EventCenter.RemoveListener(Eventdefine.FMActive, SetActive);
	}

	private void SetActive()
	{
		StartCoroutine(FMActive());
	}

	private IEnumerator FMActive()
	{
		yield return new WaitForSeconds(1f);
		FM.gameObject.SetActive(value: false);
		yield return new WaitForSeconds(2f);
		FM.gameObject.SetActive(value: true);
	}

	private void Update()
	{
	}
}
