using System;
using UnityEngine;

public class DemoObjectAnimation : MonoBehaviour
{
	private Vector3 startPos;

	public bool mx = true;

	public bool my = true;

	public bool mz = true;

	public Vector3 speed = new Vector3(1f, 1f, 1f);

	public Vector3 frequency = new Vector3(1f, 1f, 1f);

	public Color color = Color.red;

	private void Start()
	{
		startPos = base.transform.position;
		GetComponent<Renderer>().material.color = color;
	}

	private void Update()
	{
		Vector3 zero = Vector3.zero;
		zero += (float)(mx ? 1 : 0) * Mathf.Sin(Time.realtimeSinceStartup * (float)Math.PI * frequency.x) * speed.x * base.transform.right;
		zero += (float)(my ? 1 : 0) * Mathf.Sin(Time.realtimeSinceStartup * (float)Math.PI * frequency.y) * speed.y * base.transform.up;
		zero += (float)(mz ? 1 : 0) * Mathf.Sin(Time.realtimeSinceStartup * (float)Math.PI * frequency.z) * speed.z * base.transform.forward;
		base.transform.position = startPos + zero;
	}
}
