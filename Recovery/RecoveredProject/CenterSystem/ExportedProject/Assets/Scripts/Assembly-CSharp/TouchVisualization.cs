using System.Collections.Generic;
using UnityEngine;

public class TouchVisualization : MonoBehaviour
{
	public GameObject fingerPrefab;

	public List<GameObject> fingers;

	private void Start()
	{
		fingerPrefab.SetActive(value: false);
	}

	private void Update()
	{
		if (Input.touchCount >= 2)
		{
			Touch[] touches = Input.touches;
			while (fingers.Count <= Input.touchCount)
			{
				fingers.Add(Object.Instantiate(fingerPrefab, Vector3.zero, Quaternion.identity, base.transform));
			}
			for (int i = 0; i < fingers.Count; i++)
			{
				if (i < Input.touchCount)
				{
					fingers[i].SetActive(value: true);
					fingers[i].GetComponent<RectTransform>().position = new Vector3(touches[i].position.x, touches[i].position.y, 0f);
				}
				else
				{
					fingers[i].SetActive(value: false);
				}
			}
		}
		else
		{
			for (int j = 0; j < fingers.Count; j++)
			{
				fingers[j].SetActive(value: false);
			}
		}
	}
}
