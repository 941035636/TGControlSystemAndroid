using System;
using UnityEngine;

public class FillCameraBackground : MonoBehaviour
{
	public GameObject quad;

	public Camera mainCam;

	private float CW = 1f;

	private float CH = 1f;

	private float CAspect;

	private float TW = 1f;

	private float TH = 1f;

	private float TAspect = 1f;

	private float ScaleMag = 1f;

	private Vector3 quadScale;

	[Range(0f, 1f)]
	public float Dist = 0.5f;

	private void Start()
	{
		if (mainCam == null)
		{
			mainCam = Camera.main;
		}
	}

	private void LateUpdate()
	{
		float fieldOfView = mainCam.fieldOfView;
		float nearClipPlane = mainCam.nearClipPlane;
		float farClipPlane = mainCam.farClipPlane;
		float num = nearClipPlane + (farClipPlane - nearClipPlane) * Dist;
		CAspect = (float)Screen.width / (float)Screen.height;
		CH = Mathf.Tan(fieldOfView / 2f * ((float)Math.PI / 180f)) * num * 2f;
		CW = CAspect * CH;
		ScaleMag = 1f;
		if (quad.GetComponent<Renderer>().material.mainTexture != null)
		{
			TW = quad.GetComponent<Renderer>().material.mainTexture.width;
			TH = quad.GetComponent<Renderer>().material.mainTexture.height;
		}
		TAspect = TW / TH;
		if (TAspect > CAspect)
		{
			ScaleMag = TAspect / CAspect;
			quadScale = new Vector3(CW * ScaleMag, CH, 1f);
		}
		else
		{
			ScaleMag = CAspect / TAspect;
			quadScale = new Vector3(CW, CH * ScaleMag, 1f);
		}
		quad.transform.position = mainCam.transform.position + mainCam.transform.forward * num;
		quad.transform.localScale = quadScale;
	}
}
